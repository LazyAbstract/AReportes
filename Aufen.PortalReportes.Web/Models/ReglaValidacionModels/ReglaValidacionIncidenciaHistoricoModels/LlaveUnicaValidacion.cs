using Aufen.PortalReportes.Core;
using Aufen.PortalReportes.Web.Models.DTOModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aufen.PortalReportes.Web.Models.ReglaValidacionModels.ReglaValidacionIncidenciaHistoricoModels
{
    public class LlaveUnicaValidacion : IReglaValidacion
    {
        AufenPortalReportesDataContext db = new AufenPortalReportesDataContext()
            .WithConnectionStringFromConfiguration();
        private string MensajeError { get; set; }
        public LlaveUnicaValidacion()
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
            IncidenciaHistoricoDTO dto = (IncidenciaHistoricoDTO)sujeto;
            if(db.CALENDARIO01s.Any(x => x.Fecha == dto.Fecha && x.IdCalendario == ("000000000"+dto.Rut).Right(9)))
            {
                validacion = false;
                MensajeError = "Ya existe una incidencia en CALENDARIO para este Rut en esta Fecha.";
            }
            return validacion;
        }
    }
}