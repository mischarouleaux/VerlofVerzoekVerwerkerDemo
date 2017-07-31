using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VVV.Models;

namespace VVV.Interfaces.Services
{
    public interface IManagerService
    {
        long GetUserIDByManagerID(long managerid);
        long GetManageridByUserID(long userid);
        Manager Get(long managerid);

        IQueryable<Manager> Listwithavailablemanager();

        bool IsManager(long userid);
        bool IsAvailable(long managerid);

        void SetAvailable(long managerid);
        void SetNotAvailable(long managerid);
        IQueryable<Manager> GetAll();
        //string FullManagerName(long id);

        void Save(Models.Manager manager);
        void DeleteManager(long managerid, long userid);
    }
}
