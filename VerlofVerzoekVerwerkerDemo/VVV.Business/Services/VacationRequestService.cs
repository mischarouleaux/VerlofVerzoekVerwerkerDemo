using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;

using System.Threading.Tasks;
using VVV.Interfaces.Repositories;
using VVV.Interfaces.Services;
using VVV.Interfaces;
using VVV.Models;
using VVV.Models.DisplayModels;
using System.Data.Entity.Core.Objects;

namespace VVV.Business.Services
{
    public class VacationRequestService : IVacationRequestService
    {
        //call the repositories
        private readonly IVacationRequestRepository _repository;
        private readonly IMessageRepository _messrepository;
        private readonly IUnitOfWork _uow;
        private readonly IManagerRepository _manrepository;

        //Inject the repositories
        public VacationRequestService(IVacationRequestRepository repo, IMessageRepository messrepository, IUnitOfWork uow, IManagerRepository manrepository)
        {
            _repository = repo;
            _messrepository = messrepository;
            _uow = uow;
            _manrepository = manrepository;

        }


        #region GetVacationRequest by id & time (Medewerker)
        public VacationRequest Get(long vacreqid)
        {
            return _repository.Find(vacreqid);
        }
        

        //Haalt alle verlofverzoeken op van een gebruiker.
        public IQueryable<VacationRequest> GetAll(long id)
        {
            var currentdate = DateTime.Now.Date;
            return _repository.Get(filter => filter.UserID == id && DbFunctions.TruncateTime(filter.EndDate) >= currentdate && filter.IsActive == true).OrderBy(p => p.BeginDate);
        }

        //Haalt alle verlofaanvragen op waarvan de einddatum voor vandaag ligt, en dus in het verleden.
        public IQueryable<VacationRequest> GetOldRequest(long id)
        {
            var currentdate = DateTime.Now.Date;
            return _repository.Get(filter => filter.UserID == id && DbFunctions.TruncateTime(filter.EndDate) < currentdate && filter.IsActive == true).OrderByDescending(p => p.BeginDate);
        }

        //Haalt de verlofaanvraag op die op het moment plaatsvindt.
        public IQueryable<VacationRequest> GetCurrentVacation(long id)
        {
            return _repository.Get(filter => filter.UserID == id && filter.BeginDate <= DateTime.Now && filter.EndDate >= DateTime.Now&& filter.IsApproved == true && filter.IsActive == true).OrderBy(p => p.BeginDate).Take(1);
        }

        //Haalt alle voorgestelde verlofverzoeken door de manager op.
        public IQueryable<VacationRequest> GetPropositionVacation(long id)
        {
            var currentdate = DateTime.Now.Date;
            return _repository.Get(filter => filter.UserID == id && DbFunctions.TruncateTime(filter.BeginDate) >= DateTime.Now && filter.IsInTreatment == false && filter.IsRejected == false && filter.IsApproved == false && filter.IsActive == true).OrderBy(p => p.BeginDate);
        }

        //Haalt eerste 3 voorgestelde verlofverzoeken door de manager op.
        public IQueryable<VacationRequest> GetPropositionVacationBy3(long id)
        {
            var currentdate = DateTime.Now.Date;
            return _repository.Get(filter => filter.UserID == id && DbFunctions.TruncateTime(filter.BeginDate) >= currentdate && filter.IsInTreatment == false && filter.IsRejected == false && filter.IsApproved == false && filter.IsActive == true).Take(3).OrderBy(p => p.BeginDate);
        }

        //Haalt alle geaccepteerde verlofaanvragen van een bepaalde gebruiker op.
        public IQueryable<VacationRequest> GetAcceptedVacation(long id)
        {
            var currentdate = DateTime.Now.Date;
            return _repository.Get(filter => filter.UserID == id && DbFunctions.TruncateTime(filter.BeginDate) >=currentdate && filter.IsApproved == true && filter.IsActive == true).OrderBy(p => p.BeginDate);
        }

        //Haalt de eerste 3 geaccepteerde verlofaanvragen van een gebruiker op.
        public IQueryable<VacationRequest> GetAcceptedVacationBy3(long id)
        {
            var currentdate = DateTime.Now.Date;
            return _repository.Get(filter => filter.UserID == id && DbFunctions.TruncateTime(filter.BeginDate) >= currentdate && filter.IsApproved == true && filter.IsActive == true).Take(3).OrderBy(p => p.BeginDate);
        }

        //Haalt alle afgewezen verlofaanvraag van een bepaalde gebruiker op.
        public IQueryable<VacationRequest> GetRejectedVacation(long id)
        {
            var currentdate = DateTime.Now.Date;
            return _repository.Get(filter => filter.UserID == id && DbFunctions.TruncateTime(filter.BeginDate) >= currentdate && filter.IsRejected == true && filter.IsActive == true).OrderBy(p => p.BeginDate);
        }

