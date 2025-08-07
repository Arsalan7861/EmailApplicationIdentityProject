﻿using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace EmailApplicationIdentity.Entities
{
    public class Message
    {
        public int MessageId { get; set; }
        public string? SenderEmail { get; set; }
        public string ReceiverEmail { get; set; }
        public string Subject { get; set; }
        public DateTime SendDate { get; set; }
        public string MesssageDetail { get; set; }
        public bool IsRead { get; set; }
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
    }
}
