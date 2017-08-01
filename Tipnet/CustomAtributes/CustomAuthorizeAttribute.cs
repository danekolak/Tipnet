using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Tipnet.Repository;

namespace Tipnet.CustomAtributes
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if(string.IsNullOrEmpty(SessionPersister.Username))
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new {controller = "Home", action ="Index"}));

            else
            {
                PlayerDB db = new PlayerDB();
                CustomPrincipal mp = new CustomPrincipal(db.DohvatiPlayera(SessionPersister.Username));

                if(!mp.IsInRole(Roles))
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Home", action = "Error" }));
            }
        }

    }
}