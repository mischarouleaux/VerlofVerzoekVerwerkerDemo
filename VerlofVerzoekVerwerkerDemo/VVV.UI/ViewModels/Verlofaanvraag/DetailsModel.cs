using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VVV.Models;
using PagedList;

namespace VVV.UI.ViewModels.Verlofaanvraag
{
    public class DetailsModel
    {
        public long vacreqID { get; set; }
        public long UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Reason { get; set; }
        public string ReasonRejection { get; set; }
        public bool IsActive { get; set; }
        public bool IsCommunicated { get; set; }
        public bool HasDeadlines { get; set; }
        public string BeginDate { get; set; }
        public string BeginTimeHour { get; set; }
        public string BeginTimeMinute { get; set; }
        public string EndDate { get; set; }
        public string EndTimeHour { get; set; }
        public string EndTimeMinute { get; set; }

        public string OldBeginDate { get; set; }
        public string OldBeginTimeHour { get; set; }
        public string OldBeginTimeMinute { get; set; }
        public string OldEndDate { get; set; }
        public string OldEndTimeHour { get; set; }
        public string OldEndTimeMinute { get; set; }
        public bool OldTotalDays { get; set; }

        public bool IsAvailable { get; set; }

        public bool TotalDays { get; set; }
        public bool IsAccepted { get; set; }
        public bool IsRejected { get; set; }
        public bool IsInTreatment { get; set; }

        public string ManagerName { get; set; }
        public string ManagerMail { get; set; }

        

        public double DaysRequest { get; set; }
        public int HoursRequest { get; set; }
        public int MinutesRequest { get; set; }
        public string TimeRequest { get; set; }

        public double DaysCurrent { get; set; }
        public int HoursCurrent { get; set; }
        public int MinutesCurrent { get; set; }
        public string TimeCurrent { get; set; }

        public double DaysRest { get; set; }
        public int HoursRest { get; set; }
        public int MinutesRest { get; set; }

        public IPagedList<VacationRequest> Intersections { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }

        


        //Haalt de desbetreffende data uit de controller op om weer te geven in de view.
        public DetailsModel(VacationRequest vacreq, ApplicationUser user, ApplicationUser man)
        {
            vacreqID = vacreq.VacationID;
            UserID = vacreq.UserID;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Email = user.Email;
            Reason = vacreq.Reason;
            ReasonRejection = vacreq.ReasonRejection;
            IsActive = vacreq.IsActive;
            IsCommunicated = vacreq.IsCommunicated;
            HasDeadlines = vacreq.HasDeadlines;

            ManagerName = man.FirstName + " " + man.LastName;
            ManagerMail = man.Email;

            BeginDate = vacreq.BeginDate.Day.ToString() + "/" + vacreq.BeginDate.Month.ToString() + "/" + vacreq.BeginDate.Year.ToString();

            BeginTimeHour = vacreq.BeginDate.Hour.ToString();
            if (vacreq.BeginDate.Hour < 10)
            {
                BeginTimeHour = "0" + vacreq.BeginDate.Hour.ToString();
            }
            BeginTimeMinute = vacreq.BeginDate.Minute.ToString();
            if (vacreq.BeginDate.Minute < 10)
            {
                BeginTimeMinute = "0" + vacreq.BeginDate.Minute.ToString();
            }

            EndDate = vacreq.EndDate.Day.ToString() + "/" + vacreq.EndDate.Month.ToString() + "/" + vacreq.EndDate.Year.ToString();

            EndTimeHour = vacreq.EndDate.Hour.ToString();
            if (vacreq.EndDate.Hour < 10)
            {
                EndTimeHour = "0" + vacreq.EndDate.Hour.ToString();
            }
            EndTimeMinute = vacreq.EndDate.Minute.ToString();
            if (vacreq.EndDate.Minute < 10)
            {
                EndTimeMinute = "0" + vacreq.EndDate.Minute.ToString();
            }


            DaysRequest = Convert.ToDouble(vacreq.TotalMinutes / 480);
            HoursRequest = (vacreq.TotalMinutes % 480) / 60;
            MinutesRequest = (vacreq.TotalMinutes % 480) % 60;

            IsAccepted = vacreq.IsApproved;
            IsRejected = vacreq.IsRejected;
            IsInTreatment = vacreq.IsInTreatment;
            TotalDays = vacreq.IsTotalDays;

        }