        //Haal de top 3 afgewezen verlofaanvragen van een bepaalde gebruiker op
        public IQueryable<VacationRequest> GetRejectedVacationBy3(long id)
        {
            var currentdate = DateTime.Now.Date;
            return _repository.Get(filter => filter.UserID == id && DbFunctions.TruncateTime(filter.BeginDate) >= currentdate && filter.IsRejected == true && filter.IsActive == true).Take(3).OrderBy(p => p.BeginDate);
        }

        //Haalt de verlofverzoeken op de nog in behandeling zijn van een bepaalde gebruiker
        public IQueryable<VacationRequest> GetInTreatmentVacation (long id)
        {
            var currentdate = DateTime.Now.Date;
            return _repository.Get(filter => filter.UserID == id && DbFunctions.TruncateTime(filter.BeginDate) >= currentdate && filter.IsInTreatment == true && filter.IsActive == true).OrderBy(p => p.BeginDate);
        }

        //Haal de top 3 verlofverzoeken op die nog in behandeling zijn van een bepaalde gebruiker.
        public IQueryable<VacationRequest> GetInTreatmentVacationBy3(long id)
        {
            var currentdate = DateTime.Now.Date;
            return _repository.Get(filter => filter.UserID == id && DbFunctions.TruncateTime(filter.BeginDate) >= currentdate && filter.IsInTreatment == true && filter.IsActive == true).Take(3).OrderBy(p => p.BeginDate);
        }

        //Haalt een specifieke verlofaanvraag op door middel van het verlofid.
        public VacationRequest Getbyid(long id)
        {
            return _repository.Find(id);
        }
        #endregion

        #region GetVacationRequest Manager
        //Haalt alle verlofverzoeken op die in behandeling zijn, op basis van een managerid
        public IQueryable<VacationRequest> GetVacationRequestInTreatmentByManager(long managerid)
        {
            var currentdate = DateTime.Now.Date;
            return _repository.Get(filter => filter.ApplicationUser.Manager == managerid && filter.IsInTreatment == true && DbFunctions.TruncateTime(filter.BeginDate) >= currentdate && filter.IsActive == true, i => i.ApplicationUser).OrderBy(p => p.BeginDate);
        }

        //Haalt 3 verlofverzoeken op die in behandeling zijn, geordend op begindatum. Op basis van een managerid
        public IQueryable<VacationRequest> GetVacationRequestInTreatmentByManagerBy3(long managerid)
        {
            var currentdate = DateTime.Now.Date;
            return _repository.Get(filter => filter.ApplicationUser.Manager == managerid && DbFunctions.TruncateTime(filter.BeginDate) >= currentdate && filter.IsInTreatment == true && filter.IsActive == true, i => i.ApplicationUser).Take(3).OrderBy(p => p.BeginDate);
        }

        //Haalt 3 verlofverzoeken op die in behandleing zijn, geordend op laatst toegevoegd. Op basis van een managerid
        public IQueryable<VacationRequest> GetVacationRequestInTreatmentNewByManagerBy3(long managerid)
        {
            var currentdate = DateTime.Now.Date;
            return _repository.Get(filter => filter.ApplicationUser.Manager == managerid && filter.IsInTreatment == true && DbFunctions.TruncateTime(filter.BeginDate) >= currentdate && filter.IsActive == true, i => i.ApplicationUser).Take(3).OrderByDescending(p => p.DateSubmission);
        }

        //Haalt alle geaccepteerde verlofverzoeken op, op basis van een managerid.
        public IQueryable<VacationRequest> GetVacationRequestAcceptedByManager(long managerid)
        {
            var currentdate = DateTime.Now.Date;
            return _repository.Get(filter => filter.ApplicationUser.Manager == managerid && filter.IsApproved == true && DbFunctions.TruncateTime(filter.BeginDate) >= currentdate && filter.IsActive == true, i => i.ApplicationUser).OrderBy(p => p.BeginDate);
        }

        //Haalt alle afgewezen verlofverzoeken op, op basis van een managerid.
        public IQueryable<VacationRequest> GetVacationRequestRejectedByManager(long managerid)
        {
            var currentdate = DateTime.Now.Date;
            return _repository.Get(filter => filter.ApplicationUser.Manager == managerid && filter.IsRejected == true && DbFunctions.TruncateTime(filter.BeginDate) >= currentdate && filter.IsActive == true, i => i.ApplicationUser).OrderBy(p => p.BeginDate);
        }
        
        //Haalt alle oude verlofverzoeken op, op basis van een managerid.
        public IQueryable<VacationRequest> GetOldVacationRequestByManager(long managerid)
        {
            var currentdate = DateTime.Now.Date;
            return _repository.Get(filter => filter.ApplicationUser.Manager == managerid && DbFunctions.TruncateTime(filter.BeginDate) < currentdate && filter.IsActive == true, i => i.ApplicationUser).OrderByDescending(p => p.BeginDate);
        }

