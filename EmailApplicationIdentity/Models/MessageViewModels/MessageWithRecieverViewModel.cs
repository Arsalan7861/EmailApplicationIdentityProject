﻿namespace EmailApplicationIdentity.Models.MessageViewModels
{
    public class MessageWithRecieverViewModel
    {
        public int MessageId { get; set; }
        public string Subject { get; set; }
        public string MessageDetail { get; set; }
        public DateTime SendDate { get; set; }
        public string RecieverEmail { get; set; }
        public string RecieverName { get; set; }
        public string RecieverSurname { get; set; }
        public string CategoryName { get; set; }
    }
}
