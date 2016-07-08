using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aufen.PortalReportes.Web.Models.DTOModels
{
    public class IncidenciaHistoricoDTO
    {
        public string Fecha { get; set; }
        public string Rut { get; set; }
        public string IdIncidencia { get; set; }
        public string IdTipoDia { get; set; }
    }
}