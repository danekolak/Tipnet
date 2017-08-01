using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Tipnet.DTOs
{
    public class ChangeEmailDTO
    {
        [Required(ErrorMessageResourceType = typeof(ErrorMessages.ErrorMessages), ErrorMessageResourceName = "EmailRequired")]
        [Display(Name = "New Email")]
        [EmailAddress]
        [Remote("EmailExists", "Player", HttpMethod = "POST", ErrorMessageResourceType = typeof(ErrorMessages.ErrorMessages), ErrorMessageResourceName = "EmailExists")]
        public string Email { get; set; }

        [Display(Name = "Confirm Email")]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages.ErrorMessages), ErrorMessageResourceName = "ConfirmEmailRequired")]
        [System.ComponentModel.DataAnnotations.Compare("Email", ErrorMessageResourceType = typeof(ErrorMessages.ErrorMessages), ErrorMessageResourceName = "EmailMatch")]
        public string ConfirmEmail { get; set; }
    }
}