using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aufen.PortalReportes.Web.Models.DTOModels
{
    public class TurnoHistoricoDTO
    {
        public string Rut { get; set; }
        public string IdHorario { get; set; }
        public string IdCalendario { get; set; }
        public string FechaDesde { get; set; }
        public string FechaHasta { get; set; }
    }
}