using Aufen.PortalReportes.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Aufen.PortalReportes.Web.Models.ReportesModels
{
    public class ListarReporteFormModel
    {
        public Rut Rut { get; set; }

        [DisplayName("Fecha Desde")]
        [Required]
        public DateTime? FechaDesde { get; set; }

        [DisplayName("Fecha Hasta")]
        [Required]
        public DateTime? FechaHasta { get; set; }

        [DisplayName("Empresa")]
        [Required]
        public IEnumerable<string> Empresa { get; set; }

        [DisplayName("Departamento")]
        [Required]
        public IEnumerable<string> Departamento { get; set; }

        public IEnumerable<int> IdTipoReportes { get; set; }
    }
}
