using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VVV.Business.Mail
{
    class MailTemplateArguments
    {
        //Confirmation VacationRequest
        public const string User = "<!--User-->";
        public const string Reason = "<!--Reason-->";
        public const string ReasonRejection = "<!--ReasonRejection-->";
        public const string BeginDate = "<!--BeginDate-->";
        public const string BeginTime = "<!--BeginTime-->";
        public const string EndDate = "<!--EndDate-->";
        public const string EndTime = "<!--EndTime-->";
        public const string TotalDays = "<!--TotalDays-->";

        //ResetPassword
        public const string Link = "<!--link-->";
        public const string ValidityInMinutes = "<!--validityinminutes-->";
        public const string ValidityInDays = "<!--validityindays-->";

        public const string WelcomeLink = "<!--WelcomeLink-->";

        //NewVacationRequestManager
        public const string NameManager = "<!--NaamManager-->";
        public const string NameEmployee = "<!--NaamMedewerker-->";
        public const string ReasonVacationRequest = "<!--RedenVerlofverzoek-->";
        public const string NewBeginDate = "<!--BeginDatum-->";
        public const string NewEndDate = "<!--EindDatum-->";
    }
}
