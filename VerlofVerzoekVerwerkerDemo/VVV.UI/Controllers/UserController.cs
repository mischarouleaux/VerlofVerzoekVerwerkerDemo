using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using VVV.Models;
using VVV.Business.Identity;
using VVV.Business.Mail;
using VVV.UI.Attributes;
using VVV.Interfaces.Services;
using VVV.UI.ViewModels.User;
using VVV.UI.Helpers;

namespace VVV.UI.Controllers
{
    public class UserController : BaseController
    {
        private IApplicationUserService _userService;
        private IManagerService _manService;
        private IMutationsVacationService _mutvacService;
        private IVacationRequestService _vacreqService;
        private IDepartmentService _depService;

        public UserController(IApplicationUserService userService, IManagerService manService, IMutationsVacationService mutvacService, IVacationRequestService vacreqService, IDepartmentService depService)
        {
            _userService = userService;
            _manService = manService;
            _mutvacService = mutvacService;
            _vacreqService = vacreqService;
            _depService = depService;
        }
        // GET: User

        #region Create HR-Medewerker

        [ClaimsAuthorize(UserRoles.HRManager)]
        public ActionResult CreateHR()
        {
            //Dropdown van de managers voorbereiden        
            var managers = _manService.GetAll();
            List<object> listmanagers = new List<object>();
            foreach (var item in managers)
            {
                listmanagers.Add(new
                {
                    ManagerID = item.ManagerID,
                    Name = item.ApplicationUser.FirstName + " " + item.ApplicationUser.LastName
                });
            }
            var dropdownmanagers = new SelectList(listmanagers, "ManagerID", "Name");

            //Dropdown van de afdeling voorbereiden
            var departments = _depService.GetAllDepartments();
            List<object> listdepartments = new List<object>();
            foreach (var item in departments)
            {
                listdepartments.Add(new
                {
                    DepartmentID = item.DepartmentID,
                    Name = item.DepartmentName
                });
            }
            var dropdowndepartments = new SelectList(listdepartments, "DepartmentID", "Name");

            var model = new EditModel();
            model.previousUrl = Request.UrlReferrer;
            model.FirstManagers = dropdownmanagers;
            model.SecondManagers = dropdownmanagers;
            model.Departments = dropdowndepartments;
            model.IsMedewerker = true;
            return View(model);
        }

        [ClaimsAuthorize(UserRoles.HRManager)]
        [HttpPost]
        public async Task<ActionResult> CreateHR([Bind(Include = "FirstName, LastName, Email, SelectedFirstManager, SelectedSecondManager, SelectedDepartment, IsMedewerker, IsManager, IsHRManager, previousUrl")] EditModel input)
        {
            var user = input.ToApplicationUser();

            //Zet data klaar voor wanneer de gegevens niet goed zijn ingevuld, dan wordt de webpagina opnieuw terug gestuurd.
            var managers = _manService.GetAll();
            List<object> listmanagers = new List<object>();
            foreach (var item in managers)
            {
                listmanagers.Add(new
                {
                    ManagerID = item.ManagerID,
                    Name = item.ApplicationUser.FirstName + " " + item.ApplicationUser.LastName
                });
            }
            var dropdownmanagers = new SelectList(listmanagers, "ManagerID", "Name");

            var departments = _depService.GetAllDepartments();
            List<object> listdepartments = new List<object>();
            foreach (var item in departments)
            {
                listdepartments.Add(new
                {
                    DepartmentID = item.DepartmentID,
                    Name = item.DepartmentName
                });
            }
            var dropdowndepartments = new SelectList(listdepartments, "DepartmentID", "Name");

            var model = new EditModel();
            input.FirstManagers = dropdownmanagers;
            input.SecondManagers = dropdownmanagers;
            input.Departments = dropdowndepartments;

            //Creeert foutmeldingen waar nodig.
            if (_userService.UserExists(user.Email))
            {
                ShowMessage("Deze gebruiker bestaat al", MessageType.Danger);
                return View("CreateHR", input);
            }

            if (_userService.EmailExists(user.Email))
            {
                ShowMessage("Deze gebruiker bestaat al", MessageType.Danger);
                return View("CreateHR", input);
            }



            if (user.IsMedewerker == false)
            {
                ShowMessage("Een gebruiker moet minimaal de rechten krijgen van medewerker", MessageType.Danger);
                return View("CreateHR", input);
            }


            if (user.FirstName == null)
            {
                ShowMessage("U heeft de volgende waarde niet ingevuld: Voornaam", MessageType.Danger);
                return View("CreateHR", input);
            }
            if (user.LastName == null)
            {
                ShowMessage("U heeft de volgende waarde niet ingevuld: Achternaam", MessageType.Danger);
                return View("CreateHR", input);
            }
            if (user.Email == null)
            {
                ShowMessage("U heeft de volgende waarde niet ingevuld: E-mailadres", MessageType.Danger);
                return View("CreateHR", input);
            }

            var expiryDate = DateTime.Now.AddDays(Properties.Settings.Default.FirstPasswordResetTimeOutInDays);
            var token = string.Format("{0}|{1}", user.UserID, expiryDate.ToString("yyyyMMddHHmmss")).HashToMd5();

           


            //Slaat de veranderingen op in de database.
            if (ModelState.IsValid)
            {

                _userService.Save(user);

                token = HttpUtility.UrlEncode(token);

                var link = string.Format(@"<a href=""{0}"">link</a>", Url.Action("Login", "Account", new { t = token }, Request.Url.Scheme));
                await MailHelper.Welcome(user.Email, link);

                if (input.IsManager == true)
                {
                    if (_manService.IsManager(input.UserID) == false)
                    {
                        var manager = input.ToManager(user);

                        if (ModelState.IsValid)
                        {
                            _manService.Save(manager);
                        }
                    }
                    else
                    {
                        var manager = _manService.Get(_manService.GetManageridByUserID(input.UserID));
                        input.ToManager(user, manager);

                        if (ModelState.IsValid)
                        {
                            _manService.Save(manager);
                        }
                    }
                }

                TempData["Saved"] = "true";

                return RedirectToAction("IndexHR", "User");
            }

            //Wanneer er iets is misgegaan, wordt de webpagina opnieuw getoond.
            return View(input);
        }

