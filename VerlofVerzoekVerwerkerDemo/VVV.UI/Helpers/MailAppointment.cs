using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Text;
using VVV.Interfaces.Services;


namespace VVV.UI.Helpers
{
    public class Appointment
    {
        public string CreateIcs(string subject, string location, DateTime startDate, DateTime endDate)
        {
            StringBuilder sb = new StringBuilder();
            
            sb.AppendLine("BEGIN:VCALENDAR");
            sb.AppendLine("PRODID:-//Schedule a meeting");
            sb.AppendLine("VERSION:2.0");
            sb.AppendLine("METHOD:REQUEST");
            sb.AppendLine("BEGIN:VEVENT");


            //sb.AppendLine(string.Format("DTSTAMP:{0:yyyyMMddTHHmmssZ}", DateTime.Now.AddHours(-1)));
            sb.AppendLine(string.Format("DTSTART:{0:yyyyMMddTHHmmssZ}", startDate.AddHours(-1)));
            //sb.AppendLine(string.Format("DTSTAMP:{0:yyyyMMddTHHmmssZ}", DateTime.Now));
            sb.AppendLine(string.Format("DTEND:{0:yyyyMMddTHHmmssZ}", endDate.AddHours(-1)));

            sb.AppendLine("SUMMARY:" + subject);
            sb.AppendLine("LOCATION:" + location);
            sb.AppendLine("END:VEVENT");
            sb.AppendLine("END:VCALENDAR");
            
            return sb.ToString();

        }
    }
}
