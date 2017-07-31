using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Net.Mail;
using System.Text;
using System.Web.Mvc;
using DayPilot.Web.Mvc;
using DayPilot.Web.Mvc.Events.Month;
using VVV.Interfaces.Services;
using VVV.Business.Identity;
using VVV.Business.Mail;
using VVV.UI.Attributes;
using VVV.UI.ViewModels.Verlofaanvraag;
using VVV.UI.Helpers;
using PagedList;
using System.Threading.Tasks;




namespace VVV.UI.Controllers 
{
    public class VerlofaanvraagController : BaseController
    {
        private IVacationRequestService _vacreqService;
        private IMutationsVacationService _mutvacService;
        private IApplicationUserService _userService;
        private IManagerService _manService;
        private IMessageService _messService;
        private IHolidayService _holService;

        public VerlofaanvraagController(IVacationRequestService vacreqService, IMutationsVacationService mutvacService, IApplicationUserService userService, IManagerService manService, IMessageService messService, IHolidayService holService)
        {
            _vacreqService = vacreqService;
            _mutvacService = mutvacService;
            _userService = userService;
            _manService = manService;
            _messService = messService;
            _holService = holService;
        }

        #region Index Medewerker        
        //Haalt data op uit de interfaces laag, geeft deze weer (Alle verlofaanvragen, op userid. Tevens worden het resterende verlof van de gebruiker opgehaald).
        [ClaimsAuthorize(UserRoles.Medewerker)]
        public ActionResult Index(int? page, int? modelpage)
        {
            var model = new IndexModel();
            if(page != null)
            {
                model.Page = page.Value;
            }
            long id = model.userid;
            model.VacationRequest = _vacreqService.GetAll(id).ToPagedList(model.Page, model.PageSize);
            int TotalMinutes = _mutvacService.GetMinutesVacationByID(id);

            model.Time = (TotalMinutes / 60) + " uur (" + (TotalMinutes / 480) + " dagen & " + ((TotalMinutes % 480) / 60) + " uur) " + ((TotalMinutes % 480) % 60) + " minuten";
            

            //Zorgt ervoor dat er een berciht komt, dat het verlofverzoek juist is aangemaakt/ aangepast.
            var Msg = TempData["Saved"] as string;
            if (Msg != null && Msg == "true")
            {
                model.HasSuccesMessage = true;
            }
            else
            {
                model.HasSuccesMessage = false;
            }
            //Zorgt ervoor dat er een bericht komt, dat het verlofverzoek juist is verwijdert.
            Msg = TempData["Deleted"] as string;
            if (Msg != null && Msg == "true")
            {
                model.HasDeletedMessage = true;
            }
            else
            {
                model.HasDeletedMessage = false;
            }

            return View(model);
        }

        [ClaimsAuthorize(UserRoles.Medewerker)]
        public ActionResult AcceptedVacation()
        {
            var model = new IndexModel();
            long id = model.userid;
            model.VacationRequest = _vacreqService.GetAcceptedVacation(id).ToPagedList(model.Page, model.PageSize);
            return View(model);
        }

        [ClaimsAuthorize(UserRoles.Medewerker)]
        public ActionResult PropositionVacation()
        {
            var model = new IndexModel();
            long id = model.userid;
            model.VacationRequest = _vacreqService.GetPropositionVacation(id).ToPagedList(model.Page, model.PageSize);
            return View(model);
        }

        [ClaimsAuthorize(UserRoles.Medewerker)]
        public ActionResult RejectedVacation()
        {
            var model = new IndexModel();
            long id = model.userid;
            model.VacationRequest = _vacreqService.GetRejectedVacation(id).ToPagedList(model.Page, model.PageSize);
            return View(model);
        }

        [ClaimsAuthorize(UserRoles.Medewerker)]
        public ActionResult InTreatmentVacation()
        {
            var model = new IndexModel();
            long id = model.userid;
            model.VacationRequest = _vacreqService.GetInTreatmentVacation(id).ToPagedList(model.Page, model.PageSize);
            return View(model);
        }


        //Haalt data op uit de interfaces laag, geeft deze weer (verlofaanvragen waarvan de begindatum & einddatum voor de huidige datum ligt, verlof dat is vergaan).
        [ClaimsAuthorize(UserRoles.Medewerker)]
        public ActionResult OldIndex()
        {
            var model = new IndexModel();
            long id = model.userid;
            model.VacationRequest = _vacreqService.GetOldRequest(id).ToPagedList(model.Page, model.PageSize);

            return View(model);
        }

        //Haalt data op uit de interfaces laag, geeft deze weer (Verlof dat vandaag plaatsvindt, dat tevens goed is gekeurd door de manager).
        [ClaimsAuthorize(UserRoles.Medewerker)]
        public ActionResult CurrentVacation()
        {
            var model = new IndexModel();
            long id = model.userid;
            model.VacationRequest = _vacreqService.GetCurrentVacation(id).ToPagedList(model.Page, model.PageSize);

            return View(model);
        }

        #endregion

        #region Index Manager
        [ClaimsAuthorize(UserRoles.Manager)]
        public ActionResult IndexManager()
        {
            long userid = SecurityHelper.GetUserId();
            long managerid = _manService.GetManageridByUserID(userid);

            
            var model = new IndexModel();
            model.VacationRequest = _vacreqService.GetVacationRequestInTreatmentByManager(managerid).ToPagedList(model.Page, model.PageSize);

            var AcceptedMsg = TempData["Accepted"] as string;
            if (AcceptedMsg != null && AcceptedMsg == "true")
            {
                model.HasAcceptedMessage = true;
            }
            else
            {
                model.HasAcceptedMessage = false;
            }
            var RejectedMsg = TempData["Rejected"] as string;
            if (RejectedMsg != null && RejectedMsg == "true")
            {
                model.HasRejectedMessage = true;
            }
            else
            {
                model.HasRejectedMessage = false;
            }
            var PropositionMsg = TempData["Proposition"] as string;
            if (PropositionMsg != null && PropositionMsg == "true")
            {
                model.HasProporsitionMessage = true;
            }
            else
            {
                model.HasProporsitionMessage = false;
            }
            return View(model);
        }

        [ClaimsAuthorize(UserRoles.Manager)]
        public ActionResult AcceptedVacationManager()
        {
            long userid = SecurityHelper.GetUserId();
            long managerid = _manService.GetManageridByUserID(userid);

            var model = new IndexModel();
            model.VacationRequest = _vacreqService.GetVacationRequestAcceptedByManager(managerid).ToPagedList(model.Page, model.PageSize);

            return View(model);
        }

        [ClaimsAuthorize(UserRoles.Manager)]
        public ActionResult RejectedVacationManager()
        {
            long userid = SecurityHelper.GetUserId();
            long managerid = _manService.GetManageridByUserID(userid);

            var model = new IndexModel();
            model.VacationRequest = _vacreqService.GetVacationRequestRejectedByManager(managerid).ToPagedList(model.Page, model.PageSize);

            return View(model);
        }

        [ClaimsAuthorize(UserRoles.Manager)]
        public ActionResult OldIndexManager()
        {
            long userid = SecurityHelper.GetUserId();
            long managerid = _manService.GetManageridByUserID(userid);

            var model = new IndexModel();
            model.VacationRequest = _vacreqService.GetOldVacationRequestByManager(managerid).ToPagedList(model.Page, model.PageSize);

            return View(model);
        }



