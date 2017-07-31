using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VVV.Models;
using PagedList;

namespace VVV.UI.ViewModels.User
{
    public class MutationsModel
    {
        public IPagedList<MutationsVacation> Mutations { get; set; }
        public long UserID { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }

        

        
        public MutationsModel(long userid)
        {
            UserID = userid;
            Page = 1;
            PageSize = Properties.Settings.Default.PageSize;
            Mutations = new List<MutationsVacation>().ToPagedList(this.Page, this.PageSize);
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