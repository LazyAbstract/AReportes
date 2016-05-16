using Aufen.PortalReportes.Core;
using Aufen.PortalReportes.Web.Models.DTOModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aufen.PortalReportes.Web.Models.ReglaValidacionModels.ReglaValidacionTurnoHistoricoModels
{
    public class FechaDesdeFormatoValidacion : IReglaValidacion
    {
        AufenPortalReportesDataContext db = new AufenPortalReportesDataContext()
            .WithConnectionStringFromConfiguration();
        private string MensajeError { get; set; }
        public FechaDesdeFormatoValidacion()
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
            DateTime dateValue;
            if (!DateTime.TryParse(dto.FechaDesde, out dateValue))
            {
                validacion = false;
                MensajeError = "No se pudo leer la fecha de inicio";
            }
            return validacion;
        }
    }
}