        #endregion

        #region GetVacationRequest SecondManager        

        //Haalt alle geaccepteerde verlofverzoeken op, op basis van een managerid.
        public IQueryable<VacationRequest> GetVacationRequestAcceptedBySecondManager(long managerid)
        {
            var currentdate = DateTime.Now.Date;
            return _repository.Get(filter => filter.SecondManagerID == managerid && filter.IsApproved == true && DbFunctions.TruncateTime(filter.EndDate) >= currentdate && filter.IsActive == true, i => i.ApplicationUser).OrderBy(p => p.BeginDate);
        }

        //Haalt alle afgewezen verlofverzoeken op, op basis van een managerid.
        public IQueryable<VacationRequest> GetVacationRequestRejectedBySecondManager(long managerid)
        {
            var currentdate = DateTime.Now.Date;
            return _repository.Get(filter => filter.SecondManagerID == managerid && filter.IsRejected == true && DbFunctions.TruncateTime(filter.EndDate) >= currentdate && filter.IsActive == true, i => i.ApplicationUser).OrderBy(p => p.BeginDate);
        }

        public IQueryable<VacationRequest> GetVacationRequestInTreatmentBySecondManager(long managerid, List<long> availablemanagers)
        {
            var currentdate = DateTime.Now.Date;
            var query = _repository.Get(filter => filter.SecondManagerID == managerid);
            List<long> id = new List<long>(); 
            foreach(var item in query)
            {
                if (availablemanagers.Contains(item.ManagerID))
                {

                }
                else
                {
                    id.Add(item.ManagerID);
                }
            }

            query = _repository.Get(filter => id.Contains(filter.ManagerID) && filter.IsInTreatment == true && DbFunctions.TruncateTime(filter.BeginDate) >= currentdate && filter.IsActive == true, i => i.ApplicationUser).OrderBy(p => p.BeginDate);

            return query;
        }

        //Checkt of er verlofverzoeken zijn die beoordeelt moeten worden, terwijl de eerste manager afwezig is. 
        public bool FirstManagerHasVacation(long managerid, List<long> availablemanagers)
        {
            var query = _repository.Get(filter => filter.SecondManagerID == managerid && filter.IsInTreatment == true && filter.BeginDate > DateTime.Now);
            if (query.Count() == 0)
            {
                return false;
            }
            List<long> id = new List<long>();
            foreach (var item in query)
            {
                if (availablemanagers.Contains(item.ManagerID))
                {

                }
                else
                {
                    id.Add(item.ManagerID);
                }
            }

            if (id.Count != 0)
            {
                return true;
            }

            return false;
        }

        //Haalt alle oude verlofverzoeken op, op basis van een managerid.
        public IQueryable<VacationRequest> GetOldVacationRequestBySecondManager(long managerid)
        {
            var currentdate = DateTime.Now.Date;
            return _repository.Get(filter => filter.SecondManagerID == managerid && DbFunctions.TruncateTime(filter.EndDate) < currentdate && filter.IsActive == true, i => i.ApplicationUser).OrderByDescending(p => p.BeginDate);
        }

        #endregion

        #region Gebruiker heeft vakantie
        //Gaat in de database zoeken naar verlofaanvragen die niet zijn afgewezen en die in dezelfde tijd vallen. Een gebruiker kan niet twee keer in dezelfde tijd verlof hebben.
        public bool HasVacation (DateTime BeginTime, DateTime EndTime, long userid)
        {
            return _repository.Get(
                filter => ((BeginTime < filter.BeginDate && EndTime > filter.BeginDate && EndTime < filter.EndDate) || (BeginTime > filter.BeginDate && EndTime < filter.EndDate) || (BeginTime > filter.BeginDate && BeginTime < filter.EndDate && EndTime > filter.EndDate) || (BeginTime < filter.BeginDate && EndTime > filter.EndDate) || (BeginTime > filter.BeginDate && EndTime < filter.EndDate) || (BeginTime > filter.BeginDate && EndTime == filter.EndDate) || (BeginTime == filter.BeginDate && EndTime < filter.EndDate) || (BeginTime == filter.BeginDate && EndTime == filter.EndDate)) && filter.UserID == userid && filter.IsRejected == false && filter.IsActive == true).ToList().Count != 0;
        }

