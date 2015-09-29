using Aufen.PortalReportes.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aufen.PortalReportes.Web.Models.ReportesModels
{
    public class LibroInasistenciaDTO
    {
        public DateTime? Fecha { get; set; }
        public Rut Rut { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string IdEmpresa { get; set; }
        public string IdDepartamento { get; set; }
        public DateTime? EntradaTeorica { get; set; }
        public DateTime? SalidaTeorica { get; set; }
        public string Observacion { get; set; }

        public TimeSpan HorasPactadas
        {
            get
            {
                TimeSpan buffer = new TimeSpan();
                if (this.EntradaTeorica.HasValue && this.SalidaTeorica.HasValue)
                {
                    buffer = this.SalidaTeorica.Value.Subtract(this.EntradaTeorica.Value);
                }
                return buffer;
            }
        }
    }

    public static class LibroInasistenciaDTOHelpers
    {
        /// <summary>
        ///     A partir de un DTO de resultado del SP calculo la jornada teórica utilizando las filas que tienen los datos de salida y entrada teórica
        /// </summary>
        /// <param name="lista">Enumeración de sp_LibroAtrasosResultDTO para hacer el calculo</param>
        /// <returns>Las Horas y minutos correspondientes al cálculo de la hora jornada teórica en formato de texto HH:mm</returns>
        public static string CalculaJornada(this IEnumerable<LibroInasistenciaDTO> lista)
        {
            return lista.Any(x => x.SalidaTeorica.HasValue && x.EntradaTeorica.HasValue) ?
                        new DateTime(lista.Where(x => x.SalidaTeorica.HasValue && x.EntradaTeorica.HasValue)
                        .Sum(x => x.SalidaTeorica.Value.Subtract(x.EntradaTeorica.Value).Ticks))
                            .ToString("HH:mm") : String.Empty;
        }
    }
}