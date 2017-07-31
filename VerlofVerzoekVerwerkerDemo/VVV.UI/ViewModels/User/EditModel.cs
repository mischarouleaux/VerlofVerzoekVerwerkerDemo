using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation.Attributes;
using Newtonsoft.Json;
using System.Web;
using VVV.Models;
using PagedList;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using VVV.Business.Identity;
using VVV.UI.Helpers;
using System.ComponentModel.DataAnnotations;



namespace VVV.UI.ViewModels.User
{
    public class EditModel
    {
        public long UserID { get; set; }
        public string UserName { get; set; }       
        public string FirstName { get; set; }
        public string  LastName { get; set; }

        
        public string Email { get; set; }
        public string Department { get; set; }

        public bool IsMedewerker { get; set; }
        public bool IsManager { get; set; }
        public bool IsHRManager { get; set; }
        public bool IsActive { get; set; }

        public string ManagerUserName { get; set; }
        public string ManagerFirstName { get; set; }
        public string ManagerLastName { get; set; }
        public string ManagerEmail { get; set; }

        public bool HasSecondManager { get; set; }

        public string SecondManagerUserName { get; set; }
        public string SecondManagerFirstName { get; set; }
        public string SecondManagerLastName { get; set; }
        public string SecondManagerEmail { get; set; }

        public int Days { get; set; }
        public int Hours { get; set; }
        public int Minutes { get; set; }

        public string VacationDaysInDays { get; set; }
        public string VacationDaysInHours { get; set; }
        public string VacationDaysInMinutes { get; set; }

        public SelectList FirstManagers { get; set; }
        public long SelectedFirstManager { get; set; }
        public SelectList SecondManagers { get; set; }
        public long SelectedSecondManager { get; set; }

        public SelectList Departments { get; set; }
        public long SelectedDepartment { get; set; }

        public bool IsAvailable { get; set; }
        public bool IsNotAvailable { get; set; }

        public System.Uri previousUrl { get; set; }
        public bool HasSuccesMessage { get; set; }
        

        public EditModel(ApplicationUser user)
        {
            UserID = user.UserID;
            
            
            FirstName = user.FirstName;
            LastName = user.LastName;
            Email = user.Email;
            Department = user.Department1.DepartmentName;

            

            IsMedewerker = user.IsMedewerker;
            IsManager = user.IsManager;
            IsHRManager = user.IsHRManager;
        }


        public EditModel(ApplicationUser user, ApplicationUser manager, int mutvac, bool? available = null)
        {
            UserID = user.UserID;
            
            FirstName = user.FirstName;
            LastName = user.LastName;
            Email = user.Email;
            Department = user.Department1.DepartmentName;

            IsMedewerker = user.IsMedewerker;
            IsManager = user.IsManager;
            IsHRManager = user.IsHRManager;
            IsActive = user.IsActive;

            
            ManagerFirstName = manager.FirstName;
            ManagerLastName = manager.LastName;
            ManagerEmail = manager.Email;
            HasSecondManager = false;

            Days = 0;
            Hours = 0;
            Minutes = 0;

            var daysindays = mutvac / 480;
            var hoursindays = (mutvac % 480) / 60;
            var minutesindays = (mutvac % 480) % 60;
            VacationDaysInDays = daysindays.ToString() + " Dagen, " + hoursindays.ToString() + " uur " + " & " + minutesindays.ToString() + " minuten ";

            var hoursinhours = mutvac / 60;
            var minutesinhours = mutvac % 60;
            VacationDaysInHours = hoursinhours.ToString() + " Uur & " + minutesinhours.ToString() + " minuten";

            VacationDaysInMinutes = mutvac.ToString() + " Minuten";


            if (available != null)
            {
                if (available == true)
                {
                    IsAvailable = true;
                    IsNotAvailable = false;
                }

                if (available == false)
                {
                    IsAvailable = false;
                    IsNotAvailable = true;
                }
            }
        }

        public EditModel(ApplicationUser user, ApplicationUser manager, ApplicationUser secondmanager, int mutvac, bool? available)
        {
            UserID = user.UserID;
            
            FirstName = user.FirstName;
            LastName = user.LastName;
            Email = user.Email;
            Department = user.Department1.DepartmentName;

            IsMedewerker = user.IsMedewerker;
            IsManager = user.IsManager;
            IsHRManager = user.IsHRManager;
            IsActive = user.IsActive;

            
            ManagerFirstName = manager.FirstName;
            ManagerLastName = manager.LastName;
            ManagerEmail = manager.Email;


            HasSecondManager = true;
            
            SecondManagerFirstName = secondmanager.FirstName;
            SecondManagerLastName = secondmanager.LastName;
            SecondManagerEmail = secondmanager.Email;

            Days = 0;
            Hours = 0;
            Minutes = 0;

            var daysindays = mutvac / 480;
            var hoursindays = (mutvac % 480) / 60;
            var minutesindays = (mutvac % 480) % 60; 
            VacationDaysInDays = daysindays.ToString() + " Dagen, " + hoursindays.ToString() + " uur " + " & " + minutesindays.ToString() + " minuten ";

            var hoursinhours = mutvac / 60;
            var minutesinhours = mutvac % 60;
            VacationDaysInHours = hoursinhours.ToString() + " Uur & " + minutesinhours.ToString() + " minuten";

            VacationDaysInMinutes = mutvac.ToString() + " Minuten" ;


            if (available != null)
            {
                if (available == true)
                {
                    IsAvailable = true;
                    IsNotAvailable = false;
                }

                if (available == false)
                {
                    IsAvailable = false;
                    IsNotAvailable = true;
                }
            }
        }

        

        public EditModel() { }

        public MutationsVacation ToMutationsVacation(MutationsVacation mutvac = null)
        {
            if (mutvac == null)
            {
                mutvac = new MutationsVacation();
                mutvac.UserID = UserID;
                mutvac.DateCreated = DateTime.Now;
                mutvac.CreateUser = SecurityHelper.GetUserId();

                mutvac.VacationModification = (Days * 480) + (Hours * 60) + (Minutes);
            }

            mutvac.ChangeUser = SecurityHelper.GetUserId();
            mutvac.DateChanged = DateTime.Now;

            return mutvac;
        }

        public ApplicationUser ToApplicationUser(ApplicationUser user = null)
        {
            if (user == null)
            {
                user = new ApplicationUser();
                user.IsActive = true;
                user.IsBlocked = false;               
                user.DateCreated = DateTime.Now;
                user.CreateUser = SecurityHelper.GetUserId();
                
                
            }

            user.DateChanged = DateTime.Now;
            user.ChangeUser = SecurityHelper.GetUserId();

            
            user.FirstName = FirstName;
            user.LastName = LastName;
            user.Email = Email;            
            user.Manager = SelectedFirstManager;
            user.SecondManager = SelectedSecondManager;
            user.Department = SelectedDepartment;
            user.IsMedewerker = true;
            user.IsManager = IsManager;
            user.IsHRManager = IsHRManager;
            

            return user;
        }

        public Manager ToManager(ApplicationUser user, Manager manager = null)
        {
            if (manager == null)
            {
                manager = new Manager();
                manager.IsAvailable = true;
            }

            if(IsAvailable == true)
            {
                manager.IsAvailable = true;
            }
            if(IsNotAvailable == true)
            {
                manager.IsAvailable = false;
            }

            manager.UserID = user.UserID;

            return manager;
        }
    }
}
