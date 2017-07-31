using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VVV.Interfaces.Services;
using VVV.Business.Identity;
using VVV.UI.Attributes;
using VVV.UI.Helpers;
using VVV.UI.ViewModels.Dashboard;
using PagedList;

namespace VVV.UI.Controllers
{
    public class DashBoardController : Controller
    {
        private IMutationsVacationService _mutvacService;
        private IVacationRequestService _vacreqService;
        private IApplicationUserService _userService;
        private IManagerService _manService;

        public DashBoardController(IMutationsVacationService mutvacService, IVacationRequestService vacreqService, IApplicationUserService userService, IManagerService manService)
        {
            _mutvacService = mutvacService;
            _vacreqService = vacreqService;
            _userService = userService;
            _manService = manService;
        }

        [ClaimsAuthorize(UserRoles.Medewerker)]
        public ActionResult UserDashboard()
        {
            var model = new UserModel();

            long id = SecurityHelper.GetUserId();
            if (id == 0)
            {
                return RedirectToAction("NoAccess", "Account");
            }

            
            
            //Verlofdagen over
            int TotalMinutes = _mutvacService.GetMinutesVacationByID(id);
            model.Time = (TotalMinutes / 60).ToString() + " uur" + " (" + (TotalMinutes / 480).ToString() + " dagen" + " & " + ((TotalMinutes % 480) / 60).ToString() + " uur) " + ((TotalMinutes % 480) % 60).ToString() + " minuten";

            //Verlofaanvragen
            //Verlof van vandaag
            model.VacationRequestToday = _vacreqService.GetCurrentVacation(id).ToPagedList(model.Page, model.PageSize);


            //Alle verlofaanvragen
            model.VacationRequest = _vacreqService.GetAll(id).ToPagedList(model.Page, model.PageSize);

            //Geaccepteerde verlofaanvragen
            model.VacationRequestAccepted = _vacreqService.GetAcceptedVacationBy3(id).ToPagedList(model.Page, model.PageSize);

            //Afgewezen verlofaanvragen
            model.VacationRequestRejected = _vacreqService.GetRejectedVacationBy3(id).ToPagedList(model.Page, model.PageSize);

            //Verlofaanvragen in behandeling
            model.VacationRequestInTreatment = _vacreqService.GetInTreatmentVacationBy3(id).ToPagedList(model.Page, model.PageSize);

            //Voorgestelde verlofverzoeken
            model.VacationRequestProposition = _vacreqService.GetPropositionVacationBy3(id).ToPagedList(model.Page, model.PageSize);

            //Gebruikersgegevens
            model.currentuser = _userService.GetById(id).ToPagedList(model.Page, model.PageSize);


            //Bijbehorende managerid van de huidige gebruiker
            long managerid = _userService.GetManageridByUserId(id);
            //Manager voor de huidige gebruiker
            long manageruserid = _manService.GetUserIDByManagerID(managerid);

            model.currentmanager = _userService.GetById(manageruserid).ToPagedList(model.Page, model.PageSize);
            

            return View(model);
        }

        [ClaimsAuthorize(UserRoles.HRManager)]
        public ActionResult HRManagerDashboard()
        {
            var model = new HRModel();

            long id = SecurityHelper.GetUserId();
            if (id == 0)
            {
                return RedirectToAction("NoAccess", "Account");
            }

            //Verlofdagen over
            int TotalMinutes = _mutvacService.GetMinutesVacationByID(id);
            model.Time = (TotalMinutes / 60).ToString() + " uur" + " (" + (TotalMinutes / 480).ToString() + " dagen" + " & " + ((TotalMinutes % 480) / 60).ToString() + " uur) " + ((TotalMinutes % 480) % 60).ToString() + " minuten";

            //Verlofaanvragen
            //Verlof van vandaag
            model.VacationRequestToday = _vacreqService.GetCurrentVacation(id).ToPagedList(model.Page, model.PageSize);


            //Alle verlofaanvragen
            model.VacationRequest = _vacreqService.GetAll(id).ToPagedList(model.Page, model.PageSize);

            //Geaccepteerde verlofaanvragen
            model.VacationRequestAccepted = _vacreqService.GetAcceptedVacationBy3(id).ToPagedList(model.Page, model.PageSize);

            //Afgewezen verlofaanvragen
            model.VacationRequestRejected = _vacreqService.GetRejectedVacationBy3(id).ToPagedList(model.Page, model.PageSize);

            //Verlofaanvragen in behandeling
            model.VacationRequestInTreatment = _vacreqService.GetInTreatmentVacationBy3(id).ToPagedList(model.Page, model.PageSize);

            //Voorgestelde verlofverzoeken
            model.VacationRequestProposition = _vacreqService.GetPropositionVacationBy3(id).ToPagedList(model.Page, model.PageSize);

            //Gebruikersgegevens
            model.currentuser = _userService.GetById(id).ToPagedList(model.Page, model.PageSize);


            //Bijbehorende managerid van de huidige gebruiker
            long managerid = _userService.GetManageridByUserId(id);
            //Manager voor de huidige gebruiker
            long manageruserid = _manService.GetUserIDByManagerID(managerid);

            model.currentmanager = _userService.GetById(manageruserid).ToPagedList(model.Page, model.PageSize);


            return View(model);
        }

