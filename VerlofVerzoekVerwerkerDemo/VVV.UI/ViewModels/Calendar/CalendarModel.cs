using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VVV.Models;

namespace VVV.UI.ViewModels.Calendar
{
    public class CalendarModel
    {
        public long ApplicationID { get; set; }
        public long ServicesID { get; set; }
        public long ConsultancyID { get; set; }
        public long KnowledgeID { get; set; }

        public CalendarModel(long applicationid, long servicesid, long consultancyid, long knowledgeid)
        {
            ApplicationID = applicationid;
            ServicesID = servicesid;
            ConsultancyID = consultancyid;
            KnowledgeID = knowledgeid;
        }
    }

    
}