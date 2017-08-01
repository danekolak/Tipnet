using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.AccessControl;
using System.Web;

namespace Tipnet.DTOs
{
    public class NewPasswordDTO
    {
        [Display(Name = "Password", ResourceType = typeof(ErrorMessages.ErrorMessages))]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages.ErrorMessages), ErrorMessageResourceName = "PasswordRequired")]
        [StringLength(40, MinimumLength = 8, ErrorMessageResourceType = typeof(ErrorMessages.ErrorMessages), ErrorMessageResourceName = "PasswordLength")]
        [RegularExpression("^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*[!#$()?{}|*+,^.\\-+&=%_:;~@])(?=\\S+$).{8,}$",
            ErrorMessageResourceType = typeof(ErrorMessages.ErrorMessages), ErrorMessageResourceName = "PasswordExpression")]
        public string Password { get; set; }

        [Display(Name = "ConfirmPassword", ResourceType = typeof(ErrorMessages.ErrorMessages))]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages.ErrorMessages), ErrorMessageResourceName = "ConfirmPasswordRequired")]
        [Compare("Password", ErrorMessageResourceType = typeof(ErrorMessages.ErrorMessages), ErrorMessageResourceName = "PasswordMatch")]
        public string ConfirmPassword { get; set; }

        public string PrGuid { get; set; }
    }
}