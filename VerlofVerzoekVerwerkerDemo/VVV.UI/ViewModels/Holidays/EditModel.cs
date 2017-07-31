using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentValidation.Attributes;
using Newtonsoft.Json;

using VVV.Models;
using PagedList;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using VVV.Business.Identity;
using VVV.UI.Helpers;
using System.ComponentModel.DataAnnotations;

namespace VVV.UI.ViewModels.Holidays
{
    public class EditModel
    {
        public long HolidayID { get; set; }
        public string Description { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }


        public EditModel() { }

        public EditModel(Holiday holiday)
        {
            HolidayID = holiday.HolidayID;
            Description = holiday.Description;
            Date = holiday.Date;
        }

        public Holiday ToHoliday(Holiday holiday = null)
        {
            if (holiday == null)
            {
                holiday = new Holiday();
            }

            holiday.Date = Date;
            holiday.Description = Description;

            return holiday;
        }
    }
}