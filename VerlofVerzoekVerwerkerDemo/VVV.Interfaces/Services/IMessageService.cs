using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VVV.Models;

namespace VVV.Interfaces.Services
{
    public interface IMessageService
    {
        Message Get(long id);

        IQueryable<Message> GetAllMessagesByUserID(long userid);
        IQueryable<Message> GetAllDeletedMessageByUserID(long userid);

        void MailNewVacationRequest(ApplicationUser user, ApplicationUser manager, VacationRequest vacreq);

        void MailVacationRequestAccepted(ApplicationUser user, ApplicationUser manager, VacationRequest vacreq);

        void MailVacationRequestRejected(ApplicationUser user, ApplicationUser manager, VacationRequest vacreq);

        #region Delete
        void ToTrash(long id);
        void UndoTrash(long id);
        void Delete(long id);
        #endregion

        #region Unread
        int GetUnreadInboxMessages(long userid);
        int GetUnreadDeletedMessages(long userid);
        #endregion

        #region ToRead
        void ToRead(long id);
        #endregion
    }
}
