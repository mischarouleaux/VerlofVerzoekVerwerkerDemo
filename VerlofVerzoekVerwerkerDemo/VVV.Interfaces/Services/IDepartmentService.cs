using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VVV.Models;

namespace VVV.Interfaces.Services
{
    public interface IDepartmentService
    {
        IQueryable<Department> GetAllDepartments();

        long GetApplicationsID();
        long GetServicesID();
        long GetConsultancyID();
        long GetKnowledgeID();
    }
}
