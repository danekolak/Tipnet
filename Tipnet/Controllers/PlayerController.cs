using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using CaptchaMvc.HtmlHelpers;
using Tipnet.CustomAtributes;
using Tipnet.DTOs;
using Tipnet.Models;
using Tipnet.Repository;

namespace Tipnet.Controllers
{
    public class PlayerController : ControllerBase
    {
        private PlayerDB db = new PlayerDB();


        // GET: Player
        public ActionResult Index()
        {
            return View();
        }

        // GET: Player/Create
        [AnonymousOnlyAttr]
        public ActionResult Create()
        {
            try
            {
                var listaDrzava = db.GetCountries();
                ViewBag.Country = new SelectList(listaDrzava, "Id", "Ime");

                return View();
            }
            catch (Exception ex)
            {
                Elmah.ErrorLog.GetDefault(System.Web.HttpContext.Current).Log(new Elmah.Error(ex));
                ViewBag.Toast = "error";
                ViewBag.ErrMess = ErrorMessages.ErrorMessages.ServerError;
            }
            return View("Index");

           
        }

        // POST: Player/Create
        [HttpPost]
        [AnonymousOnlyAttr]
        [ValidateAntiForgeryToken()]
        public ActionResult Create(Player player)
        {
            try
            {
                var listaDrzava = db.GetCountries();
                ViewBag.Country = new SelectList(listaDrzava, "Id", "Ime");

                if (this.IsCaptchaValid("Captcha is not valid"))
                {
                    if (ModelState.IsValid)
                    {
                        if ((LevenshteinDistance(player.Password, player.Ime) > 4) &&
                            (LevenshteinDistance(player.Password, player.Prezime) > 4) &&
                            (LevenshteinDistance(player.Password, player.Username) > 4))
                        {

                            player.Salt = CreateSalt();
                            player.Password = GenerateSHA256Hash(player.Password, player.Salt);


                            if (db.InsertPlayer(player))
                            {
                                ActivationMail(player.Email);

                                return RedirectToAction("Index");
                            }


                        }

                        ModelState.AddModelError("Password", ErrorMessages.ErrorMessages.LevenhsteinError);
                        return View();

                    }

                }

                ViewBag.ErrMessage = ErrorMessages.ErrorMessages.CaptchaNotValid;
                return View("Create");
            }
            catch (Exception ex)
            {
                Elmah.ErrorLog.GetDefault(System.Web.HttpContext.Current).Log(new Elmah.Error(ex));
                ViewBag.Toast = "error";
                ViewBag.ErrMess = ErrorMessages.ErrorMessages.ServerError;
            }
            return View("Index");


        }

        //-------------------- Provjeravanje zauzeca usernama i emaila -----------------------------

        public JsonResult UsernameExists(string username)
        {

            var UAEList = new List<UsernameAndEmailDTO>();
            UAEList = db.GetAllUsernameAndEmails();

            foreach (var item in UAEList)
            {
                if ((String.Equals(username, Convert.ToString(item.Username), StringComparison.OrdinalIgnoreCase)))
                {
                    return Json(false);
                }

            }
            return Json(true);

        }

        public JsonResult EmailExists(string email)
        {

            var UAEList = new List<UsernameAndEmailDTO>();
            UAEList = db.GetAllUsernameAndEmails();


            foreach (var item in UAEList)
            {
                if ((String.Equals(email, Convert.ToString(item.Email), StringComparison.OrdinalIgnoreCase)))
                {
                    return Json(false);
                }

            }
            return Json(true);

        }

        // ------------------------------------------------------------------------------------------------------------

        [AuthorizeUserAttr]
        [HttpGet]
        public ActionResult Edit()
        {
            try
            {
                var session = Session["login"] as LogIn;
                var username = session.Username;
                var sessionId = HttpContext.Request.Cookies["login"]["Id"];

                var listaDrzava = db.GetCountries();

                ViewBag.Country = new SelectList(listaDrzava, "Id", "Ime");

                var editData = db.DohvatiPodatkeZaIzmjenu(username, sessionId);

                return View(editData);
            }
            catch (Exception ex)
            {
                Elmah.ErrorLog.GetDefault(System.Web.HttpContext.Current).Log(new Elmah.Error(ex));
                ViewBag.Toast = "error";
                ViewBag.ErrMess = ErrorMessages.ErrorMessages.ServerError;
            }
            return View("Index");
            
        }

