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
        private int mes { get; set; }
        private int ano { get; set; }
        public FechaDesdeFormatoValidacion()
        {
            MensajeError = String.Empty;
        }

        public FechaDesdeFormatoValidacion(int _mes, int _ano) : this()
        {
            mes = _mes;
            ano = _ano;
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
            else if(DateTime.Parse(dto.FechaDesde).Month != mes || DateTime.Parse(dto.FechaDesde).Year != ano)
            {
                validacion = false;
                MensajeError = "La fecha desde está fuera del periodo indicado.";
            }
            return validacion;
        }
    }
}