        #endregion

        #region Index SecondManager
        [ClaimsAuthorize(UserRoles.Manager)]
        public ActionResult IndexSecondManager()
        {
            long userid = SecurityHelper.GetUserId();
            long managerid = _manService.GetManageridByUserID(userid);

            List<long> list = new List<long>();
            list = _manService.Listwithavailablemanager().Select(p => p.ManagerID).ToList();
            
            var vacationrequests = _vacreqService.GetVacationRequestInTreatmentBySecondManager(managerid, list);

            var model = new IndexModel();
            model.VacationRequest = vacationrequests.ToPagedList(model.Page, model.PageSize);

            return View(model);
        }

        [ClaimsAuthorize(UserRoles.Manager)]
        public ActionResult AcceptedVacationSecondManager()
        {
            long userid = SecurityHelper.GetUserId();
            long managerid = _manService.GetManageridByUserID(userid);

            var model = new IndexModel();
            model.VacationRequest = _vacreqService.GetVacationRequestAcceptedBySecondManager(managerid).ToPagedList(model.Page, model.PageSize);

            return View(model);
        }

        [ClaimsAuthorize(UserRoles.Manager)]
        public ActionResult RejectedVacationSecondManager()
        {
            long userid = SecurityHelper.GetUserId();
            long managerid = _manService.GetManageridByUserID(userid);

            var model = new IndexModel();
            model.VacationRequest = _vacreqService.GetVacationRequestRejectedBySecondManager(managerid).ToPagedList(model.Page, model.PageSize);

            return View(model);
        }

        [ClaimsAuthorize(UserRoles.Manager)]
        public ActionResult OldIndexSecondManager()
        {
            long userid = SecurityHelper.GetUserId();
            long managerid = _manService.GetManageridByUserID(userid);

            var model = new IndexModel();
            model.VacationRequest = _vacreqService.GetOldVacationRequestBySecondManager(managerid).ToPagedList(model.Page, model.PageSize);

            return View(model);
        }

        #endregion

        #region Create
        //Geeft de pagina weer om een verlof aanvraag te doen
        [ClaimsAuthorize(UserRoles.Medewerker)]
        public ActionResult Create()
        {
            //Get UserID van huidige gebruiker
            long id = SecurityHelper.GetUserId();
            //Haal aantal verlofminuten op van de huidige gebruiker
            int TotalMinutes = _mutvacService.GetMinutesVacationByID(id);

            //Haal managerinformatie op van de huidige gebruiker
            long managerid = _userService.GetManageridByUserId(id);
            long? secondmanagerid = _userService.GetSecondManageridByUserID(id);
            
            var manager = _userService.Get(_manService.GetUserIDByManagerID(managerid));

            if (secondmanagerid != null)
            {
                var secondmanager = _userService.Get(_manService.GetUserIDByManagerID(secondmanagerid.Value));

                EditModel secondmanagermodel = new EditModel(TotalMinutes, manager, secondmanager);
                secondmanagermodel.previousUrl = Request.UrlReferrer;
                
                return View(secondmanagermodel);
            }

            //Vul het editmodel met data, wordt niet meer gebruikt. Een medewerker heeft altijd een tweede manager.
            EditModel model = new EditModel(TotalMinutes, manager);
            model.previousUrl = Request.UrlReferrer;

            

            return View(model);
        }