        #endregion       

        #region Edit
        [ClaimsAuthorize(UserRoles.HRManager)]
        public ActionResult Edit(long? userid)
        {
            if (userid != null)
            {
                var user = _userService.Get(userid.Value);

                var model = new EditModel(user);

                long firstmanagerid = user.Manager;
                long? secondmanagerid = user.SecondManager;

                long departmentid = user.Department;

                //Bereidt de managerdropdown voor
                var managers = _manService.GetAll();
                List<object> listmanagers = new List<object>();
                foreach (var item in managers)
                {
                    listmanagers.Add(new
                    {
                        ManagerID = item.ManagerID,
                        Name = item.ApplicationUser.FirstName + " " + item.ApplicationUser.LastName
                    });
                }
                var dropdownmanagers = new SelectList(listmanagers, "ManagerID", "Name");

                //Dropdown van de afdeling voorbereiden
                var departments = _depService.GetAllDepartments();
                List<object> listdepartments = new List<object>();
                foreach (var item in departments)
                {
                    listdepartments.Add(new
                    {
                        DepartmentID = item.DepartmentID,
                        Name = item.DepartmentName
                    });
                }
                

                //Vul de huidige afdeling in
                model.SelectedDepartment = departmentid;
                var dropdowndepartments = new SelectList(listdepartments, "DepartmentID", "Name", model.SelectedDepartment);
                model.Departments = dropdowndepartments;


                //Vindt de geselecteerde manager, vult deze in het model.
                model.SelectedFirstManager = firstmanagerid;
                var dropdownfirstmanager = new SelectList(listmanagers, "ManagerID", "Name", model.SelectedFirstManager);
                model.FirstManagers = dropdownfirstmanager;

               
                if (secondmanagerid != null)
                {
                    model.SelectedSecondManager = secondmanagerid.Value;
                    var dropdownsecondmanager = new SelectList(listmanagers, "ManagerID", "Name", model.SelectedSecondManager);
                    model.SecondManagers = dropdownsecondmanager;
                }
                else
                {
                    var dropdownsecondmanager = new SelectList(listmanagers, "ManagerID", "Name");
                    model.SecondManagers = dropdownsecondmanager;
                }

                model.IsMedewerker = true;

                //Zorgt ervoor dat de vorige pagina wordt onthouden, zodat wanneer er op annuleren wordt gedrukt. We terug kunnen naar dezelfde pagina.
                model.previousUrl = Request.UrlReferrer;

                return View(model);
            }

            return RedirectToAction("IndexHR", "User");

        }

