using System;
using System.Collections.Generic;

namespace VVV.Models
{
    public partial class Department
    {
        public Department()
        {
            this.ApplicationUsers = new List<ApplicationUser>();
        }

        public long DepartmentID { get; set; }
        public string DepartmentName { get; set; }
        public virtual ICollection<ApplicationUser> ApplicationUsers { get; set; }
    }
}
