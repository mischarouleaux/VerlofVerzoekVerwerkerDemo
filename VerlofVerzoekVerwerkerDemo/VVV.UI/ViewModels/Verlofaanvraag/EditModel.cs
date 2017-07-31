using Foolproof;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using VVV.Business.Identity;
using VVV.Models;
using VVV.UI.Helpers;
using PagedList;


namespace VVV.UI.ViewModels.Verlofaanvraag
{
    
    public class EditModel
    {
        public long VacationID { get; set; }
        public long UserId { get; set; }
        public string Name { get; set; }

        [Required(ErrorMessage = "Voer een reden in.")]
        public string Reason { get; set; }

        public string RejectionReason { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime BeginDate { get; set; }
        public DateTime OldBeginDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime EndDate { get; set; }
        public DateTime OldEndDate { get; set; }

        [Required]
        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true)]
        public TimeSpan BeginTime { get; set; }

        [Required]
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:hh\\:mm}", ApplyFormatInEditMode = true)]
        public TimeSpan EndTime { get; set; }

        [Required]
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:hh\\:mm}", ApplyFormatInEditMode = true)]
        public TimeSpan UserBeginTime { get; set; }

        [Required]
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:hh\\:mm}", ApplyFormatInEditMode = true)]
        public TimeSpan UserEndTime { get; set; }

        [Required]
        public bool HasBreak { get; set; }
        [Required]
        [DataType(DataType.Duration)]
        public int BreakTime { get; set; }


        [Required]
        public bool IsCommunicated { get; set; }
        [Required]
        public bool HasDeadlines { get; set; }

        public bool CompleteDays { get; set; }
        public bool OldCompleteDays { get; set; }
        public int OldTotalMinutes { get; set; }

        public string Time { get; set; }
        public double Days { get; set; }
        public int Hours { get; set; }
        public int Minutes { get; set; }

        public ApplicationUser CurrentManager { get; set; }

        public bool HasSecondManager { get; set; }
        public ApplicationUser SecondManager { get; set; }

        public int Page { get; set; }
        public int PageSize { get; set; }
        public System.Uri previousUrl { get; set; }



        //Zorgt ervoor dat data in de view wordt geladen van het desbetreffende verlofaanvraag
        public EditModel(VacationRequest vacreq)
        {
            VacationID = vacreq.VacationID;
            UserId = vacreq.UserID;
            Reason = vacreq.Reason;
            BeginDate = vacreq.BeginDate;
            EndDate = vacreq.EndDate;

            
         

        }

        public object GetRouteValues(bool resetpage = false)
        {
            return new
            {
                page = resetpage ? 1 : Page,
                pagesize = PageSize
            };
        }

        public EditModel(int time, ApplicationUser currentmanager)
        {
            Page = 1;
            PageSize = Properties.Settings.Default.PageSize;
            CurrentManager = currentmanager;
            Time = (time / 60).ToString() + " uur" + " (" + (time / 480).ToString() + " dagen" + " & " + ((time % 480) / 60).ToString() + " uur) " + ((time % 480) % 60).ToString() + " minuten";
            BeginDate = DateTime.Now;
            BeginTime = new TimeSpan(08, 00, 00);
            EndDate = DateTime.Now;
            EndTime = new TimeSpan(09, 00, 00);

            HasSecondManager = false;

            
        }

        public EditModel(int time, ApplicationUser manager, ApplicationUser secondmanager)
        {
            Page = 1;
            PageSize = Properties.Settings.Default.PageSize;
            CurrentManager = manager;
            HasSecondManager = true;
            SecondManager = secondmanager;
            BeginDate = DateTime.Now;
            BeginTime = new TimeSpan(08, 00, 00);
            UserBeginTime = new TimeSpan(08, 00, 00);
            EndDate = DateTime.Now;
            EndTime = new TimeSpan(09, 00, 00);
            UserEndTime = new TimeSpan(16, 00, 00);
            Time = (time / 60).ToString() + " uur" + " (" + (time / 480).ToString() + " dagen" + " & " + ((time % 480) / 60).ToString() + " uur) " + ((time % 480) % 60).ToString() + " minuten";
            Days = Convert.ToDouble(time / 480);
            Hours = (time % 480) / 60;
            Minutes = (time % 480) % 60;

            CompleteDays = true;
            
        }

        public EditModel(int time, ApplicationUser currentmanager, VacationRequest vacreq, ApplicationUser user)
        {
            Page = 1;
            PageSize = Properties.Settings.Default.PageSize;
            CurrentManager = currentmanager;
            Days = Convert.ToDouble(time / 480);
            Hours = (time % 480) / 60;
            Minutes = (time % 480) % 60;
            Reason = vacreq.Reason;
            BeginDate = vacreq.BeginDate.Date;
            BeginTime = new TimeSpan(vacreq.BeginDate.Hour, vacreq.BeginDate.Minute, vacreq.BeginDate.Second);
            EndDate = vacreq.EndDate.Date;
            EndTime = new TimeSpan(vacreq.EndDate.Hour, vacreq.EndDate.Minute, vacreq.EndDate.Second);
            UserId = vacreq.UserID;
            VacationID = vacreq.VacationID;
            CompleteDays = vacreq.IsTotalDays;
            Name = user.FirstName + " " + user.LastName;
            IsCommunicated = vacreq.IsCommunicated;
            HasDeadlines = vacreq.HasDeadlines;

            UserBeginTime = new TimeSpan(vacreq.BeginOfDay.Value.Hour, vacreq.BeginOfDay.Value.Minute, 00);
            UserEndTime = new TimeSpan(vacreq.EndOfDay.Value.Hour, vacreq.EndOfDay.Value.Minute, 00);

            OldBeginDate = vacreq.BeginDate;
            OldEndDate = vacreq.EndDate;
            OldCompleteDays = vacreq.IsTotalDays;
            OldTotalMinutes = vacreq.TotalMinutes;
            
            if (vacreq.BeginOfDay == null && vacreq.EndOfDay == null)
            {
                UserBeginTime = new TimeSpan(08, 00, 00);
                UserEndTime = new TimeSpan(16, 00, 00);
            }
            else
            {
                UserBeginTime = new TimeSpan(vacreq.BeginOfDay.Value.Hour, vacreq.BeginOfDay.Value.Minute, 00);
                UserEndTime = new TimeSpan(vacreq.EndOfDay.Value.Hour, vacreq.EndOfDay.Value.Minute, 00);
            }
        }

        public EditModel() { }

        public VacationRequest VacationRequestRejection(VacationRequest vacreq)
        {
            vacreq.ReasonRejection = RejectionReason;
            vacreq.AssessedByID = SecurityHelper.GetUserId();
            vacreq.IsApproved = false;
            vacreq.IsInTreatment = false;
            vacreq.IsRejected = true;

            return vacreq;
        }

        public VacationRequest ToEditVacationRequest(VacationRequest vacreq = null)
        {
            
            if (vacreq == null)
            {
                vacreq = new VacationRequest();
                vacreq.UserID = UserId;
                vacreq.VacationID = VacationID;
                vacreq.ReasonRejection = RejectionReason;
                vacreq.DateSubmission = DateTime.Now;
                vacreq.IsRejected = false;
                vacreq.IsApproved = false;
                vacreq.IsInTreatment = false;
                vacreq.IsActive = true;
                vacreq.DateCreated = DateTime.Now;                
                vacreq.CreateUser = SecurityHelper.GetUserId();               
                vacreq.IsTotalDays = CompleteDays;

                
            }

            if (vacreq.IsApproved == false && vacreq.IsRejected == false && vacreq.IsInTreatment == false)
            {
                vacreq.UserID = UserId;
                vacreq.VacationID = VacationID;
                vacreq.ReasonRejection = RejectionReason;
                vacreq.DateSubmission = DateTime.Now;
                vacreq.IsRejected = false;
                vacreq.IsApproved = false;
                vacreq.IsInTreatment = false;
                vacreq.IsActive = true;
                vacreq.DateCreated = DateTime.Now;
                vacreq.CreateUser = SecurityHelper.GetUserId();
                vacreq.IsTotalDays = CompleteDays;

                
                vacreq.AssessedByID = SecurityHelper.GetUserId();
            }
            else
            {
                vacreq.IsRejected = false;
                vacreq.IsApproved = false;
                vacreq.IsInTreatment = true;
                vacreq.IsTotalDays = CompleteDays;
            }
            vacreq.ChangeUser = SecurityHelper.GetUserId();
            vacreq.DateChanged = DateTime.Now;
            vacreq.BeginOfDay = BeginDate.Date.Add(UserBeginTime);
            vacreq.EndOfDay = EndDate.Date.Add(UserEndTime);

            if (CompleteDays == true)
            {
                TimeSpan BeginTime = new TimeSpan(9, 00, 0);                
                vacreq.BeginDate = BeginDate.Add(BeginTime);


                TimeSpan EndTime = new TimeSpan(17, 00, 0);                
                vacreq.EndDate = EndDate.Add(EndTime);

                vacreq.BeginOfDay = BeginDate;
                vacreq.EndOfDay = EndDate;
            }

            //De tijd wordt toegevoegd aan de datetime die zal worden opgeslagen
            else
                {
                BeginDate = BeginDate.Add(BeginTime);
                vacreq.BeginDate = BeginDate;

                EndDate = EndDate.Add(EndTime);
                vacreq.EndDate = EndDate;

                vacreq.BeginOfDay = BeginDate.Date.Add(UserBeginTime);
                vacreq.EndOfDay = EndDate.Date.Add(UserEndTime);
            }

            //Verschil in dagen wordt berekend
            TimeSpan DifferenceInDays = EndDate - BeginDate;
            int TotalDays = DifferenceInDays.Days;
            if (DifferenceInDays.Hours == 8)
            {
                TotalDays = TotalDays + 1;
            }

            if (CompleteDays == true)
            {
                TotalDays = TotalDays + 1;
            }

            //Het totaal aantal minuten in de verlofaanvraag worden berekend (als checkbox "hele dagen" is aangevinkt).
            if (CompleteDays == true)
            {
                int CountHours = 0;
                DateTime t = BeginDate;
                for (int i = 1; i <= TotalDays; i++)
                {
                    if (t.DayOfWeek == DayOfWeek.Monday || t.DayOfWeek == DayOfWeek.Tuesday || t.DayOfWeek == DayOfWeek.Wednesday || t.DayOfWeek == DayOfWeek.Thursday || t.DayOfWeek == DayOfWeek.Friday)
                    {
                        CountHours = CountHours + 8;
                    }

                    if (t.DayOfWeek == DayOfWeek.Saturday || t.DayOfWeek == DayOfWeek.Sunday)
                    {
                        CountHours = CountHours + 0;
                    }
                    t = t.AddDays(1);

                }
                int TotalMinutes = CountHours * 60;
                vacreq.TotalMinutes = TotalMinutes;
            }

            if (CompleteDays == false)
            {
                DifferenceInDays = EndDate.Date - BeginDate.Date;
                

                if (DifferenceInDays.Days < 1)
                {
                    int DifferenceHours = 0;
                    int DifferenceMinutes = 0;

                    if (BeginTime < EndTime)
                    {
                        DifferenceHours = EndTime.Hours - BeginTime.Hours;

                        if (EndTime.Minutes == BeginTime.Minutes)
                        {
                            DifferenceMinutes = 0;
                        }

                        if (BeginTime.Minutes < EndTime.Minutes)
                        {
                            DifferenceMinutes = EndTime.Minutes - BeginTime.Minutes;
                        }

                        if (BeginTime.Minutes > EndTime.Minutes)
                        {
                            if (EndTime.Minutes == 0)
                            {
                                DifferenceMinutes = 60 - BeginTime.Minutes;
                                DifferenceHours = DifferenceHours - 1;
                            }

                            if (BeginTime.Minutes > EndTime.Minutes && EndTime.Minutes != 0)
                            {
                                DifferenceMinutes = 60 - (BeginTime.Minutes - EndTime.Minutes);
                                DifferenceHours = DifferenceHours - 1;
                            }
                        }


                        vacreq.TotalMinutes = (DifferenceHours * 60) + DifferenceMinutes;
                    }
                }

                if (DifferenceInDays.Days >= 1 && DifferenceInDays.Days < 2)
                {
                    int DifferenceHours = 0;
                    int DifferenceMinutes = 0;
                    int MinutesFirstDay = 0;
                    int MinutesSecondDay = 0;

                    DifferenceHours = (UserEndTime.Hours - BeginTime.Hours) + (EndTime.Hours - UserBeginTime.Hours);

                    if (UserEndTime.Minutes == BeginTime.Minutes)
                    {
                        MinutesFirstDay = 0;
                    }

                    if (UserEndTime.Minutes > BeginTime.Minutes)
                    {
                        MinutesFirstDay = (UserEndTime.Minutes - BeginTime.Minutes);
                    }

                    if (UserEndTime.Minutes < BeginTime.Minutes)
                    {
                        if (UserEndTime.Minutes == 0)
                        {
                            MinutesFirstDay = 60 - BeginTime.Minutes;
                            DifferenceHours = DifferenceHours - 1;
                        }

                        if (UserEndTime.Minutes != 0)
                        {
                            MinutesFirstDay = 60 - (BeginTime.Minutes - UserEndTime.Minutes);
                            DifferenceHours = DifferenceHours - 1;
                        }
                    }

                    if (EndTime.Minutes == UserBeginTime.Minutes)
                    {
                        MinutesSecondDay = 0;
                    }

                    if (EndTime.Minutes > UserBeginTime.Minutes)
                    {
                        MinutesSecondDay = (EndTime.Minutes - UserBeginTime.Minutes);
                    }

                    if (EndTime.Minutes < UserBeginTime.Minutes)
                    {
                        if (EndTime.Minutes == 0)
                        {
                            MinutesSecondDay = 60 - UserBeginTime.Minutes;
                            DifferenceHours = DifferenceHours - 1;
                        }

                        if (EndTime.Minutes != 0)
                        {
                            MinutesSecondDay = 60 - (UserBeginTime.Minutes - EndTime.Minutes);
                            DifferenceHours = DifferenceHours - 1;
                        }
                    }

                    DifferenceMinutes = MinutesFirstDay + MinutesSecondDay;
                    if (DifferenceMinutes > 59)
                    {
                        DifferenceHours = DifferenceHours + 1;
                        DifferenceMinutes = DifferenceMinutes - 60;
                    }

                    vacreq.TotalMinutes = (DifferenceHours * 60) + DifferenceMinutes;

                }

                if (DifferenceInDays.Days >= 2)
                {
                    int totaldaysbetween = 0;
                    DateTime startDate = BeginDate;
                    for (int i = 1; i <= DifferenceInDays.Days; i++)
                    {
                        if (startDate.DayOfWeek == DayOfWeek.Monday || startDate.DayOfWeek == DayOfWeek.Tuesday || startDate.DayOfWeek == DayOfWeek.Wednesday || startDate.DayOfWeek == DayOfWeek.Thursday || startDate.DayOfWeek == DayOfWeek.Friday)
                        {
                            totaldaysbetween = totaldaysbetween + 1;
                        }

                        startDate = startDate.AddDays(1);
                    }

                    totaldaysbetween = totaldaysbetween - 1;


                    int DifferenceHours = 0;
                    int DifferenceMinutes = 0;
                    int MinutesFirstDay = 0;
                    int MinutesSecondDay = 0;

                    DifferenceHours = (UserEndTime.Hours - BeginTime.Hours) + (EndTime.Hours - UserBeginTime.Hours);

                    if (UserEndTime.Minutes == BeginTime.Minutes)
                    {
                        MinutesFirstDay = 0;
                    }

                    if (UserEndTime.Minutes > BeginTime.Minutes)
                    {
                        MinutesFirstDay = (UserEndTime.Minutes - BeginTime.Minutes);
                    }

                    if (UserEndTime.Minutes < BeginTime.Minutes)
                    {
                        if (UserEndTime.Minutes == 0)
                        {
                            MinutesFirstDay = 60 - BeginTime.Minutes;
                            DifferenceHours = DifferenceHours - 1;
                        }

                        if (UserEndTime.Minutes != 0)
                        {
                            MinutesFirstDay = 60 - (BeginTime.Minutes - UserEndTime.Minutes);
                            DifferenceHours = DifferenceHours - 1;
                        }
                    }

                    if (EndTime.Minutes == UserBeginTime.Minutes)
                    {
                        MinutesSecondDay = 0;
                    }

                    if (EndTime.Minutes > UserBeginTime.Minutes)
                    {
                        MinutesSecondDay = (EndTime.Minutes - UserBeginTime.Minutes);
                    }

                    if (EndTime.Minutes < UserBeginTime.Minutes)
                    {
                        if (EndTime.Minutes == 0)
                        {
                            MinutesSecondDay = 60 - UserBeginTime.Minutes;
                            DifferenceHours = DifferenceHours - 1;
                        }

                        if (EndTime.Minutes != 0)
                        {
                            MinutesSecondDay = 60 - (UserBeginTime.Minutes - EndTime.Minutes);
                            DifferenceHours = DifferenceHours - 1;
                        }
                    }

                    DifferenceMinutes = MinutesFirstDay + MinutesSecondDay;
                    if (DifferenceMinutes > 59)
                    {
                        DifferenceHours = DifferenceHours + 1;
                        DifferenceMinutes = DifferenceMinutes - 60;
                    }

                    var totalhoursindays = DifferenceHours;
                    TimeSpan DifferenceBeginEndTime = (new TimeSpan(vacreq.EndOfDay.Value.Hour, vacreq.EndOfDay.Value.Minute, 00)) - (new TimeSpan(vacreq.BeginOfDay.Value.Hour, vacreq.BeginOfDay.Value.Minute, 00));

                    vacreq.TotalMinutes = (totalhoursindays * 60) + DifferenceMinutes + (Convert.ToInt32(DifferenceBeginEndTime.TotalMinutes) * totaldaysbetween);
                }


            }
            vacreq.Reason = Reason;
            if (vacreq.IsTotalDays == true)
            {
                vacreq.BeginDate = BeginDate.Add(new TimeSpan(08, 00, 00));
                vacreq.EndDate = EndDate.Add(new TimeSpan(18, 00, 00));
            }
            vacreq.IsCommunicated = IsCommunicated;
            vacreq.HasDeadlines = HasDeadlines;

            return vacreq;
        }

        //Haalt de data (verlofaanvraag) op uit de View, geeft het door aan de controller
        public VacationRequest ToVacationRequest(VacationRequest vacreq = null)
        {
            if (vacreq == null)
            {
                vacreq = new VacationRequest();
                vacreq.UserID = SecurityHelper.GetUserId();
                vacreq.ReasonRejection = "";
                vacreq.DateSubmission = DateTime.Now;
                vacreq.IsRejected = false;
                vacreq.IsApproved = false;
                vacreq.IsInTreatment = true;
                vacreq.IsActive = true;
                vacreq.DateCreated = DateTime.Now;
                vacreq.DateChanged = DateTime.Now;
                vacreq.CreateUser = SecurityHelper.GetUserId();
                vacreq.ChangeUser = SecurityHelper.GetUserId();
                vacreq.IsTotalDays = CompleteDays;
                
                
            }

            //Wanneer de checkbox "hele dagen" is aangevinkt, wordt er uitgegaan van 8 uur verlof per opgegeven dag. De begintijd en eindtijd wordt vastgezet
            if(CompleteDays == true)
            {
                TimeSpan BeginTime = new TimeSpan(9, 00, 0);
                
                vacreq.BeginDate = BeginDate.Add(BeginTime);


                TimeSpan EndTime = new TimeSpan(17, 00, 0);
                
                vacreq.EndDate = EndDate.Add(EndTime);
                vacreq.BeginOfDay = BeginDate;
                vacreq.EndOfDay = EndDate;
            }

            //De tijd wordt toegevoegd aan de datetime die zal worden opgeslagen
            else
            {
                BeginDate = BeginDate.Add(BeginTime);
                vacreq.BeginDate = BeginDate;

                EndDate = EndDate.Add(EndTime);
                vacreq.EndDate = EndDate;

                vacreq.BeginOfDay = BeginDate.Date.Add(UserBeginTime);
                vacreq.EndOfDay = EndDate.Date.Add(UserEndTime);
            }

            //Verschil in dagen wordt berekend
            TimeSpan DifferenceDayInHours = UserEndTime - UserBeginTime;
            if (DifferenceDayInHours.TotalMinutes > 480)
            {
                int minutesoverload = (DifferenceDayInHours.Minutes % 480) * -1 ;
                UserEndTime = UserEndTime.Add(new TimeSpan(0, minutesoverload, 0));
               
            }
            TimeSpan differencehours = EndTime - BeginTime;
            if (differencehours.TotalMinutes > 480)
            {
                int minutesoverload = (differencehours.Minutes % 480) * -1;
                EndTime = EndTime.Add(new TimeSpan(0, minutesoverload, 0));
            }
            TimeSpan DifferenceInDays = EndDate - BeginDate;
            int TotalDays = DifferenceInDays.Days;
            if (DifferenceInDays.Hours == 8)
            {
                TotalDays = TotalDays + 1;
            }

            if (CompleteDays == true)
            {
                TotalDays = TotalDays + 1;
            }
            
            //Het totaal aantal minuten in de verlofaanvraag worden berekend (als checkbox "hele dagen" is aangevinkt).
            if (CompleteDays == true)
            {
                int CountHours = 0;
                DateTime t = BeginDate;
                for (int i = 1; i <= TotalDays; i++)
                {
                    if (t.DayOfWeek == DayOfWeek.Monday || t.DayOfWeek == DayOfWeek.Tuesday || t.DayOfWeek == DayOfWeek.Wednesday || t.DayOfWeek == DayOfWeek.Thursday || t.DayOfWeek == DayOfWeek.Friday)
                    {
                        CountHours = CountHours + 8;
                    }
            
                    if (t.DayOfWeek == DayOfWeek.Saturday || t.DayOfWeek == DayOfWeek.Sunday)
                    {
                        CountHours = CountHours + 0;
                    }
                    t = t.AddDays(1);
            
                }
                int TotalMinutes = CountHours * 60;
                vacreq.TotalMinutes = TotalMinutes;
            }

            if (CompleteDays == false)
            {
                DifferenceInDays = EndDate.Date - BeginDate.Date;
                

                if (DifferenceInDays.Days < 1)
                {
                    int DifferenceHours = 0;
                    int DifferenceMinutes = 0;

                    if (BeginTime < EndTime)
                    {
                        DifferenceHours = EndTime.Hours - BeginTime.Hours;

                        if (EndTime.Minutes == BeginTime.Minutes)
                        {
                            DifferenceMinutes = 0;
                        }

                        if (BeginTime.Minutes < EndTime.Minutes)
                        {
                            DifferenceMinutes = EndTime.Minutes - BeginTime.Minutes;
                        }

                        if (BeginTime.Minutes > EndTime.Minutes)
                        {
                            if (EndTime.Minutes == 0 )
                            {
                                DifferenceMinutes = 60 - BeginTime.Minutes;
                                DifferenceHours = DifferenceHours - 1;
                            }

                            if (BeginTime.Minutes > EndTime.Minutes && EndTime.Minutes != 0)
                            {
                                DifferenceMinutes = 60 - (BeginTime.Minutes - EndTime.Minutes);
                                DifferenceHours = DifferenceHours - 1;
                            }
                        }


                        vacreq.TotalMinutes = (DifferenceHours * 60) + DifferenceMinutes;
                    }

                    if (vacreq.TotalMinutes > 480)
                    {
                        vacreq.TotalMinutes = 480;
                    }

                }

                if (DifferenceInDays.Days >= 1 && DifferenceInDays.Days < 2)
                {
                    int DifferenceHours = 0;
                    int DifferenceMinutes = 0;
                    int MinutesFirstDay = 0;
                    int MinutesSecondDay = 0;

                    DifferenceHours = (UserEndTime.Hours - BeginTime.Hours) + (EndTime.Hours - UserBeginTime.Hours);

                    if (UserEndTime.Minutes == BeginTime.Minutes)
                    {
                        MinutesFirstDay = 0;
                    }

                    if (UserEndTime.Minutes > BeginTime.Minutes)
                    {
                        MinutesFirstDay = (UserEndTime.Minutes - BeginTime.Minutes);
                    }

                    if (UserEndTime.Minutes < BeginTime.Minutes)
                    {
                        if (UserEndTime.Minutes == 0)
                        {
                            MinutesFirstDay = 60 - BeginTime.Minutes;
                            DifferenceHours = DifferenceHours - 1;
                        }

                        if (UserEndTime.Minutes != 0)
                        {
                            MinutesFirstDay = 60 - (BeginTime.Minutes - UserEndTime.Minutes);
                            DifferenceHours = DifferenceHours - 1;
                        }
                    }

                    if (EndTime.Minutes == UserBeginTime.Minutes)
                    {
                        MinutesSecondDay = 0;
                    }

                    if (EndTime.Minutes > UserBeginTime.Minutes)
                    {
                        MinutesSecondDay = (EndTime.Minutes - UserBeginTime.Minutes);
                    }

                    if (EndTime.Minutes < UserBeginTime.Minutes)
                    {
                        if (EndTime.Minutes == 0)
                        {
                            MinutesSecondDay = 60 - UserBeginTime.Minutes;
                            DifferenceHours = DifferenceHours - 1;
                        }

                        if (EndTime.Minutes != 0)
                        {
                            MinutesSecondDay = 60 - (UserBeginTime.Minutes - EndTime.Minutes);
                            DifferenceHours = DifferenceHours - 1;
                        }
                    }

                    DifferenceMinutes = MinutesFirstDay + MinutesSecondDay;
                    if (DifferenceMinutes > 59)
                    {
                        DifferenceHours = DifferenceHours + 1;
                        DifferenceMinutes = DifferenceMinutes - 60;
                    }

                    vacreq.TotalMinutes = (DifferenceHours * 60) + DifferenceMinutes;

                }

                if (DifferenceInDays.Days >= 2)
                {
                    int totaldaysbetween = 0;
                    DateTime startDate = BeginDate;
                    for (int i = 1; i <= DifferenceInDays.Days; i++)
                    {
                        if (startDate.DayOfWeek == DayOfWeek.Monday || startDate.DayOfWeek == DayOfWeek.Tuesday || startDate.DayOfWeek == DayOfWeek.Wednesday || startDate.DayOfWeek == DayOfWeek.Thursday || startDate.DayOfWeek == DayOfWeek.Friday)
                        {
                            totaldaysbetween = totaldaysbetween + 1;
                        }

                        startDate = startDate.AddDays(1);
                    }

                    totaldaysbetween = totaldaysbetween - 1;
                    

                    int DifferenceHours = 0;
                    int DifferenceMinutes = 0;
                    int MinutesFirstDay = 0;
                    int MinutesSecondDay = 0;

                    DifferenceHours = (UserEndTime.Hours - BeginTime.Hours) + (EndTime.Hours - UserBeginTime.Hours);

                    if (UserEndTime.Minutes == BeginTime.Minutes)
                    {
                        MinutesFirstDay = 0;
                    }

                    if (UserEndTime.Minutes > BeginTime.Minutes)
                    {
                        MinutesFirstDay = (UserEndTime.Minutes - BeginTime.Minutes);
                    }

                    if (UserEndTime.Minutes < BeginTime.Minutes)
                    {
                        if (UserEndTime.Minutes == 0)
                        {
                            MinutesFirstDay = 60 - BeginTime.Minutes;
                            DifferenceHours = DifferenceHours - 1;
                        }

                        if (UserEndTime.Minutes != 0)
                        {
                            MinutesFirstDay = 60 - (BeginTime.Minutes - UserEndTime.Minutes);
                            DifferenceHours = DifferenceHours - 1;
                        }
                    }

                    if (EndTime.Minutes == UserBeginTime.Minutes)
                    {
                        MinutesSecondDay = 0;
                    }

                    if (EndTime.Minutes > UserBeginTime.Minutes)
                    {
                        MinutesSecondDay = (EndTime.Minutes - UserBeginTime.Minutes);
                    }

                    if (EndTime.Minutes < UserBeginTime.Minutes)
                    {
                        if (EndTime.Minutes == 0)
                        {
                            MinutesSecondDay = 60 - UserBeginTime.Minutes;
                            DifferenceHours = DifferenceHours - 1;
                        }

                        if (EndTime.Minutes != 0)
                        {
                            MinutesSecondDay = 60 - (UserBeginTime.Minutes - EndTime.Minutes);
                            DifferenceHours = DifferenceHours - 1;
                        }
                    }

                    DifferenceMinutes = MinutesFirstDay + MinutesSecondDay;
                    if (DifferenceMinutes > 59)
                    {
                        DifferenceHours = DifferenceHours + 1;
                        DifferenceMinutes = DifferenceMinutes - 60;
                    }

                    var totalhoursindays = DifferenceHours;
                    TimeSpan DifferenceBeginEndTime = (new TimeSpan(vacreq.EndOfDay.Value.Hour, vacreq.EndOfDay.Value.Minute, 00)) - (new TimeSpan(vacreq.BeginOfDay.Value.Hour, vacreq.BeginOfDay.Value.Minute, 00));
                    if (DifferenceBeginEndTime.TotalMinutes > 480)
                    {
                        var minutesoverload = (DifferenceBeginEndTime.TotalMinutes % 480) *-1;
                        DifferenceBeginEndTime = DifferenceBeginEndTime.Add(new TimeSpan(0,Convert.ToInt32(minutesoverload), 0));
                    }
                    
                    vacreq.TotalMinutes = (totalhoursindays * 60) + DifferenceMinutes + (Convert.ToInt32(DifferenceBeginEndTime.TotalMinutes) * totaldaysbetween);
                }

                
            }
            
            vacreq.Reason = Reason;
            if (vacreq.IsTotalDays == true)
            {
                vacreq.BeginDate = BeginDate.Add(new TimeSpan(08, 00, 00));
                vacreq.EndDate = EndDate.Add(new TimeSpan(18, 00, 00));
            }            
            vacreq.HasDeadlines = HasDeadlines;
            vacreq.IsCommunicated = IsCommunicated;

            return vacreq;
        }
    }
}