        public bool HasVacation(DateTime BeginTime, DateTime EndTime, long userid, long vacreqid)
        {
            return _repository.Get(
                filter => ((BeginTime < filter.BeginDate && EndTime > filter.BeginDate && EndTime < filter.EndDate) || (BeginTime > filter.BeginDate && EndTime < filter.EndDate) || (BeginTime > filter.BeginDate && BeginTime < filter.EndDate && EndTime > filter.EndDate) || (BeginTime < filter.BeginDate && EndTime > filter.EndDate) || (BeginTime > filter.BeginDate && EndTime < filter.EndDate) || (BeginTime > filter.BeginDate && EndTime == filter.EndDate) || (BeginTime == filter.BeginDate && EndTime < filter.EndDate) || (BeginTime == filter.BeginDate && EndTime == filter.EndDate)) && filter.UserID == userid && filter.IsRejected == false && filter.VacationID != vacreqid && filter.IsActive == true).ToList().Count != 0;
        }
        #endregion

        #region Gebruikes hebben vakantie
        //Haalt alle verlofverzoeken op die in dezelfde periode plaatsvinden.
        public IQueryable<VacationRequest> GetUsersHasVacation(DateTime BeginDate, DateTime EndDate, long userid, long managerid)
        {            
            var query = _repository.Get(filter => filter.IsActive == true && filter.UserID != userid && filter.ManagerID == managerid && (filter.IsApproved == true || filter.IsInTreatment == true || (filter.IsApproved == false && filter.IsRejected == false && filter.IsInTreatment == false)) &&
            ((filter.BeginDate < BeginDate && (filter.EndDate > BeginDate && filter.EndDate < EndDate)) || (filter.BeginDate == BeginDate && (filter.EndDate > BeginDate && filter.EndDate < EndDate)) || (filter.BeginDate > BeginDate && filter.EndDate < EndDate) || (filter.BeginDate > BeginDate && filter.EndDate == EndDate) || ((filter.BeginDate > BeginDate && filter.BeginDate < EndDate) && filter.EndDate > EndDate) || (filter.BeginDate < BeginDate && filter.EndDate > EndDate) || (filter.BeginDate == BeginDate && filter.EndDate == EndDate)), i => i.ApplicationUser).OrderBy(p => p.BeginDate);
            return query; 
        }
        #endregion

        #region Check state
        public bool IsAccepted(long vacreqid)
        {
            var query = _repository.Get(filter => filter.VacationID == vacreqid).FirstOrDefault();

            if (query.IsApproved == true)

            {
                return true;
            }

            else
            {
                return false;
            }
        }

        public bool IsRejected(long vacreqid)
        {
            var query = _repository.Get(filter => filter.VacationID == vacreqid).FirstOrDefault();

            if(query.IsRejected == true)
            {
                return true;
            }

            else
            {
                return false;
            }
        }

        public bool IsInTreatment(long vacreqid)
        {
            var query = _repository.Get(filter => filter.VacationID == vacreqid).FirstOrDefault();

            if(query.IsInTreatment == true)
            {
                return true;
            }

            else
            {
                return false;
            }
        }

        #endregion

        #region Accept
        public void Accept(long vacreqid, long assessedbyid)
        {
            VacationRequest vacreq = Get(vacreqid);
            vacreq.AssessedByID = assessedbyid;
            vacreq.IsApproved = true;
            vacreq.IsRejected = false;
            vacreq.IsInTreatment = false;
            Save(vacreq);
        }

        #endregion

        #region Reject
        public void Reject(long vacreqid, long assessedbyid)
        {            
            VacationRequest vacreq = Get(vacreqid);
            vacreq.AssessedByID = assessedbyid;
            vacreq.IsApproved = false;
            vacreq.IsRejected = true;
            vacreq.IsInTreatment = false;
            Save(vacreq);

        }

        #endregion

        #region Edit vacationrequest

        public void ChangeManagerID(long userid, long Newmanagerid)
        {
            var query = _repository.Get(filter => filter.UserID == userid && filter.IsActive == true);

            foreach (var item in query)
            {
                item.ManagerID = Newmanagerid;

                _repository.Edit(item);
                               
            }
            _uow.Commit();                        
        }

        public void ChangeSecondManagerID(long userid, long Newsecondmanagerid)
        {
            var query = _repository.Get(filter => filter.UserID == userid);

            foreach(var item in query)
            {
                item.SecondManagerID = Newsecondmanagerid;

                _repository.Edit(item);
            }
            _uow.Commit();
        }

        #endregion

        #region Get Oldinformation
        public DateTime GetOldBeginDate(long vacationid)
        {
            return _repository.Get(filter => filter.VacationID == vacationid && filter.IsActive == true).FirstOrDefault().BeginDate;
        }

        public DateTime GetOldEndDate(long vacationid)
        {
            return _repository.Get(filter => filter.VacationID == vacationid && filter.IsActive == true).FirstOrDefault().EndDate;
        }
        public int GetOldTotalMinutes(long vacationid)
        {
            return _repository.Get(filter => filter.VacationID == vacationid && filter.IsActive == true).FirstOrDefault().TotalMinutes;
        }
        public bool GetOldIsTotalDays(long vacationid)
        {
            return _repository.Get(filter => filter.VacationID == vacationid && filter.IsActive == true).FirstOrDefault().IsTotalDays;
        }