        [HttpPost]
        [ClaimsAuthorize(UserRoles.Medewerker)]
        public async Task<ActionResult> Create([Bind(Include = "Reason, CompleteDays, BeginTime, EndTime, Days, Hours, Minutes, BeginHour, BeginMinute, BeginDate, EndDate, IsCommunicated, HasDeadlines, previousUrl, UserBeginTime, UserEndTime")] EditModel input)
        {
            var vacreq = input.ToVacationRequest();
            

            //Get UserID van huidige gebruiker
            long id = SecurityHelper.GetUserId();
            //Haal aantal verlofminuten op van de huidige gebruiker
            int TotalMinutes = _mutvacService.GetMinutesVacationByID(id);

            //Haal managerinformatie op van de huidige gebruiker
            long managerid = _userService.GetManageridByUserId(id);
            long? secondmanagerid = _userService.GetSecondManageridByUserID(id);

            var currentmanager = _userService.Get(_manService.GetUserIDByManagerID(managerid));

            if (secondmanagerid != null)
            {
                var secondmanager = _userService.Get(_manService.GetUserIDByManagerID(secondmanagerid.Value));
                input.HasSecondManager = true;
                input.SecondManager = secondmanager;

                vacreq.SecondManagerID = secondmanagerid;
            }


            //Slaat de bijbehorende gebruiker op
            vacreq.ApplicationUser = _userService.Get(id);
            //Slaat de bijbehorende manager op
            vacreq.ManagerID = managerid;
            vacreq.AssessedByID = null;

            //Vul de input met algemene data (rechterkolom).
            input.CurrentManager = currentmanager;
            input.Time = (TotalMinutes / 60).ToString() + " uur" + " (" + (TotalMinutes / 480).ToString() + " dagen" + " & " + ((TotalMinutes % 480) / 60).ToString() + " uur) " + ((TotalMinutes % 480) % 60).ToString() + " minuten";
            input.Days = Convert.ToDouble(TotalMinutes / 480);
            input.Hours = (TotalMinutes % 480) / 60;
            input.Minutes = (TotalMinutes % 480) % 60;
                                                
            //Foutmeldingen, sommige worden alleen gecontroleerd op het moment dat de medewerker een tijd moet aangeven. Dus niet wanneer "Hele dagen" true is.
            if (vacreq.BeginDate.Date < DateTime.Today)
            {
                ShowMessage("Begindatum van het verlof niet correct, verlof aanvragen in het verleden is niet mogelijk.", MessageType.Danger);
                return View("Create", input);
            }

            if (vacreq.EndDate.Date < vacreq.BeginDate.Date)
            {
                ShowMessage("Einddatum kan niet voor de begindatum liggen.", MessageType.Danger);
                return View("Create", input);
            }

            if (vacreq.IsTotalDays == false)
            {
                if (vacreq.EndDate < vacreq.BeginDate)
                {
                    ShowMessage("Eindtijd kan niet voor de begintijd liggen", MessageType.Danger);
                    return View("Create", input);
                }
            }
            

            if (vacreq.BeginDate.DayOfWeek == DayOfWeek.Saturday || vacreq.BeginDate.DayOfWeek == DayOfWeek.Sunday || vacreq.EndDate.DayOfWeek == DayOfWeek.Saturday || vacreq.EndDate.DayOfWeek == DayOfWeek.Sunday)
            {
                ShowMessage("Begin- of einddatum kan niet in het weekend vallen. Selecteer een datum die op de dagen: maandag, dinsdag, woensdag, donderdag of vrijdag valt.", MessageType.Danger);
                return View("Create", input);
            }

            if (vacreq.IsTotalDays == false)
            {
                if (vacreq.BeginDate.Hour < 8 || vacreq.BeginDate.Hour >= 18 && vacreq.BeginDate.Minute >= 1)
                {
                    ShowMessage("Begintijd niet binnen kantooruren", MessageType.Danger);
                    return View("Create", input);
                }

                if (vacreq.BeginOfDay.Value.Hour < 8 || vacreq.BeginOfDay.Value.Hour >= 18 && vacreq.BeginOfDay.Value.Minute >= 1)
                {
                    ShowMessage("Begin van uw dag niet binnen de katooruren", MessageType.Danger);
                    return View("Create", input);
                }

                if (vacreq.BeginDate.Hour < vacreq.BeginOfDay.Value.Hour || (vacreq.BeginDate.Hour == vacreq.BeginOfDay.Value.Hour && vacreq.BeginDate.Minute < vacreq.BeginOfDay.Value.Minute))
                {
                    ShowMessage("De begintijd van uw verlofverzoek kan niet eerder zijn dan de begintijd van uw werkdag.", MessageType.Danger);
                    return View("Create", input);
                }
            }

            if (vacreq.IsTotalDays == false)
            {
                if (vacreq.EndDate.Hour < 8 || vacreq.EndDate.Hour >= 18 && vacreq.EndDate.Minute >= 1)
                {
                    ShowMessage("Eindtijd niet binnen de kantooruren.", MessageType.Danger);
                    return View("Create", input);
                }

                if (vacreq.EndOfDay.Value.Hour < 8 || vacreq.EndOfDay.Value.Hour >= 18 && vacreq.EndOfDay.Value.Minute >= 1)
                {
                    ShowMessage("Einde van uw werkdag valt niet binnen de kantooruren.", MessageType.Danger);
                    return View("Create", input);
                }

                if (vacreq.EndDate.Hour > vacreq.EndOfDay.Value.Hour || (vacreq.EndDate.Hour == vacreq.EndOfDay.Value.Hour && vacreq.EndDate.Minute > vacreq.EndOfDay.Value.Minute))
                {
                    ShowMessage("De eindtijd van uw verlofverzoek kan niet later zijn dan de eindtijd van uw werkdag", MessageType.Danger);
                    return View("Create", input);
                }
            }


            if (vacreq.IsTotalDays == false)
            {
                if (vacreq.BeginDate == vacreq.EndDate)
                {
                    ShowMessage("De eindtjd van uw verlofverzoek kan niet gelijk zijn aan de begintijd van het verlofverzoek.", MessageType.Danger);
                    return View("Create", input);
                }

                if (vacreq.BeginOfDay == vacreq.EndOfDay)
                {
                    ShowMessage("Het begin van uw werkdag kan niet gelijk zijn aan het einde van uw werkdag.", MessageType.Danger);
                    return View("Create", input);
                }
                
                
                if ((vacreq.EndOfDay.Value.Hour - vacreq.BeginOfDay.Value.Hour) > 8 || (((vacreq.EndOfDay.Value.Hour - vacreq.BeginOfDay.Value.Hour) == 8) && (vacreq.EndOfDay.Value.Minute - vacreq.EndOfDay.Value.Minute) >= 1))
                {
                    ShowMessage("Uw werkdag kan niet langer zijn dan 8 uur. Maak of uw werkdag langer, of neem langer pauze per dag!", MessageType.Danger);
                    return View("Create", input);
                }
            }

            if (_vacreqService.HasVacation(vacreq.BeginDate, vacreq.EndDate, vacreq.UserID))
            {
                ShowMessage("U heeft in deze periode al verlof aangevraagd.", MessageType.Danger);
                return View("Create", input);
            }

            var user = _userService.Get(SecurityHelper.GetUserId());
            var manager = _userService.Get(_manService.GetUserIDByManagerID(_userService.GetManageridByUserId(id)));


            if (ModelState.IsValid)
            {                
                
                //Bericht in berichtencentrum
                _messService.MailNewVacationRequest(user, manager, vacreq);

                string begindatum;
                string einddatum;

                if (vacreq.IsTotalDays == true)
                {
                    begindatum = vacreq.BeginDate.Day.ToString() + "/" + vacreq.BeginDate.Month.ToString() + "/" + vacreq.BeginDate.Year.ToString();
                    einddatum = vacreq.EndDate.Day.ToString() + "/" + vacreq.EndDate.Month.ToString() + "/" + vacreq.EndDate.Year.ToString() + ", de medewerker zal hele dagen afwezig";
                }
                else
                {
                    begindatum = vacreq.BeginDate.Day.ToString() + "/" + vacreq.BeginDate.Month.ToString() + "/" + vacreq.BeginDate.Year.ToString() + ", " + vacreq.BeginDate.ToString("HH:mm") + " uur";
                    einddatum = vacreq.EndDate.Day.ToString() + "/" + vacreq.EndDate.Month.ToString() + "/" + vacreq.EndDate.Year.ToString() + ", " + vacreq.EndDate.ToString("HH:mm") + " uur";
                }

                //Mail naar de manager
                await MailHelper.NewVacationRequestManager(manager.Email, manager.FirstName + " " + manager.LastName, user.FirstName + " " + user.LastName, vacreq.Reason, begindatum, einddatum);

                //Wordt gebruikt voor een melding te genereren op de volgende pagina dat het opslaan gelukt is.
                _vacreqService.Save(vacreq);
                TempData["Saved"] = "true";

                return RedirectToAction("Index", "Verlofaanvraag");
            }

            return View(input);
        }

        #endregion

        #region Create Manager (voorstel maken)

        [ClaimsAuthorize(UserRoles.Manager)]
        public ActionResult CreateManager(long? id)
        {
            if (id != null)
            {
                var vacreq = _vacreqService.Get(id.Value);

                //long userid = vacreq.UserID;

                var user = _userService.Get(vacreq.UserID);

                int TotalMinutes = _mutvacService.GetMinutesVacationByID(vacreq.UserID);

                //long managerid = _userService.GetManageridByUserId(vacreq.UserID).Manager;
                long manageruserid = _manService.GetUserIDByManagerID(_userService.GetManageridByUserId(vacreq.UserID));


                var currentmanager = _userService.Get(manageruserid);

                EditModel model = new EditModel(TotalMinutes, currentmanager, vacreq, user);

                return View(model);
            }

            return RedirectToAction("IndexManager", "Verlofaanvraag");
            
        }

