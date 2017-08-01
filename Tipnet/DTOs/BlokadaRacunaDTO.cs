using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Tipnet.DTOs
{
    public class BlokadaRacunaDTO
    {
        [Required(ErrorMessageResourceType = typeof(ErrorMessages.ErrorMessages), ErrorMessageResourceName = "LockDurationRequired")]
        [Display(Name = "LockDuration", ResourceType = typeof(ErrorMessages.ErrorMessages))]
        public string TrajanjeBlokade { get; set; }

        [Required(ErrorMessageResourceType = typeof(ErrorMessages.ErrorMessages), ErrorMessageResourceName = "LockReasonRequired")]
        [Display(Name = "LockReason", ResourceType = typeof(ErrorMessages.ErrorMessages))]
        public string RazlogBlokade { get; set; }

        [Display(Name = "Explanation", ResourceType = typeof(ErrorMessages.ErrorMessages))]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages.ErrorMessages), ErrorMessageResourceName = "ExplanationRequired")]
        public string Obrazlozenje { get; set; }

        public DateTime TimeStampBlokade { get; set; }
        public bool RacunBlokiran { get; set; }
        public bool RacunTrajnoBlokiran { get; set; }
    }
}   