        public DetailsModel(VacationRequest vacreq, ApplicationUser user, ApplicationUser man, int mutvac)
        {
            Page = 1;
            PageSize = Properties.Settings.Default.PageSize;

            vacreqID = vacreq.VacationID;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Email = user.Email;
            Reason = vacreq.Reason;
            ReasonRejection = vacreq.ReasonRejection;
            IsActive = vacreq.IsActive;
            IsCommunicated = vacreq.IsCommunicated;
            HasDeadlines = vacreq.HasDeadlines;
            ManagerName = man.FirstName + " " + man.LastName;

            //Nieuwe datum
            BeginDate = vacreq.BeginDate.Day.ToString() + "/" + vacreq.BeginDate.Month.ToString() + "/" + vacreq.BeginDate.Year.ToString();

            BeginTimeHour = vacreq.BeginDate.Hour.ToString();
            if (vacreq.BeginDate.Hour < 10)
            {
                BeginTimeHour = "0" + vacreq.BeginDate.Hour.ToString();
            }
            BeginTimeMinute = vacreq.BeginDate.Minute.ToString();
            if (vacreq.BeginDate.Minute < 10)
            {
                BeginTimeMinute = "0" + vacreq.BeginDate.Minute.ToString();
            }

            EndDate = vacreq.EndDate.Day.ToString() + "/" + vacreq.EndDate.Month.ToString() + "/" + vacreq.EndDate.Year.ToString();

            EndTimeHour = vacreq.EndDate.Hour.ToString();
            if (vacreq.EndDate.Hour < 10)
            {
                EndTimeHour = "0" + vacreq.EndDate.Hour.ToString();
            }
            EndTimeMinute = vacreq.EndDate.Minute.ToString();
            if (vacreq.EndDate.Minute < 10)
            {
                EndTimeMinute = "0" + vacreq.EndDate.Minute.ToString();
            }

            //Oude datum
            OldBeginDate = vacreq.OldBeginDate.Value.Day.ToString() + "/" + vacreq.OldBeginDate.Value.Month.ToString() + "/" + vacreq.OldBeginDate.Value.Year.ToString();

            OldBeginTimeHour = vacreq.OldBeginDate.Value.Hour.ToString();
            if (vacreq.OldBeginDate.Value.Hour < 10)
            {
                OldBeginTimeHour = "0" + vacreq.OldBeginDate.Value.Hour.ToString();
            }
            OldBeginTimeMinute = vacreq.OldBeginDate.Value.Minute.ToString();
            if (vacreq.OldBeginDate.Value.Minute < 10)
            {
                OldBeginTimeMinute = "0" + vacreq.OldBeginDate.Value.Minute.ToString();
            }

            OldEndDate = vacreq.OldEndDate.Value.Day.ToString() + "/" + vacreq.OldEndDate.Value.Month.ToString() + "/" + vacreq.OldEndDate.Value.Year.ToString();

            OldEndTimeHour = vacreq.OldEndDate.Value.Hour.ToString();
            if (vacreq.OldEndDate.Value.Hour < 10)
            {
                OldEndTimeHour = "0" + vacreq.OldEndDate.Value.Hour.ToString();
            }
            OldEndTimeMinute = vacreq.OldEndDate.Value.Minute.ToString();
            if (vacreq.OldEndDate.Value.Minute < 10)
            {
                OldEndTimeMinute = "0" + vacreq.OldEndDate.Value.Minute.ToString();
            }


            //Aantal dagen, uren & minutes van de verlofaanvraag
            DaysRequest = Convert.ToDouble(vacreq.TotalMinutes / 480);
            HoursRequest = (vacreq.TotalMinutes % 480) / 60;
            MinutesRequest = (vacreq.TotalMinutes % 480) % 60;
            TimeRequest = (vacreq.TotalMinutes / 60).ToString() + " uur (" + (vacreq.TotalMinutes / 480).ToString() + " dagen & " + ((vacreq.TotalMinutes % 480) / 60).ToString() + " uur) " + ((vacreq.TotalMinutes % 480) % 60).ToString() + " minuten";

            //Huidige verlofdagen van de gebruiker 
            DaysCurrent = Convert.ToDouble(mutvac / 480);
            HoursCurrent = (mutvac % 480) / 60;
            MinutesCurrent = (mutvac % 480) % 60;
            TimeCurrent = (mutvac / 60).ToString() + " uur (" + (mutvac / 480).ToString() + " dagen & " + ((mutvac % 480) / 60).ToString() + " uur) " + ((mutvac % 480) % 60).ToString() + " minuten";

            //Verlofdagen resterend naar het accepteren van deze aanvragen
            int TotalMinutes = mutvac - vacreq.TotalMinutes;
            DaysRest = Convert.ToDouble(TotalMinutes / 480);
            HoursRest = (TotalMinutes % 480) / 60;
            MinutesRest = (TotalMinutes % 480) % 60;

            IsAccepted = vacreq.IsApproved;
            IsRejected = vacreq.IsRejected;
            IsInTreatment = vacreq.IsInTreatment;
            TotalDays = vacreq.IsTotalDays;

            //Verlofverzoeken die in dezelfde periode plaatsvinden
            Intersections = new List<VacationRequest>().ToPagedList(this.Page, this.PageSize);



        }

