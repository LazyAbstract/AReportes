using Aufen.PortalReportes.Web.Models.ReportesModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Aufen.PortalReportes.Web.Controllers
{
    public class ReportesController : Controller
    {
        public ActionResult ListarReporte(ListarReporteFormModel FORM)
        {
            ListarReporteViewModel model = new ListarReporteViewModel(FORM);
            if (ModelState.IsValid)
            {
                
            }
            return View(model);
        }
    }
}
