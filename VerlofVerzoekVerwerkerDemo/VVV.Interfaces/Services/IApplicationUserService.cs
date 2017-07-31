using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VVV.Models;


namespace VVV.Interfaces.Services
{
    public interface IApplicationUserService
    {
        IQueryable<ApplicationUser> GetAllUsers();
        IQueryable<ApplicationUser> GetAllDeletedUsers();
        IQueryable<ApplicationUser> GetUsersByManager(long managerid);        
        IQueryable<ApplicationUser> GetUsersDepartmentFilter(string SearchQuery, bool IsApplications, bool IsServices, bool IsKnowledge, bool IsConsultancy);
        bool UserExists(string username);
        bool EmailExists(string email);        
        IQueryable<ApplicationUser> GetById(long id);
        string GetUsername(long userid);
        ApplicationUser Get(long id);        
        ApplicationUser Get(string email);
        bool UserIsActive(long userid);
        long GetManageridByUserId(long id);
        long? GetSecondManageridByUserID(long id);
        string GetNameById(long id);        
        bool HasMedewerkers(long managerid);
        long LastManager(long userid);
        long? LastSecondManager(long userid);
        int CountMedewerkers();
        int CountManagers();
        int CountHRMedewerkers();
        void Save(Models.ApplicationUser user);        
        void SoftDelete(long userid);
        void UndoSoftDelete(long userid);
    }
}
