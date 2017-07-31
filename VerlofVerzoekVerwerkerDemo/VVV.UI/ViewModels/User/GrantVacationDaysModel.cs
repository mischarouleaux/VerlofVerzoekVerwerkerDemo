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
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using VVV.Business.Identity;
using VVV.UI.Helpers;

namespace VVV.UI.ViewModels.User
{
    public class GrantVacationDaysModel
    {
        
        public MultiSelectList Users { get; set; }
        public long[] SelectedID { get; set; }                    
        public int Days { get; set; }
        public int Hours { get; set; }
        public int Minutes { get; set; }

        public int TotalMinutes { get; set; }

        public string SearchQuery { get; set; }

        public bool IsApplications { get; set; }
        public bool IsServices { get; set; }
        public bool IsKnowledgeCenter { get; set; }
        public bool IsConsultancy { get; set; }

        public GrantVacationDaysModel() { }


        public GrantVacationDaysModel(bool settrue)
        {
            IsApplications = true;
            IsServices = true;
            IsKnowledgeCenter = true;
            IsConsultancy = true;
        }

        public int CalculateMinutes(int Days, int Hours, int Minutes)
        {
            var i = Hours * 60;
            TotalMinutes = (Days * 480) + (Hours * 60) + Minutes;

           return TotalMinutes;
        }

        public MutationsVacation ToMutationsVacation(long UserID, MutationsVacation mutvac = null)
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

    }
}