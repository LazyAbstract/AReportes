using Aufen.PortalReportes.Core;
using Aufen.PortalReportes.Web.Models.DTOModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aufen.PortalReportes.Web.Models.ReglaValidacionModels.ReglaValidacionIncidenciaHistoricoModels
{
    public class IncidenciaExisteValidacion : IReglaValidacion
    {
        AufenPortalReportesDataContext db = new AufenPortalReportesDataContext()
            .WithConnectionStringFromConfiguration();
        private string MensajeError { get; set; }
        public IncidenciaExisteValidacion()
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
            if (String.IsNullOrWhiteSpace(dto.IdIncidencia))
            {
                validacion = false;
                MensajeError = "El Código de Incidencia no puede ser vacío.";
            }
            else
            {
                var incidencia = ("0000" + dto.IdIncidencia).Right(4);
                if (!db.INCIDENCIAS01s.Any(x => x.Codigo == incidencia))
                {
                    validacion = false;
                    MensajeError = "El Código de Incidencia no se encuentra en la base de datos.";
                }
            }
            return validacion;
        }
    }
}