        [HttpPost]
        [ClaimsAuthorize(UserRoles.Manager)]
        public ActionResult CreateManager([Bind(Include = "VacationID, UserID, RejectionReason, Reason, CompleteDays, BeginTime, EndTime, BeginHour, BeginMinute, BeginDate, EndDate, IsCommunicated, HasDeadlines, UserBeginTime, UserEndTime")] EditModel input)
        {
            {
                DateTime OldBeginDate = _vacreqService.GetOldBeginDate(input.VacationID);
                DateTime OldEndDate = _vacreqService.GetOldEndDate(input.VacationID);
                int OldTotalMinutes = _vacreqService.GetOldTotalMinutes(input.VacationID);
                bool OldIsTotalDays = _vacreqService.GetOldIsTotalDays(input.VacationID);
                bool IsCommunicated = _vacreqService.Get(input.VacationID).IsCommunicated;
                bool HasDeadlines = _vacreqService.Get(input.VacationID).HasDeadlines;

                
                var vacreq = _vacreqService.Get(input.VacationID);
                vacreq.IsApproved = false;
                vacreq.IsRejected = false;
                vacreq.IsInTreatment = false;
                    
                input.ToEditVacationRequest(vacreq);

                vacreq.OldBeginDate = OldBeginDate;
                vacreq.OldEndDate = OldEndDate;
                vacreq.OldTotalMinutes = OldTotalMinutes;
                vacreq.IsOldTotalDays = OldIsTotalDays;

                vacreq.IsCommunicated = IsCommunicated;
                vacreq.HasDeadlines = HasDeadlines;

                //Get UserID van huidige gebruiker
                long id = vacreq.UserID;
                //Haal aantal verlofminuten op van de huidige gebruiker
                int TotalMinutes = _mutvacService.GetMinutesVacationByID(id);

                //Haal managerinformatie op van de huidige gebruiker
                long managerid = _userService.GetManageridByUserId(id);
                long? secondmanagerid = _userService.GetSecondManageridByUserID(id);

                var currentmanager = _userService.Get(_manService.GetUserIDByManagerID(managerid));

                if (secondmanagerid != null)
                {
                    var secondmanager = _userService.Get(_manService.GetUserIDByManagerID(secondmanagerid.Value));
                    input.HasSecondManager = true;
                    input.SecondManager = secondmanager;
                }
                //Slaat de bijbehorende gebruiker op
                vacreq.ApplicationUser = _userService.Get(id);
                //Slaat de bijbehorende manager op
                vacreq.ManagerID = managerid;

                //Vul de input met algemene data (rechterkolom).
                input.CurrentManager = currentmanager;
                input.Days = Convert.ToDouble(TotalMinutes / 480);
                input.Hours = (TotalMinutes % 480) / 60;
                input.Minutes = (TotalMinutes % 480) % 60;
                input.Name = vacreq.ApplicationUser.FirstName + " " + vacreq.ApplicationUser.LastName;
                input.IsCommunicated = IsCommunicated;
                input.HasDeadlines = HasDeadlines;

                //Foutmeldingen, sommige worden alleen gecontroleerd op het moment dat de medewerker een tijd moet aangeven. Dus niet wanneer "Hele dagen" true is.
                if (vacreq.BeginDate.Date < DateTime.Today)
                {
                    ShowMessage("Begindatum van het verlof niet correct, verlof voorstellen in het verleden is niet mogelijk.", MessageType.Danger);
                    return View("CreateManager", input);
                }

                if (vacreq.EndDate.Date < vacreq.BeginDate.Date)
                {
                    ShowMessage("Einddatum van het verlofverzoek kan niet voor de begindatum van het verlofverzoek liggen.", MessageType.Danger);
                    return View("CreateManager", input);
                }

                if (vacreq.IsTotalDays == false)
                {
                    if (vacreq.EndDate < vacreq.BeginDate)
                    {
                        ShowMessage("Eindtijd van het verlofverzoek kan niet voor de begintijd van het verlofverzoek liggen", MessageType.Danger);
                        return View("CreateManager", input);
                    }
                }


                if (vacreq.BeginDate.DayOfWeek == DayOfWeek.Saturday || vacreq.BeginDate.DayOfWeek == DayOfWeek.Sunday || vacreq.EndDate.DayOfWeek == DayOfWeek.Saturday || vacreq.EndDate.DayOfWeek == DayOfWeek.Sunday)
                {
                    ShowMessage("Begin- of einddatum kan niet in het weekend vallen. Selecteer een datum die op de dagen: maandag, dinsdag, woensdag, donderdag of vrijdag valt.", MessageType.Danger);
                    return View("CreateManager", input);
                }

                if (vacreq.IsTotalDays == false)
                {
                    if (vacreq.BeginDate.Hour < 8 || vacreq.BeginDate.Hour >= 18 && vacreq.BeginDate.Minute >= 1)
                    {
                        ShowMessage("Begintijd niet binnen kantooruren", MessageType.Danger);
                        return View("CreateManager", input);
                    }

                    if (vacreq.BeginOfDay.Value.Hour < 8 || vacreq.BeginOfDay.Value.Hour >= 18 && vacreq.BeginOfDay.Value.Minute >= 1)
                    {
                        ShowMessage("Begin van uw dag niet binnen de katooruren", MessageType.Danger);
                        return View("CreateManager", input);
                    }

                    if (vacreq.BeginDate.Hour < vacreq.BeginOfDay.Value.Hour || (vacreq.BeginDate.Hour == vacreq.BeginOfDay.Value.Hour && vacreq.BeginDate.Minute < vacreq.BeginOfDay.Value.Minute))
                    {
                        ShowMessage("De begintijd van uw verlofverzoek kan niet eerder zijn dan de begintijd van uw werkdag.", MessageType.Danger);
                        return View("CreateManager", input);
                    }
                }

                if (vacreq.IsTotalDays == false)
                {
                    if (vacreq.EndDate.Hour < 8 || vacreq.EndDate.Hour >= 18 && vacreq.EndDate.Minute >= 1)
                    {
                        ShowMessage("Eindtijd niet binnen de kantooruren.", MessageType.Danger);
                        return View("CreateManager", input);
                    }

                    if (vacreq.EndOfDay.Value.Hour < 8 || vacreq.EndOfDay.Value.Hour >= 18 && vacreq.EndOfDay.Value.Minute >= 1)
                    {
                        ShowMessage("Einde van uw werkdag valt niet binnen de kantooruren.", MessageType.Danger);
                        return View("CreateManager", input);
                    }

                    if (vacreq.EndDate.Hour > vacreq.EndOfDay.Value.Hour || (vacreq.EndDate.Hour == vacreq.EndOfDay.Value.Hour && vacreq.EndDate.Minute > vacreq.EndOfDay.Value.Minute))
                    {
                        ShowMessage("De eindtijd van uw verlofverzoek kan niet later zijn dan de eindtijd van uw werkdag", MessageType.Danger);
                        return View("CreateManager", input);
                    }
                }


                if (vacreq.IsTotalDays == false)
                {
                    if (vacreq.BeginDate == vacreq.EndDate)
                    {
                        ShowMessage("De eindtjd van uw verlofverzoek kan neit gelijk zijn aan de begintijd van het verlofverzoek.", MessageType.Danger);
                        return View("CreateManager", input);
                    }

                    if (vacreq.BeginOfDay == vacreq.EndOfDay)
                    {
                        ShowMessage("Het begin van de werkdag kan niet gelijk zijn aan het einde van de werkdag.", MessageType.Danger);
                        return View("CreateManager", input);
                    }

                    if ((vacreq.EndOfDay.Value.Hour - vacreq.BeginOfDay.Value.Hour) > 8 || (((vacreq.EndOfDay.Value.Hour - vacreq.BeginOfDay.Value.Hour) == 8) && (vacreq.EndOfDay.Value.Minute - vacreq.EndOfDay.Value.Minute) >= 1))
                    {
                        ShowMessage("De werkdag kan niet langer zijn dan 8 uur. Let op, bij uw werkdag wordt de pauze niet meegerekend!", MessageType.Danger);
                        return View("CreateManager", input);
                    }
                }

                if (_vacreqService.HasVacation(vacreq.BeginDate, vacreq.EndDate, vacreq.UserID, vacreq.VacationID))
                {
                    ShowMessage("De medewerker heeft in deze periode al verlof aangevraagd.", MessageType.Danger);
                    return View("CreateManager", input);
                }


                if (ModelState.IsValid)
                {
                    
                    TempData["Proposition"] = "true";
                    _vacreqService.Save(vacreq);

                   return RedirectToAction("IndexManager", "Verlofaanvraag");
                }

                return View(input);
            }

            
        }
        #endregion

