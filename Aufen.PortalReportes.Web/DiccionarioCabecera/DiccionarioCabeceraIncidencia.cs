using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aufen.PortalReportes.Web.DiccionarioCabecera
{
    public static class DiccionarioCabeceraIncidencia
    {
        public static Dictionary<string, string> GetDiccionarioIncidenciaHistorico()
        {
            Dictionary<string, string> diccionario = new Dictionary<string, string>();
            diccionario.Add("Fecha", "Fecha");
            diccionario.Add("Rut", "Rut");
            diccionario.Add("Codigo Incidencia", "IdIncidencia");
            diccionario.Add("Codigo Tipo Dia", "IdTipoDia");
            return diccionario;
        }
    }
}