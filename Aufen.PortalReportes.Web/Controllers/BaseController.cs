using Aufen.PortalReportes.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Aufen.PortalReportes.Web.Controllers
{
    public class BaseController : Controller
    {
        public string Mensaje
        {
            set
            {
                TempData["Mensaje"] = value;
            }
        }

        public AufenPortalReportesDataContext db = new AufenPortalReportesDataContext()
            .WithConnectionStringFromConfiguration();
    }
}
