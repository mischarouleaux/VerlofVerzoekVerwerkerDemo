using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VVV.Models;
using VVV.UI.Controllers;
using VVV.UI.Helpers;
using PagedList;

namespace VVV.UI.ViewModels.Holidays
{
    public class IndexModel
    {
        public IPagedList<Holiday> Holidays { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public bool HasSavedMessage { get; set; }
        public bool HasChangedMessage { get; set; }
        public bool HasDeleteMessage { get; set; }


        public IndexModel()
        {
            Page = 1;
            PageSize = Properties.Settings.Default.PageSize;
            Holidays = new List<Holiday>().ToPagedList(this.Page, this.PageSize);
        }
    }
}