        #region Edit Medewerker

        public ActionResult EditMedewerker(long? id)
        {
            if (id != null)
            {
                var vacreq = _vacreqService.Get(id.Value);

                if (vacreq.UserID != SecurityHelper.GetUserId())
                {
                    return RedirectToAction("NoAccess", "Account");
                }

                var user = _userService.Get(vacreq.UserID);

                int TotalMinutes = _mutvacService.GetMinutesVacationByID(vacreq.UserID);

                long manageruserid = _manService.GetUserIDByManagerID(_userService.GetManageridByUserId(vacreq.UserID));

                var currentmanager = _userService.Get(manageruserid);

                EditModel model = new EditModel(TotalMinutes, currentmanager, vacreq, user);

                return View(model);
            }

            return RedirectToAction("Index", "Verlofaanvraag");
        }

        [HttpPost]
        public async Task<ActionResult> EditMedewerker([Bind(Include = "VacationID, UserID,  Reason, CompleteDays, BeginTime, EndTime, BeginHour, BeginMinute, BeginDate, EndDate, IsCommunicated, HasDeadlines, UserBeginTime, UserEndTime")] EditModel input)
        {
            {
                var oldbegindate = _vacreqService.Get(input.VacationID).BeginDate;
                var oldenddate = _vacreqService.Get(input.VacationID).EndDate;
                var vacreq = _vacreqService.Get(input.VacationID);

                input.ToEditVacationRequest(vacreq);

                //Get UserID van huidige gebruiker
                long id = vacreq.UserID;
                //Haal aantal verlofminuten op van de huidige gebruiker
                int TotalMinutes = _mutvacService.GetMinutesVacationByID(id);

                //Haal managerinformatie op van de huidige gebruiker
                long managerid = _userService.GetManageridByUserId(id);
                
                var currentmanager = _userService.Get(_manService.GetUserIDByManagerID(managerid));

                //Slaat de bijbehorende gebruiker op
                vacreq.ApplicationUser = _userService.Get(id);
                //Slaat de bijbehorende manager op
                vacreq.ManagerID = managerid;

                //Vul de input met algemene data (rechterkolom).
                input.CurrentManager = currentmanager;
                input.Days = Convert.ToDouble(TotalMinutes / 480);
                input.Hours = (TotalMinutes % 480) / 60;
                input.Minutes = (TotalMinutes % 480) % 60;
                input.Name = vacreq.ApplicationUser.FirstName + " " + vacreq.ApplicationUser.LastName;

                //Foutmeldingen, sommige worden alleen gecontroleerd op het moment dat de medewerker een tijd moet aangeven. Dus niet wanneer "Hele dagen" true is.
                if (vacreq.BeginDate.Date < DateTime.Today)
                {
                    ShowMessage("Begindatum van het verlof niet correct, verlof aanvragen in het verleden is niet mogelijk.", MessageType.Danger);
                    return View("EditMedewerker", input);
                }

                if (vacreq.EndDate.Date < vacreq.BeginDate.Date)
                {
                    ShowMessage("Einddatum kan niet voor de begindatum liggen.", MessageType.Danger);
                    return View("EditMedewerker", input);
                }

                if (vacreq.IsTotalDays == false)
                {
                    if (vacreq.EndDate < vacreq.BeginDate)
                    {
                        ShowMessage("Eindtijd kan niet voor de begintijd liggen", MessageType.Danger);
                        return View("EditMedewerker", input);
                    }
                }


                if (vacreq.BeginDate.DayOfWeek == DayOfWeek.Saturday || vacreq.BeginDate.DayOfWeek == DayOfWeek.Sunday || vacreq.EndDate.DayOfWeek == DayOfWeek.Saturday || vacreq.EndDate.DayOfWeek == DayOfWeek.Sunday)
                {
                    ShowMessage("Begin- of einddatum kan niet in het weekend vallen. Selecteer een datum die op de dagen: maandag, dinsdag, woensdag, donderdag of vrijdag valt.", MessageType.Danger);
                    return View("EditMedewerker", input);
                }

                if (vacreq.IsTotalDays == false)
                {
                    if (vacreq.BeginDate.Hour < 8 || vacreq.BeginDate.Hour >= 18 && vacreq.BeginDate.Minute >= 1)
                    {
                        ShowMessage("Begintijd niet binnen kantooruren", MessageType.Danger);
                        return View("EditMedewerker", input);
                    }

                    if (vacreq.BeginOfDay.Value.Hour < 8 || vacreq.BeginOfDay.Value.Hour >= 18 && vacreq.BeginOfDay.Value.Minute >= 1)
                    {
                        ShowMessage("Begin van uw dag niet binnen de katooruren", MessageType.Danger);
                        return View("EditMedewerker", input);
                    }

                    if (vacreq.BeginDate.Hour < vacreq.BeginOfDay.Value.Hour || (vacreq.BeginDate.Hour == vacreq.BeginOfDay.Value.Hour && vacreq.BeginDate.Minute < vacreq.BeginOfDay.Value.Minute))
                    {
                        ShowMessage("De begintijd van uw verlofverzoek kan niet eerder zijn dan de begintijd van uw werkdag.", MessageType.Danger);
                        return View("EditMedewerker", input);
                    }
                }

                if (vacreq.IsTotalDays == false)
                {
                    if (vacreq.EndDate.Hour < 8 || vacreq.EndDate.Hour >= 18 && vacreq.EndDate.Minute >= 1)
                    {
                        ShowMessage("Eindtijd niet binnen de kantooruren.", MessageType.Danger);
                        return View("EditMedewerker", input);
                    }

                    if (vacreq.EndOfDay.Value.Hour < 8 || vacreq.EndOfDay.Value.Hour >= 18 && vacreq.EndOfDay.Value.Minute >= 1)
                    {
                        ShowMessage("Einde van uw werkdag valt niet binnen de kantooruren.", MessageType.Danger);
                        return View("EditMedewerker", input);
                    }

                    if (vacreq.EndDate.Hour > vacreq.EndOfDay.Value.Hour || (vacreq.EndDate.Hour == vacreq.EndOfDay.Value.Hour && vacreq.EndDate.Minute > vacreq.EndOfDay.Value.Minute))
                    {
                        ShowMessage("De eindtijd van uw verlofverzoek kan niet later zijn dan de eindtijd van uw werkdag", MessageType.Danger);
                        return View("EditMedewerker", input);
                    }
                }


                if (vacreq.IsTotalDays == false)
                {
                    if (vacreq.BeginDate == vacreq.EndDate)
                    {
                        ShowMessage("De eindtjd van uw verlofverzoek kan neit gelijk zijn aan de begintijd van het verlofverzoek.", MessageType.Danger);
                        return View("EditMedewerker", input);
                    }

                    if (vacreq.BeginOfDay == vacreq.EndOfDay)
                    {
                        ShowMessage("Het begin van uw werkdag kan niet gelijk zijn aan het einde van uw werkdag.", MessageType.Danger);
                        return View("EditMedewerker", input);
                    }

                    if ((vacreq.EndOfDay.Value.Hour - vacreq.BeginOfDay.Value.Hour) > 8 || (((vacreq.EndOfDay.Value.Hour - vacreq.BeginOfDay.Value.Hour) == 8) && (vacreq.EndOfDay.Value.Minute - vacreq.EndOfDay.Value.Minute) >= 1))
                    {
                        ShowMessage("Uw werkdag kan niet langer zijn dan 8 uur. Let op, bij uw werkdag wordt de pauze niet meegerekend!", MessageType.Danger);
                        return View("EditMedewerker", input);
                    }
                }

                if (_vacreqService.HasVacation(vacreq.BeginDate, vacreq.EndDate, vacreq.UserID, vacreq.VacationID))
                {
                    ShowMessage("U heeft in deze periode al verlof aangevraagd.", MessageType.Danger);
                    return View("EditMedewerker", input);
                }

                if (ModelState.IsValid)
                {
                    string begindatum;
                    string einddatum;

                    if (vacreq.IsTotalDays == true)
                    {
                        begindatum = vacreq.BeginDate.Day.ToString() + "/" + vacreq.BeginDate.Month.ToString() + "/" + vacreq.BeginDate.Year.ToString();
                        einddatum = vacreq.EndDate.Day.ToString() + "/" + vacreq.EndDate.Month.ToString() + "/" + vacreq.EndDate.Year.ToString() + ", de medewerker zal hele dagen afwezig";
                    }
                    else
                    {
                        begindatum = vacreq.BeginDate.Day.ToString() + "/" + vacreq.BeginDate.Month.ToString() + "/" + vacreq.BeginDate.Year.ToString() + ", " + vacreq.BeginDate.ToString("HH:mm") + " uur";
                        einddatum = vacreq.EndDate.Day.ToString() + "/" + vacreq.EndDate.Month.ToString() + "/" + vacreq.EndDate.Year.ToString() + ", " + vacreq.EndDate.ToString("HH:mm") + " uur";
                    }

                    //Mail naar de manager
                    await MailHelper.NewVacationRequestManager(currentmanager.Email, currentmanager.FirstName + " " + currentmanager.LastName, vacreq.ApplicationUser.FirstName + " " + vacreq.ApplicationUser.LastName, vacreq.Reason, begindatum, einddatum);


                    _vacreqService.Save(vacreq);

                    TempData["Saved"] = "true";

                    return RedirectToAction("Details", "Verlofaanvraag", new { id = input.VacationID });
                }

                return View(input);
            }


        }

