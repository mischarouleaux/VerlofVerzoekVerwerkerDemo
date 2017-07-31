using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VVV.Models;
using VVV.Models.DisplayModels;

namespace VVV.Interfaces.Services
{
    public interface IHolidayService
    {
        IQueryable<Holiday> GetAll();
        List<CalendarDisplay> GetHolidaysForCalendar();
        Holiday GetByID(long holidayid);
        DateTime OldDate(long holidayid);
        List<Holiday> HolidaysInPeriod(DateTime BeginDate, DateTime EndDate);
        bool HolidayExists(DateTime date, long holidayid);
        void Save(Holiday holiday);
        void Delete(long holidayid);
    }
}
