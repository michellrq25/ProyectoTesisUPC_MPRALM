using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ProyectoTesisUPC_v2.Permisos
{
    public class SessionAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContextBase)
        {
            return httpContextBase.Session["Usuario"] != null;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult(
                         new RouteValueDictionary {
                        { "Controller", "Account" },
                        { "Action", "Login" }
                     });
        }
    }
}