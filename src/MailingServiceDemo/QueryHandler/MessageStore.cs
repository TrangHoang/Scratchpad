﻿using System.Linq;
using MailingServiceDemo.Database;
using MailingServiceDemo.Query;
using MailingServiceDemo.ReadModel;
using Reusables.Cqrs;

namespace MailingServiceDemo.QueryHandler
{
    public class MessageStore : IQueryHandler<MostUrgentMessage, OutboxMessage>
    {
        private readonly IDbContext _database;

        public MessageStore(IDbContext database)
        {
            _database = database;
        }

        public OutboxMessage Handle(MostUrgentMessage query)
        {
            return _database.Set<OutboxMessage>()
                            .OrderByDescending(message => message.Priority)
                            .ThenBy(message => message.QueuedAt)
                            .First();
        }
    }
}