        #endregion

        #region Calendar

        public List<CalendarDisplay> GetAllAcceptedEvents()
        {
            var query = _repository.Get(filter => filter.IsActive == true && filter.IsApproved == true, i => i.ApplicationUser);

            List<CalendarDisplay> list = new List<CalendarDisplay>();

            foreach (var item in query)
            {
                //Verlofverzoek wordt in de agenda geplaatst als: Hele dag
                if (item.IsTotalDays == true)
                {
                    list.Add(new CalendarDisplay
                    {
                        id = item.VacationID.ToString(),
                        title = item.ApplicationUser.FirstName + " " + item.ApplicationUser.LastName,
                        allDay = item.IsTotalDays,
                        start = item.BeginDate,
                        end = item.EndDate,
                    });                                                
                }

                else
                {
                    //Bereken het verschil in dagen tussen de eind- & begintijd
                    var DifferenceInDays = item.EndDate.Date - item.BeginDate.Date;


                    if (DifferenceInDays.Days < 1)
                    {
                        if ((item.EndDate.Hour - item.BeginDate.Hour) == 8)
                        {
                            list.Add(new CalendarDisplay
                            {
                                id = item.VacationID.ToString(),
                                title = item.ApplicationUser.FirstName + " " + item.ApplicationUser.LastName,
                                allDay = true,
                                start = item.BeginDate,
                                end = item.EndDate,
                            });
                        }
                        else

                        {
                            list.Add(new CalendarDisplay
                            {
                                id = item.VacationID.ToString(),
                                title = item.ApplicationUser.FirstName + " " + item.ApplicationUser.LastName,
                                allDay = false,
                                start = item.BeginDate,
                                end = item.EndDate,
                            });
                        }
                        
                    }

                    if (DifferenceInDays.Days >= 1 && DifferenceInDays.Days < 2)
                    {
                        if ((item.EndOfDay.Value.Hour - item.BeginDate.Hour) == 8)
                        {
                            list.Add(new CalendarDisplay
                            {
                                id = item.VacationID.ToString(),
                                title = item.ApplicationUser.FirstName + " " + item.ApplicationUser.LastName,
                                allDay = true,
                                start = item.BeginDate,
                                end = new DateTime(item.BeginDate.Year, item.BeginDate.Month, item.BeginDate.Day).Add(new TimeSpan(item.EndOfDay.Value.Hour, item.EndOfDay.Value.Minute, 00)),
                            });
                        }
                        else
                        {
                            list.Add(new CalendarDisplay
                            {
                                id = item.VacationID.ToString(),
                                title = item.ApplicationUser.FirstName + " " + item.ApplicationUser.LastName,
                                allDay = false,
                                start = item.BeginDate,
                                end = new DateTime(item.BeginDate.Year, item.BeginDate.Month, item.BeginDate.Day).Add(new TimeSpan(item.EndOfDay.Value.Hour, item.EndOfDay.Value.Minute, 00)),
                            });
                        }

                        if ((item.EndDate.Hour - item.BeginOfDay.Value.Hour) == 8)
                        {
                            list.Add(new CalendarDisplay
                            {
                                id = item.VacationID.ToString(),
                                title = item.ApplicationUser.FirstName + " " + item.ApplicationUser.LastName,
                                allDay = true,
                                start = new DateTime(item.EndDate.Year, item.EndDate.Month, item.EndDate.Day).Add(new TimeSpan(item.BeginOfDay.Value.Hour, item.BeginOfDay.Value.Minute, 00)),
                                end = item.EndDate,
                            });
                        }
                        else
                        {
                            list.Add(new CalendarDisplay
                            {
                                id = item.VacationID.ToString(),
                                title = item.ApplicationUser.FirstName + " " + item.ApplicationUser.LastName,
                                allDay = false,
                                start = new DateTime(item.EndDate.Year, item.EndDate.Month, item.EndDate.Day).Add(new TimeSpan(item.BeginOfDay.Value.Hour, item.BeginOfDay.Value.Minute, 00)),
                                end = item.EndDate,
                            });
                        }
                        
                        
                    }

                    //Voor verlofverzoeken die meer dan twee dagen zijn, dit zodat de eerste en laatste dag 
                    if (DifferenceInDays.Days >= 2)
                    {
                        if ((item.EndOfDay.Value.Hour - item.BeginDate.Hour) == 8)
                        {
                            list.Add(new CalendarDisplay
                            {
                                id = item.VacationID.ToString(),
                                title = item.ApplicationUser.FirstName + " " + item.ApplicationUser.LastName,
                                allDay = true,
                                start = item.BeginDate,
                                end = new DateTime(item.BeginDate.Year, item.BeginDate.Month, item.BeginDate.Day).Add(new TimeSpan(item.EndOfDay.Value.Hour, item.EndOfDay.Value.Minute, 00)),
                            });
                        }
                        else
                        {
                            list.Add(new CalendarDisplay
                            {
                                id = item.VacationID.ToString(),
                                title = item.ApplicationUser.FirstName + " " + item.ApplicationUser.LastName,
                                allDay = false,
                                start = item.BeginDate,
                                end = new DateTime(item.BeginDate.Year, item.BeginDate.Month, item.BeginDate.Day).Add(new TimeSpan(item.EndOfDay.Value.Hour, item.EndOfDay.Value.Minute, 00)),
                            });

                        }

                        //Tusentijdse dagen
                        var startTotaldays = item.BeginDate.AddDays(1);
                        var endTotaldays = item.EndDate.AddDays(-1);
                        list.Add(new CalendarDisplay
                        {
                            id = item.VacationID.ToString(),
                            title = item.ApplicationUser.FirstName + " " + item.ApplicationUser.LastName,
                            allDay = true,
                            start = startTotaldays,
                            end = endTotaldays,
                        });

                        if ((item.EndOfDay.Value.Hour - item.BeginDate.Hour) == 8)
                        {
                            list.Add(new CalendarDisplay
                            {
                                id = item.VacationID.ToString(),
                                title = item.ApplicationUser.FirstName + " " + item.ApplicationUser.LastName,
                                allDay = true,
                                start = new DateTime(item.EndDate.Year, item.EndDate.Month, item.EndDate.Day).Add(new TimeSpan(item.BeginOfDay.Value.Hour, item.BeginOfDay.Value.Minute, 00)),
                                end = item.EndDate,
                            });
                        }
                        else
                        {
                            list.Add(new CalendarDisplay
                            {
                                id = item.VacationID.ToString(),
                                title = item.ApplicationUser.FirstName + " " + item.ApplicationUser.LastName,
                                allDay = false,
                                start = new DateTime(item.EndDate.Year, item.EndDate.Month, item.EndDate.Day).Add(new TimeSpan(item.BeginOfDay.Value.Hour, item.BeginOfDay.Value.Minute, 00)),
                                end = item.EndDate,
                            });
                        }                        
                    }
                }
            }

            return list;
        }


