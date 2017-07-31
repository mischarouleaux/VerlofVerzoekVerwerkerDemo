using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VVV.UI.Helpers;

namespace VVV.UI.Controllers
{

    /// <summary>
    /// Handles implementing basic controller functions for re-use.
    /// </summary>
    public class BaseController : Controller
    {

        #region constructors

        public BaseController()
        {
            ViewBag.UnreadMessages = "(5)";
        }

        #endregion

        

        /// <summary>
        /// Shows a message on the screen.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="messageType"></param>
        public void ShowMessage(string message, MessageType messageType, bool shouldSurviveRedirect = false)
        {
            if (!shouldSurviveRedirect)
            {
                ViewBag.Message = message;
                ViewBag.MessageType = messageType;
            }
            else
            {
                TempData["Message"] = message;
                TempData["MessageType"] = messageType;
            }
        }

    }
}