using Aufen.PortalReportes.Core;
using Aufen.PortalReportes.Web.Models.DTOModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aufen.PortalReportes.Web.Models.ReglaValidacionModels.ReglaValidacionTurnoHistoricoModels
{
    public class FechaDesdeMenorFechaHastaValidacion : IReglaValidacion
    {
         AufenPortalReportesDataContext db = new AufenPortalReportesDataContext()
            .WithConnectionStringFromConfiguration();
        private string MensajeError { get; set; }
        public FechaDesdeMenorFechaHastaValidacion()
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
            if (dto.FechaDesdeAsDateTime > dto.FechaHastaAsDateTime)
            {
                validacion = false;
                MensajeError = "La fecha de inicio no puede ser mayor a la fecha de termino del periodo";
            }
            return validacion;
        }
    }
}