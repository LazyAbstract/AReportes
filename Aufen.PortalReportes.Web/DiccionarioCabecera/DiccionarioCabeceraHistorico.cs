using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aufen.PortalReportes.Web.DiccionarioCabecera
{
    public static class DiccionarioCabeceraHistorico
    {     
        public static Dictionary<string, string> GetDiccionarioTurnoHistorico()
        {
            Dictionary<string, string> diccionario = new Dictionary<string, string>();
            diccionario.Add("Codigo", "Rut");
            diccionario.Add("N° Turno", "IdHorario");
            diccionario.Add("Calendario", "IdCalendario");
            diccionario.Add("Fecha_ini", "FechaDesde");
            diccionario.Add("Fecha_term", "FechaHasta");
            return diccionario;
        }
    }
}