using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Tipnet.DTOs
{
    public class EditDataDTO
    {
        [Display(Name = "StreetName", ResourceType = typeof(ErrorMessages.ErrorMessages))]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages.ErrorMessages), ErrorMessageResourceName = "StreetRequired")]
        public string Ulica { get; set; }

        [Display(Name = "HouseNumber", ResourceType = typeof(ErrorMessages.ErrorMessages))]
        public string KucniBroj { get; set; }

        [Display(Name = "City", ResourceType = typeof(ErrorMessages.ErrorMessages))]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages.ErrorMessages), ErrorMessageResourceName = "CityRequired")]
        public string Grad { get; set; }

        [Required(ErrorMessageResourceType = typeof(ErrorMessages.ErrorMessages), ErrorMessageResourceName = "ZipRequired")]
        [Display(Name = "HouseNumber", ResourceType = typeof(ErrorMessages.ErrorMessages))]
        public int PostanskiBroj { get; set; }

        [Required(ErrorMessageResourceType = typeof(ErrorMessages.ErrorMessages), ErrorMessageResourceName = "CountryRequired")]
        [Display(Name = "Country", ResourceType = typeof(ErrorMessages.ErrorMessages))]
        public string Drzava { get; set; }

        [Display(Name = "Language", ResourceType = typeof(ErrorMessages.ErrorMessages))]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages.ErrorMessages), ErrorMessageResourceName = "LanguageRequired")]
        public string Jezik { get; set; }

        [Display(Name = "PhoneNumber", ResourceType = typeof(ErrorMessages.ErrorMessages))]
        public string BrojTelefona { get; set; }

        [Display(Name = "MobilePhoneNumber", ResourceType = typeof(ErrorMessages.ErrorMessages))]
        public string BrojMobilnog { get; set; }

        [Display(Name = "Title", ResourceType = typeof(ErrorMessages.ErrorMessages))]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages.ErrorMessages), ErrorMessageResourceName = "TitleRequired")]
        public string Oslovljavanje { get; set; }
    }
}