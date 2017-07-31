using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VVV.Interfaces.Services;
using VVV.Business.Identity;
using VVV.UI.Attributes;
using VVV.UI.Helpers;
using VVV.UI.ViewModels.Calendar;

namespace VVV.UI.Controllers
{
    public class CalendarController : Controller
    {

        private IVacationRequestService _vacreqService;
        private IDepartmentService _depService;
        private IHolidayService _holService;

        public CalendarController(IVacationRequestService vacreqService, IDepartmentService depService, IHolidayService holService)
        {
            _vacreqService = vacreqService;
            _depService = depService;
            _holService = holService;
        }

        #region Calendars
        public ActionResult MainCalendar()
        {
            long applicationsid = _depService.GetApplicationsID();
            long servicesid = _depService.GetServicesID();
            long consultancyid = _depService.GetConsultancyID();
            long knowledgeid = _depService.GetKnowledgeID();

            var model = new CalendarModel(applicationsid, servicesid, consultancyid, knowledgeid);


            return View(model);
        }

        public ActionResult ApplicationsCalendar()
        {
            long applicationsid = _depService.GetApplicationsID();
            long servicesid = _depService.GetServicesID();
            long consultancyid = _depService.GetConsultancyID();
            long knowledgeid = _depService.GetKnowledgeID();

            var model = new CalendarModel(applicationsid, servicesid, consultancyid, knowledgeid);

            return View(model);
        }

        public ActionResult ConsultancyCalendar()
        {
            long applicationsid = _depService.GetApplicationsID();
            long servicesid = _depService.GetServicesID();
            long consultancyid = _depService.GetConsultancyID();
            long knowledgeid = _depService.GetKnowledgeID();

            var model = new CalendarModel(applicationsid, servicesid, consultancyid, knowledgeid);

            return View(model);
        }

        public ActionResult KnowledgeCalendar()
        {
            long applicationsid = _depService.GetApplicationsID();
            long servicesid = _depService.GetServicesID();
            long consultancyid = _depService.GetConsultancyID();
            long knowledgeid = _depService.GetKnowledgeID();

            var model = new CalendarModel(applicationsid, servicesid, consultancyid, knowledgeid);

            return View(model);
        }

        public ActionResult ServicesCalendar()
        {
            long applicationsid = _depService.GetApplicationsID();
            long servicesid = _depService.GetServicesID();
            long consultancyid = _depService.GetConsultancyID();
            long knowledgeid = _depService.GetKnowledgeID();

            var model = new CalendarModel(applicationsid, servicesid, consultancyid, knowledgeid);

            return View(model);
        }
        #endregion

        #region Json results
        public JsonResult GetEvents()
        {
            var activities = _vacreqService.GetAllAcceptedEvents();
            foreach (var item in activities)
            {
                item.url = Url.Action("Details", "Verlofaanvraag", new { id = item.id });
            }
            var holidays = _holService.GetHolidaysForCalendar();

            foreach (var item in holidays)
            {
                activities.Add(item);
            }

            foreach (var item in activities)
            {
                item.start = item.start.AddHours(DateTime.Now.Hour - DateTime.UtcNow.Hour);
                item.end = item.end.AddHours(DateTime.Now.Hour - DateTime.UtcNow.Hour);
            }

            return Json(activities, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSelectedEvents(long? departmentid)
        {
            if (departmentid != null)
            {
                var activities = _vacreqService.GetAcceptedEventsByDepartment(departmentid.Value);
                foreach (var item in activities)
                {
                    item.url = Url.Action("Details", "Verlofaanvraag", new { id = item.id });
                }

                var holidays = _holService.GetHolidaysForCalendar();

                foreach (var item in holidays)
                {
                    activities.Add(item);
                }

                foreach (var item in activities)
                {
                    item.start = item.start.AddHours(DateTime.Now.Hour - DateTime.UtcNow.Hour);
                    item.end = item.end.AddHours(DateTime.Now.Hour - DateTime.UtcNow.Hour);
                }

                return Json(activities, JsonRequestBehavior.AllowGet);
            }

            return GetEvents();
        }
        #endregion

    }
}