using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ProyectoTesisUPC_v2.Permisos
{
    public class ValidarRolAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var rol = filterContext.HttpContext.Session["cod_rol"].ToString();

            if (rol == "C")
            {
                var values = new RouteValueDictionary(new
                {
                    controller = "Prediccion",
                    action = "Prediccion"
                });

                filterContext.Result = new RedirectToRouteResult(values);
            }
            base.OnActionExecuting(filterContext);
        }
    }
}