        [AuthorizeUserAttr]
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult Edit(EditDataDTO editedData)
        {
            try
            {
                var session = Session["login"] as LogIn;
                var username = session.Username;
                var sessionId = HttpContext.Request.Cookies["login"]["Id"];


                if (!ModelState.IsValid) return View();

                return db.EditData(editedData, username, sessionId) ? View("Index") : View();
            }
            catch (Exception ex)
            {
                Elmah.ErrorLog.GetDefault(System.Web.HttpContext.Current).Log(new Elmah.Error(ex));
                ViewBag.Toast = "error";
                ViewBag.ErrMess = ErrorMessages.ErrorMessages.ServerError;
            }
            return View("Index");
            
        }

        //----------------------------------------------  PROMJENA PASSWORDA --------------------------------------

        [HttpGet]
        [AuthorizeUserAttr]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [AuthorizeUserAttr]
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult ChangePassword(NewPasswordDTO noviPass)
        {
            try
            {
                var sessionData = (LogIn)Session["login"];
                var player = db.DohvatiPlayera(sessionData.Username);


                // koristim PrGuid iz objekta za spremanje starog passworda da ne pravim novi DTO      !!!
                var stariPass = GenerateSHA256Hash(noviPass.PrGuid, player.Salt);

                if (!ModelState.IsValid) return View();

                if (stariPass == player.Password)
                {
                    var noviPassHash = GenerateSHA256Hash(noviPass.Password, player.Salt);

                    if (noviPassHash != stariPass)
                    {

                        if ((LevenshteinDistance(player.Password, player.Ime) > 4) &&
                            (LevenshteinDistance(player.Password, player.Prezime) > 4) &&
                            (LevenshteinDistance(player.Password, player.Username) > 4))
                        {
                            if (db.UpdatePasswordaUsername(noviPassHash, player.Salt, player.Username))
                            {
                                ViewBag.ErrMessage = "Password uspijesno promjenjen.";
                                return RedirectToAction("Index");
                            }
                            ModelState.AddModelError("Password", ErrorMessages.ErrorMessages.ServerError);
                            return View();
                        }

                        ModelState.AddModelError("Password", ErrorMessages.ErrorMessages.LevenhsteinError);
                        return View();
                    }

                    ModelState.AddModelError("Password", ErrorMessages.ErrorMessages.SamePassError);
                    return View();
                }

                ModelState.AddModelError("PrGuid", ErrorMessages.ErrorMessages.WrongPassword);


                return View();
            }
            catch (Exception ex)
            {
                Elmah.ErrorLog.GetDefault(System.Web.HttpContext.Current).Log(new Elmah.Error(ex));
                ViewBag.Toast = "error";
                ViewBag.ErrMess = ErrorMessages.ErrorMessages.ServerError;
            }
            return View("Index");

            
        }


        // ---------------------------------------   PROMJENA MAILA    ----------------------------------------------------

        [HttpGet]
        [AuthorizeUserAttr]
        public ActionResult ChangeEmail()
        {
            try
            {
                var sessionData = (LogIn)Session["login"];
                ViewBag.OldMail = db.GetEmailByUsername(sessionData.Username);

                return View();
            }
            catch (Exception ex)
            {
                Elmah.ErrorLog.GetDefault(System.Web.HttpContext.Current).Log(new Elmah.Error(ex));
                ViewBag.Toast = "error";
                ViewBag.ErrMess = ErrorMessages.ErrorMessages.ServerError;
            }
            return View("Index");
            
        }

