using Aufen.PortalReportes.Core;
using Aufen.PortalReportes.Web.Models.DTOModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aufen.PortalReportes.Web.Models.ReglaValidacionModels.ReglaValidacionTurnoHistoricoModels
{
    public class CalendarioExisteValidacion : IReglaValidacion
    {
        AufenPortalReportesDataContext db = new AufenPortalReportesDataContext()
            .WithConnectionStringFromConfiguration();
        private string MensajeError { get; set; }
        public CalendarioExisteValidacion()
        {
            MensajeError = String.Empty;
        }

        public string Mensaje
        {
            get { return MensajeError; }
        }

        public bool ValidaRegla(object sujeto)
        {
            bool validacion = true;
            TurnoHistoricoDTO dto = (TurnoHistoricoDTO)sujeto;
            if (!db.CALENDARIO01s.Any(x => x.IdCalendario == dto.IdCalendario))
            {
                validacion = false;
                MensajeError = "No se pudo encontrar el codigo de calendario en la base de datos";
            }
            return validacion;
        }
    }
}