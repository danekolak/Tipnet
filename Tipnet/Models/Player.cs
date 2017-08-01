using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Policy;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using Microsoft.Ajax.Utilities;

namespace Tipnet.Models
{
    public class Player
    {

        public int Id { get; set; }

        [Display(Name = "FirstName", ResourceType = typeof(ErrorMessages.ErrorMessages))]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages.ErrorMessages), ErrorMessageResourceName = "FirstNameRequired")]
        public string Ime { get; set; }

        [Display(Name = "SecondName", ResourceType = typeof(ErrorMessages.ErrorMessages))]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages.ErrorMessages), ErrorMessageResourceName = "SecondNameRequired")]
        public string Prezime { get; set; }

        // ------------------------------ PASSWORD  ---------------------------
        [Display(Name = "Password", ResourceType = typeof(ErrorMessages.ErrorMessages))]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages.ErrorMessages), ErrorMessageResourceName = "PasswordRequired")]
        [StringLength(40, MinimumLength = 8, ErrorMessageResourceType = typeof(ErrorMessages.ErrorMessages), ErrorMessageResourceName = "PasswordLength")]
        [RegularExpression("^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*[!#$()?{}|*+,^.\\-+&=%_:;~@])(?=\\S+$).{8,}$",
            ErrorMessageResourceType = typeof(ErrorMessages.ErrorMessages), ErrorMessageResourceName = "PasswordExpression")]
        public string Password { get; set; }

        [Display(Name = "ConfirmPassword", ResourceType = typeof(ErrorMessages.ErrorMessages))]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages.ErrorMessages), ErrorMessageResourceName = "ConfirmPasswordRequired")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessageResourceType = typeof(ErrorMessages.ErrorMessages), ErrorMessageResourceName = "PasswordMatch")]
        public string ConfirmPassword { get; set; }

        // ---------------------------------------------------------------------------

        public string Salt { get; set; }


        [Required(ErrorMessageResourceType = typeof(ErrorMessages.ErrorMessages), ErrorMessageResourceName = "BirthdayRequired")]
        [Display(Name = "Birthdate", ResourceType = typeof(ErrorMessages.ErrorMessages))]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DatumRodenja { get; set; }

        [Required(ErrorMessageResourceType = typeof(ErrorMessages.ErrorMessages), ErrorMessageResourceName = "EmailRequired")]
        [Display(Name = "Email", ResourceType = typeof(ErrorMessages.ErrorMessages))]
        [EmailAddress]
        [Remote("EmailExists", "Player", HttpMethod = "POST", ErrorMessageResourceType = typeof(ErrorMessages.ErrorMessages), ErrorMessageResourceName = "EmailExists")]
        public string Email { get; set; }

        public bool EmailConfirmed { get; set; }

        [Display(Name = "StreetName", ResourceType = typeof(ErrorMessages.ErrorMessages))]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages.ErrorMessages), ErrorMessageResourceName = "StreetRequired")]
        public string Ulica { get; set; }

        [Display(Name = "HouseNumber", ResourceType = typeof(ErrorMessages.ErrorMessages))]
        public string KucniBroj { get; set; }

        [Display(Name = "City", ResourceType = typeof(ErrorMessages.ErrorMessages))]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages.ErrorMessages), ErrorMessageResourceName = "CityRequired")]
        public string Grad { get; set; }

        [Required(ErrorMessageResourceType = typeof(ErrorMessages.ErrorMessages), ErrorMessageResourceName = "ZipRequired")]
        [Display(Name = "ZipCode", ResourceType = typeof(ErrorMessages.ErrorMessages))]
        public int PostanskiBroj { get; set; }

        [Required(ErrorMessageResourceType = typeof(ErrorMessages.ErrorMessages), ErrorMessageResourceName = "CountryRequired")]
        [Display(Name = "Country", ResourceType = typeof(ErrorMessages.ErrorMessages))]
        public int Drzava { get; set; }

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

        [Required(ErrorMessageResourceType = typeof(ErrorMessages.ErrorMessages), ErrorMessageResourceName = "UsernameRequired")]
        [Display(Name = "Username", ResourceType = typeof(ErrorMessages.ErrorMessages))]
        //[StringLength(20,MinimumLength = 6)]
        [RegularExpression("^[A-Za-z0-9_-]{6,20}$",
            ErrorMessageResourceType = typeof(ErrorMessages.ErrorMessages), ErrorMessageResourceName = "UsernameExpression")]
        [Remote("UsernameExists", "Player", HttpMethod = "POST", ErrorMessageResourceType = typeof(ErrorMessages.ErrorMessages), ErrorMessageResourceName = "UsernameExists")]
        public string Username { get; set; }

        public string Role { get; set; }

    }
}