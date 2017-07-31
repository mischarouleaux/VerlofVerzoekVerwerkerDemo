using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VVV.Models;
using PagedList;
using VVV.UI.Helpers;

namespace VVV.UI.ViewModels.Verlofaanvraag
{
    public class IndexModel
    {
        public long vacid { get; set; }
        public long userid { get; set; }
        public IPagedList<VacationRequest> VacationRequest { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string IsAccepted { get; set; }
        public double Days { get; set; }
        public int Hours { get; set; }
        public int Minutes { get; set; }
        public string Time { get; set; }
        public ApplicationUser user { get; set; }
        public bool HasSuccesMessage { get; set; }
        public bool HasAcceptedMessage { get; set; }
        public bool HasRejectedMessage { get; set; }
        public bool HasProporsitionMessage { get; set; }
        public bool HasDeletedMessage { get; set; }

        

        //Zorgt voor de koppeling tussen de controller & de view.
        public IndexModel()
        {
            userid = SecurityHelper.GetUserId();
            Page = 1;
            PageSize = Properties.Settings.Default.PageSize;
            VacationRequest = new List<VacationRequest>().ToPagedList(this.Page, this.PageSize);
        }

        //TODO Waarvoor zorgt dit?
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