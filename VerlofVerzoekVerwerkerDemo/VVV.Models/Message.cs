using System;
using System.Collections.Generic;

namespace VVV.Models
{
    public partial class Message
    {
        public long MessageID { get; set; }
        public long SenderID { get; set; }
        public long ReceiverID { get; set; }
        public long VacationRequestID { get; set; }
        public string Subject { get; set; }
        public string MessageText { get; set; }
        public bool IsRead { get; set; }
        public System.DateTime DateSend { get; set; }
        public bool IsDeleted { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
        public virtual VacationRequest VacationRequest { get; set; }
    }
}
