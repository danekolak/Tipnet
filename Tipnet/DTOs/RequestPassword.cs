using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Tipnet.DTOs
{
    public class RequestPassword
    {
        [Required(ErrorMessageResourceType = typeof(ErrorMessages.ErrorMessages), ErrorMessageResourceName = "BirthdayRequired")]
        [Display(Name = "Birthdate", ResourceType = typeof(ErrorMessages.ErrorMessages))]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DatumRodenja { get; set; }

        [Required(ErrorMessageResourceType = typeof(ErrorMessages.ErrorMessages), ErrorMessageResourceName = "UsernameRequired")]
        [Display(Name = "Username", ResourceType = typeof(ErrorMessages.ErrorMessages))]
        public string Username { get; set; }
    }
}