        #endregion

        #region Details Medewerker
        [ClaimsAuthorize(UserRoles.Medewerker)]
        public ActionResult Details(long? id)
        {
            if(id != null)
            {
                var vacreq = _vacreqService.Getbyid(id.Value);
                long userid = vacreq.UserID;
                var user = _userService.Get(userid);
                
                if (vacreq.IsInTreatment == true)
                {
                    var manager = _userService.Get(_manService.GetUserIDByManagerID(user.Manager));
                    var model = new DetailsModel(vacreq, user, manager);
                    return View(model);
                }

                if (vacreq.AssessedByID != null)
                {
                    long? assessedbyid = vacreq.AssessedByID;
                    
                    var assessor = _userService.Get(assessedbyid.Value);
                    
                    var assessedbymodel = new DetailsModel(vacreq, user, assessor);
                    return View(assessedbymodel);
                }
                
            }
            return RedirectToAction("Index", "Verlofaanvraag");

        }
        #endregion

        #region Details Manager
        [ClaimsAuthorize(UserRoles.Manager)]
        public ActionResult DetailsManager(long? id)
        {
            if (id != null)
            {
                var vacreq = _vacreqService.Getbyid(id.Value);
                long userid = vacreq.UserID;
                var user = _userService.Get(userid);
                var manager = _userService.Get(_manService.GetUserIDByManagerID(user.Manager));

                var model = new DetailsModel(vacreq, user, manager);
                return View(model);
            }

            return RedirectToAction("IndexManager", "Verlofaanvraag");
        }

        #endregion

        #region Assess PropositionRequest
        [ClaimsAuthorize(UserRoles.Medewerker)]
        public ActionResult AssessRequest(long? id)
        {
            if (id != null)
            {
                var vacreq = _vacreqService.Getbyid(id.Value);

                if (vacreq.UserID != SecurityHelper.GetUserId())
                {
                    return RedirectToAction("NoAccess", "Account");
                }

                long userid = vacreq.UserID;
                var user = _userService.Get(userid);
                var man = _userService.Get(vacreq.AssessedByID.Value);

                int mutvac = _mutvacService.GetMinutesVacationByID(userid);

                var model = new DetailsModel(vacreq, user, man, mutvac);

                return View(model);
            }
            return RedirectToAction("PropositionVacation", "Verlofaanvraag");
        }
        #endregion

        #region Assess Request Manager
        [ClaimsAuthorize(UserRoles.Manager)]
        public ActionResult AssessRequestManager(long? id)
        {
            if (id != null)
            {
                var vacreq = _vacreqService.Getbyid(id.Value);

                if (vacreq.ManagerID != _manService.GetManageridByUserID(SecurityHelper.GetUserId()) && vacreq.SecondManagerID != _manService.GetManageridByUserID(SecurityHelper.GetUserId()))
                {
                    return RedirectToAction("NoAccess", "Account");
                }



                long userid = vacreq.UserID;
                var user = _userService.Get(userid);

                int mutvac = _mutvacService.GetMinutesVacationByID(userid);
                var Intersections = _vacreqService.GetUsersHasVacation(vacreq.BeginDate, vacreq.EndDate, vacreq.UserID, vacreq.ManagerID);

                bool IsAvailable = _manService.IsAvailable(_manService.GetManageridByUserID(SecurityHelper.GetUserId()));
                var model = new DetailsModel(vacreq, user, mutvac, IsAvailable);
                model.Intersections = Intersections.ToPagedList(model.Page, model.PageSize);
                
                
                return View(model);
            }
            return RedirectToAction("IndexManager", "Verlofaanvraag");
        }



        #endregion

        #region Accept
        [ClaimsAuthorize(UserRoles.Medewerker)]
        public ActionResult AcceptProposition(long? id)
        {
            if (id != null)
            {
                var vacreq = _vacreqService.Get(id.Value);

                vacreq.IsInTreatment = true;               

                _vacreqService.Save(vacreq);

                return RedirectToAction("PropositionVacation", "Verlofaanvraag");
            }
            return RedirectToAction("PropositionVacation", "Verlofaanvraag");
        }

