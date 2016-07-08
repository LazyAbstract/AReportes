using Aufen.PortalReportes.Core;
using Aufen.PortalReportes.Web.Models.DTOModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aufen.PortalReportes.Web.Models.ReglaValidacionModels.ReglaValidacionIncidenciaHistoricoModels
{
    public class RutExisteValidacion : IReglaValidacion
    {
        AufenPortalReportesDataContext db = new AufenPortalReportesDataContext()
            .WithConnectionStringFromConfiguration();
        private string MensajeError { get; set; }
        public RutExisteValidacion()
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
            if (!String.IsNullOrWhiteSpace(dto.Rut))
            {
                var rut = ("000000000" + dto.Rut).Right(9);
                if(!db.EMPLEADOS01s.Any(x => x.Codigo == rut))
                {
                    validacion = false;
                    MensajeError = "El Rut no se encuentra en la base de datos.";
                }               
            }
            return validacion;
        }
    }
}