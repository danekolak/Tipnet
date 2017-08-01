using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Web;
using System.Web.Configuration;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using CaptchaMvc.HtmlHelpers;
using Microsoft.Ajax.Utilities;
using Tipnet.CustomAtributes;
using Tipnet.DTOs;
using Tipnet.Helper;
using Tipnet.Models;
using Tipnet.Repository;

namespace Tipnet.Controllers
{
    public class AccountController : ControllerBase
    {
        PlayerDB db = new PlayerDB();

        // GET: Account
        [HttpPost]
        public ActionResult LogIn(LoginFormDTO credentials, string ReturnUrl)
        {
            try
            {
                var saltAndPass = new SaltAndPassDTO();

                var UAEList = new List<UsernameAndEmailDTO>();
                UAEList = db.GetAllUsernameAndEmails();

                ViewBag.HasEmail = "false";

                foreach (var item in UAEList)
                {
                    if (
                    (String.Equals(credentials.Username, Convert.ToString(item.Username),
                        StringComparison.OrdinalIgnoreCase)))
                    {
                        if (item.EmailConfirmed)
                        {
                            if (!item.AccDisabled)
                            {
                                saltAndPass = db.GetSaltAndPassword(credentials.Username);

                                if (RacunBlokiran(saltAndPass.PogresanPass, saltAndPass.VrijemeBlokade,
                                    credentials.Username))
                                {
                                    if (saltAndPass.Password ==
                                        (GenerateSHA256Hash(credentials.Password, saltAndPass.Salt)))
                                    {
                                        db.KriviPassword(0, credentials.Username);
                                        ViewBag.ErrMess = ErrorMessages.ErrorMessages.LoginSuccess;
                                        ViewBag.Toast = "success";

                                        // Session and cookies part

                                        //-----------------------
                                        SessionPersister.Username = credentials.Username;
                                        //-----------------------

                                        string guid = Convert.ToString(Guid.NewGuid());

                                        //Session["login"] = new LogIn() { Id = guid, Username = credentials.Username };
                                        Session.Add("login", new LogIn() {Id = guid, Username = credentials.Username});
                                        Session["logOffTime"] = db.GetTimeOffData(credentials.Username).AutoOdjava;

                                        var expirationTime =
                                            Convert.ToInt32(
                                                System.Configuration.ConfigurationManager.AppSettings[
                                                    "VrijemeIstekaPrijave"]);

                                        Response.Cookies["login"]["Id"] = guid;
                                        Response.Cookies["login"]["username"] = credentials.Username;
                                        Response.Cookies["login"].Expires = DateTime.Now.AddMinutes(expirationTime);
                                        
                                        Response.Cookies["timestamp"].Value = DateTime.Now.ToString();
                                        
                                        db.UpisiSessionId(credentials.Username, guid);

                                        // ------------------------
                                        if (!string.IsNullOrWhiteSpace(ReturnUrl))
                                        {
                                            return Redirect(ReturnUrl);
                                        }

                                        return View("LoginState");
                                    }

                                    else
                                    {
                                        saltAndPass.PogresanPass++;
                                        db.KriviPassword(saltAndPass.PogresanPass, credentials.Username);

                                        ViewBag.ErrMess = ErrorMessages.ErrorMessages.WrongPassword;
                                        ViewBag.Toast = "warning";
                                        return View("LoginState");
                                    }
                                }

                                ViewBag.ErrMess = ErrorMessages.ErrorMessages.AccountBlocked;
                                ViewBag.Toast = "error";
                                return View("LoginState");
                            }

                            ViewBag.ErrMess = ErrorMessages.ErrorMessages.AccountPermaBlocked;
                            ViewBag.Toast = "error";
                            return View("LoginState");

                        }

                        ViewBag.ErrMess = ErrorMessages.ErrorMessages.EmailNotConfirmed;
                        ViewBag.HasEmail = "true";
                        ViewBag.Toast = "error";
                        ViewBag.Email = item.Email;

                        return View("LoginState");

                    }
                    ViewBag.Toast = "error";
                    ViewBag.ErrMess = ErrorMessages.ErrorMessages.WrongUsername;

                }
                return View("LoginState");
            }
            catch (Exception ex)
            {
                Elmah.ErrorLog.GetDefault(System.Web.HttpContext.Current).Log(new Elmah.Error(ex));
                ViewBag.Toast = "error";
                ViewBag.ErrMess = ErrorMessages.ErrorMessages.ServerError;
            }

            return View("LoginState");
        }

