using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VVV.Models.Enums
{
    public enum UserRole
    {
        [Description("Medewerker")]
        Medewerker = 0,
        [Description("Manager")]
        Manager = 1,
        [Description("HRManager")]
        HRManager = 2
    }
}
