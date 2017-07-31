using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;





namespace VVV.Business.Mail
{
    public static class MailHelper
    {
        public static async Task<bool> NewVacationRequestManager(string toUser, string namemanager, string nameemployee, string reasonrequest, string begindate, string enddate)
        {
            var subject = "U heeft een nieuw verlofverzoek.";
            var message = Properties.Resources.NewVacationRequestManager
                .Replace(MailTemplateArguments.NameManager, namemanager)
                .Replace(MailTemplateArguments.NameEmployee, nameemployee)
                .Replace(MailTemplateArguments.ReasonVacationRequest, reasonrequest)
                .Replace(MailTemplateArguments.NewBeginDate, begindate)
                .Replace(MailTemplateArguments.NewEndDate, enddate);
            
            return await SendAsync(Properties.Settings.Default.MailSender, toUser, subject, message);
        }

        public static async Task<bool> RequestAcceptedManagerAsync(string toUser, string user, string Reason, string BeginDate, string BeginTime, string EndDate, string EndTime, string TotalDays)
        {
            var subject = "VerlofVerzoek geaccepteerd door uw manager";
            var message = Properties.Resources.RequestAcceptedManager
                .Replace(MailTemplateArguments.User, user)
                .Replace(MailTemplateArguments.Reason, Reason)
                .Replace(MailTemplateArguments.BeginDate, BeginDate)
                .Replace(MailTemplateArguments.BeginTime, BeginTime)
                .Replace(MailTemplateArguments.EndDate, EndDate)
                .Replace(MailTemplateArguments.EndTime, EndTime)
                .Replace(MailTemplateArguments.TotalDays, TotalDays);
            

            return await SendAsync(Properties.Settings.Default.MailSender, toUser, subject, message);
        }

        public static async Task<bool> RequestRejectedAsync(string toUser, string ReasonRejection, string Reason, string BeginDate, string BeginTime, string EndDate, string EndTime, string TotalDays)
        {
            var subject = "VerlofVerzoek afgewezen door uw manager";
            var message = Properties.Resources.RequestRejected
                .Replace(MailTemplateArguments.ReasonRejection, ReasonRejection)
                .Replace(MailTemplateArguments.Reason, Reason)
                .Replace(MailTemplateArguments.BeginDate, BeginDate)
                .Replace(MailTemplateArguments.BeginTime, BeginTime)
                .Replace(MailTemplateArguments.EndDate, EndDate)
                .Replace(MailTemplateArguments.EndTime, EndTime)
                .Replace(MailTemplateArguments.TotalDays, TotalDays);


            return await SendAsync(Properties.Settings.Default.MailSender, toUser, subject, message);
        }

        public static async Task<bool> RequestRejectedManagerAsync(string user, string toUser, string ReasonRejection, string Reason, string BeginDate, string BeginTime, string EndDate, string EndTime, string TotalDays)
        {
            var subject = "VerlofVerzoek afgewezen door uw manager";
            var message = Properties.Resources.RequestRejectedManager
                .Replace(MailTemplateArguments.User, user)
                .Replace(MailTemplateArguments.ReasonRejection, ReasonRejection)
                .Replace(MailTemplateArguments.Reason, Reason)
                .Replace(MailTemplateArguments.BeginDate, BeginDate)
                .Replace(MailTemplateArguments.BeginTime, BeginTime)
                .Replace(MailTemplateArguments.EndDate, EndDate)
                .Replace(MailTemplateArguments.EndTime, EndTime)
                .Replace(MailTemplateArguments.TotalDays, TotalDays);


            return await SendAsync(Properties.Settings.Default.MailSender, toUser, subject, message);
        }


        public static async Task<bool> SendAppointmentAsync(string toUser, string Reason, string BeginDate, string BeginTime, string EndDate, string EndTime, string TotalDays, DateTime startDate, DateTime endDate, Attachment attachment)
        {
            var subject = "VerlofVerzoek geaccepteerd";
            var message = Properties.Resources.RequestAccepted
                .Replace(MailTemplateArguments.Reason, Reason)
                .Replace(MailTemplateArguments.BeginDate, BeginDate)
                .Replace(MailTemplateArguments.BeginTime, BeginTime)
                .Replace(MailTemplateArguments.EndDate, EndDate)
                .Replace(MailTemplateArguments.EndTime, EndTime)
                .Replace(MailTemplateArguments.TotalDays, TotalDays);

            return await SendAttachmentAsync(Properties.Settings.Default.MailSender, toUser, subject, message, attachment);
        }