        public List<CalendarDisplay> GetAcceptedEventsByDepartment(long departmentid)
        {
            var query = _repository.Get(filter => filter.ApplicationUser.Department == departmentid && filter.IsActive == true && filter.IsApproved == true, i => i.ApplicationUser);

            List<CalendarDisplay> list = new List<CalendarDisplay>();

            foreach (var item in query)
            {
                //Verlofverzoek wordt in de agenda geplaatst als: Hele dag
                if (item.IsTotalDays == true)
                {
                    list.Add(new CalendarDisplay
                    {
                        id = item.VacationID.ToString(),
                        title = item.ApplicationUser.FirstName + " " + item.ApplicationUser.LastName,
                        allDay = item.IsTotalDays,
                        start = item.BeginDate,
                        end = item.EndDate,
                    });
                }

                else
                {
                    //Bereken het verschil in dagen tussen de eind- & begintijd
                    var DifferenceInDays = item.EndDate.Date - item.BeginDate.Date;


                    if (DifferenceInDays.Days < 1)
                    {
                        if ((item.EndDate.Hour - item.BeginDate.Hour) == 8)
                        {
                            list.Add(new CalendarDisplay
                            {
                                id = item.VacationID.ToString(),
                                title = item.ApplicationUser.FirstName + " " + item.ApplicationUser.LastName,
                                allDay = true,
                                start = item.BeginDate,
                                end = item.EndDate,
                            });
                        }
                        else

                        {
                            list.Add(new CalendarDisplay
                            {
                                id = item.VacationID.ToString(),
                                title = item.ApplicationUser.FirstName + " " + item.ApplicationUser.LastName,
                                allDay = false,
                                start = item.BeginDate,
                                end = item.EndDate,
                            });
                        }

                    }

                    if (DifferenceInDays.Days >= 1 && DifferenceInDays.Days < 2)
                    {
                        if ((item.EndOfDay.Value.Hour - item.BeginDate.Hour) == 8)
                        {
                            list.Add(new CalendarDisplay
                            {
                                id = item.VacationID.ToString(),
                                title = item.ApplicationUser.FirstName + " " + item.ApplicationUser.LastName,
                                allDay = true,
                                start = item.BeginDate,
                                end = new DateTime(item.BeginDate.Year, item.BeginDate.Month, item.BeginDate.Day).Add(new TimeSpan(item.EndOfDay.Value.Hour, item.EndOfDay.Value.Minute, 00)),
                            });
                        }
                        else
                        {
                            list.Add(new CalendarDisplay
                            {
                                id = item.VacationID.ToString(),
                                title = item.ApplicationUser.FirstName + " " + item.ApplicationUser.LastName,
                                allDay = false,
                                start = item.BeginDate,
                                end = new DateTime(item.BeginDate.Year, item.BeginDate.Month, item.BeginDate.Day).Add(new TimeSpan(item.EndOfDay.Value.Hour, item.EndOfDay.Value.Minute, 00)),
                            });
                        }

                        if ((item.EndDate.Hour - item.BeginOfDay.Value.Hour) == 8)
                        {
                            list.Add(new CalendarDisplay
                            {
                                id = item.VacationID.ToString(),
                                title = item.ApplicationUser.FirstName + " " + item.ApplicationUser.LastName,
                                allDay = true,
                                start = new DateTime(item.EndDate.Year, item.EndDate.Month, item.EndDate.Day).Add(new TimeSpan(item.BeginOfDay.Value.Hour, item.BeginOfDay.Value.Minute, 00)),
                                end = item.EndDate,
                            });
                        }
                        else
                        {
                            list.Add(new CalendarDisplay
                            {
                                id = item.VacationID.ToString(),
                                title = item.ApplicationUser.FirstName + " " + item.ApplicationUser.LastName,
                                allDay = false,
                                start = new DateTime(item.EndDate.Year, item.EndDate.Month, item.EndDate.Day).Add(new TimeSpan(item.BeginOfDay.Value.Hour, item.BeginOfDay.Value.Minute, 00)),
                                end = item.EndDate,
                            });
                        }


                    }

                    //Voor verlofverzoeken die meer dan twee dagen zijn, dit zodat de eerste en laatste dag 
                    if (DifferenceInDays.Days >= 2)
                    {
                        if ((item.EndOfDay.Value.Hour - item.BeginDate.Hour) == 8)
                        {
                            list.Add(new CalendarDisplay
                            {
                                id = item.VacationID.ToString(),
                                title = item.ApplicationUser.FirstName + " " + item.ApplicationUser.LastName,
                                allDay = true,
                                start = item.BeginDate,
                                end = new DateTime(item.BeginDate.Year, item.BeginDate.Month, item.BeginDate.Day).Add(new TimeSpan(item.EndOfDay.Value.Hour, item.EndOfDay.Value.Minute, 00)),
                            });
                        }
                        else
                        {
                            list.Add(new CalendarDisplay
                            {
                                id = item.VacationID.ToString(),
                                title = item.ApplicationUser.FirstName + " " + item.ApplicationUser.LastName,
                                allDay = false,
                                start = item.BeginDate,
                                end = new DateTime(item.BeginDate.Year, item.BeginDate.Month, item.BeginDate.Day).Add(new TimeSpan(item.EndOfDay.Value.Hour, item.EndOfDay.Value.Minute, 00)),
                            });

                        }

                        //Tusentijdse dagen
                        var startTotaldays = item.BeginDate.AddDays(1);
                        var endTotaldays = item.EndDate.AddDays(-1);
                        list.Add(new CalendarDisplay
                        {
                            id = item.VacationID.ToString(),
                            title = item.ApplicationUser.FirstName + " " + item.ApplicationUser.LastName,
                            allDay = true,
                            start = startTotaldays,
                            end = endTotaldays,
                        });

                        if ((item.EndOfDay.Value.Hour - item.BeginDate.Hour) == 8)
                        {
                            list.Add(new CalendarDisplay
                            {
                                id = item.VacationID.ToString(),
                                title = item.ApplicationUser.FirstName + " " + item.ApplicationUser.LastName,
                                allDay = true,
                                start = new DateTime(item.EndDate.Year, item.EndDate.Month, item.EndDate.Day).Add(new TimeSpan(item.BeginOfDay.Value.Hour, item.BeginOfDay.Value.Minute, 00)),
                                end = item.EndDate,
                            });
                        }
                        else
                        {
                            list.Add(new CalendarDisplay
                            {
                                id = item.VacationID.ToString(),
                                title = item.ApplicationUser.FirstName + " " + item.ApplicationUser.LastName,
                                allDay = false,
                                start = new DateTime(item.EndDate.Year, item.EndDate.Month, item.EndDate.Day).Add(new TimeSpan(item.BeginOfDay.Value.Hour, item.BeginOfDay.Value.Minute, 00)),
                                end = item.EndDate,
                            });
                        }
                    }
                }
            }
            return list;
        }


