using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Tipnet.Helper;
using Tipnet.Repository;

namespace Tipnet.Controllers
{
    public class ControllerBase : Controller
    {
        PlayerDB db = new PlayerDB();

        internal string CreateSalt()
        {
            int brojBytaSalta = 10;
            var rng = new RNGCryptoServiceProvider();
            var buff = new byte[brojBytaSalta];

            rng.GetBytes(buff);

            return Convert.ToBase64String(buff);
        }

        internal string GenerateSHA256Hash(string input, string salt)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(input + salt);
            var sha256hashstring = new SHA256Managed();

            byte[] hash = sha256hashstring.ComputeHash(bytes);

            return ByteArrayToString(hash);

        }

        private static string ByteArrayToString(byte[] ba)
        {
            var hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

        public void ActivationMail(string email)
        {
            var mailGuid = Guid.NewGuid();

            string link = HttpContext.Request.Url.Scheme + "://" + HttpContext.Request.Url.Authority + 
                Url.Action("Confirmation", "Account", new { key = Convert.ToString(mailGuid) });

            db.NoviMailGuid(Convert.ToString(mailGuid), email);

            WebMail.Send(email, "Potvrdite svoj racun", link);

        }

        public bool RacunBlokiran(int brojPokusaja, DateTime vrijemeBlokade, string username)
        {
            var maxBrPokusaja = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["BrojNeuspjelihPokusaja"]);
            var trajanjeBlokade = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["VrijemeBlokadeUMinutama"]);

            if ((vrijemeBlokade.AddMinutes(trajanjeBlokade) < DateTime.Now))
            {
                if (brojPokusaja < maxBrPokusaja)
                {
                    return true;
                }
                db.DodajVrijemeBlokade(username);
                db.KriviPassword(0, username);
            }

            return false;
        }

        protected override IAsyncResult BeginExecuteCore(AsyncCallback callback, object state)
        {
            string cultureName = null;

            //read the culture cookie from Request

            HttpCookie cultureCookie = Request.Cookies["language"];
            if (cultureCookie != null)
                cultureName = cultureCookie.Value;
            else

                cultureName = Request.UserLanguages != null && Request.UserLanguages.Length > 0
                    ? Request.UserLanguages[0] : //get it from HTTP header AcceptLanguages
                    null;

            //Validate culture name
            cultureName = CultureHelper.GetImplementedCulture(cultureName); //safe

            //Modify current thread's cultures
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(cultureName);
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;

            return base.BeginExecuteCore(callback, state);

        }

        public int LevenshteinDistance(string source, string target)
        {
            if (String.IsNullOrEmpty(source))
            {
                if (String.IsNullOrEmpty(target)) return 0;
                return target.Length;
            }
            if (String.IsNullOrEmpty(target)) return source.Length;

            if (source.Length > target.Length)
            {
                var temp = target;
                target = source;
                source = temp;
            }

            var m = target.Length;
            var n = source.Length;
            var distance = new int[2, m + 1];
            // Initialize the distance 'matrix'
            for (var j = 1; j <= m; j++) distance[0, j] = j;

            var currentRow = 0;
            for (var i = 1; i <= n; ++i)
            {
                currentRow = i & 1;
                distance[currentRow, 0] = i;
                var previousRow = currentRow ^ 1;
                for (var j = 1; j <= m; j++)
                {
                    var cost = (target[j - 1] == source[i - 1] ? 0 : 1);
                    distance[currentRow, j] = Math.Min(Math.Min(
                                distance[previousRow, j] + 1,
                                distance[currentRow, j - 1] + 1),
                                distance[previousRow, j - 1] + cost);
                }
            }
            return distance[currentRow, m];
        }

    }
}