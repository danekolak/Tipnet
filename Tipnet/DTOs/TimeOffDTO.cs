using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.AccessControl;
using System.Web;

namespace Tipnet.DTOs
{
    public class TimeOffDTO
    {
        [Required(ErrorMessageResourceType = typeof(ErrorMessages.ErrorMessages), ErrorMessageResourceName = "AutoLogOffRequired")]
        [Display(Name = "AutoLogOff", ResourceType = typeof(ErrorMessages.ErrorMessages))]
        public int AutoOdjava { get; set; }

        public DateTime AutoOdjavaTimeStamp { get; set; }
        public bool AutoOdjavaFlag { get; set; }

        [Required(ErrorMessageResourceType = typeof(ErrorMessages.ErrorMessages), ErrorMessageResourceName = "BettingLimitRequired")]
        [Display(Name = "BettingTimeout", ResourceType = typeof(ErrorMessages.ErrorMessages))]
        public int PauzaKladenja { get; set; }

        public DateTime PauzaKladenjaTimeStamp { get; set; }
        public bool PauzaKladenjaFlag { get; set; }
    }
}