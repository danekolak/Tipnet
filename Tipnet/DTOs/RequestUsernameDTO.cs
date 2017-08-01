using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Tipnet.DTOs
{
    public class RequestUsernameDTO
    {
        [Required(ErrorMessageResourceType = typeof(ErrorMessages.ErrorMessages), ErrorMessageResourceName = "BirthdayRequired")]
        [Display(Name = "Birthdate", ResourceType = typeof(ErrorMessages.ErrorMessages))]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DatumRodenja { get; set; }

        [Required(ErrorMessageResourceType = typeof(ErrorMessages.ErrorMessages), ErrorMessageResourceName = "EmailRequired")]
        [Display(Name = "Email", ResourceType = typeof(ErrorMessages.ErrorMessages))]
        [EmailAddress]
        public string Email { get; set; }
    }
}