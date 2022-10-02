using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProyectoTesisUPC_v2.Permisos
{
    public class ValidarSesionAttribute : ActionFilterAttribute
    {        
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var NombreControlador = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            var NombreAccion = filterContext.ActionDescriptor.ActionName;
            
            if (filterContext.HttpContext.Session["IdUsuario"] == null)
            {
                if (!NombreControlador.Equals("Account") && !NombreAccion.Equals("Login"))
                {
                    filterContext.Result = new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary(new { action = "Login", Controller = "Account" }));
                }                       
            } 
            base.OnActionExecuting(filterContext);
        }        
    }
}