using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace Tipnet.Helper
{
    public class CultureHelper
    {
        //Valid cultures
        private static readonly List<string> _validCultures = new List<string> { "hr", "hr-HR", "hr-BA", "en", "en-AU", "en-BZ", "en-CA", "en-029", "en-IN", "en-IE", "en-JM", "en-MY", "en-NZ", "en-PH", "en-SG", "en-ZA", "en-TT", "en-GB", "en-US" };

        //Include only cultures you are implementing
        private static readonly List<string> _cultures = new List<string> { "en-US", "hr-HR" };

        /// <summary>
        /// Returns true if the language is a right-to-left language. Otherwise, false.
        /// </summary>
        public static bool IsRightToLeft()
        {
            return Thread.CurrentThread.CurrentCulture.TextInfo.IsRightToLeft;
        }

        /// <summary>
        /// Returns a valid culture name based on "name" parameter. If "name" is not valid, it returns the default culture "en-US"
        /// </summary>
        /// <param name="name" />Culture's name (e.g. en-US)</param>
        public static string GetImplementedCulture(string name)
        {
            if (string.IsNullOrEmpty(name))
                return GetDefaultCulture();

            //check if it is valid culture
            if (_validCultures.Where(c => c.Equals(name, StringComparison.CurrentCultureIgnoreCase)).Count() == 0)
                return GetDefaultCulture();
            //if it si implemented accepte it
            if (_cultures.Where(c => c.Equals(name, StringComparison.CurrentCultureIgnoreCase)).Count() > 0)
                return name;
            // Find a close match. For example, if you have "en-US" defined and the user requests "en-GB", 
            // the function will return closes match that is "en-US" because at least the language is the
            var n = GetNeutralCulture(name);
            foreach (var c in _cultures)
            {
                if (c.StartsWith(n))
                    return c;
            }

            return GetDefaultCulture();
        }

        public static string GetDefaultCulture()
        {
            return _cultures[0];
        }

        public static string GetCurrentCulture()
        {
            return Thread.CurrentThread.CurrentCulture.Name;
        }

        public static string GetCurrentNeutralCulture(string name)
        {
            return GetNeutralCulture(Thread.CurrentThread.CurrentCulture.Name);
        }

        public static string GetNeutralCulture(string name)
        {
            if (!name.Contains("-")) return name;

            return name.Split('-')[0]; // Read first part only E.g. "en"
        }
    }
}