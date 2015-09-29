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
        public SelectList Departamentos { get; set; }
        public IEnumerable<TipoReporte> TipoReportes { get; set; }
        //public IEnumerable<SelectListItemDepartamento> Departamentos { get; set; }

        public ListarReporteViewModel()
        {
            FORM = new ListarReporteFormModel();
            AufenPortalReportesDataContext db = new AufenPortalReportesDataContext()
             .WithConnectionStringFromConfiguration();
            Empresas = new SelectList(db.EMPRESAs, "Codigo", "Descripcion");
            Departamentos = new SelectList(db.vw_Ubicaciones, "Codigo", "Descripcion");
            //Departamentos = db.vw_Ubicaciones
            //    .Select(x => new SelectListItemDepartamento()
            //    {
            //        Value = x.Codigo.ToString(),
            //        Text = x.Descripcion,
            //        OptGroup = x.IdEmpresa
            //    });

            TipoReportes = ReportesHelper.GetTipoReportes();
        }

        public ListarReporteViewModel(ListarReporteFormModel form)
            : this()
        {
            FORM = form;
        }
    }

    public class SelectListItemDepartamento
    {
        public string Value { get; set; }
        public string Text { get; set; }
        public bool Selected { get; set; }
        public string OptGroup { get; set; }
    }
}