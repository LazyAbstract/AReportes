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

        public DateTime? FechaHastaAsDateTime
        {
            get
            {
                DateTime buffer = DateTime.Now;
                if (DateTime.TryParse(FechaHasta, out buffer))
                {
                    return buffer;
                }
                return null;
            }
        }

        public DateTime? FechaDesdeAsDateTime
        {
            get
            {
                DateTime buffer = DateTime.Now;
                if (DateTime.TryParse(FechaDesde, out buffer))
                {
                    return buffer;
                }
                return null;
            }
        }
    }
}