using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aufen.PortalReportes.Core
{
    public class TipoReporte
    {
        public const int LibroAtrasos = 1;
        public const int AsistenciaLegal = 2;
        public const int LibroSobreTiempo = 4;
        public const int PruebaNico = 3;

        public int IdTipoReporte { get; set; }
        public string Nombre { get; set; }

    }

    public static class ReportesHelper
    {
        public static IEnumerable<TipoReporte> GetTipoReportes()
        {
            List<TipoReporte> lista = new List<TipoReporte>();
            lista.Add(new TipoReporte() { IdTipoReporte = TipoReporte.LibroAtrasos, Nombre = "Libro de Atrasos" });
            lista.Add(new TipoReporte() { IdTipoReporte = TipoReporte.AsistenciaLegal, Nombre = "Asistencia Legal" });
            lista.Add(new TipoReporte() { IdTipoReporte = TipoReporte.LibroSobreTiempo, Nombre = "SobreTiempo" });
            //lista.Add(new TipoReporte() { IdTipoReporte = TipoReporte.PruebaNico, Nombre = "PruebaNico" });

            return lista;
        }
    }
}