        [HttpPost]
        [ClaimsAuthorize(UserRoles.HRManager)]
        public ActionResult Edit([Bind(Include = "UserID, FirstName, LastName, Email, SelectedFirstManager, SelectedSecondManager, SelectedDepartment, IsMedewerker, IsManager, IsHRManager, previousUrl")] EditModel input)
        {
            long lastmanager = _userService.LastManager(input.UserID);
            long? lastsecondmanager = _userService.LastSecondManager(input.UserID);

            var user = _userService.Get(input.UserID);
            

            input.ToApplicationUser(user);

            input.IsMedewerker = true;
            


            //Zet alle benodigde informatie klaar voor wanneer de pagina terug moet worden gestuurd in het geval van foutieve informatie.
            long firstmanagerid = user.Manager;
            long? secondmanagerid = user.SecondManager;

            long departmentid = user.Department;

            var managers = _manService.GetAll();
            List<object> listmanagers = new List<object>();
            foreach (var item in managers)
            {
                listmanagers.Add(new
                {
                    ManagerID = item.ManagerID,
                    Name = item.ApplicationUser.FirstName + " " + item.ApplicationUser.LastName
                });
            }

            var dropdownmanagers = new SelectList(listmanagers, "ManagerID", "Name");

            //Bereidt de dropdown voor afdelingen voor, vult deze in met de laatste waarde
            var departments = _depService.GetAllDepartments();
            List<object> listdepartments = new List<object>();
            foreach(var item in departments)
            {
                listdepartments.Add(new
                {
                    DepartmentID = item.DepartmentID,
                    Name = item.DepartmentName
                });
            }

            input.SelectedDepartment = departmentid;
            var dropdowndepartments = new SelectList(listdepartments, "DepartmentID", "Name", input.SelectedDepartment);
            input.Departments = dropdowndepartments;

            //Vindt de geselecteerde manager, vult deze in het model.
            input.SelectedFirstManager = firstmanagerid;
            var dropdownfirstmanager = new SelectList(listmanagers, "ManagerID", "Name", input.SelectedFirstManager);
            input.FirstManagers = dropdownfirstmanager;


            if (secondmanagerid != null)
            {
                input.SelectedSecondManager = secondmanagerid.Value;
                var dropdownsecondmanager = new SelectList(listmanagers, "ManagerID", "Name", input.SelectedSecondManager);
                input.SecondManagers = dropdownsecondmanager;
            }
            else
            {
                var dropdownsecondmanager = new SelectList(listmanagers, "ManagerID", "Name");
                input.SecondManagers = dropdownsecondmanager;
            }


            //Maak de foutmeldingen aan
            //Check eerst of de gebruikersnaam verandert is, zo ja, dan kijk of deze al bestaat in de database.
            string lastusername = _userService.GetUsername(input.UserID);
            if (input.Email != lastusername)
            {
                if (_userService.UserExists(input.Email))
                {
                    ShowMessage("Deze gebruiker bestaat al", MessageType.Danger);
                    return View("Edit", input);
                }
            }
            

            if (user.IsMedewerker == false)
            {
                ShowMessage("Een gebruiker moet minimaal de rechten krijgen van medewerker", MessageType.Danger);
                return View("Edit", input);
            }

            
            if (user.FirstName == null)
            {
                ShowMessage("U heeft de volgende waarde niet ingevuld: Voornaam", MessageType.Danger);
                return View(input);
            }
            if (user.LastName == null)
            {
                ShowMessage("U heeft de volgende waarde niet ingevuld: Achternaam", MessageType.Danger);
                return View("Edit", input);
            }
            if (user.Email == null)
            {
                ShowMessage("U heeft de volgende waarde niet ingevuld: E-mailadres", MessageType.Danger);
                return View("Edit", input);
            }

            if (input.IsManager == true)
            {
                if (_manService.IsManager(input.UserID) == false)
                {
                    var manager = input.ToManager(user);

                    if (ModelState.IsValid)
                    {
                        _manService.Save(manager);
                    }
                }
                else
                {
                    var manager = _manService.Get(_manService.GetManageridByUserID(input.UserID));
                    input.ToManager(user, manager);

                    if (ModelState.IsValid)
                    {
                        _manService.Save(manager);
                    }
                }
            }

            if (input.IsManager == false)
            {
                if (_manService.IsManager(user.UserID) == true)
                {
                    long managerid = _manService.GetManageridByUserID(user.UserID);
                    _manService.DeleteManager(managerid, user.UserID);
                }
            }

            //Checkt of de eerste/ tweede manager is veranderd. Wanneer dit is gebeurt, zullen de verlofverzoeken ook worden aangepast.
            
            if (lastmanager != user.Manager)
            {
                _vacreqService.ChangeManagerID(user.UserID, user.Manager);
            }

            if (lastsecondmanager != 0)
            {
                if (lastsecondmanager != user.SecondManager)
                {
                    _vacreqService.ChangeSecondManagerID(user.UserID, user.SecondManager.Value);
                }
            }

            //Bewerkt de gebruiker in de databse.
            if (ModelState.IsValid)
            {
                _userService.Save(user);

                TempData["Saved"] = "true";
                return RedirectToAction("DetailsManager", "User", new { userid = input.UserID });
            }

            return View(input);
        }

        #endregion

