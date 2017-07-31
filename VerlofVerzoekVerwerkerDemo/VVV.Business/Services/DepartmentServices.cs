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
using System.Linq.Dynamic;

namespace VVV.Business.Services
{
    public class DepartmentService  : IDepartmentService
    {
        private readonly IDepartmentRepository _repository;
        private readonly IUnitOfWork _uow;

        public DepartmentService(IDepartmentRepository repo, IUnitOfWork uow)
        {
            _repository = repo;
            _uow = uow;
        }

        //Haalt alle afdelingen op van het bedrijf
        public IQueryable<Department> GetAllDepartments()
        {
            return _repository.Get();
        }

        #region Get ID's

        public long GetApplicationsID()
        {
            return _repository.Get(filter => filter.DepartmentID == 1).Select(a => a.DepartmentID).FirstOrDefault();
        }

        public long GetServicesID()
        {
            return _repository.Get(filter => filter.DepartmentID == 2).Select(a => a.DepartmentID).FirstOrDefault();
        }

        public long GetConsultancyID()
        {
            return _repository.Get(filter => filter.DepartmentID == 3).Select(a => a.DepartmentID).FirstOrDefault();
        }

        public long GetKnowledgeID()
        {
            return _repository.Get(filter => filter.DepartmentID == 4).Select(a => a.DepartmentID).FirstOrDefault();
        }

        #endregion

    }
}
