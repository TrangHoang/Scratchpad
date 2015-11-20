﻿using System;
using System.Linq;
using MailingServiceDemo.Database;
using MailingServiceDemo.Event;
using MailingServiceDemo.Model;
using Reusables.EventSourcing;

namespace MailingServiceDemo.EventHandler
{
    public class StoreKeeper : IEventSubscriber<MailRequestReceived>,
                               IEventSubscriber<DeliveryNeeded>,
                               IEventSubscriber<MessageSent>,
                               IEventSubscriber<SendingFailed>,
                               IEventSubscriber<FaultAnalysisRequired>,
                               IEventSubscriber<ManualAnalysisRequired>
    {
        private readonly IDbContext _dbContext;
        private readonly IEventPublisher _eventPublisher;
        private readonly IApplicationSettings _settings;

        public StoreKeeper(IDbContext dbContext, IEventPublisher eventPublisher, IApplicationSettings settings)
        {
            _dbContext = dbContext;
            _eventPublisher = eventPublisher;
            _settings = settings;
        }

        public void Handle(MailRequestReceived @event)
        {
            foreach (var mailMessage in @event.Messages)
            {
                _dbContext.Set<OutboxMessage>().Add(new OutboxMessage
                                                    {
                                                        Id = Guid.NewGuid(),
                                                        RequestId = @event.Id,
                                                        Message = mailMessage,
                                                        Priority = (int) mailMessage.Priority,
                                                        QueuedAt = DateTime.UtcNow
                                                    });
            }

            _eventPublisher.Publish(new OutboxManagementNeeded());
        }

        public void Handle(DeliveryNeeded @event)
        {
            var delivery = @event.Message;
            var ongoingMessage = new OngoingMessage
                                 {
                                     Id = delivery.Id,
                                     RequestId = delivery.RequestId,
                                     Message = delivery.Message,
                                     Priority = delivery.Priority,
                                 };

            _dbContext.Set<OngoingMessage>().Add(ongoingMessage);

            _dbContext.Set<OutboxMessage>().Remove(delivery.Id);

            _eventPublisher.Publish(new DeliveryReady {Message = ongoingMessage});
        }

        public void Handle(MessageSent @event)
        {
            _dbContext.Set<SentMessage>().Add(new SentMessage
                                              {
                                                  Id = @event.MessageId,
                                                  RequestId = @event.RequestId,
                                                  Message = @event.Message,
                                                  SentAt = @event.SentAt
                                              });

            _dbContext.Set<OngoingMessage>().Remove(@event.MessageId);

            _eventPublisher.Publish(new OutboxManagementNeeded());
        }

        public void Handle(SendingFailed @event)
        {
            _dbContext.Set<FaultMessage>().Add(new FaultMessage
                                               {
                                                   RequestId = @event.RequestId,
                                                   MessageId = @event.MessageId,
                                                   Message = @event.Message,
                                                   Reason = @event.Reason,
                                                   TriedAt = @event.TriedAt
                                               });

            _eventPublisher.Publish(new FaultAnalysisRequired
                                    {
                                        MessageId = @event.MessageId,
                                        Message = @event.Message
                                    });

            _eventPublisher.Publish(new OutboxManagementNeeded());
        }

        public void Handle(FaultAnalysisRequired @event)
        {
            var attemptCount = _dbContext.Set<FaultMessage>().Count(message => message.MessageId == @event.MessageId);

            if (attemptCount >= _settings.MaxAttempt)
            {
                _eventPublisher.Publish(new ManualAnalysisRequired
                                        {
                                            MessageId = @event.MessageId,
                                            Message = @event.Message
                                        });

                return;
            }

            var ongoingMessage = _dbContext.Set<OngoingMessage>().GetById(@event.MessageId);

            _dbContext.Set<OutboxMessage>().Add(new OutboxMessage
                                                {
                                                    Id = @event.MessageId,
                                                    RequestId = ongoingMessage.RequestId,
                                                    Message = ongoingMessage.Message,
                                                    Priority = ongoingMessage.Priority,
                                                    QueuedAt = DateTime.UtcNow
                                                });

            _dbContext.Set<OngoingMessage>().Remove(@event.MessageId);
        }

        public void Handle(ManualAnalysisRequired @event)
        {
            _dbContext.Set<SuspiciousMessage>().Add(new SuspiciousMessage
                                                    {
                                                        MessageId = @event.MessageId,
                                                        Message = @event.Message
                                                    });

            _dbContext.Set<OngoingMessage>().Remove(@event.MessageId);
        }
    }
}