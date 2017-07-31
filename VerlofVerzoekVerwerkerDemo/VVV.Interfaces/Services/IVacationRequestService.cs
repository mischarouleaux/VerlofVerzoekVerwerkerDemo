using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VVV.Models;
using VVV.Models.DisplayModels;

namespace VVV.Interfaces.Services
{
    public interface IVacationRequestService
    {
        
        IQueryable<VacationRequest> GetAll(long id);
        IQueryable<VacationRequest> GetOldRequest(long id);
        IQueryable<VacationRequest> GetCurrentVacation(long id);
        IQueryable<VacationRequest> GetPropositionVacation(long id);
        IQueryable<VacationRequest> GetPropositionVacationBy3(long id);
        IQueryable<VacationRequest> GetAcceptedVacation(long id);
        IQueryable<VacationRequest> GetAcceptedVacationBy3(long id);
        IQueryable<VacationRequest> GetRejectedVacation(long id);
        IQueryable<VacationRequest> GetRejectedVacationBy3(long id);
        IQueryable<VacationRequest> GetInTreatmentVacation(long id);
        IQueryable<VacationRequest> GetInTreatmentVacationBy3(long id);
        IQueryable<VacationRequest> GetUsersHasVacation(DateTime BeginDate, DateTime EndDate, long userid, long managerid);
        VacationRequest Getbyid(long id);
        VacationRequest Get(long id);

        IQueryable<VacationRequest> GetVacationRequestInTreatmentByManager(long managerid);
        IQueryable<VacationRequest> GetVacationRequestInTreatmentByManagerBy3(long managerid);
        IQueryable<VacationRequest> GetVacationRequestInTreatmentNewByManagerBy3(long managerid);
        IQueryable<VacationRequest> GetVacationRequestAcceptedByManager(long managerid);
        IQueryable<VacationRequest> GetVacationRequestRejectedByManager(long managerid);
        IQueryable<VacationRequest> GetOldVacationRequestByManager(long managerid);

        IQueryable<VacationRequest> GetVacationRequestInTreatmentBySecondManager(long managerid, List<long> availablemanagers);
        //IQueryable<VacationRequest> GetVacationRequestInTreatmentByManagerBy3(long managerid);
        //IQueryable<VacationRequest> GetVacationRequestInTreatmentNewByManagerBy3(long managerid);
        IQueryable<VacationRequest> GetVacationRequestAcceptedBySecondManager(long managerid);
        IQueryable<VacationRequest> GetVacationRequestRejectedBySecondManager(long managerid);
        IQueryable<VacationRequest> GetOldVacationRequestBySecondManager(long managerid);

        List<VacationRequestInHolidays> CheckUsersHaveVacation(DateTime Date);
        //IQueryable<VacationRequest> CheckUsersHaveVacation(DateTime date);

        bool IsAccepted(long vacreqid);
        bool IsRejected(long vacreqid);
        bool IsInTreatment(long vacreqid);

        bool FirstManagerHasVacation(long managerid, List<long> availablemanagers);

        bool HasVacation(DateTime BeginTime, DateTime EndTime, long userid);
        bool HasVacation(DateTime BeginTime, DateTime EndTime, long userid, long vacreqid);

        DateTime GetOldBeginDate(long vacationid);
        DateTime GetOldEndDate(long vacationid);
        int GetOldTotalMinutes(long vacationid);
        bool GetOldIsTotalDays(long vacationid);
        
        List<CalendarDisplay> GetAllAcceptedEvents();
        List<CalendarDisplay> GetAcceptedEventsByDepartment(long departmentid);

        void Accept(long vacreqid, long assessedbyid);
        void Reject(long vacreqid, long assessedbyid);

        void ChangeManagerID(long userid, long Newmanagerid);
        void ChangeSecondManagerID(long userid, long Newsecondmanagerid);

        void Save(Models.VacationRequest vacationrequest);
        Task SaveAsync(Models.VacationRequest vacationrequest);
        void Delete(long id);
        void SoftDelete(long vacationid);
    }
}
