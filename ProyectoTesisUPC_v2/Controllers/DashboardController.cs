using ProyectoTesisUPC_v2.Models;
using ProyectoTesisUPC_v2.Permisos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProyectoTesisUPC_v2.Controllers
{
    [SessionAuthorize]
    public class DashboardController : Controller
    {
        // GET: Dashboard
        public ActionResult Dashboard()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Dashboard(DashboardModel model)
        {
            return View(model);
        }
    }
}