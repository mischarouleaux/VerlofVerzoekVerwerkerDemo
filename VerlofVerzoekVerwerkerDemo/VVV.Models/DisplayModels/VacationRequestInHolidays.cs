using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VVV.Models.DisplayModels
{
    public class VacationRequestInHolidays
    {
        public long UserID { get; set; }
        public long VacationID { get; set; }
        public int TotalMinutes { get; set; }
    }
}