        [HttpPost]
        [AuthorizeUserAttr]
        [ValidateAntiForgeryToken()]
        public ActionResult ChangeEmail(ChangeEmailDTO newMail)
        {
            try
            {
                var sessionData = (LogIn)Session["login"];
                ViewBag.OldMail = db.GetEmailByUsername(sessionData.Username);

                if (!ModelState.IsValid) return View();

                var prGuid = Guid.NewGuid().ToString().Substring(0, 10);

                string link = HttpContext.Request.Url.Scheme + "://" + HttpContext.Request.Url.Authority +
                              Url.Action("ChangeEmailConfirmation", "Player", new { key = prGuid, mail = newMail.Email });

                db.ChangeEmailInit(prGuid, sessionData.Username);

                WebMail.Send(newMail.Email, ErrorMessages.ErrorMessages.ChangeEmailHead, ErrorMessages.ErrorMessages.ChangeEmailBody + " " + link);

                ViewBag.ErrMess = ErrorMessages.ErrorMessages.EmailSent + " " + newMail.Email + ". " + ErrorMessages.ErrorMessages.LinkValidity;
            }
            catch (Exception ex)
            {
                Elmah.ErrorLog.GetDefault(System.Web.HttpContext.Current).Log(new Elmah.Error(ex));
                ViewBag.Toast = "error";
                ViewBag.ErrMess = ErrorMessages.ErrorMessages.ServerError;
            }

            return View("Index");
            
        }

        [AuthorizeUserAttr]
        public ActionResult ChangeEmailConfirmation(string key, string mail)
        {

            string mcGuid = Request.QueryString["key"];
            string newMail = Request.QueryString["mail"];

            var mailInfo = db.GetEmailChangeInfo(mcGuid);

            if (mailInfo != DateTime.MinValue && DateTime.Now < mailInfo.AddMinutes(30))
            {
                if (db.ChangeMailFinish(mcGuid, newMail))
                {
                    ViewBag.ErrMess = ErrorMessages.ErrorMessages.EmailChanged + " " + newMail;
                    return View("Index");
                }

                ViewBag.ErrMess = ErrorMessages.ErrorMessages.Error;
                return View("Index");
            }


            ViewBag.ErrMess = ErrorMessages.ErrorMessages.LinkNotValid;
            return View("Index");
        }

        // --------------------------------------------------------------------------------------------------------------
        // --------------------------------------    BLOKADA RACUNA    --------------------------------------------------

        [HttpGet]
        [AuthorizeUserAttr]
        public ActionResult BlokadaRacuna()
        {
            try
            {
                var sessionData = (LogIn)Session["login"];

                var currentBlockData = db.GetBlockData(sessionData.Username);

                return View(currentBlockData);
            }
            catch (Exception ex)
            {
                Elmah.ErrorLog.GetDefault(System.Web.HttpContext.Current).Log(new Elmah.Error(ex));
                ViewBag.Toast = "error";
                ViewBag.ErrMess = ErrorMessages.ErrorMessages.ServerError;
            }
            return View("Index");
            
        }

        [HttpPost]
        [AuthorizeUserAttr]
        public ActionResult BlokadaRacuna(BlokadaRacunaDTO blokada)
        {
            try
            {
                var sessionData = (LogIn)Session["login"];
                var currentBlockData = db.GetBlockData(sessionData.Username);

                if (currentBlockData.TimeStampBlokade.AddDays(Convert.ToInt32(currentBlockData.TrajanjeBlokade)) >
                    DateTime.Now)
                    currentBlockData.RacunBlokiran = true;


                if (!ModelState.IsValid) return View();

                if (!currentBlockData.RacunBlokiran)
                {
                    if (blokada.TrajanjeBlokade == "Trajno")
                        blokada.RacunTrajnoBlokiran = true;

                    if (db.SetBlockData(blokada, sessionData.Username))
                    {
                        if (blokada.TrajanjeBlokade == "Trajno")
                        {
                            ViewBag.ErrMess = ErrorMessages.ErrorMessages.AccountPermaBlocked;
                            return RedirectToAction("LogOff", "Account");
                        }

                        ViewBag.ErrMess = ErrorMessages.ErrorMessages.AccountBlockedTill + " " + DateTime.Now.AddDays(Convert.ToInt32(blokada.TrajanjeBlokade)) + ".";
                        return View("Index");
                    }
                }

                ModelState.AddModelError("TrajanjeBlokade", ErrorMessages.ErrorMessages.AccountAlreadyBlocked + " " + currentBlockData.TimeStampBlokade.AddDays(Convert.ToInt32(currentBlockData.TrajanjeBlokade)) + ".");
                return View();
            }
            catch (Exception ex)
            {
                Elmah.ErrorLog.GetDefault(System.Web.HttpContext.Current).Log(new Elmah.Error(ex));
                ViewBag.Toast = "error";
                ViewBag.ErrMess = ErrorMessages.ErrorMessages.ServerError;
            }
            return View("Index");

            
        }