        public DetailsModel(VacationRequest vacreq, ApplicationUser user, int mutvac, bool ManagerIsAvailable)
        {
            Page = 1;
            PageSize = Properties.Settings.Default.PageSize;

            IsAvailable = ManagerIsAvailable;
            vacreqID = vacreq.VacationID;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Email = user.Email;
            Reason = vacreq.Reason;
            ReasonRejection = vacreq.ReasonRejection;
            IsActive = vacreq.IsActive;
            IsCommunicated = vacreq.IsCommunicated;
            HasDeadlines = vacreq.HasDeadlines;

            BeginDate = vacreq.BeginDate.Day.ToString() + "/"+ vacreq.BeginDate.Month.ToString() + "/" + vacreq.BeginDate.Year.ToString();

            BeginTimeHour = vacreq.BeginDate.Hour.ToString();
            if (vacreq.BeginDate.Hour < 10)
            {
                BeginTimeHour = "0" + vacreq.BeginDate.Hour.ToString();
            }
            BeginTimeMinute = vacreq.BeginDate.Minute.ToString();
            if (vacreq.BeginDate.Minute < 10)
            {
                BeginTimeMinute = "0" + vacreq.BeginDate.Minute.ToString();
            }

            EndDate = vacreq.EndDate.Day.ToString() + "/" + vacreq.EndDate.Month.ToString() + "/" + vacreq.EndDate.Year.ToString();

            EndTimeHour = vacreq.EndDate.Hour.ToString();
            if (vacreq.EndDate.Hour < 10)
            {
                EndTimeHour = "0" + vacreq.EndDate.Hour.ToString();
            }
            EndTimeMinute = vacreq.EndDate.Minute.ToString();
            if (vacreq.EndDate.Minute < 10)
            {
                EndTimeMinute = "0" + vacreq.EndDate.Minute.ToString();
            }

            

            //Aantal dagen, uren & minutes van de verlofaanvraag
            DaysRequest = Convert.ToDouble(vacreq.TotalMinutes / 480);
            HoursRequest = (vacreq.TotalMinutes % 480) / 60;
            MinutesRequest = (vacreq.TotalMinutes % 480) % 60;

            //Huidige verlofdagen van de gebruiker 
            DaysCurrent = Convert.ToDouble(mutvac / 480);
            HoursCurrent = (mutvac % 480) / 60;
            MinutesCurrent = (mutvac % 480) % 60;

            //Verlofdagen resterend naar het accepteren van deze aanvragen
            int TotalMinutes = mutvac - vacreq.TotalMinutes;
            DaysRest = Convert.ToDouble(TotalMinutes / 480);
            HoursRest = (TotalMinutes % 480) / 60;
            MinutesRest = (TotalMinutes % 480) % 60;

            IsAccepted = vacreq.IsApproved;
            IsRejected = vacreq.IsRejected;
            IsInTreatment = vacreq.IsInTreatment;
            TotalDays = vacreq.IsTotalDays;

            //Verlofverzoeken die in dezelfde periode plaatsvinden
            Intersections = new List<VacationRequest>().ToPagedList(this.Page, this.PageSize);



        }
    }
}