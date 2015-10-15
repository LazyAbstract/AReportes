using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Aufen.PortalReportes.Core;

namespace Aufen.PortalReportes.Web.Models.ReportesModels
{
    public class GetDepartamentosViewModel
    {
        public IEnumerable<EMPRESA> Empresas { get; set; }
        public IEnumerable<vw_Ubicacione> Vw_Ubicaciones { get; set; }
        public IEnumerable<string> CodigoDepartamentos { get; set; }

        public GetDepartamentosViewModel()
        {
            CodigoDepartamentos = new List<string>();
            Empresas = new List<EMPRESA>();
        }

        public GetDepartamentosViewModel(IEnumerable<string> codigoEmpresas, IEnumerable<string> codigoDepartamentos)
            : this()
        {
            AufenPortalReportesDataContext db = new AufenPortalReportesDataContext()
            .WithConnectionStringFromConfiguration();
            if (codigoEmpresas != null && codigoEmpresas.Any(x => !String.IsNullOrEmpty(x)))
            {
                Empresas = db.EMPRESAs.Where(x => codigoEmpresas.Contains(x.Codigo));
                Vw_Ubicaciones = db.vw_Ubicaciones.Where(x => codigoEmpresas.Contains(x.IdEmpresa));
            }
            if (codigoDepartamentos != null && codigoDepartamentos.Any(x => !String.IsNullOrEmpty(x)))
            {
                CodigoDepartamentos = codigoDepartamentos;
            }
        }
    }
}