        #region DetailsManager
        [ClaimsAuthorize(UserRoles.Medewerker)]
        public ActionResult CheckCurrentUser()
        {             
            return RedirectToAction("DetailsManager", "User", new { userid = SecurityHelper.GetUserId() });                             
        }

        
        public ActionResult DetailsManager(long? userid)
        {
            if (userid != null)
            {
                var user = _userService.Get(userid.Value);

                if (SecurityHelper.UserHasRole(UserRoles.HRManager) || user.Manager == _manService.GetManageridByUserID(SecurityHelper.GetUserId()) || user.SecondManager == _manService.GetManageridByUserID(SecurityHelper.GetUserId()) || user.UserID == SecurityHelper.GetUserId())
                {
                    if (user.IsManager == true)
                    {
                        long currentmanagerid = _manService.GetManageridByUserID(userid.Value);
                        bool IsAvailable = _manService.IsAvailable(currentmanagerid);

                        int mutvac = _mutvacService.GetMinutesVacationByID(userid.Value);

                        long managerid = user.Manager;
                        long? secondmanagerid = user.SecondManager;

                        var manager = _userService.Get(_manService.GetUserIDByManagerID(managerid));

                        if (secondmanagerid != null)
                        {


                            var secondmanager = _userService.Get(_manService.GetUserIDByManagerID(secondmanagerid.Value));
                            var secondmanagermodel = new EditModel(user, manager, secondmanager, mutvac, IsAvailable);

                            var Msg = TempData["Saved"] as string;
                            if (Msg != null && Msg == "true")
                            {
                                secondmanagermodel.HasSuccesMessage = true;
                            }
                            else
                            {
                                secondmanagermodel.HasSuccesMessage = false;
                            }

                            return View(secondmanagermodel);
                        }


                        var model = new EditModel(user, manager, mutvac, IsAvailable);

                        

                        return View(model);
                    }

                    if (user.IsHRManager || user.IsMedewerker)
                    {
                        int mutvac = _mutvacService.GetMinutesVacationByID(userid.Value);

                        long managerid = user.Manager;
                        long? secondmanagerid = user.SecondManager;

                        var manager = _userService.Get(_manService.GetUserIDByManagerID(managerid));

                        if (secondmanagerid != null)
                        {


                            var secondmanager = _userService.Get(_manService.GetUserIDByManagerID(secondmanagerid.Value));
                            var secondmanagermodel = new EditModel(user, manager, secondmanager, mutvac, null);

                            var Message = TempData["Saved"] as string;
                            if (Message!= null && Message == "true")
                            {
                                secondmanagermodel.HasSuccesMessage = true;
                            }
                            else
                            {
                                secondmanagermodel.HasSuccesMessage = false;
                            }
                            return View(secondmanagermodel);
                        }


                        var model = new EditModel(user, manager, mutvac);

                        var Msg = TempData["Saved"] as string;
                        if (Msg != null && Msg == "true")
                        {
                            model.HasSuccesMessage = true;
                        }
                        else
                        {
                            model.HasSuccesMessage = false;
                        }

                        return View(model);
                    }
                }

                

                return RedirectToAction("NoAccess", "Account");
            }

            return RedirectToAction("IndexHR", "User");
        }

        [HttpPost]
        [ClaimsAuthorize(false, UserRoles.Manager, UserRoles.HRManager)]
        public ActionResult DetailsManager([Bind(Include = "UserID, Days, Hours, Minutes")] EditModel input)
        {
            //In de prd versie kunnen geen bewerkingen zijn op verlofdagen
            return RedirectToAction("Login", "Account");

            //var mutvac = input.ToMutationsVacation();
            //
            //long? userid = mutvac.UserID;
            //
            //
            //if (userid != null)
            //{
            //    var user = _userService.Get(userid.Value);
            //
            //    //De gebruiker de rol: Manager
            //    if (user.IsManager == true)
            //    {
            //        long currentmanagerid = _manService.GetManageridByUserID(userid.Value);
            //        bool IsAvailable = _manService.IsAvailable(currentmanagerid);
            //
            //        int vacationdays = _mutvacService.GetMinutesVacationByID(userid.Value);
            //
            //        long managerid = user.Manager;
            //        long? secondmanagerid = user.SecondManager;
            //
            //        var manager = _userService.Get(_manService.GetUserIDByManagerID(managerid));
            //
            //        //De gebruiker heeft twee managers, de veranderingen worden opgeslagen en de pagina wordt teruggestuurd.
            //        if (secondmanagerid != null)
            //        {
            //            var secondmanager = _userService.Get(_manService.GetUserIDByManagerID(secondmanagerid.Value));
            //
            //            var secondmanagermodel = new EditModel(user, manager, secondmanager, vacationdays, IsAvailable);
            //
            //            if (mutvac.VacationModification == 0)
            //            {
            //
            //                ShowMessage("Vul een tijd aan verlof in dat groter is dan 0", MessageType.Danger);
            //                return View("DetailsManager", secondmanagermodel);
            //            }
            //
            //            if (ModelState.IsValid)
            //            {
            //                _mutvacService.Save(mutvac);
            //
            //                TempData["Saved"] = "true";
            //                vacationdays = _mutvacService.GetMinutesVacationByID(userid.Value);
            //                
            //                secondmanagermodel.Days = 0;
            //                secondmanagermodel.Hours = 0;
            //                secondmanagermodel.Minutes = 0;
            //
            //                ModelState.Clear();
            //                return RedirectToAction("DetailsManager", "User", new { userid = secondmanagermodel.UserID});
            //
            //            }
            //        }
            //
            //        //De gebruiker heeft maar een manager, de veranderingen worden opgeslagen en de pagina wordt teruggestuurd.
            //        if (ModelState.IsValid)
            //        {
            //            _mutvacService.Save(mutvac);
            //
            //            ShowMessage("Opslaan gelukt", MessageType.Info);
            //            vacationdays = _mutvacService.GetMinutesVacationByID(userid.Value);
            //
            //            var firstmanagermodel = new EditModel(user, manager, vacationdays, IsAvailable);
            //
            //            return View("DetailsManager", firstmanagermodel);
            //        }
            //
            //        //Het opslaan is niet gelukt.
            //        ShowMessage("Opslaan niet gelukt", MessageType.Danger);
            //        return View("DetailsManager", input);
            //    }
            //
            //    //De gebruiker heeft geen rol als manager
            //    else
            //    {
            //        int vacationdays = _mutvacService.GetMinutesVacationByID(userid.Value);
            //
            //        long managerid = user.Manager;
            //        long? secondmanagerid = user.SecondManager;
            //
            //        var manager = _userService.Get(_manService.GetUserIDByManagerID(managerid));
            //
            //        if (secondmanagerid != null)
            //        {
            //            var secondmanager = _userService.Get(_manService.GetUserIDByManagerID(secondmanagerid.Value));
            //
            //            if (ModelState.IsValid)
            //            {
            //                _mutvacService.Save(mutvac);
            //
            //                ShowMessage("Opslaan gelukt", MessageType.Info);
            //                vacationdays = _mutvacService.GetMinutesVacationByID(userid.Value);
            //
            //                var secondmanagermodel = new EditModel(user, manager, secondmanager, vacationdays, null);
            //
            //                return RedirectToAction("DetailsManager", "User", new { userid = secondmanagermodel.UserID });
            //
            //            }
            //        }
            //
            //        if (ModelState.IsValid)
            //        {
            //            _mutvacService.Save(mutvac);
            //
            //            ShowMessage("Opslaan gelukt", MessageType.Info);
            //            vacationdays = _mutvacService.GetMinutesVacationByID(userid.Value);
            //
            //            var firstmanagermodel = new EditModel(user, manager, vacationdays);
            //
            //            return View("DetailsManager", firstmanagermodel);
            //        }
            //
            //        //Het opslaan is niet gelukt.
            //        ShowMessage("Opslaan niet gelukt", MessageType.Danger);
            //        return View("DetailsManager", input);
            //    }                
            //}
            //
            //return RedirectToAction("IndexHR", "User");
        }

