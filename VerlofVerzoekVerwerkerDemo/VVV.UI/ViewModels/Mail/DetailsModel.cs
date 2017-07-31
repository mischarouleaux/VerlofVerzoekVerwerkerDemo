using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VVV.Models;
using VVV.UI.Controllers;
using PagedList;

namespace VVV.UI.ViewModels.Mail
{
    public class DetailsModel
    {
        public long MessageID { get; set; }
        public long ManagerID { get; set; }
        public long VacationRequestID { get; set; }

        public VacationRequest vacreq { get;set; }

        public string NameSender { get; set; }
        public string NameReceiver { get; set; }

        public string Subject { get; set; }
        public string Message { get; set; }

        public bool IsRead { get; set; }
        public bool IsDeleted { get; set; }

        public DateTime DateSend { get; set; }
        public int HourSend { get; set; }
        public int MinuteSend { get; set; }

        public int Page { get; set; }
        public int PageSize { get; set; }
        public IPagedList<Message> Messages { get; set; }

        public int unreadinbox { get; set; }
        public int unreaddeleted { get; set; }

        public DetailsModel(int UnreadInbox, int UnreadDeleted)
        {
            Page = 1;
            PageSize = Properties.Settings.Default.PageSize;
           

            unreadinbox = UnreadInbox;
            unreaddeleted = UnreadDeleted;
        }

        public DetailsModel(int UnreadInbox, int UnreadDeleted, Message message)
        {
            Page = 1;
            PageSize = Properties.Settings.Default.PageSize;
            

            unreadinbox = UnreadInbox;
            unreaddeleted = UnreadDeleted;

            MessageID = message.MessageID;
            VacationRequestID = message.VacationRequestID;
            vacreq = message.VacationRequest;
            Subject = message.Subject;
            Message = message.MessageText;
            DateSend = message.DateSend;

            IsRead = message.IsRead;
            IsDeleted = message.IsDeleted;

            
        }

        public DetailsModel()
        {
            Page = 1;
            PageSize = Properties.Settings.Default.PageSize;
            Messages = new List<Message>().ToPagedList(this.Page, this.PageSize);
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