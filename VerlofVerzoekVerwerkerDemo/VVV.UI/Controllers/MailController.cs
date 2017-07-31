using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Net.Mail;
using System.Text;
using System.Web.Mvc;
using DayPilot.Web.Mvc;
using DayPilot.Web.Mvc.Events.Month;
using VVV.Interfaces.Services;
using VVV.Business.Identity;
using VVV.UI.Attributes;
using VVV.UI.ViewModels.Mail;
using VVV.UI.Helpers;
using PagedList;
using VVV.Business.Mail;
using System.Threading.Tasks;


namespace VVV.UI.Controllers
{
    public class MailController : BaseController
    {
        private IMessageService _MessService;
        private IVacationRequestService _vacreqService;
        private IManagerService _manService;

        public MailController(IMessageService messService, IVacationRequestService vacreqService, IManagerService manService)
        {
            _MessService = messService;
            _vacreqService = vacreqService;
            _manService = manService;
        }

        #region Index
        [ClaimsAuthorize(UserRoles.Medewerker)]
        public ActionResult Inbox()
        {
            long userid = SecurityHelper.GetUserId();

            int UnreadInbox = _MessService.GetUnreadInboxMessages(userid);
            int UnreadDeleted = _MessService.GetUnreadDeletedMessages(userid);
            var model = new IndexModel(UnreadInbox, UnreadDeleted);


            model.Messages = _MessService.GetAllMessagesByUserID(userid).ToPagedList(model.Page, model.PageSize);


            return View(model);
        }

        [ClaimsAuthorize(UserRoles.Medewerker)]
        public ActionResult DeletedIndex()
        {
            long userid = SecurityHelper.GetUserId();

            int UnreadInbox = _MessService.GetUnreadInboxMessages(userid);
            int UnreadDeleted = _MessService.GetUnreadDeletedMessages(userid);
            var model = new IndexModel(UnreadInbox, UnreadDeleted);


            model.Messages = _MessService.GetAllDeletedMessageByUserID(userid).ToPagedList(model.Page, model.PageSize);


            return View(model);
        }
        #endregion

        #region Details
        [ClaimsAuthorize(UserRoles.Medewerker)]
        public ActionResult Details(long? id)
        {
            if (id != null)
            {
                var message = _MessService.Get(id.Value);

                _MessService.ToRead(message.MessageID);
                long userid = SecurityHelper.GetUserId();

                int UnreadInbox = _MessService.GetUnreadInboxMessages(userid);
                int UnreadDeleted = _MessService.GetUnreadDeletedMessages(userid);

                var model = new DetailsModel(UnreadInbox, UnreadDeleted, message);

                long? managerid = _manService.GetManageridByUserID(SecurityHelper.GetUserId());
                if (managerid != null)
                {
                    model.ManagerID = managerid.Value;
                }

                model.Messages = _MessService.GetAllMessagesByUserID(userid).ToPagedList(model.Page, model.PageSize);

                return View(model);
            }
            return RedirectToAction("Inbox", "Mail");

        }


        [ClaimsAuthorize(UserRoles.Medewerker)]
        public ActionResult DetailsDeleted(long? id)
        {
            if (id != null)
            {
                var message = _MessService.Get(id.Value);
                _MessService.ToRead(message.MessageID);
                long userid = SecurityHelper.GetUserId();

                int UnreadInbox = _MessService.GetUnreadInboxMessages(userid);
                int UnreadDeleted = _MessService.GetUnreadDeletedMessages(userid);

                var model = new DetailsModel(UnreadInbox, UnreadDeleted, message);

                model.Messages = _MessService.GetAllDeletedMessageByUserID(userid).ToPagedList(model.Page, model.PageSize);

                return View(model);
            }
            return RedirectToAction("Inbox", "Mail");

        }
        #endregion

        #region Trash
        [ClaimsAuthorize(UserRoles.Medewerker)]
        public ActionResult ToTrash(long? id)
        {
            if (id != null)
            {
                long messageid = id.Value;
                _MessService.ToTrash(messageid);
                return RedirectToAction("Inbox", "Mail");
            }

            return RedirectToAction("Inbox", "Mail");
        }

        [ClaimsAuthorize(UserRoles.Medewerker)]
        public ActionResult UndoTrash(long? id)
        {
            if (id != null)
            {
                long messageid = id.Value;
                _MessService.UndoTrash(messageid);
                return RedirectToAction("Inbox", "Mail");
            }
            return RedirectToAction("DeletedIndex", "Mail");
        }
        #endregion

        #region Delete
        [ClaimsAuthorize(UserRoles.Medewerker)]
        public ActionResult Delete(long? id)
        {
            if (id != null)
            {
                long messageid = id.Value;
                _MessService.Delete(messageid);
                return RedirectToAction("DeletedIndex", "Mail");
            }

            return RedirectToAction("DeletedIndex", "Mail");
        }
        #endregion

        #region Read/ unread
        [ClaimsAuthorize(UserRoles.Medewerker)]
        public ActionResult IsRead(long? id)
        {
            if (id != null)
            {
                long messageid = id.Value;

                _MessService.ToRead(messageid);

                return RedirectToAction("Inbox", "Mail");
            }

            return RedirectToAction("Inbox", "Mail");
        }


        public string UnreadMessages()
        {
            int countmessages = _MessService.GetUnreadInboxMessages(SecurityHelper.GetUserId());

            return countmessages.ToString();
        }
        #endregion

        #region Count unread messages
        public PartialViewResult GetMessages()
        {
            IndexModel model = new IndexModel();
            model.unreadinbox = _MessService.GetUnreadInboxMessages(SecurityHelper.GetUserId());

            return PartialView("~/Views/Shared/Partials/_PartialUnreadMessages", model);
        }
        #endregion

    }
}