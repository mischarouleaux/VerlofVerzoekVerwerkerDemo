using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VVV.Models;
using PagedList;

namespace VVV.UI.ViewModels.Verlofaanvraag
{
    public class DeleteModel
    { 
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Reason { get; set; }
        public DateTime BeginDate { get; set; }
        public TimeSpan BeginTime { get; set; }
        public DateTime EndDate { get; set; }
        public TimeSpan EndTime { get; set; }

        public string IsAccepted { get; set; }

        public double Days { get; set; }
        public int Hours { get; set; }
        public int Minutes { get; set; }


        //Haalt de desbetreffende data uit de controller op om weer te geven in de view.
        public DeleteModel(VacationRequest vacreq, ApplicationUser user)
        {
            FirstName = user.FirstName;
            LastName = user.LastName;
            Email = user.Email;
            Reason = vacreq.Reason;
            BeginDate = vacreq.BeginDate.Date;
            BeginTime = new TimeSpan(vacreq.BeginDate.Hour, vacreq.BeginDate.Minute, 00);
            EndDate = vacreq.EndDate.Date;
            EndTime = new TimeSpan(vacreq.EndDate.Hour, vacreq.EndDate.Minute, 00);
            Days = Convert.ToDouble(vacreq.TotalMinutes / 480);
            Hours = (vacreq.TotalMinutes % 480) / 60 ;
            Minutes = (vacreq.TotalMinutes % 480) % 60;

            if (vacreq.IsApproved == true)
            {
                IsAccepted = "Ja";
            }

            else
            {
                IsAccepted = "Nee";
            }
            
        }
    }
}