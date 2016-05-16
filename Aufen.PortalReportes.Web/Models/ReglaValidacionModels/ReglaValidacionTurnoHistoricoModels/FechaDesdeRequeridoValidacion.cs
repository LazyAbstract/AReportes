using Aufen.PortalReportes.Core;
using Aufen.PortalReportes.Web.Models.DTOModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aufen.PortalReportes.Web.Models.ReglaValidacionModels.ReglaValidacionTurnoHistoricoModels
{
    public class FechaDesdeRequeridoValidacion : IReglaValidacion
    {
        AufenPortalReportesDataContext db = new AufenPortalReportesDataContext()
            .WithConnectionStringFromConfiguration();
        private string MensajeError { get; set; }
        public FechaDesdeRequeridoValidacion()
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
            if(String.IsNullOrWhiteSpace(dto.FechaDesde))
            {
                validacion = false;
                MensajeError = "La fecha de inicio no puede ser vacía"; 
            }
            return validacion;
        }
    }
}