        #endregion

        #region User have vacation in holiday period
        public List<VacationRequestInHolidays> CheckUsersHaveVacation(DateTime Date)
        {
            var query = _repository.Get(filter => filter.IsActive == true && filter.IsApproved == true && ((DbFunctions.TruncateTime(filter.BeginDate) == Date && DbFunctions.TruncateTime(filter.EndDate) == Date) || (DbFunctions.TruncateTime(filter.BeginDate) < Date && DbFunctions.TruncateTime(filter.EndDate) == Date) || (DbFunctions.TruncateTime(filter.BeginDate) == Date && DbFunctions.TruncateTime(filter.EndDate) > Date) || (DbFunctions.TruncateTime(filter.BeginDate) < Date && DbFunctions.TruncateTime(filter.EndDate) > Date))).OrderBy(p => p.BeginDate);

            int minutes = 0;
            List<VacationRequestInHolidays> list = new List<VacationRequestInHolidays>();

            if (query.Count() > 0)
            {
                foreach (var vacreq in query)
                {
                    if (vacreq.IsTotalDays == true)
                    {
                        minutes = 480; //Gelijk aan 8 uur werken.
                    }

                    else
                    {
                        if (vacreq.BeginDate.Date == Date.Date && vacreq.EndDate.Date == Date.Date)
                        {
                            TimeSpan difference = vacreq.EndDate.Subtract(vacreq.BeginDate);
                            minutes = (difference.Hours * 60) + (difference.Minutes);
                        }

                        if (vacreq.BeginDate.Date < Date.Date && vacreq.EndDate.Date == Date.Date)
                        {
                            TimeSpan endtime = new TimeSpan(vacreq.EndDate.Hour, vacreq.EndDate.Minute, 00);
                            TimeSpan begintime = new TimeSpan(vacreq.BeginOfDay.Value.Hour, vacreq.BeginOfDay.Value.Minute, 00);
                            TimeSpan difference = endtime.Subtract(begintime);
                            minutes = (difference.Hours * 60) + (difference.Minutes);
                        }

                        if (vacreq.BeginDate.Date == Date.Date && vacreq.EndDate.Date > Date.Date)
                        {
                            TimeSpan begintime = new TimeSpan(vacreq.BeginDate.Hour, vacreq.BeginDate.Minute, 00);
                            TimeSpan endtime = new TimeSpan(vacreq.EndOfDay.Value.Hour, vacreq.EndOfDay.Value.Minute, 00);
                            TimeSpan difference = endtime.Subtract(begintime);
                            minutes = (difference.Hours * 60) + (difference.Minutes);
                        }

                        if (vacreq.BeginDate.Date < Date.Date && vacreq.EndDate.Date > Date.Date)
                        {
                            TimeSpan begintime = new TimeSpan(vacreq.BeginOfDay.Value.Hour, vacreq.BeginOfDay.Value.Minute, 00);
                            TimeSpan endtime = new TimeSpan(vacreq.EndOfDay.Value.Hour, vacreq.EndOfDay.Value.Minute, 00);
                            TimeSpan difference = endtime.Subtract(begintime);
                            minutes = (difference.Hours * 60) + difference.Minutes;
                        }
                    }

                    list.Add(new VacationRequestInHolidays
                    {
                        UserID = vacreq.UserID,
                        VacationID = vacreq.VacationID,
                        TotalMinutes = minutes
                    });


                }
            }

            return list;
        }
        #endregion