        #endregion

        #region Delete
        [ClaimsAuthorize(UserRoles.HRManager)]
        public ActionResult Delete(long? id)
        {
            var user = _userService.Get(id.Value);
            
            long managerid = user.Manager;
            long? secondmanagerid = user.SecondManager;

            var manager = _userService.Get(_manService.GetUserIDByManagerID(managerid));
            int mutvac = _mutvacService.GetMinutesVacationByID(id.Value);

            if (secondmanagerid != null)
            {
                var secondmanager = _userService.Get(_manService.GetUserIDByManagerID(secondmanagerid.Value));
                var secondmanagermodel = new EditModel(user, manager, secondmanager, mutvac, null);
                if (user.IsManager == true)
                {
                    if (_userService.HasMedewerkers(_manService.GetManageridByUserID(user.UserID)) == true)
                    {
                        ShowMessage("Gebruiker kan niet verwijderd worden, de gebruiker is manager en heeft medewerkers. Vervang eerst bij deze KEMBIT'ers de manager, voordat u deze KEMBIT'er kan verwijderen.", MessageType.Danger);
                        return View("DetailsManager", secondmanagermodel);

                    }


                }

                if (user.IsHRManager == true)
                {
                    if (_userService.CountManagers() == 1)
                    {
                        ShowMessage("Gebruiker kan niet verwijderd worden. Deze gebruiker is de laatste HR-medewerker in dit systeem. Ken eerst een andere medewerker deze rol toe voordat u deze persoon verwijdert.", MessageType.Danger);
                        return View("DetailsManager");
                    }
                }
                
            }


            var model = new EditModel(user, manager, mutvac);

            if (user.IsManager == true)
            {
                if (_userService.HasMedewerkers(_manService.GetManageridByUserID(user.UserID)) == true)
                {
                    ShowMessage("Gebruiker kan niet verwijdert worden, de gebruiker is manager en heeft medewerkers. Vervang eerst bij deze medewerkers de manager, voordat u deze persoon kan verwijderen.", MessageType.Danger);
                    return View("DetailsManager", model);
                        
                }

                
            }

            if (user.IsHRManager == true)
            {
                if (_userService.CountManagers() == 1) 
                {
                    ShowMessage("Gebruiker kan niet verwijderd worden. Deze gebruiker is de laatste HR-medewerker in dit systeem. Ken eerst een andere medewerker deze rol toe voordat u deze persoon verwijdert.", MessageType.Danger);
                    return View("DetailsManager");
                }
            }

            _userService.SoftDelete(user.UserID);
            TempData["Changed"] = "true";

            return RedirectToAction("IndexHR", "User");
        }

        [ClaimsAuthorize(UserRoles.HRManager)]
        public ActionResult UndoDelete(long userid)
        {
            _userService.UndoSoftDelete(userid);
            TempData["Changed"] = "true";

            return RedirectToAction("DetailsManager", "User", new { userid = userid });
        }




        #endregion

        #region Index HR-Medewerker
        [ClaimsAuthorize(false, UserRoles.Manager, UserRoles.HRManager)]
        public ActionResult IndexHR(int? page, int? modelpage)
        {
            IndexModel model = new IndexModel();

            if (page != null)
            {
                model.Page = page.Value;
            }

            var Msg = TempData["Changed"] as string;
            if (Msg != null && Msg == "true")
            {
                model.HasSuccesMessage = true;
            }
            else
            {
                model.HasSuccesMessage = false;
            }

            Msg = TempData["Saved"] as string;
            if (Msg != null && Msg == "true")
            {
                model.HasSavedMessage = true;
            }
            else
            {
                model.HasSavedMessage = false;
            }


            if (SecurityHelper.UserHasRole(UserRoles.HRManager))
            {
                var users = _userService.GetAllUsers();

                model.ApplicationUsers = users.ToPagedList(model.Page, model.PageSize);
                

                return View(model);
            }

            long userid = SecurityHelper.GetUserId();
            long managerid = _manService.GetManageridByUserID(userid);
            model.ApplicationUsers = _userService.GetUsersByManager(managerid).ToPagedList(model.Page, model.PageSize);       

            return View(model);
        }

