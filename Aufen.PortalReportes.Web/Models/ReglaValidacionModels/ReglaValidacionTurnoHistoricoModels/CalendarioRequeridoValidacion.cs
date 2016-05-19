using Aufen.PortalReportes.Core;
using Aufen.PortalReportes.Web.Models.DTOModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aufen.PortalReportes.Web.Models.ReglaValidacionModels.ReglaValidacionTurnoHistoricoModels
{
    public class CalendarioRequeridoValidacion : IReglaValidacion
    {
        AufenPortalReportesDataContext db = new AufenPortalReportesDataContext()
            .WithConnectionStringFromConfiguration();
        private string MensajeError { get; set; }
        public CalendarioRequeridoValidacion()
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
            if(String.IsNullOrWhiteSpace(dto.IdCalendario))
            {
                validacion = false;
                MensajeError = "El calendario no puede ser vacío"; 
            }
            else if(dto.IdCalendario.Length != 9)
            {
                validacion = false;
                MensajeError = "El calendario no puede tener menos de 9 carateres."; 
            }
            return validacion;
        }
    }
}