        // --------------------------------------------------------------------------------------------------------------
        // ---------------------------------------   AUTOMATSKA ODJAVA  -------------------------------------------------

        [HttpGet]
        [AuthorizeUserAttr]
        public ActionResult AutomatskaOdjava()
        {
            try
            {
                var sessionData = (LogIn)Session["login"];

                var currentTimeOffData = db.GetTimeOffData(sessionData.Username);

                return View(currentTimeOffData);
            }
            catch (Exception ex)
            {
                Elmah.ErrorLog.GetDefault(System.Web.HttpContext.Current).Log(new Elmah.Error(ex));
                ViewBag.Toast = "error";
                ViewBag.ErrMess = ErrorMessages.ErrorMessages.ServerError;
            }
            return View("Index");
            
        }

        [HttpPost]
        [AuthorizeUserAttr]
        public ActionResult AutomatskaOdjava(TimeOffDTO timeOff)
        {
            try
            {
                var sessionData = (LogIn)Session["login"];
                var currentTimeOffData = db.GetTimeOffData(sessionData.Username);

                if (!ModelState.IsValid) return View();

                timeOff.AutoOdjavaFlag = true;
                timeOff.PauzaKladenjaFlag = true;
                timeOff.AutoOdjavaTimeStamp = DateTime.Now;
                timeOff.PauzaKladenjaTimeStamp = DateTime.Now;


                if ((timeOff.AutoOdjava == 0) &&
                    (DateTime.Now > currentTimeOffData.AutoOdjavaTimeStamp.AddDays(7)))
                {
                    timeOff.AutoOdjavaFlag = false;
                    timeOff.PauzaKladenjaFlag = false;
                    timeOff.PauzaKladenja = 0;
                    timeOff.AutoOdjavaTimeStamp = DateTime.MinValue;
                    timeOff.PauzaKladenjaTimeStamp = DateTime.MinValue;
                    currentTimeOffData.PauzaKladenjaTimeStamp = DateTime.MinValue;
                }


                if ((timeOff.AutoOdjava <= currentTimeOffData.AutoOdjava) ||
                    (DateTime.Now > currentTimeOffData.AutoOdjavaTimeStamp.AddDays(7)))
                {
                    if ((timeOff.PauzaKladenja >= currentTimeOffData.PauzaKladenja) ||
                        (DateTime.Now >= currentTimeOffData.PauzaKladenjaTimeStamp.AddDays(7)))
                    {

                        if (db.SetTimeOffData(timeOff, sessionData.Username))
                        {

                            ViewBag.ErrMess = "Automatska odjava postavljena na " + timeOff.AutoOdjava + " sati, a pauza klađenja na " + timeOff.PauzaKladenja + " sati.";
                            return View("Index");

                        }
                    }

                    ModelState.AddModelError("PauzaKladenja", "Pauzu klađenja trenutno samo možete povecati, a smanjiti ili deaktivirati tek " +
                                                              currentTimeOffData.PauzaKladenjaTimeStamp.AddDays(7));
                    return View();
                }

                ModelState.AddModelError("AutoOdjava", "Trajanje prijave trenutno samo možete smanjiti, a povecati ili deaktivirati tek " +
                                                       currentTimeOffData.AutoOdjavaTimeStamp.AddDays(7));
                return View();
            }
            catch (Exception ex)
            {
                Elmah.ErrorLog.GetDefault(System.Web.HttpContext.Current).Log(new Elmah.Error(ex));
                ViewBag.Toast = "error";
                ViewBag.ErrMess = ErrorMessages.ErrorMessages.ServerError;
            }
            return View("Index");
            
        }

