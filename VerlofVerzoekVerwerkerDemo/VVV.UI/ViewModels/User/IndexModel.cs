using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using VVV.Models;
using VVV.UI.Controllers;
using VVV.UI.Helpers;
using PagedList;

namespace VVV.UI.ViewModels.User
{
    public class IndexModel
    {
        public IPagedList<ApplicationUser> ApplicationUsers { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public bool HasSavedMessage { get; set; }
        public bool HasSuccesMessage { get; set; }


        public string SearchQuery { get; set; }

        public IndexModel()
        {
            Page = 1;
            PageSize = Properties.Settings.Default.PageSize;
            ApplicationUsers = new List<ApplicationUser>().ToPagedList(this.Page, this.PageSize);
            SearchQuery = string.Empty;
        }
        public object GetRouteValues(bool resetpage = false)
        {
            return new
            {
                search = SearchQuery,
                page = resetpage ? 1 : Page,
                pagesize = PageSize
            };
        }

    }
}