        [ClaimsAuthorize(UserRoles.Manager)]
        public ActionResult ManagerDashboard()
        {
            var model = new ManagerModel();

            long userid = SecurityHelper.GetUserId();
            if (userid == 0)
            {
                return RedirectToAction("NoAccess", "Account");
            }

            long managerid = _manService.GetManageridByUserID(userid);
            model.IsAvailable = _manService.IsAvailable(managerid);

            List<long> list = new List<long>();
            list = _manService.Listwithavailablemanager().Select(p => p.ManagerID).ToList();

            model.FirstManagerNotAvailable = _vacreqService.FirstManagerHasVacation(managerid, list);

            //Verlofdagen over
            int TotalMinutes = _mutvacService.GetMinutesVacationByID(userid);
            model.Time = (TotalMinutes / 60).ToString() + " uur" + " (" + (TotalMinutes / 480).ToString() + " dagen" + " & " + ((TotalMinutes % 480) / 60).ToString() + " uur) " + ((TotalMinutes % 480) % 60).ToString() + " minuten"; 
            

            //Verlofaanvragen
            //Verlof van vandaag
            model.VacationRequestToday = _vacreqService.GetCurrentVacation(userid).ToPagedList(model.Page, model.PageSize);

            //Openstaande verlofverzoeken van onderstaande medewerkers van de manager
            model.VacationRequestAssess = _vacreqService.GetVacationRequestInTreatmentByManagerBy3(managerid).ToPagedList(model.Page, model.PageSize);
            //Openstaande verlofverzoeken van onderstaande medewerkers van de manager die als laatste zijn toegevoegd
            model.VacationRequestAssessNew = _vacreqService.GetVacationRequestInTreatmentNewByManagerBy3(managerid).ToPagedList(model.Page, model.PageSize);

            //Alle verlofaanvragen
            model.VacationRequest = _vacreqService.GetAll(userid).ToPagedList(model.Page, model.PageSize);

            //Geaccepteerde verlofaanvragen
            model.VacationRequestAccepted = _vacreqService.GetAcceptedVacationBy3(userid).ToPagedList(model.Page, model.PageSize);

            //Afgewezen verlofaanvragen
            model.VacationRequestRejected = _vacreqService.GetRejectedVacationBy3(userid).ToPagedList(model.Page, model.PageSize);

            //Verlofaanvragen in behandeling
            model.VacationRequestInTreatment = _vacreqService.GetInTreatmentVacationBy3(userid).ToPagedList(model.Page, model.PageSize);

            //Verlofaanvragen die voorgesteld zijn
            model.VacationRequestProposition = _vacreqService.GetPropositionVacationBy3(userid).ToPagedList(model.Page, model.PageSize);

            //Gebruikersgegevens
            model.currentuser = _userService.GetById(userid).ToPagedList(model.Page, model.PageSize);


            //Bijbehorende managerid van de huidige gebruiker
            long userhasmanagerid = _userService.Get(userid).Manager;
            //Manager voor de huidige gebruiker
            long manageruserid = _manService.GetUserIDByManagerID(userhasmanagerid);

            model.currentmanager = _userService.GetById(manageruserid).ToPagedList(model.Page, model.PageSize);


            return View(model);
        }
    }
}