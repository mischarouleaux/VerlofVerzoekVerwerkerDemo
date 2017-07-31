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
using System.Data.Entity;

namespace VVV.Business.Services
{
    public class HolidayService : IHolidayService
    {
        private readonly IHolidayRepository _repository;
        private readonly IUnitOfWork _uow;

        public HolidayService(IHolidayRepository repo, IUnitOfWork uow)
        {
            _repository = repo;
            _uow = uow;
        }

        #region Get Holidays
        public IQueryable<Holiday> GetAll()
        {
            return _repository.Get(filter => filter.Date >= DbFunctions.TruncateTime(DateTime.Now)).OrderBy(p => p.Date);
        }

        //Haalt een feestdag op, op basis van het id
        public Holiday GetByID(long holidayid)
        {
            return _repository.Get(filter => filter.HolidayID == holidayid).FirstOrDefault();
        }

        //Haalt de oude datum op van een feestdag, ter vergelijking of deze veranderd is in het editten.
        public DateTime OldDate(long holidayid)
        {
            return _repository.Get(filter => filter.HolidayID == holidayid).FirstOrDefault().Date;
        }
        #endregion

        #region Calendar
        //Zet alle feestdagen in de agenda van gebruikers
        public List<CalendarDisplay> GetHolidaysForCalendar()
        {
            var query = _repository.Get();

            List<CalendarDisplay> list = new List<CalendarDisplay>();


            foreach (var item in query)
            {
                list.Add(new CalendarDisplay
                {
                    id = item.HolidayID.ToString(),
                    title = item.Description,
                    allDay = true,
                    start = item.Date,
                    end = item.Date
                });
            }
            return list;
        }

        #endregion

        #region Holiday exists
        //Bekijkt of er al een feestdag is gepland op deze dag
        public bool HolidayExists(DateTime date, long holidayid)
        {
            return _repository.Get(filter => filter.Date == date && filter.HolidayID != holidayid).ToList().Count != 0;
        }
        #endregion

        #region Holidays in a period
        //Bekijkt welke feestdagen in een periode liggen
        public List<Holiday> HolidaysInPeriod(DateTime BeginDate, DateTime EndDate)
        {
            var query = _repository.Get(filter => ((DbFunctions.TruncateTime(BeginDate) == filter.Date && DbFunctions.TruncateTime(EndDate) == filter.Date) || (DbFunctions.TruncateTime(BeginDate) < filter.Date && DbFunctions.TruncateTime(EndDate) == filter.Date) || (DbFunctions.TruncateTime(BeginDate) == filter.Date && DbFunctions.TruncateTime(EndDate) > filter.Date) || (DbFunctions.TruncateTime(BeginDate) < filter.Date && DbFunctions.TruncateTime(EndDate) > filter.Date))).OrderBy(p => p.Date);

            List<Holiday> list = new List<Holiday>();
            foreach (var item in query)
            {
                list.Add(new Holiday
                {
                    HolidayID = item.HolidayID,
                    Description = item.Description,
                    Date = item.Date
                });
            }

            return list;
        }
        #endregion

        #region Save
        public void Save(Models.Holiday holiday)
        {
            if (holiday == null)
            {
                throw new ArgumentException("holiday");
            }

            if (holiday.HolidayID == 0)
            {
                _repository.Add(holiday);
            }
            else
            {
                _repository.Edit(holiday);
            }

            _uow.Commit();
        }
        #endregion

        #region Delete
        public void Delete(long holidayid)
        {
            _repository.Delete(holidayid);
            _uow.Commit();
        }
        #endregion




    }
    
}
