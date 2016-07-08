using Aufen.PortalReportes.Core;
using Aufen.PortalReportes.Web.Models.DTOModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aufen.PortalReportes.Web.Models.ReglaValidacionModels.ReglaValidacionIncidenciaHistoricoModels
{
    public class TipoDiaExisteValidacion : IReglaValidacion
    {
        AufenPortalReportesDataContext db = new AufenPortalReportesDataContext()
           .WithConnectionStringFromConfiguration();
        private string MensajeError { get; set; }
        public TipoDiaExisteValidacion()
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
            if (String.IsNullOrWhiteSpace(dto.IdTipoDia))
            {
                validacion = false;
                MensajeError = "El Código de Tipo Día no puede ser vacío.";
            }
            else if(dto.IdTipoDia.Length > 1)
            {
                validacion = false;
                MensajeError = "El Código de Tipo Día debe ser un único caracater.";
            }
            else
            {
                if (!db.TIPO_DIA01s.Any(x => x.Codigo == Convert.ToChar(dto.IdTipoDia)))
                {
                    validacion = false;
                    MensajeError = "El Código de Tipo Día no se encuentra en la base de datos.";
                }
            }
            return validacion;
        }
    }
}