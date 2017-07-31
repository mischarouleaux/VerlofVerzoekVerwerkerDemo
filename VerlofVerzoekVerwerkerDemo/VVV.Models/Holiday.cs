using System;
using System.Collections.Generic;

namespace VVV.Models
{
    public partial class Holiday
    {
        public long HolidayID { get; set; }
        public string Description { get; set; }
        public System.DateTime Date { get; set; }
    }
}
