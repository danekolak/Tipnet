using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tipnet.Models;

namespace Tipnet.CustomAtributes
{
    public class AnonymousOnlyAttr : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            try
            {
                var cookie = httpContext.Request.Cookies["login"]["Id"];
                var session = httpContext.Session["login"] as LogIn;

                if (cookie != null && session != null)
                    if (cookie == session.Id)
                        return false;
                return false;
            }
            catch (Exception x)
            {
                return true;
            }

        }
    }
}