        [ClaimsAuthorize(false, UserRoles.Manager, UserRoles.HRManager)]
        [HttpPost]
        public ActionResult IndexHR(int? page, int? pagesize, IndexModel model)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("IndexHRFind", model.GetRouteValues());
            }

            return View(model);
        }

        [ClaimsAuthorize(false, UserRoles.Manager, UserRoles.HRManager)]
        public ActionResult IndexHRFind(int page, int pagesize, string search = "")
        {

            IndexModel model = new IndexModel()
            {
                Page = page,
                PageSize = pagesize,
                SearchQuery = search
            };            

            var users = _userService.GetAllUsers();

            if (!String.IsNullOrEmpty(search))
            {
                users = users.Where(filter => filter.LastName.Contains(search) || filter.FirstName.Contains(search));
            }

            model.ApplicationUsers = users.ToPagedList(model.Page, model.PageSize);

            return View("IndexHR", model);
        }

        [ClaimsAuthorize(false, UserRoles.Manager, UserRoles.HRManager)]
        [HttpPost]
        public ActionResult IndexHRFind(IndexModel input, int? page, int? modelpage)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("Index", input.GetRouteValues(true));
            }

            return View(input);
        }


        public ActionResult IndexHRDeleted(int? page, int? modelpage)
        {
            IndexModel model = new IndexModel();

            if (page != null)
            {
                model.Page = page.Value;
            }

            if (SecurityHelper.UserHasRole(UserRoles.HRManager))
            {
                var users = _userService.GetAllDeletedUsers();

                model.ApplicationUsers = users.ToPagedList(model.Page, model.PageSize);


                return View(model);
            }

            return RedirectToAction("IndexHR", "User");
        }
        [ClaimsAuthorize(false, UserRoles.Manager, UserRoles.HRManager)]
        [HttpPost]
        public ActionResult IndexHRDeleted(int? page, int? pagesize, IndexModel model)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("IndexHRDeletedFind", model.GetRouteValues());
            }

            return View(model);
        }

        [ClaimsAuthorize(false, UserRoles.Manager, UserRoles.HRManager)]
        public ActionResult IndexHRDeletedFind(int page, int pagesize, string search = "")
        {

            IndexModel model = new IndexModel()
            {
                Page = page,
                PageSize = pagesize,
                SearchQuery = search
            };

            var users = _userService.GetAllDeletedUsers();

            if (!String.IsNullOrEmpty(search))
            {
                users = users.Where(filter => filter.LastName.Contains(search) || filter.FirstName.Contains(search));
            }

            model.ApplicationUsers = users.ToPagedList(model.Page, model.PageSize);

            return View("IndexHRDeleted", model);
        }

        [ClaimsAuthorize(false, UserRoles.Manager, UserRoles.HRManager)]
        [HttpPost]
        public ActionResult IndexHRDeletedFind(IndexModel input, int? page, int? modelpage)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("IndexHRDeleted", input.GetRouteValues(true));
            }

            return View(input);
        }


        #endregion
        
        #region Change Availability
        [ClaimsAuthorize(false, UserRoles.HRManager, UserRoles.HRManager)]
        public ActionResult SetAvailable(long? userid)
        {
            if (userid != null)
            {
                long managerid = _manService.GetManageridByUserID(userid.Value);
                _manService.SetAvailable(managerid);
                
                return RedirectToAction("DetailsManager", new { userid = userid });
            }

            return RedirectToAction("IndexHR", "User");
        }

        [ClaimsAuthorize(false, UserRoles.HRManager, UserRoles.HRManager)]
        public ActionResult SetNotAvailable(long? userid)
        {
            if (userid != null)
            {
                long managerid = _manService.GetManageridByUserID(userid.Value);
                _manService.SetNotAvailable(managerid);

                return RedirectToAction("DetailsManager", new { userid = userid });
            }

            return RedirectToAction("IndexHR", "User");
        }

        #endregion

        #region IndexMutations
        [ClaimsAuthorize(UserRoles.Medewerker)]
        public ActionResult IndexMutations(long? userid, int? page, int? modelpage)
        {
            //De mutaties op de verlofstand is niet benodigd in de prd versie
            return RedirectToAction("Login", "Account");

            //if (userid != null)
            //{
            //    var model = new MutationsModel(userid.Value);
            //
            //    if (page != null)
            //    {
            //        model.Page = page.Value;
            //    }
            //    var user = _userService.Get(userid.Value);
            //
            //
            //    model.Mutations = _mutvacService.GetMutationsByUserID(userid.Value).ToPagedList(model.Page, model.PageSize);
            //
            //    return View(model);
            //    
            //}
            //
            //return RedirectToAction("Login", "Account");
        }

        #endregion

        #region Grant VacationDays to multiple users

        [ClaimsAuthorize(false, UserRoles.Manager, UserRoles.HRManager)]
        public ActionResult GrantVacationDays()
        {
            //Function isn't in use in production. Redirect the user back to the index page
            return RedirectToAction("IndexHR", "User");

            
            //GrantVacationDaysModel model = new GrantVacationDaysModel(true);            
            //
            ////Als de gebruiker de rol HR heeft, krijgt deze alle gebruikers te zien
            //if (SecurityHelper.UserHasRole(UserRoles.HRManager))
            //{
            //    //Zorgt dat er een selectlist wordt gemaakt met alle gebruikers
            //    var Users = _userService.GetAllUsers();
            //    
            //    List<object> listusers = new List<object>();
            //
            //    foreach (var item in Users)
            //    {
            //        listusers.Add(new
            //        {
            //            UserID = item.UserID,
            //            Name = string.Format("{0} ----  {1}", (item.FirstName + " " + item.LastName), item.Department1.DepartmentName)
            //        });
            //    }
            //
            //    var dropdownusers = new SelectList(listusers, "UserID", "Name");
            //
            //    model.Users = dropdownusers;
            //}
            //else
            //{
            //    //Als de gebruiker de rol Manager heeft, krijgt de gebruiker alleen zijn medewerkers te zien
            //    if (SecurityHelper.UserHasRole(UserRoles.Manager))
            //    {
            //        var Users = _userService.GetUsersByManager(_manService.GetManageridByUserID(SecurityHelper.GetUserId()));
            //        
            //        List<object> listusers = new List<object>();
            //
            //        foreach (var item in Users)
            //        {
            //            listusers.Add(new
            //            {
            //                UserID = item.UserID,
            //                Name = item.FirstName + " " + item.LastName
            //            });
            //        }
            //
            //        var dropdownusers = new SelectList(listusers, "UserID", "Name");
            //
            //        model.Users = dropdownusers;
            //    }
            //
            //}
            //
            //return View("GrantVacationDays", model);
            
        }

        [ClaimsAuthorize(false, UserRoles.Manager, UserRoles.HRManager)]
        [HttpPost]
        public ActionResult GrantVacationDays([Bind(Include = "SelectedID, Days, Hours, Minutes, SearchQuery, IsApplications, IsServices, IsKnowledgeCenter, IsConsultancy")] GrantVacationDaysModel input)
        {
            //Function isn't in use in production. Redirect the user back to the index page
            return RedirectToAction("IndexHR", "User");

            //if (Request.Form["filterbutton"] != null)
            //{
            //    long itapplications = _depService.GetApplicationsID();
            //    long itservices = _depService.GetServicesID();
            //    long itknowledge = _depService.GetKnowledgeID();
            //    long itconsultancy = _depService.GetConsultancyID();
            //
            //    GrantVacationDaysModel model = new GrantVacationDaysModel
            //    {
            //        IsApplications = input.IsApplications,
            //        IsServices = input.IsServices,
            //        IsKnowledgeCenter = input.IsKnowledgeCenter,
            //        IsConsultancy = input.IsConsultancy,
            //        SearchQuery = input.SearchQuery
            //    };
            //    
            //
            //    //Als de gebruiker de rol HR heeft, krijgt deze alle gebruikers te zien
            //    if (SecurityHelper.UserHasRole(UserRoles.HRManager))
            //    {
            //
            //        //Zorgt dat er een selectlist wordt gemaakt met alle gebruikers
            //        var Users = _userService.GetAllUsers();
            //        if (model.IsApplications = true || model.IsServices == true || model.IsKnowledgeCenter == true || model.IsConsultancy == true)
            //        {
            //            Users = _userService.GetUsersDepartmentFilter(input.SearchQuery, input.IsApplications, input.IsServices, input.IsKnowledgeCenter, input.IsConsultancy);
            //        }
            //        else
            //        {
            //            Users = Users.Where(p => p.FirstName.Contains(input.SearchQuery) || p.LastName.Contains(input.SearchQuery));
            //        }
            //
            //        
            //
            //        List<object> listusers = new List<object>();
            //
            //        foreach (var item in Users)
            //        {
            //            listusers.Add(new
            //            {
            //                UserID = item.UserID,
            //                Name = string.Format("{0} ----  {1}", (item.FirstName + " " + item.LastName), item.Department1.DepartmentName)
            //            });
            //        }
            //
            //        var dropdownusers = new SelectList(listusers, "UserID", "Name");
            //
            //        model.Users = dropdownusers;
            //    }
            //    else
            //    {
            //        //Als de gebruiker de rol Manager heeft, krijgt de gebruiker alleen zijn medewerkers te zien
            //        if (SecurityHelper.UserHasRole(UserRoles.Manager))
            //        {
            //            long managerid = _manService.GetManageridByUserID(SecurityHelper.GetUserId());
            //            var Users = _userService.GetUsersByManager(_manService.GetManageridByUserID(SecurityHelper.GetUserId()));
            //            //Zorgt dat er een selectlist wordt gemaakt met alle gebruikers
            //
            //            
            //            if (model.IsApplications = true || model.IsServices == true || model.IsKnowledgeCenter == true || model.IsConsultancy == true)
            //            {
            //                Users = _userService.GetUsersDepartmentFilter(input.SearchQuery, input.IsApplications, input.IsServices, input.IsKnowledgeCenter, input.IsConsultancy);
            //            }
            //            else
            //            {
            //                Users = Users.Where(p => p.FirstName.Contains(input.SearchQuery) || p.LastName.Contains(input.SearchQuery));
            //            }
            //
            //            
            //            Users = Users.Where(p => p.Manager == managerid);
            //
            //            List<object> listusers = new List<object>();
            //
            //            foreach (var item in Users)
            //            {
            //                listusers.Add(new
            //                {
            //                    UserID = item.UserID,
            //                    Name = string.Format("{0} ----  {1}", (item.FirstName + " " + item.LastName), item.Department1.DepartmentName)
            //                });
            //            }
            //
            //            var dropdownusers = new SelectList(listusers, "UserID", "Name");
            //
            //            model.Users = dropdownusers;
            //        }
            //
            //    }
            //
            //    return View("GrantVacationDays", model);
            //}
            //else
            //{
            //    List<long> list = new List<long>();
            //    foreach (long item in input.SelectedID)
            //    {
            //        if (item != 0)
            //        {
            //            list.Add(item);
            //        }
            //    }
            //
            //    var totalminutes = input.CalculateMinutes(input.Days, input.Hours, input.Minutes);
            //    
            //
            //    foreach (long item in list)
            //    {
            //        var model = input.ToMutationsVacation(item);
            //        _mutvacService.Save(model);
            //        TempData["Changed"] = "true";
            //        
            //
            //    }
            //    return RedirectToAction("IndexHR", "User");
            //}
            
        }

        [ClaimsAuthorize(false, UserRoles.Manager, UserRoles.HRManager)]
        public ActionResult GrantVacationDaysFind(bool isapplications, bool isservices, bool isknowledgecenter, bool isconsultancy, string search = "")
        {
            //Function isn't in use in production. Redirect the user back to the index page
            return RedirectToAction("IndexHR", "Account");

            //GrantVacationDaysModel model = new GrantVacationDaysModel
            //{
            //    IsApplications = isapplications,
            //    IsServices = isservices,
            //    IsKnowledgeCenter = isknowledgecenter,
            //    IsConsultancy = isconsultancy,
            //    SearchQuery = search
            //};
            //
            ////Als de gebruiker de rol HR heeft, krijgt deze alle gebruikers te zien
            //if (SecurityHelper.UserHasRole(UserRoles.HRManager))
            //{
            //    //Zorgt dat er een selectlist wordt gemaakt met alle gebruikers
            //    var Users = _userService.GetAllUsers();
            //    if (!String.IsNullOrEmpty(search))
            //    {
            //        Users = Users.Where(filter => filter.LastName.Contains(search) || filter.FirstName.Contains(search)).OrderByDescending(p => p.FirstName);
            //    }
            //    List<object> listusers = new List<object>();
            //
            //    foreach (var item in Users)
            //    {
            //        listusers.Add(new
            //        {
            //            UserID = item.UserID,
            //            Name = string.Format("{0} ----  {1}", (item.FirstName + " " + item.LastName), item.Department1.DepartmentName)
            //        });
            //    }
            //
            //    var dropdownusers = new SelectList(listusers, "UserID", "Name");
            //
            //    model.Users = dropdownusers;
            //}
            //else
            //{
            //    //Als de gebruiker de rol Manager heeft, krijgt de gebruiker alleen zijn medewerkers te zien
            //    if (SecurityHelper.UserHasRole(UserRoles.Manager))
            //    {
            //        var Users = _userService.GetUsersByManager(_manService.GetManageridByUserID(SecurityHelper.GetUserId()));
            //        if (!String.IsNullOrEmpty(search))
            //        {
            //            Users = Users.Where(filter => filter.LastName.Contains(search) || filter.FirstName.Contains(search)).OrderByDescending(p => p.FirstName);
            //        }
            //        List<object> listusers = new List<object>();
            //
            //        foreach (var item in Users)
            //        {
            //            listusers.Add(new
            //            {
            //                UserID = item.UserID,
            //                Name = item.FirstName + " " + item.LastName
            //            });
            //        }
            //
            //        var dropdownusers = new SelectList(listusers, "UserID", "Name");
            //
            //        model.Users = dropdownusers;
            //    }
            //
            //}
            //
            //return View("IndexHR", model);
        }

        [ClaimsAuthorize(false, UserRoles.Manager, UserRoles.HRManager)]
        [HttpPost]
        public ActionResult GrantVacationDaysFind(IndexModel input)
        {
            //Function isn't in use in production. Redirect the user back to the index page
            return RedirectToAction("IndexHR", "Account");

            //if (ModelState.IsValid)
            //{
            //    return RedirectToAction("Index", input.GetRouteValues(true));
            //}
            //
            //return View(input);
        }
        #endregion
    }
}