        public ActionResult Confirmation(string key)
        {
            try
            {
                var listaGuidova = db.GetAllGuids();

                string guid = Request.QueryString["key"];

                foreach (var g in listaGuidova)
                {
                    if (guid == g)
                    {
                        db.ConfirmAcc(guid);
                        ViewBag.ErrMess = ErrorMessages.ErrorMessages.EmailConfirmed + " " + guid;
                        return View("LoginState");
                    }
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorLog.GetDefault(System.Web.HttpContext.Current).Log(new Elmah.Error(ex));
                ViewBag.Toast = "error";
                ViewBag.ErrMess = ErrorMessages.ErrorMessages.ServerError;
            }
           
            return View("LoginState");
        }

        public ActionResult ResendMail(string email)
        {
            try
            {
                ActivationMail(email);

                ViewBag.ErrMess = ErrorMessages.ErrorMessages.EmailSent + " " + email;
                return View("LoginState");
            }
            catch (Exception ex)
            {
                Elmah.ErrorLog.GetDefault(System.Web.HttpContext.Current).Log(new Elmah.Error(ex));
                ViewBag.Toast = "error";
                ViewBag.ErrMess = ErrorMessages.ErrorMessages.ServerError;
            }
            return View("LoginState");
            
        }

        [AnonymousOnlyAttr]
        public ActionResult ZaboravljenUsername()
        {

            return View();
        }

        [HttpPost]
        [AnonymousOnlyAttr]
        [ValidateAntiForgeryToken()]
        public ActionResult ZaboravljenUsername(RequestUsernameDTO requestForm)
        {
            try
            {
                if (this.IsCaptchaValid(ErrorMessages.ErrorMessages.CaptchaNotValid))
                {
                    if (ModelState.IsValid)
                    {
                        var birthdateAndUsername = db.GetBirthdate(requestForm.Email);

                        if (birthdateAndUsername != null)
                        {
                            if (birthdateAndUsername.DatumRodenja == requestForm.DatumRodenja)
                            {
                                WebMail.Send(requestForm.Email, ErrorMessages.ErrorMessages.Username, ErrorMessages.ErrorMessages.UsernameMail + " " + birthdateAndUsername.Username);

                                ViewBag.ErrMessage = ErrorMessages.ErrorMessages.UsernameMailSent + " " + requestForm.Email;
                                return View("LoginState");
                            }

                            ViewBag.ErrMessage = ErrorMessages.ErrorMessages.EmailAndBirthdateError;
                            return View();
                        }

                        ViewBag.ErrMessage = ErrorMessages.ErrorMessages.EmailNotExist;
                        return View();

                    }

                }

                ViewBag.ErrMessage = ErrorMessages.ErrorMessages.CaptchaNotValid;
                return View();
            }
            catch (Exception ex)
            {
                Elmah.ErrorLog.GetDefault(System.Web.HttpContext.Current).Log(new Elmah.Error(ex));
                ViewBag.Toast = "error";
                ViewBag.ErrMess = ErrorMessages.ErrorMessages.ServerError;
            }

            return View("LoginState");
            
        }


        // Zaboravljeni password
        [AnonymousOnlyAttr]
        public ActionResult ZaboravljenPass()
        {

            return View();
        }

        [HttpPost]
        [AnonymousOnlyAttr]
        [ValidateAntiForgeryToken()]
        public ActionResult ZaboravljenPass(RequestPassword requestForm)
        {
            try
            {
                if (this.IsCaptchaValid(ErrorMessages.ErrorMessages.CaptchaNotValid))
                {
                    if (ModelState.IsValid)
                    {
                        var birthdateAndEmail = db.GetBirthdateAndEmail(requestForm.Username);

                        if (birthdateAndEmail != null)
                        {
                            if (birthdateAndEmail.DatumRodenja == requestForm.DatumRodenja)
                            {
                                var prGuid = Guid.NewGuid();

                                string link = HttpContext.Request.Url.Scheme + "://" + HttpContext.Request.Url.Authority +
                                    Url.Action("PasswordReset", "Account", new { key = Convert.ToString(prGuid) });

                                db.PasswordResetInit(Convert.ToString(prGuid), birthdateAndEmail.Email);

                                WebMail.Send(birthdateAndEmail.Email, ErrorMessages.ErrorMessages.PasswordResetLinkHead , ErrorMessages.ErrorMessages.PasswordResetLinkBody + " " + link);

                                ViewBag.ErrMess = ErrorMessages.ErrorMessages.ResetLinkSent + " " + birthdateAndEmail.Email + ". " + ErrorMessages.ErrorMessages.LinkValidity;
                                return View("LoginState");
                            }

                            ViewBag.ErrMessage = ErrorMessages.ErrorMessages.UsernameAndBirthddateError;
                            return View();
                        }

                        ViewBag.ErrMessage = ErrorMessages.ErrorMessages.UsernameNotExist;
                        return View();

                    }

                }

                ViewBag.ErrMessage = ErrorMessages.ErrorMessages.CaptchaNotValid;
                return View();
            }
            catch (Exception ex)
            {
                Elmah.ErrorLog.GetDefault(System.Web.HttpContext.Current).Log(new Elmah.Error(ex));
                ViewBag.Toast = "error";
                ViewBag.ErrMess = ErrorMessages.ErrorMessages.ServerError;
            }

            return View("LoginState");
            
        }


        public ActionResult PasswordReset(string key)
        {
            try
            {
                string prGuid = Request.QueryString["key"];

                var prInfo = db.GetPasswordResetInfo(prGuid);


                if (prInfo != null && DateTime.Now < prInfo.PrTimeStamp.AddMinutes(30))
                {
                    TempData["flag"] = true;
                    ViewBag.PrGuid = prGuid;

                    return View("PasswordResetForm");
                }

                ViewBag.Toast = "error";
                ViewBag.ErrMess = ErrorMessages.ErrorMessages.LinkNotValid;

            }
            catch (Exception ex)
            {
                Elmah.ErrorLog.GetDefault(System.Web.HttpContext.Current).Log(new Elmah.Error(ex));
                ViewBag.Toast = "error";
                ViewBag.ErrMess = ErrorMessages.ErrorMessages.ServerError;
            }

            return View("LoginState");
            
        }

        [NonAction]
        protected ActionResult PasswordResetForm()
        {
            if (Convert.ToBoolean(TempData["flag"]))
                return View();
            return View("LoginState");
        }

        [HttpPost]
        [AnonymousOnlyAttr]
        [ValidateAntiForgeryToken()]
        public ActionResult PasswordResetForm(NewPasswordDTO newPassword)
        {

            if (this.IsCaptchaValid(ErrorMessages.ErrorMessages.CaptchaNotValid))
            {
                if (ModelState.IsValid)
                {
                    var salt = CreateSalt();
                    var password = GenerateSHA256Hash(newPassword.Password, salt);

                    if (db.UpdatePassworda(password, salt, newPassword.PrGuid))
                    {
                        ViewBag.ErrMess = ErrorMessages.ErrorMessages.PasswordChangeSuccess;
                        return View("LoginState");
                    }

                }

                ViewBag.ErrMess = ErrorMessages.ErrorMessages.Error;
                return View();
            }

            ViewBag.ErrMessage = ErrorMessages.ErrorMessages.CaptchaNotValid;
            return View();
        }

        public ActionResult LogOff()
        {
            Session.Clear();
            Session.Abandon();

            HttpCookie myCookie = new HttpCookie("login");
            myCookie.Expires = DateTime.Now.AddDays(-1d);
            Response.Cookies.Add(myCookie);

            return RedirectToAction("Index", "Home");
        }

        public ActionResult SetCulture(string culture)
        {
            //Validate input
            culture = CultureHelper.GetImplementedCulture(culture);
            //Save culture in a cookie
            HttpCookie cookie = Request.Cookies["language"];
            if (cookie != null)
                cookie.Value = culture;
            else
            {
                cookie = new HttpCookie("language");
                cookie.Value = culture;
                cookie.Expires = DateTime.Now.AddYears(1);
            }
            Response.Cookies.Add(cookie);
            return RedirectToAction("Index","Home");
        }

        
    }
}