        [ClaimsAuthorize(UserRoles.Manager)]
        public async Task<ActionResult> Accept(long id)
        {
            var vacreq = _vacreqService.Get(id);

            if (vacreq.ManagerID != _manService.GetManageridByUserID(SecurityHelper.GetUserId()) && vacreq.SecondManagerID != _manService.GetManageridByUserID(SecurityHelper.GetUserId()))
            {
                return RedirectToAction("NoAccess", "Account");
            }

            _vacreqService.Accept(id, SecurityHelper.GetUserId());
            _mutvacService.SubtractMinutes(vacreq.UserID, SecurityHelper.GetUserId(), vacreq.TotalMinutes, vacreq);

            TempData["Accepted"] = "true";



            var listholidaysinperiod = _holService.HolidaysInPeriod(vacreq.BeginDate, vacreq.EndDate);

            int minutes = 0;
            int totalminutes = 0;

            foreach (var item in listholidaysinperiod)
            {
                if (vacreq.IsTotalDays == true)
                {
                    minutes = 480; //Gelijk aan 8 uur werken.
                }

                else
                {
                    if (vacreq.BeginDate.Date == item.Date && vacreq.EndDate.Date == item.Date)
                    {
                        TimeSpan difference = vacreq.EndDate.Subtract(vacreq.BeginDate);
                        minutes = (difference.Hours * 60) + (difference.Minutes);
                    }

                    if (vacreq.BeginDate.Date < item.Date && vacreq.EndDate.Date == item.Date)
                    {
                        TimeSpan endtime = new TimeSpan(vacreq.EndDate.Hour, vacreq.EndDate.Minute, 00);
                        TimeSpan begintime = new TimeSpan(vacreq.BeginOfDay.Value.Hour, vacreq.BeginOfDay.Value.Minute, 00);
                        TimeSpan difference = endtime.Subtract(begintime);
                        minutes = (difference.Hours * 60) + (difference.Minutes);
                    }

                    if (vacreq.BeginDate.Date == item.Date && vacreq.EndDate.Date > item.Date)
                    {
                        TimeSpan begintime = new TimeSpan(vacreq.BeginDate.Hour, vacreq.BeginDate.Minute, 00);
                        TimeSpan endtime = new TimeSpan(vacreq.EndOfDay.Value.Hour, vacreq.EndOfDay.Value.Minute, 00);
                        TimeSpan difference = endtime.Subtract(begintime);
                        minutes = (difference.Hours * 60) + (difference.Minutes);
                    }

                    if (vacreq.BeginDate.Date < item.Date && vacreq.EndDate.Date > item.Date)
                    {
                        TimeSpan begintime = new TimeSpan(vacreq.BeginOfDay.Value.Hour, vacreq.BeginOfDay.Value.Minute, 00);
                        TimeSpan endtime = new TimeSpan(vacreq.EndOfDay.Value.Hour, vacreq.EndOfDay.Value.Minute, 00);
                        TimeSpan difference = endtime.Subtract(begintime);
                        minutes = (difference.Hours * 60) + difference.Minutes;
                    }
                }

                totalminutes = totalminutes + minutes;
            }

            if (totalminutes > 0)
            {
                _mutvacService.AddMinutes(vacreq.UserID, SecurityHelper.GetUserId(), totalminutes);
            }

            

            //Data benodigd voor het versturen van mail in eigen berichtencentrum
            var usermail = _userService.Get(vacreq.UserID);
            var managermail = _userService.Get(SecurityHelper.GetUserId());

            _messService.MailVacationRequestAccepted(usermail, managermail, vacreq);

            //Maak het ics file aan en geef deze door.
            string ics = new Appointment().CreateIcs(vacreq.Reason, "", vacreq.BeginDate, vacreq.EndDate);

            MemoryStream ms = new MemoryStream();
            UTF8Encoding enc = new UTF8Encoding();
            byte[] arrByData = enc.GetBytes(ics);
            ms.Write(arrByData, 0, arrByData.Length);
            ms.Position = 0;

            
            Attachment attachment = new Attachment(ms, "Appointment.ics");



            //Zet de informatie van het verlofverzoek klaar            
            string Reason = vacreq.Reason;
            string BeginDate = vacreq.BeginDate.Day.ToString() + "/" + vacreq.BeginDate.Month.ToString() + "/" + vacreq.BeginDate.Year.ToString();
            string EndDate = vacreq.EndDate.Day.ToString() + "/" + vacreq.EndDate.Month.ToString() + "/" + vacreq.EndDate.Year.ToString();
            string BeginTime = "";
            string EndTime = "";
            string TotalDays = "";

            if(vacreq.IsTotalDays == true)
            {
                BeginTime = "/";
                EndTime = "/";
                TotalDays = "Ja";
            }
            else
            {
                if (vacreq.BeginDate.Hour < 10)
                {
                    BeginTime = "0" + vacreq.BeginDate.Hour.ToString();
                }
                else
                {
                    BeginTime = vacreq.BeginDate.Hour.ToString();
                }
                if (vacreq.BeginDate.Minute < 10)
                {
                    BeginTime = BeginTime + ":" + "0" + vacreq.BeginDate.Minute.ToString();
                }
                else
                {
                    BeginDate = BeginTime + ":" + vacreq.BeginDate.Minute.ToString();
                }
                
                if (vacreq.EndDate.Hour < 10)
                {
                    EndTime = "0" + vacreq.EndDate.Hour.ToString();
                }
                else
                {
                    EndTime = vacreq.EndDate.Hour.ToString();
                }
                if (vacreq.EndDate.Minute < 10)
                {
                    EndTime = EndTime + ":" + "0" + vacreq.EndDate.Minute.ToString();
                }
                else
                {
                    EndTime = EndTime + ":" + vacreq.EndDate.Minute.ToString();
                }

                TotalDays = "Nee";
            }

            //Krijg de gebruikerinformatie en verzend de mail
            var user = _userService.Get(vacreq.UserID);
            string Name = user.FirstName + " " + user.LastName;
            //await MailHelper.RequestAcceptedAsync(user.Email, Reason, BeginDate, BeginTime, EndDate, EndTime, TotalDays);
            await MailHelper.SendAppointmentAsync(user.Email, Reason, BeginDate, BeginTime, EndDate, EndTime, TotalDays, vacreq.BeginDate, vacreq.EndDate, attachment);

            //Krijg de manager informatie en verzend de mail
            long managerid = user.Manager;
            long manageruserid = _manService.GetUserIDByManagerID(managerid);
            var manager = _userService.Get(manageruserid);
            await MailHelper.RequestAcceptedManagerAsync(manager.Email, Name,  Reason, BeginDate, BeginTime, EndDate, EndTime, TotalDays);

            //Geef het overzicht opnieuw weer
            return RedirectToAction("IndexManager");
        }

        #endregion

        #region Reject
        [ClaimsAuthorize(UserRoles.Medewerker)]
        public ActionResult RejectProposition(long? id)
        {
            if (id != null)
            {
                var vacreq = _vacreqService.Get(id.Value);

                vacreq.IsApproved = false;
                vacreq.IsInTreatment = false;
                vacreq.IsRejected = true;

                _vacreqService.Save(vacreq);
                

                return RedirectToAction("PropositionVacation", "Verlofaanvraag");
            }

            return RedirectToAction("PropositionVacation", "Verlofaanvraag");
        }

        [ClaimsAuthorize(UserRoles.Manager)]
        public ActionResult RejectRequest(long? id)
        {
            if(id != null)
            {
                var vacreq = _vacreqService.Get(id.Value);
                var model = new EditModel(vacreq);
                return View(model);
            }
            return RedirectToAction("IndexManager", "Verlofaanvraag");
        }

