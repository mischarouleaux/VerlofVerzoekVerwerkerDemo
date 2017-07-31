using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VVV.Interfaces.Services;
using VVV.Interfaces.Repositories;
using VVV.Interfaces;
using VVV.Models;


namespace VVV.Business.Services
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _repository;
        private readonly IUnitOfWork _uow;


        public MessageService(IMessageRepository repo, IUnitOfWork uow)
        {
            _repository = repo;
            _uow = uow;
        }

        #region GetMessages
        //Haalt een specifiek bericht op, op basis van het id. 
        public Message Get(long id)
        {
            return _repository.Get(filter => filter.MessageID == id, i => i.VacationRequest).FirstOrDefault();
        }
        //Haalt alle berichten op van een gebruiker op.
        public IQueryable<Message> GetAllMessagesByUserID(long userid)
        {
            return _repository.Get(filter => filter.ReceiverID == userid && filter.IsDeleted == false).OrderByDescending(p => p.DateSend);
        }
        //Haalt alle verwijderde berichten op van een gebruiker.
        public IQueryable<Message> GetAllDeletedMessageByUserID(long userid)
        {
            return _repository.Get(filter => filter.ReceiverID == userid && filter.IsDeleted == true).OrderByDescending(p => p.DateSend);
        }

        #endregion

        #region Create Mails
        //Maakt een nieuw bericht waarin een gebruiker bericht krijgt dat een gebruiker een nieuwe verlofverzoek heeft ingediend.
        public void MailNewVacationRequest(ApplicationUser user, ApplicationUser manager, VacationRequest vacreq)
        {
            Message message = new Message();
            message.ApplicationUser = user;
            message.DateSend = DateTime.Now;
            message.IsDeleted = false;
            message.IsRead = false;
            message.SenderID = user.UserID;
            message.ReceiverID = manager.UserID;
            message.Subject = "U heeft een nieuw verlofverzoek";
            message.MessageText = "Geachte " + manager.FirstName + "," + "@" + "U heeft een nieuw verlofverzoek ontvangen van " + user.FirstName + " " + user.LastName + "@" + "Graag deze te beoordelen" + "@" + "Met vriendelijke groet," + "@" + "De VerlofVerzoekVerwerker";

            message.VacationRequestID = vacreq.VacationID;
            message.VacationRequest = vacreq;

            _repository.Add(message);
            _uow.Commit();
        }
        
        //Maakt een nieuw bericht aan wanneer er een verlofverzoek is geaccepteerd.
        public void MailVacationRequestAccepted(ApplicationUser user, ApplicationUser manager, VacationRequest vacreq)
        {
            Message message = new Message();
            message.ApplicationUser = user;
            message.DateSend = DateTime.Now;
            message.IsDeleted = false;
            message.IsRead = false;
            message.SenderID = manager.UserID;
            message.ReceiverID = vacreq.UserID;
            message.Subject = "Uw verlofverzoek is geaccepteerd";
            message.MessageText = "Geachte " + user.FirstName + "," + "@" + "Uw verlofverzoek is geaccepteerd door uw manager " + manager.FirstName + " " + manager.LastName + "@" + "Met vriendelijke groet," + "@" + "De VerlofVerzoekVerwerker";

            message.VacationRequestID = vacreq.VacationID;
            message.VacationRequest = vacreq;

            _repository.Add(message);
            _uow.Commit();

        }

        //Maakt een nieuw bericht aan wanneer er een verlofverzoek is afgewezen.
        public void MailVacationRequestRejected(ApplicationUser user, ApplicationUser manager, VacationRequest vacreq)
        {
            Message message = new Message();
            message.ApplicationUser = user;
            message.DateSend = DateTime.Now;
            message.IsDeleted = false;
            message.IsRead = false;
            message.SenderID = manager.UserID;
            message.ReceiverID = vacreq.UserID;
            message.Subject = "Uw verlofverzoek is afgewezen";
            message.MessageText = "Geachte " + user.FirstName + "," + "@" + "Uw verlofverzoek is afgewezen door uw manager " + manager.FirstName + " " + manager.LastName + "@" + "Met vriendelijke groet," + "@" + "De VerlofVerzoekVerwerker";

            message.VacationRequestID = vacreq.VacationID;
            message.VacationRequest = vacreq;

            _repository.Add(message);
            _uow.Commit();
        }

        //Maakt een nieuw bericht aan wanneer de manager een tegenbod heeft gedaan richting een medewerker.
        public void MailVacantionRequestProposition(ApplicationUser user, ApplicationUser manager, VacationRequest vacreq)
        {
            Message message = new Message();
            message.ApplicationUser = user;
            message.DateSend = DateTime.Now;
            message.IsDeleted = false;
            message.IsRead = false;
            message.SenderID = manager.UserID;
            message.ReceiverID = vacreq.UserID;
            message.Subject = "Uw manager heeft een voorstel gedaan";
            message.MessageText = "Geachte " + user.FirstName + "," + "@" + "Uw verlofverzoek is afgewezen door uw manager " + user.FirstName + " " + user.LastName + ". Er is een nieuw voorstel gedaan!" + "@" + "Met vriendelijke groet," + "@" + "De VerlofVerzoekVerwerker";

            message.VacationRequestID = vacreq.VacationID;
            message.VacationRequest = vacreq;

            _repository.Add(message);
            _uow.Commit();

        }

        #endregion

        #region Delete

        //Plaats een bericht naar de "prullenmand" van het berichtencentrum.
        public void ToTrash(long id)
        {
            long messageid = id;

            var query = _repository.Get(filter => filter.MessageID == messageid).FirstOrDefault();

            query.IsDeleted = true;
            _uow.Commit();
        }

        //Plaats een bericht terug naar de inbox.
        public void UndoTrash(long id)
        {
            long messageid = id;

            var query = _repository.Get(filter => filter.MessageID == messageid).FirstOrDefault();

            query.IsDeleted = false;
            _uow.Commit();
        }

        //Verwijdert een bericht.
        public void Delete(long id)
        {
            _repository.Delete(id);
            _uow.Commit();
        }
        #endregion

        #region Unread Count

        //Telt alle niet gelezen berichten op
        public int GetUnreadInboxMessages(long userid)
        {
            return _repository.Get(filter => filter.ReceiverID == userid && filter.IsDeleted == false && filter.IsRead == false).Count();
        }

        //Telt alle verwijderde niet gelezen berichten op.
        public int GetUnreadDeletedMessages(long userid)
        {
            return _repository.Get(filter => filter.ReceiverID == userid && filter.IsDeleted == true && filter.IsRead == false).Count();
        }

        #endregion

        #region ToRead
        //Zorgt ervoor dat een bericht naar gelezen wordt geplaatst.
        public void ToRead(long id)
        {
            long messageid = id;

            var query = Get(messageid);

            query.IsRead = true;

            _uow.Commit();
        }

        #endregion
    }
}
