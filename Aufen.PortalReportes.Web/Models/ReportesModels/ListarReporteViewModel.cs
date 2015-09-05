using Aufen.PortalReportes.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Aufen.PortalReportes.Web.Models.ReportesModels
{
    public class ListarReporteViewModel
    {
        public ListarReporteFormModel FORM { get; set; }
        public SelectList Empresas { get; set; }
        public IEnumerable<TipoReporte> TipoReportes { get; set; }

        public ListarReporteViewModel()
        {
            FORM = new ListarReporteFormModel();
            AufenPortalReportesDataContext db = new AufenPortalReportesDataContext()
            .WithConnectionStringFromConfiguration();
            Empresas = new SelectList(db.EMPRESAs, "Codigo", "Descripcion");
            TipoReportes = ReportesHelper.GetTipoReportes();
        }

        public ListarReporteViewModel(ListarReporteFormModel form):this()
        {
            FORM = form;
        }
    }
}