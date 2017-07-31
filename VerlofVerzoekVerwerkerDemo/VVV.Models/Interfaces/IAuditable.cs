using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VVV.Models.Interfaces
{
    public interface IAuditable
    {
        DateTime? DateCreated { get; set; }
        DateTime? DateChanged { get; set; }
        string CreateUser { get; set; }
        string ChangeUser { get; set; }
    }
}

