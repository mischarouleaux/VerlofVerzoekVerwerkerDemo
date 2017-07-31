using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VVV.Models;
using VVV.UI.Helpers;
using PagedList;

namespace VVV.UI.ViewModels.User
{
    public class DetailsModel
    {
        public long UserID { get; set; }
        
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Department { get; set; }
        public bool IsMedewerker { get; set; }
        public bool IsManager { get; set; }
        public bool IsHRManager { get; set; }
        public bool IsDeleted { get; set; }

        public string ManagerUserName { get; set; }
        public string ManagerFirstName { get; set; }
        public string ManagerLastName { get; set; }
        public string ManagerEmail { get; set; }

        public bool HasSecondManager { get; set; }

        public string SecondManagerUserName { get; set; }
        public string SecondManagerFirstName { get; set; }
        public string SecondManagerLastName { get; set; }
        public string SecondManagerEmail { get; set; }

        public bool HasSuccesMessage { get; set; }

        

        public DetailsModel(ApplicationUser user, ApplicationUser manager)
        {
            UserID = SecurityHelper.GetUserId();
            
            FirstName = user.FirstName;
            LastName = user.LastName;
            Email = user.Email;
            Department = user.Department1.DepartmentName;

            IsMedewerker = user.IsMedewerker;
            IsManager = user.IsManager;
            IsHRManager = user.IsHRManager;
            IsDeleted = user.IsActive;

            
            ManagerFirstName = manager.FirstName;
            ManagerLastName = manager.LastName;
            ManagerEmail = manager.Email;
            HasSecondManager = false;
        }

            

        public  DetailsModel(ApplicationUser user, ApplicationUser manager, ApplicationUser secondmanager)
        {
            UserID = SecurityHelper.GetUserId();
            
            FirstName = user.FirstName;
            LastName = user.LastName;
            Email = user.Email;
            Department = user.Department1.DepartmentName;

            IsMedewerker = user.IsMedewerker;
            IsManager = user.IsManager;
            IsHRManager = user.IsHRManager;
            IsDeleted = user.IsActive;

            
            ManagerFirstName = manager.FirstName;
            ManagerLastName = manager.LastName;
            ManagerEmail = manager.Email;

            
            HasSecondManager = true;
            SecondManagerFirstName = secondmanager.FirstName;
            SecondManagerLastName = secondmanager.LastName;
            SecondManagerEmail = secondmanager.Email;
            
        }

    }
}