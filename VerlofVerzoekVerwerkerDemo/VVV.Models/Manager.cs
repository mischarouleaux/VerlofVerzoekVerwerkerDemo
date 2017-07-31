using System;
using System.Collections.Generic;

namespace VVV.Models
{
    public partial class Manager
    {
        public long ManagerID { get; set; }
        public long UserID { get; set; }
        public bool IsAvailable { get; set; }
        public Nullable<System.DateTime> From { get; set; }
        public Nullable<System.DateTime> Till { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}
