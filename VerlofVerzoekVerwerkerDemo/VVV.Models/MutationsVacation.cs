using System;
using System.Collections.Generic;

namespace VVV.Models
{
    public partial class MutationsVacation
    {
        public long MutationID { get; set; }
        public long UserID { get; set; }
        public Nullable<long> VacationID { get; set; }
        public int VacationModification { get; set; }
        public System.DateTime DateCreated { get; set; }
        public System.DateTime DateChanged { get; set; }
        public long CreateUser { get; set; }
        public long ChangeUser { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
        public virtual ApplicationUser ApplicationUser1 { get; set; }
        public virtual VacationRequest VacationRequest { get; set; }
    }
}
