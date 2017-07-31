using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VVV.Interfaces.Repositories;
using VVV.Interfaces.Services;
using VVV.Interfaces;
using VVV.Models;
using System.Linq.Dynamic;


namespace VVV.Business.Services
{
    public class ApplicationUserService : IApplicationUserService
    {
        //De gebruikte repositories in de cs bestand.
        private readonly IApplicationUserRepository _repository;
        private readonly IDepartmentService _depService;
        private readonly IUnitOfWork _uow;

        //Inject the repository.
        public ApplicationUserService(IApplicationUserRepository repo, IDepartmentService depService, IUnitOfWork uow)
        {
            _repository = repo;
            _depService = depService;
            _uow = uow;
        }

        #region Manager
        //Haalt alle gebruikers op die onder een manager vallen, op basis van een managerid.
        public IQueryable<ApplicationUser> GetUsersByManager(long managerid)
        {
            return _repository.Get(filter => filter.Manager == managerid && filter.IsActive == true).OrderBy(p => p.FirstName);
        }

        //Haalt het managerid op dat hoort bij het opgegeven userid.
        public long GetManageridByUserId(long id)
        {
            return _repository.Get(filter => filter.UserID == id).Select(p => p.Manager).FirstOrDefault();
        }

        public long? GetSecondManageridByUserID(long id)
        {
            return _repository.Get(filter => filter.UserID == id).Select(p => p.SecondManager).FirstOrDefault();
        }

        //Bekijkt of een manager medewerkers onder zich heeft staan.
        public bool HasMedewerkers(long managerid)
        {
            var query = _repository.Get(filter => filter.Manager == managerid || filter.SecondManager == managerid).Count();

            if (query > 0)
            {
                return true;
            }

            return false;
        }

        //Haalt de records op die horen bij het opgegeven id.
        public IQueryable<ApplicationUser> GetById(long id)
        {
            return _repository.Get(filter => filter.UserID == id).OrderBy(p => p.UserID);
        }

        #endregion

        #region HR-Medewerker

        public IQueryable<ApplicationUser> GetAllUsers()
        {
            return _repository.Get(filter => filter.IsActive == true, i => i.Department1).OrderBy(p => p.LastName);
        }
        public IQueryable<ApplicationUser> GetAllDeletedUsers()
        {
            return _repository.Get(filter => filter.IsActive == false).OrderBy(p => p.LastName);
        }



        #endregion

        #region UserExists

        //Gaat in de tabel van gebruikers kijken of een gebruikersnaam al bestaat.
        public bool UserExists(string email, long userid = 0)
        {
            return _repository.GetReadOnly(filter => filter.Email == email && filter.UserID != userid && filter.IsActive == true).ToList().Count != 0;
        }

        public bool UserExists(string email)
        {
            return _repository.GetReadOnly(filter => filter.Email == email && filter.IsActive == true).ToList().Count != 0;
        }

        public bool EmailExists(string email)
        {
            return _repository.Get(filter => filter.Email == email).ToList().Count != 0;
        }

        #endregion

        #region Department

        public IQueryable<ApplicationUser> GetUsersDepartmentFilter(string SearchQuery, bool IsApplications, bool IsServices, bool IsKnowledge, bool IsConsultancy)
        {
            long itapplications = _depService.GetApplicationsID();
            long itservices = _depService.GetServicesID();
            long itknowledge = _depService.GetKnowledgeID();
            long itconsultancy = _depService.GetConsultancyID();

            var query = _repository.Get();
            if (String.IsNullOrEmpty(SearchQuery))
            {
                query = _repository
                .Get(f => f.IsActive == true && ((f.Department == itapplications && IsApplications == true) || (f.Department == itservices && IsServices) || (f.Department == itknowledge && IsKnowledge == true) || (f.Department == itconsultancy && IsConsultancy == true)), i => i.Department1).OrderBy(p => p.LastName);
            }
            else
            {
                query = _repository
                .Get(f => f.IsActive == true && (f.FirstName.Contains(SearchQuery) || f.LastName.Contains(SearchQuery)) && ((f.Department == itapplications && IsApplications == true) || (f.Department == itservices && IsServices) || (f.Department == itknowledge && IsKnowledge == true) || (f.Department == itconsultancy && IsConsultancy == true)), i => i.Department1).OrderBy(p => p.LastName);
            }


            return query;
        }
        #endregion

        #region Active
        //Bekijkt of een gebruiker verwijderd is.
        public bool UserIsActive(long userid)
        {
            var query = _repository.Get(filter => filter.UserID == userid).FirstOrDefault();
            if (query.IsActive == true)
            {
                return true;
            }
            return false;
        }

        #endregion

        #region Medewerker
        //Haalt een gebruiker op, op basis van het e-mailadres
        public ApplicationUser Get(string email)
        {
            return _repository.GetReadOnly(filter => filter.Email == email && filter.IsActive == true).FirstOrDefault();
        }

        //Haalt het e-mailadres op passende bij het opgegeven userid.
        public string GetUsername(long userid)
        {
            return _repository.Get(filter => filter.UserID == userid).Select(p => p.Email).FirstOrDefault();
        }


        //Haalt een specifieke gebruiker op bij het doorgegeven id
        public ApplicationUser Get(long id)
        {
            return _repository.Get(filter => filter.UserID == id, i => i.Department1).FirstOrDefault();
        }

        //Geeft de volledige naam terug van een gebruiker
        public string GetNameById(long id)
        {
            var query = _repository.Find(id);
            string name = query.FirstName.ToString() + " " + query.LastName.ToString();
            return name;
        }

        //Geeft de vorige manager van een medewerker terug
        public long LastManager(long userid)
        {
            return _repository.Get(filter => filter.UserID == userid).Select(p => p.Manager).FirstOrDefault();
        }

        //Geeft de vorige tweede manager van een medewerker terug
        public long? LastSecondManager(long userid)
        {
            var query = _repository.Get(filter => filter.UserID == userid).Select(p => p.SecondManager).FirstOrDefault();

            return query;
        }

        #endregion

        #region Count Users
        public int CountMedewerkers()
        {
            return _repository.Get(filter => filter.IsMedewerker == true).Count();
        }
        public int CountManagers()
        {
            return _repository.Get(filter => filter.IsManager == true).Count();
        }
        public int CountHRMedewerkers()
        {
            return _repository.Get(filter => filter.IsHRManager == true).Count();
        }

        #endregion  

        #region Save
        //Saved de veranderingen in de tabel: Applicationuser
        public void Save(Models.ApplicationUser user)
        {
            if (user == null)
                throw new ArgumentException("user");
            if (UserExists(user.Email, user.UserID))
                throw new ArgumentException("username exists");

            if (user.UserID == 0)
                _repository.Add(user);
            else
                _repository.Edit(user);

            _uow.Commit();
        }
        #endregion

        #region Delete

        //Verwijdert een rij die toebehoort aan het id (een gebruiker)
        public void Delete(long id)
        {
            _repository.Delete(id);
            _uow.Commit();
        }

        public void SoftDelete(long userid)
        {
            var user = _repository.Get(filter => filter.UserID == userid).FirstOrDefault();

            user.IsActive = false;

            Save(user);
        }

        public void UndoSoftDelete(long userid)
        {
            var user = _repository.Get(filter => filter.UserID == userid).FirstOrDefault();

            user.IsActive = true;

            Save(user);
        }

        #endregion
    }
}
