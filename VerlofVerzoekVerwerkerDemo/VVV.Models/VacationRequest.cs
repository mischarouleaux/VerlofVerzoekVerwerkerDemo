using System;
using System.Collections.Generic;

namespace VVV.Models
{
    public partial class VacationRequest
    {
        public VacationRequest()
        {
            this.Messages = new List<Message>();
            this.MutationsVacations = new List<MutationsVacation>();
        }

        public long VacationID { get; set; }
        public long ManagerID { get; set; }
        public Nullable<long> SecondManagerID { get; set; }
        public Nullable<long> AssessedByID { get; set; }
        public long UserID { get; set; }
        public System.DateTime DateSubmission { get; set; }
        public string Reason { get; set; }
        public string ReasonRejection { get; set; }
        public System.DateTime BeginDate { get; set; }
        public Nullable<System.DateTime> BeginOfDay { get; set; }
        public System.DateTime EndDate { get; set; }
        public Nullable<System.DateTime> EndOfDay { get; set; }
        public int TotalMinutes { get; set; }
        public bool IsTotalDays { get; set; }
        public bool IsCommunicated { get; set; }
        public bool HasDeadlines { get; set; }
        public Nullable<System.DateTime> OldBeginDate { get; set; }
        public Nullable<System.DateTime> OldEndDate { get; set; }
        public Nullable<int> OldTotalMinutes { get; set; }
        public Nullable<bool> IsOldTotalDays { get; set; }
        public bool IsRejected { get; set; }
        public bool IsApproved { get; set; }
        public bool IsInTreatment { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime DateCreated { get; set; }
        public System.DateTime DateChanged { get; set; }
        public long CreateUser { get; set; }
        public long ChangeUser { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
        public virtual ICollection<Message> Messages { get; set; }
        public virtual ICollection<MutationsVacation> MutationsVacations { get; set; }
    }
}
