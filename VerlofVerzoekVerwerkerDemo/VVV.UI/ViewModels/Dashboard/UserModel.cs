using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VVV.UI.Helpers;
using VVV.Models;
using PagedList;

namespace VVV.UI.ViewModels.Dashboard
{
    public class UserModel
    {
        public string Time { get; set; }
        public IPagedList<VacationRequest> VacationRequest { get; set; }
        public IPagedList<VacationRequest> VacationRequestToday { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public IPagedList<VacationRequest> VacationRequestAccepted { get; set; }
        public IPagedList<VacationRequest> VacationRequestRejected { get; set; }
        public IPagedList<VacationRequest> VacationRequestInTreatment { get; set; }
        public IPagedList<VacationRequest> VacationRequestProposition { get; set; }


        public IPagedList<ApplicationUser> currentuser { get; set; }
        public string Username { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }


        public IPagedList<ApplicationUser> currentmanager { get; set; }
        public string Managerfirstname { get; set; }
        public string Managerlastname { get; set; }




        public UserModel ()
        {
            Page = 1;
            PageSize = Properties.Settings.Default.PageSize;

            //Alle verlofaanvragen
            VacationRequest = new List<VacationRequest>().ToPagedList(this.Page, this.PageSize);

            //Verlof van vandaag
            VacationRequestToday = new List<VacationRequest>().ToPagedList(this.Page, this.PageSize);

            //Geaccepteerd verlof
            VacationRequestAccepted = new List<VacationRequest>().ToPagedList(this.Page, this.PageSize);

            //Afgewezen verlof
            VacationRequestRejected = new List<VacationRequest>().ToPagedList(this.Page, this.PageSize);

            //Verlofaanvragen in behandeling
            VacationRequestInTreatment = new List<VacationRequest>().ToPagedList(this.Page, this.PageSize);

            //Voorgestelde verlofaanvragen
            VacationRequestProposition = new List<VacationRequest>().ToPagedList(this.Page, this.PageSize);

            //Gebruikers informatie
            currentuser = new List<ApplicationUser>().ToPagedList(this.Page, this.PageSize);

            //Manager informatie
            currentmanager = new List<ApplicationUser>().ToPagedList(this.Page, this.PageSize);

        }

        public object GetRouteValues(bool resetpage = false)
        {
            return new
            {
                page = resetpage ? 1 : Page,
                pagesize = PageSize
            };
        }
    }
}