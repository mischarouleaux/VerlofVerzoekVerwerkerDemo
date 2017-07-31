using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using VVV.Models;
using VVV.Models.DisplayModels;
using VVV.Business.Identity;
using VVV.UI.Attributes;
using VVV.Interfaces.Services;
using VVV.UI.ViewModels.Holidays;
using VVV.UI.Helpers;
using PagedList;

namespace VVV.UI.Controllers
{
    public class HolidayController : BaseController
    {
        private IHolidayService _holService;
        private IVacationRequestService _vacreqService;
        private IMutationsVacationService _mutvacService;

        public HolidayController(IHolidayService holService, IVacationRequestService vacreqService, IMutationsVacationService mutvacService)
        {
            _holService = holService;
            _vacreqService = vacreqService;
            _mutvacService = mutvacService;
        }

        #region Index
        [ClaimsAuthorize(UserRoles.HRManager)]
        public ActionResult Index()
        {
            IndexModel model = new IndexModel();


            var Msg = TempData["Saved"] as string;
            if (Msg != null && Msg == "true")
            {
                model.HasSavedMessage = true;
            }
            else
            {
                model.HasSavedMessage = false;
            }
            Msg = TempData["Changed"] as string;
            if (Msg != null && Msg == "true")
            {
                model.HasChangedMessage = true;
            }
            else
            {
                model.HasChangedMessage = false;
            }
            Msg = TempData["Delete"] as string;
            if (Msg != null && Msg == "true")
            {
                model.HasDeleteMessage = true;
            }
            else
            {
                model.HasDeleteMessage = false;
            }

            model.Holidays = _holService.GetAll().ToPagedList(model.Page, model.PageSize);

            return View(model);
        }
        #endregion

        #region Create 
        [ClaimsAuthorize(UserRoles.HRManager)]
        public ActionResult Create()
        {
            EditModel model = new EditModel();
            model.Date = DateTime.Now.Date;

            return View(model);
        }

        [ClaimsAuthorize(UserRoles.HRManager)]
        [HttpPost]
        public ActionResult Create([Bind(Include = "Description, Date")] EditModel input)
        {
            var holiday = input.ToHoliday();

            var model = new EditModel();

            model.Description = holiday.Description;
            model.Date = holiday.Date;

            if (model.Date.Date < DateTime.Now.Date)
            {
                ShowMessage("De feestdag kan niet plaatsvinden voor vandaag", MessageType.Danger);
                return View("Create", model);
            }

            if (_holService.HolidayExists(holiday.Date, holiday.HolidayID))
            {
                ShowMessage("Op deze datum is al een feestdag gepland.", MessageType.Danger);
                return View("Create", model);
            }


            if (ModelState.IsValid)
            {
                _holService.Save(holiday);
                TempData["Saved"] = "true";


                var list = _vacreqService.CheckUsersHaveVacation(holiday.Date);
                foreach (var item in list)
                {
                    _mutvacService.AddMinutes(item.UserID, SecurityHelper.GetUserId(), item.TotalMinutes);
                }

                return RedirectToAction("Index", "Holiday");
            }

            return View(input);

        }

        #endregion

        #region Edit
        [ClaimsAuthorize(UserRoles.HRManager)]
        public ActionResult Edit(long? id)
        {
            if (id != null)
            {
                var holiday = _holService.GetByID(id.Value);
                EditModel model = new EditModel(holiday);

                return View(model);
            }

            return RedirectToAction("Index", "Holiday");
        }

        [ClaimsAuthorize(UserRoles.HRManager)]
        [HttpPost]
        public ActionResult Edit([Bind(Include = "HolidayID, Date, Description")] EditModel input)
        {
            var olddate = _holService.OldDate(input.HolidayID);
            var holiday = _holService.GetByID(input.HolidayID);

            input.ToHoliday(holiday);

            var model = new EditModel(holiday);

            if (model.Date.Date < DateTime.Now.Date)
            {
                ShowMessage("De feestdag kan niet plaatsvinden voor vandaag", MessageType.Danger);
                return View("Create", model);
            }

            if (_holService.HolidayExists(holiday.Date, holiday.HolidayID))
            {
                ShowMessage("Op deze datum is al een feestdag gepland.", MessageType.Danger);
                return View("Create", model);
            }

            if (ModelState.IsValid)
            {
                _holService.Save(holiday);
                TempData["Changed"] = "true";

                //Eerst kijken of er op de oude datum nog verlofverzoeken staan, hier eerst dagen aan toe voegen
                var oldlist = _vacreqService.CheckUsersHaveVacation(olddate);
                foreach (var item in oldlist)
                {
                    _mutvacService.SubtractMinutes(item.UserID, SecurityHelper.GetUserId(), item.TotalMinutes);
                }

                var list = _vacreqService.CheckUsersHaveVacation(holiday.Date);
                foreach (var item in list)
                {
                    _mutvacService.AddMinutes(item.UserID, SecurityHelper.GetUserId(), item.TotalMinutes);
                }

                return RedirectToAction("Index", "Holiday");
            }

            return View(input);

        }
        #endregion

        #region Delete
        [ClaimsAuthorize(UserRoles.HRManager)]
        public ActionResult Delete(long? id)
        {
            if (id != null)
            {
                var holiday = _holService.GetByID(id.Value);

                _holService.Delete(id.Value);
                TempData["Delete"] = "true";

                var list = _vacreqService.CheckUsersHaveVacation(holiday.Date);
                foreach (var item in list)
                {
                    _mutvacService.SubtractMinutes(item.UserID, SecurityHelper.GetUserId(), item.TotalMinutes);
                }

                return RedirectToAction("Index", "Holiday");
            }
            return RedirectToAction("Index", "Holiday");
        }
        #endregion
    }
}