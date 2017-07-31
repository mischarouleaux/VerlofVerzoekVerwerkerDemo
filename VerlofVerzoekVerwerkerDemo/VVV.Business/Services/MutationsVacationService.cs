using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VVV.Interfaces.Repositories;
using VVV.Interfaces.Services;
using VVV.Interfaces;
using VVV.Models;
using VVV.Models.DisplayModels;

namespace VVV.Business.Services
{
    public class MutationsVacationService : IMutationsVacationService
    {
        private readonly IMutationsVacationRepository _repository;
        private readonly IUnitOfWork _uow;
        private readonly IApplicationUserRepository _userrepo;

        public MutationsVacationService(IMutationsVacationRepository repo, IUnitOfWork uow, IApplicationUserRepository userrepo)
        {
            _repository = repo;
            _uow = uow;
            _userrepo = userrepo;
        }

        #region Get mutations by user
        //Haalt alle mutatie's op van een gebruiker.
        public IQueryable<MutationsVacation> GetMutationsByUserID(long id)
        {
            var query = _repository.Get(filter => filter.UserID == id, i => i.ApplicationUser, i => i.ApplicationUser1).OrderByDescending(p => p.DateCreated);

            return query;
        }

        //Berekent de resterende verlofminuten
        public int GetMinutesVacationByID(long id)
        {
            var query = 0;
            //Er wordt eerst gekeken of er records zijn met het opgegeven id, zodat er in de query geen null variabele terug kan komen.
            if (_repository.Get(filter => filter.UserID == id).Count() > 0)
            {
                query = _repository.Get(filter => filter.UserID == id).Sum(p => p.VacationModification);

            }
            else
            {
                query = 0;

            }

            return query;
        }
        #endregion

        #region Edit Minutes
        //Voegt verlofminuten toe aan een gebruiker.
        public void AddMinutes(long userid, long currentuser, int minutes)
        {
            MutationsVacation mutvac = new MutationsVacation();
            mutvac.UserID = userid;
            mutvac.VacationModification = minutes;
            mutvac.DateCreated = DateTime.Now;
            mutvac.DateChanged = DateTime.Now;
            mutvac.CreateUser = currentuser;
            mutvac.ChangeUser = currentuser;
            Save(mutvac);
        }

        //Haalt verlofminuten af van een gebruiker.
        public void SubtractMinutes(long userid, long currentuser, int minutes)
        {
            MutationsVacation mutvac = new MutationsVacation();
            mutvac.UserID = userid;
            mutvac.VacationModification = minutes * (-1);
            mutvac.DateCreated = DateTime.Now;
            mutvac.DateChanged = DateTime.Now;
            mutvac.CreateUser = currentuser;
            mutvac.ChangeUser = currentuser;
            Save(mutvac);
        }

        //Haalt verlofminuten af van een gebruiker.
        public void SubtractMinutes(long userid, long currentuser, int minutes, VacationRequest vacreq)
        {
            MutationsVacation mutvac = new MutationsVacation();
            mutvac.UserID = userid;
            mutvac.VacationRequest = vacreq;
            mutvac.VacationModification = minutes * (-1);
            mutvac.DateCreated = DateTime.Now;
            mutvac.DateChanged = DateTime.Now;
            mutvac.CreateUser = currentuser;
            mutvac.ChangeUser = currentuser;
            Save(mutvac);
        }

        #endregion

        #region Save
        //Slaat de veranderingen op.
        public void Save(Models.MutationsVacation mutvac)
        {
            if (mutvac == null)
                throw new ArgumentException("mutvac");

            if (mutvac.MutationID == 0)
                _repository.Add(mutvac);
            else
                _repository.Edit(mutvac);

            _uow.Commit();

        }
        #endregion

        #region Delete
        public void DeleteByUserID(long userid)
        {
            var query = _repository.Get(filter => filter.UserID == userid);
        }

        #endregion


    }
}
