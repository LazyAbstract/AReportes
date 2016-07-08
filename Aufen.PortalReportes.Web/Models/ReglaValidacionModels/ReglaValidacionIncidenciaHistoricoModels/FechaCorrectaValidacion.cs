using Aufen.PortalReportes.Core;
using Aufen.PortalReportes.Web.Models.DTOModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aufen.PortalReportes.Web.Models.ReglaValidacionModels.ReglaValidacionIncidenciaHistoricoModels
{
    public class FechaCorrectaValidacion : IReglaValidacion
    {
        AufenPortalReportesDataContext db = new AufenPortalReportesDataContext()
            .WithConnectionStringFromConfiguration();
        private string MensajeError { get; set; }
        public FechaCorrectaValidacion()
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
            DateTime date;
            if(String.IsNullOrEmpty(dto.Fecha))
            {
                validacion = false;
                MensajeError = "La Fecha no puede vacía.";
            }
            else if (!DateTime.TryParse(dto.Fecha, out date))
            {
                validacion = false;
                MensajeError = "No se puede leer la Fecha.";
            }
            return validacion;
        }
    }
}