        public static async Task<bool> ResetPassword(string emailReceiver, string setPasswordLink, int validity)
        {
            var subject = "Wachtwoord resetten";
            var message = Properties.Resources.ResetPassword
                .Replace(MailTemplateArguments.Link, setPasswordLink)
                .Replace(MailTemplateArguments.ValidityInMinutes, validity.ToString());

            return await SendAsync(Properties.Settings.Default.MailSender, emailReceiver, subject, message);
        }

        public static async Task<bool> ResetFirstPassword(string emailReceiver, string setPasswordLink, int validity)
        {
            var subject = "Welkom bij de VerlofVerzoekVerwerker";
            var message = Properties.Resources.ResetPassword
                .Replace(MailTemplateArguments.Link, setPasswordLink)
                .Replace(MailTemplateArguments.ValidityInDays, validity.ToString());

            return await SendAsync(Properties.Settings.Default.MailSender, emailReceiver, subject, message);
        }

        public static async Task<bool> Welcome(string emailReceiver, string Link)
        {
            var subject = "Welkom bij de VerlofVerzoekVerwerker";
            var message = Properties.Resources.Welcome
                .Replace(MailTemplateArguments.WelcomeLink, Link);

            return await SendAsync(Properties.Settings.Default.MailSender, emailReceiver, subject, message);
        }

        public static async Task<bool> ResetPasswordConfirmation(string emailReceiver, string settingsLink)
        {
            var subject = "Wachtwoord gereset";
            var message = Properties.Resources.ResetPasswordConfirmation
                .Replace(MailTemplateArguments.Link, settingsLink);

            return await SendAsync(Properties.Settings.Default.MailSender, emailReceiver, subject, message);
        }


        private static async Task<bool> SendAsync(string from, string toUser, string subject, string message)
        {
            try
            {
                using (var msg = new MailMessage())
                {
                    var fromMail = new MailAddress(from, from);
                    var toMailUser = new MailAddress(toUser);


                    // Set the mail message.
                    msg.From = fromMail;
                    msg.To.Add(toMailUser);
                    msg.Subject = subject;
                    msg.IsBodyHtml = true;
                    msg.Body = message;
                    
                    


                    // Create a mailclient.
                    using (var smtp = new SmtpClient())
                    {
                        // Set some SMTP settings.
                        smtp.Host = Properties.Settings.Default.MailHost;
                        smtp.Port = Properties.Settings.Default.MailPort;
                        smtp.EnableSsl = true; //<--stond op false

                        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                        //smtp.UseDefaultCredentials = false; //<--toegevoegd
                        smtp.Credentials = new NetworkCredential(Properties.Settings.Default.MailUsername, Properties.Settings.Default.MailPassword);

                        // Send the mail please!
                        await smtp.SendMailAsync(msg);

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private static async Task<bool> SendAttachmentAsync(string from, string toUser, string subject, string message, Attachment attachment)
        {
            try
            {
                using (var msg = new MailMessage())
                {
                    var fromMail = new MailAddress(from, from);
                    var toMailUser = new MailAddress(toUser);


                    // Set the mail message.
                    msg.From = fromMail;
                    msg.To.Add(toMailUser);
                    msg.Subject = subject;
                    msg.IsBodyHtml = true;
                    msg.Body = message;
                    msg.Attachments.Add(attachment);
                    




                    // Create a mailclient.
                    using (var smtp = new SmtpClient())
                    {
                        // Set some SMTP settings.
                        smtp.Host = Properties.Settings.Default.MailHost;
                        smtp.Port = Properties.Settings.Default.MailPort;
                        smtp.EnableSsl = true; //<--stond op false

                        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                        //smtp.UseDefaultCredentials = false; //<--toegevoegd
                        smtp.Credentials = new NetworkCredential(Properties.Settings.Default.MailUsername, Properties.Settings.Default.MailPassword);

                        // Send the mail please!
                        await smtp.SendMailAsync(msg);

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