        // --------------------------------------------------------------------------------------------------------------
        // ---------------------------------------   File upload   ------------------------------------------------------
        [HttpGet]
        [AuthorizeUserAttr]
        public ActionResult DodajDokument()
        {
            try
            {
                var sessionData = (LogIn)Session["login"];
                var slike = db.GetAllDocuments(sessionData.Username);

                ViewBag.Slike = slike;
                ViewBag.BrSlika = slike.Count();
                ViewBag.MaxBrSlika = System.Configuration.ConfigurationManager.AppSettings["MaxBrojDokumenata"];

                return View();
            }
            catch (Exception ex)
            {
                Elmah.ErrorLog.GetDefault(System.Web.HttpContext.Current).Log(new Elmah.Error(ex));
                ViewBag.Toast = "error";
                ViewBag.ErrMess = ErrorMessages.ErrorMessages.ServerError;
            }
            return View("Index");
            
        }

        [HttpPost]
        [AuthorizeUserAttr]
        public ActionResult DodajDokument(HttpPostedFileBase file)
        {
            try
            {
                var sessionData = (LogIn)Session["login"];

                var minVelicina =
                    Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["MinVelicinaSlike"]);
                var maxVelicina =
                    Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["MaxVelicinaSlike"]);
                var maxBrojDokumenata =
                    Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["MaxBrojDokumenata"]);

                if (file == null) return RedirectToAction("Index", "Home");

                var slike = db.GetAllDocuments(sessionData.Username);
                var brSlika = slike.Count();

                ViewBag.Slike = slike;
                ViewBag.BrSlika = brSlika;
                ViewBag.MaxBrSlika = maxBrojDokumenata;


                if (brSlika >= maxBrojDokumenata)
                {
                    ViewBag.Toast = "warning";
                    ViewBag.ErrMess = "Uploadan je max broj slika.";
                    return View("Index");
                }

                if (db.KorisnikVerificiran(sessionData.Username))
                {
                    ViewBag.Toast = "warning";
                    ViewBag.ErrMess = "Vaš račun već je verificiran.";
                    return View("Index");
                }

                if (file.ContentLength < minVelicina || file.ContentLength > maxVelicina)
                {
                    ModelState.AddModelError("file", "Slika ne smije biti manja od 100kB i veca od 5 MB");
                    return View();
                }

                var supportedTypes = new[] { "jpg", "jpeg", "png" };

                var extension = Path.GetExtension(file.FileName);
                if (extension != null)
                {
                    var fileExt = extension.Substring(1);

                    if (!supportedTypes.Contains(fileExt))
                    {
                        ModelState.AddModelError("file", "Invalid type. Only the following types (jpg, jpeg, png) are supported.");
                        return View();
                    }
                }

                var pic = Path.GetFileName(file.FileName);

                using (var br = new BinaryReader(file.InputStream))
                {
                    var array = br.ReadBytes(file.ContentLength);

                    if (db.DodajDokument(array, pic, sessionData.Username))
                    {
                        ViewBag.Toast = "success";
                        ViewBag.ErrMess = "Uspjesno dodano.";
                        return View("Index");
                    }
                }

                
            }
            catch (Exception ex)
            {
                Elmah.ErrorLog.GetDefault(System.Web.HttpContext.Current).Log(new Elmah.Error(ex));
                ViewBag.Toast = "error";
                ViewBag.ErrMess = ErrorMessages.ErrorMessages.ServerError;
            }
            return View("Index");
            

        }
        
        [AuthorizeUserAttr]
        public ActionResult GetImage(string name)
        {
            var sessionData = (LogIn)Session["login"];
            var images = db.GetAllDocuments(sessionData.Username);

            
            var imageData = images.First(s => s.Ime == name);
            

            return File(imageData.Slika, "image/jpg");
          
        }

        // --------------------------------------------------------------------------------------------------------------

        [CustomAuthorize(Roles = "admin")]
        public ActionResult Admin()
        {
            try
            {
                var players = db.GetAllPlayers();
                return View(players);
            }
            catch (Exception ex)
            {
                Elmah.ErrorLog.GetDefault(System.Web.HttpContext.Current).Log(new Elmah.Error(ex));
                ViewBag.Toast = "error";
                ViewBag.ErrMess = ErrorMessages.ErrorMessages.ServerError;
            }
            return View("Index");
            
        }
    }
}
