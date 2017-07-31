using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VVV.Interfaces.Services;
using VVV.Interfaces.Repositories;
using VVV.Interfaces;
using VVV.Models;


using Newtonsoft.Json;
using System.Web;
using System.Net;


namespace VVV.Business.Services
{
    public class ManagerService : IManagerService
    {
        private readonly IManagerRepository _repository;
        private readonly IUnitOfWork _uow;

        public ManagerService(IManagerRepository repo, IUnitOfWork uow)
        {
            _repository = repo;
            _uow = uow;
        }

        #region Availability
        //Zet de manager op aanwezig
        public void SetAvailable(long managerid)
        {
            var manager = _repository.Get(filter => filter.ManagerID == managerid).FirstOrDefault();

            manager.IsAvailable = true;

            Save(manager);            
        }
        
        //Zet de manager op afwezig
        public void SetNotAvailable(long managerid)
        {
            var manager = _repository.Get(filter => filter.ManagerID == managerid).FirstOrDefault();

            manager.IsAvailable = false;

            Save(manager);
        }

        public bool IsAvailable(long managerid)
        {
            var query = _repository.Get(filter => filter.ManagerID == managerid).FirstOrDefault();

            if (query.IsAvailable == true)
            {
                return true;
            }

            return false;
        }

        public IQueryable<Manager> Listwithavailablemanager()
        {
            return _repository.Get(filter => filter.IsAvailable == true);
        }
        #endregion

        #region Get ID's
        //Haalt de desbetreffende userid op van een manager (managerid). Hierdoor kan er in de applicationuser tabel worden gezocht naar informatie over de manager.
        public long GetUserIDByManagerID(long managerid)
        {
            return _repository.Get(filter => filter.ManagerID == managerid).Select(p => p.UserID).FirstOrDefault();
        }

        //Haalt de managerid op van gebruiker door middel van het userid.
        public long GetManageridByUserID(long userid)
        {
            return _repository.Get(filter => filter.UserID == userid).Select(p => p.ManagerID).FirstOrDefault();
        }

        //Wordt niet gebruikt.
        public Manager Get(long managerid)
        {
            return _repository.Get(filter => filter.ManagerID == managerid).FirstOrDefault();
        }

        #endregion

        #region Get All
        public IQueryable<Manager> GetAll()
        {
            var query = _repository.Get(i => i.ApplicationUser);

            return query;
        }
        #endregion

        #region Manager
        public string FullManagerName(long id)
        {
            var item = _repository.Get(filter => filter.ManagerID == id).FirstOrDefault();

            string name = item.ApplicationUser.FirstName + " " + item.ApplicationUser.LastName;

            return name;
        }

        public bool IsManager(long userid)
        {
            return _repository.Get(filter => filter.UserID == userid).ToList().Count != 0;
        }
        #endregion

        #region Save
        //Saved de veranderingen in de tabel: Applicationuser
        public void Save(Models.Manager manager)
        {
            if (manager == null)
                throw new ArgumentException("user");
            

            if (manager.ManagerID == 0)
                _repository.Add(manager);
            else
                _repository.Edit(manager);

            _uow.Commit();
        }
        #endregion

        #region Delete
        public void DeleteManager(long managerid, long userid)
        {
            long id = _repository.Get(filter => filter.ManagerID == managerid && filter.UserID == userid).Select(p => p.ManagerID).FirstOrDefault();

            _repository.Delete(id);
            _uow.Commit();
        }
        #endregion
    }
}
