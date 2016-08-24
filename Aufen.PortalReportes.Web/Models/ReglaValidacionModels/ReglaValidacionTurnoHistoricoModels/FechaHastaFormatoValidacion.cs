using Aufen.PortalReportes.Core;
using Aufen.PortalReportes.Web.Models.DTOModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aufen.PortalReportes.Web.Models.ReglaValidacionModels.ReglaValidacionTurnoHistoricoModels
{
    public class FechaHastaFormatoValidacion : IReglaValidacion
    {
        AufenPortalReportesDataContext db = new AufenPortalReportesDataContext()
            .WithConnectionStringFromConfiguration();
        private string MensajeError { get; set; }
        private int mes { get; set; }
        private int ano { get; set; }
        public FechaHastaFormatoValidacion()
        {
            MensajeError = String.Empty;
        }

        public FechaHastaFormatoValidacion(int _mes, int _ano) : this()
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
            if (!DateTime.TryParse(dto.FechaHasta, out dateValue))
            {
                validacion = false;
                MensajeError = "No se pudo leer la fecha de termino";
            }
            else if (DateTime.Parse(dto.FechaHasta).Month != mes || DateTime.Parse(dto.FechaHasta).Year != ano)
            {
                validacion = false;
                MensajeError = "La fecha hasta está fuera del periodo indicado.";
            }
            return validacion;
        }
    }
}