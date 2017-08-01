using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Windows.Forms.VisualStyles;
using Tipnet.Controllers;
using Tipnet.Models;
using Tipnet.Repository;

namespace Tipnet
{
    public class AuthorizeUserAttr : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            try
            {
                var cookie = httpContext.Request.Cookies["login"]["Id"];
                var session = httpContext.Session["login"] as LogIn;
                var db = new PlayerDB();
                var sessionGuid = db.GetSessionGuid(session.Username);

                if (cookie != null && session != null)
                    if (sessionGuid == session.Id)
                    {
                        if (cookie == session.Id)
                            return true;
                    }
                    

                return false;
            }
            catch (Exception e)
            {
                return false;
            }
            
        }
       
    }
}