        #region Save
        //Slaat de veranderingen op.
        public void Save (Models.VacationRequest vacationrequest)
        {
            if (vacationrequest == null)
                throw new ArgumentException("vacationrequest");

            if (vacationrequest.VacationID == 0)
            {
                _repository.Add(vacationrequest);
            }

            else
                _repository.Edit(vacationrequest);

            _uow.Commit();
        }

        public async Task SaveAsync(Models.VacationRequest vacationrequest)
        {
            if (vacationrequest == null)
                throw new ArgumentException("vacationrequest");

            if (vacationrequest.VacationID == 0)
            {
                _repository.Add(vacationrequest);
            }

            else
                _repository.Edit(vacationrequest);

            await _uow.CommitAsync();
        }

        #endregion

        #region Delete
        //Verwijdert een verlofverzoek bij id.
        public void Delete(long vacationid)
        {
            //Eerst worden berichten waarin het verlofverzoekid voorkomt, verwijdert. Dit zodat de applicatie niet vastloopt.
            var query = _messrepository.Get(filter => filter.VacationRequestID == vacationid);
            foreach (var item in query)
            {
                _messrepository.Delete(item.MessageID);
                
            }
            
            _repository.Delete(vacationid);

            
            _uow.Commit();
        }

        //Voert een softdelete uit. Hierdoor komt het verwijderde verlofverzoek niet meer voor in de tabellen, maar is de informatie wel toegankelijk voor foreign keys.
        public void SoftDelete(long vacationid)
        {
            var query = _repository.Get(filter => filter.VacationID == vacationid).FirstOrDefault();

            query.IsActive = false;

            Save(query);
        }


        #endregion


    }
}
