using System;
using System.Collections.Generic;

namespace VVV.Models
{
    public partial class ApplicationUser
    {
        public ApplicationUser()
        {
            this.Managers = new List<Manager>();
            this.Messages = new List<Message>();
            this.MutationsVacations = new List<MutationsVacation>();
            this.MutationsVacations1 = new List<MutationsVacation>();
            this.VacationRequests = new List<VacationRequest>();
        }

        public long UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public long Department { get; set; }
        public long Manager { get; set; }
        public Nullable<long> SecondManager { get; set; }
        public bool IsMedewerker { get; set; }
        public bool IsManager { get; set; }
        public bool IsHRManager { get; set; }
        public bool IsBlocked { get; set; }
        public bool IsActive { get; set; }
        public Nullable<System.DateTime> DateCreated { get; set; }
        public Nullable<System.DateTime> DateChanged { get; set; }
        public Nullable<long> CreateUser { get; set; }
        public Nullable<long> ChangeUser { get; set; }
        public virtual Department Department1 { get; set; }
        public virtual ICollection<Manager> Managers { get; set; }
        public virtual ICollection<Message> Messages { get; set; }
        public virtual ICollection<MutationsVacation> MutationsVacations { get; set; }
        public virtual ICollection<MutationsVacation> MutationsVacations1 { get; set; }
        public virtual ICollection<VacationRequest> VacationRequests { get; set; }
    }
}