        [HttpPost]
        [ClaimsAuthorize(UserRoles.Manager)]
        public async Task<ActionResult> RejectRequest([Bind(Include = "VacationID, RejectionReason")] EditModel input)
        {
            var vacreq = _vacreqService.Get(input.VacationID);
            input.VacationRequestRejection(vacreq);

            if (input.RejectionReason == null)
            {
                ShowMessage("U moet een reden aangeven voor uw verlof.", MessageType.Danger);
                return View("RejectRequest", input);
            }

            if (input.RejectionReason.Length > 50)
            {
                ShowMessage("Uw reden is te lang, gebruik niet meer dan 50 tekens.", MessageType.Danger);
                return View("RejectRequest", input);
            }

            
            _vacreqService.Save(vacreq);
            TempData["Rejected"] = "true";
            await Reject(vacreq.VacationID);

            

            return RedirectToAction("IndexManager", "Verlofaanvraag");
            
        }





        [ClaimsAuthorize(UserRoles.Manager)]
        public async Task<ActionResult> Reject(long id)
        {
            _vacreqService.Reject(id, SecurityHelper.GetUserId());

            var vacreq = _vacreqService.Get(id);

            //Data benodigd voor versturen van mail in eigen berichtencentrum
            var usermail = _userService.Get(vacreq.UserID);
            var managermail = _userService.Get(SecurityHelper.GetUserId());

            _messService.MailVacationRequestRejected(usermail, managermail, vacreq);


            //Zet de informatie van het verlofverzoek klaar
            string Reason = vacreq.Reason;
            string ReasonRejection = vacreq.ReasonRejection;
            string BeginDate = vacreq.BeginDate.Day.ToString() + "/" + vacreq.BeginDate.Month.ToString() + "/" + vacreq.BeginDate.Year.ToString();
            string EndDate = vacreq.EndDate.Day.ToString() + "/" + vacreq.EndDate.Month.ToString() + "/" + vacreq.EndDate.Year.ToString();
            string BeginTime = "";
            string EndTime = "";
            string TotalDays = "";

            if (vacreq.IsTotalDays == true)
            {
                BeginTime = "/";
                EndTime = "/";
                TotalDays = "Ja";
            }
            else
            {
                if (vacreq.BeginDate.Hour < 10)
                {
                    BeginTime = "0" + vacreq.BeginDate.Hour.ToString();
                }
                else
                {
                    BeginTime = vacreq.BeginDate.Hour.ToString();
                }
                if (vacreq.BeginDate.Minute < 10)
                {
                    BeginTime = BeginTime + ":" + "0" + vacreq.BeginDate.Minute.ToString();
                }
                else
                {
                    BeginDate = BeginTime + ":" + vacreq.BeginDate.Minute.ToString();
                }

                if (vacreq.EndDate.Hour < 10)
                {
                    EndTime = "0" + vacreq.EndDate.Hour.ToString();
                }
                else
                {
                    EndTime = vacreq.EndDate.Hour.ToString();
                }
                if (vacreq.EndDate.Minute < 10)
                {
                    EndTime = EndTime + ":" + "0" + vacreq.EndDate.Minute.ToString();
                }
                else
                {
                    EndTime = EndTime + ":" + vacreq.EndDate.Minute.ToString();
                }

                TotalDays = "Nee";
            }

            //Krijg de gebruikerinformatie en verzend de mail
            var user = _userService.Get(vacreq.UserID);
            await MailHelper.RequestRejectedAsync(user.Email, ReasonRejection, Reason, BeginDate, BeginTime, EndDate, EndTime, TotalDays);

            //Krijg de manager informatie en verzend de mail
            long managerid = user.Manager;
            long manageruserid = _manService.GetUserIDByManagerID(managerid);
            var manager = _userService.Get(manageruserid);
            string name = user.FirstName + " " + user.LastName;
            await MailHelper.RequestRejectedManagerAsync(name, manager.Email, ReasonRejection, Reason, BeginDate, BeginTime, EndDate, EndTime, TotalDays);

            //Geef het overzicht opnieuw weer
            return RedirectToAction("IndexManager");
        }

        #endregion

        #region Delete
        [ClaimsAuthorize(UserRoles.Medewerker)]
        public ActionResult Delete(long? id)
        {
            if (id != null)
            {
                var vacreq = _vacreqService.Get(id.Value);

                if (vacreq.UserID != SecurityHelper.GetUserId())
                {
                    return RedirectToAction("NoAccess", "Account");
                }
                if (_vacreqService.IsAccepted(id.Value))
                {
                    


                    //Gedeelte bekijkt of er feestdagen in het verlofverzoek zitten
                    var listholidaysinperiod = _holService.HolidaysInPeriod(vacreq.BeginDate, vacreq.EndDate);

                    int minutes = 0;
                    int totalminutes = 0;

                    foreach (var item in listholidaysinperiod)
                    {
                        if (vacreq.IsTotalDays == true)
                        {
                            minutes = 480; //Gelijk aan 8 uur werken.
                        }

                        else
                        {
                            if (vacreq.BeginDate.Date == item.Date && vacreq.EndDate.Date == item.Date)
                            {
                                TimeSpan difference = vacreq.EndDate.Subtract(vacreq.BeginDate);
                                minutes = (difference.Hours * 60) + (difference.Minutes);
                            }

                            if (vacreq.BeginDate.Date < item.Date && vacreq.EndDate.Date == item.Date)
                            {
                                TimeSpan endtime = new TimeSpan(vacreq.EndDate.Hour, vacreq.EndDate.Minute, 00);
                                TimeSpan begintime = new TimeSpan(vacreq.BeginOfDay.Value.Hour, vacreq.BeginOfDay.Value.Minute, 00);
                                TimeSpan difference = endtime.Subtract(begintime);
                                minutes = (difference.Hours * 60) + (difference.Minutes);
                            }

                            if (vacreq.BeginDate.Date == item.Date && vacreq.EndDate.Date > item.Date)
                            {
                                TimeSpan begintime = new TimeSpan(vacreq.BeginDate.Hour, vacreq.BeginDate.Minute, 00);
                                TimeSpan endtime = new TimeSpan(vacreq.EndOfDay.Value.Hour, vacreq.EndOfDay.Value.Minute, 00);
                                TimeSpan difference = endtime.Subtract(begintime);
                                minutes = (difference.Hours * 60) + (difference.Minutes);
                            }

                            if (vacreq.BeginDate.Date < item.Date && vacreq.EndDate.Date > item.Date)
                            {
                                TimeSpan begintime = new TimeSpan(vacreq.BeginOfDay.Value.Hour, vacreq.BeginOfDay.Value.Minute, 00);
                                TimeSpan endtime = new TimeSpan(vacreq.EndOfDay.Value.Hour, vacreq.EndOfDay.Value.Minute, 00);
                                TimeSpan difference = endtime.Subtract(begintime);
                                minutes = (difference.Hours * 60) + difference.Minutes;
                            }
                        }

                        totalminutes = totalminutes + minutes;
                    }
                    var TotalMinutes = vacreq.TotalMinutes - totalminutes;
                    




                    if (vacreq.BeginDate > DateTime.Now)
                    {
                        _mutvacService.AddMinutes(vacreq.UserID, SecurityHelper.GetUserId(), TotalMinutes);
                    }                                    

                    _vacreqService.SoftDelete(id.Value);

                    TempData["Deleted"] = "true";
                    return RedirectToAction("Index");
                }

                else
                {
                    _vacreqService.SoftDelete(id.Value);
                    TempData["Deleted"] = "true";
                    return RedirectToAction("Index");
                }
            }

            return RedirectToAction("Login", "Account");
            

        }

        #endregion


    }
}