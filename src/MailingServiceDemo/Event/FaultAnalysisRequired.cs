﻿using System;
using System.Net.Mail;

namespace MailingServiceDemo.Event
{
    public class FaultAnalysisRequired
    {
        public Guid MessageId { get; set; }

        public MailMessage Message { get; set; }
    }
}