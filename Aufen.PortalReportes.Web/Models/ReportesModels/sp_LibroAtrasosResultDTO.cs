using Aufen.PortalReportes.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aufen.PortalReportes.Web.Models.ReportesModels
{
    public class sp_LibroAtrasosResultDTO
    {
        public DateTime? Fecha { get; set; }
        public int? NumSemana { get; set; }
        public Rut Rut { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string IdHorario { get; set; }
        public string IdEmpresa { get; set; }
        public string IdDepartamento { get; set; }
        public DateTime Entrada { get; set; }
        public DateTime Salida { get; set; }
        public DateTime EntradaTeorica { get; set; }
        public DateTime SalidaTeorica { get; set; }
        public TimeSpan TiempoColacion { get; set; }
        public string Observacion { get; set; }
    }
}