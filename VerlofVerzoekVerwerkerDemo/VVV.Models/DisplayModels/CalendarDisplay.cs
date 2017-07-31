using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VVV.Models.DisplayModels
{
    public class CalendarDisplay
    {
        public string id { get; set; }
        public string title { get; set; }
        public bool allDay { get; set; }
        public DateTime start { get; set; }
        public DateTime end { get; set; }
        public string url { get; set; }
    }
}
