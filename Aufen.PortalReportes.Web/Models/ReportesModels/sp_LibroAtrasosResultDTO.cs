using Aufen.PortalReportes.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aufen.PortalReportes.Web.Models.ReportesModels
{
    public class sp_LibroAtrasosResultDTO
    {
        public DateTime? Fecha { get; set; }
        public int? NumSemana { get; set; }
        public Rut Rut { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string IdHorario { get; set; }
        public string IdEmpresa { get; set; }
        public string IdDepartamento { get; set; }
        public DateTime? Entrada { get; set; }
        public DateTime? Salida { get; set; }
        public DateTime? EntradaTeorica { get; set; }
        public DateTime? SalidaTeorica { get; set; }
        public TimeSpan? TiempoColacion { get; set; }
        public string Observacion { get; set; }
    }

    public static class Sp_LibroAtrasosResultDTOHelpers
    {
        /// <summary>
        ///     A partir de un DTO de resultado del SP calculo la jornada teórica utilizando las filas que tienen los datos de salida y entrada teórica
        /// </summary>
        /// <param name="lista">Enumeración de sp_LibroAtrasosResultDTO para hacer el calculo</param>
        /// <returns>Las Horas y minutos correspondientes al cálculo de la hora jornada teórica en formato de texto HH:mm</returns>
        public static string CalculaJornada(this IEnumerable<sp_LibroAtrasosResultDTO> lista)
        {
            return lista.Any(x => x.SalidaTeorica.HasValue && x.EntradaTeorica.HasValue) ?
                        new DateTime(lista.Where(x => x.SalidaTeorica.HasValue && x.EntradaTeorica.HasValue)
                        .Sum(x => x.SalidaTeorica.Value.Subtract(x.EntradaTeorica.Value).Ticks))
                            .ToString("HH:mm") : String.Empty;
        }

        /// <summary>
        ///     A partir de un DTO de resultado del SP calculo la jornada real para filas que tienen los datos de salida y entrada reales
        /// </summary>
        /// <param name="lista">Enumeración de sp_LibroAtrasosResultDTO para hacer el calculo</param>
        /// <returns>Las Horas y minutos correspondientes al cálculo de la hora jornada  realen formato de texto HH:mm</returns>
        public static string CalculaAsistencia(this IEnumerable<sp_LibroAtrasosResultDTO> lista)
        {
            return lista.Any(x => x.Salida.HasValue && x.Entrada.HasValue) ?
                new DateTime(lista.Where(x => x.Salida.HasValue && x.Entrada.HasValue)
                .Sum(x => x.Salida.Value.Subtract(x.Entrada.Value).Ticks)).ToString("HH:mm") : String.Empty;
        }

        /// <summary>
        ///     A partir de un DTO de resultado del SP calcula el atraso de entrada para filas que tengan valores para la entrada real y entrada teórica
        /// </summary>
        /// <param name="lista">Enumeración de sp_LibroAtrasosResultDTO para hacer el calculo</param>
        /// <returns>Ticks correspondientes al cálculo de los atrasos/returns>
        public static long CalculaAtrasoEntrada(this IEnumerable<sp_LibroAtrasosResultDTO> lista)
        {
            return lista.Any(x => x.EntradaTeorica.HasValue && x.Entrada.HasValue && x.Entrada > x.EntradaTeorica) ?
                lista.Where(x => x.EntradaTeorica.HasValue && x.Entrada.HasValue && x.Entrada > x.EntradaTeorica)
                .Sum(x => x.Entrada.Value.Subtract(x.EntradaTeorica.Value).Ticks) : 0;
        }

        /// <summary>
        ///     A partir de un DTO de resultado del SP calcula el atraso de Salida para filas que tengan valores para la Salida real y Salida teórica
        /// </summary>
        /// <param name="lista">Enumeración de sp_LibroAtrasosResultDTO para hacer el calculo</param>
        /// <returns>Ticks correspondientes al cálculo de los atrasos</returns>
        public static long CalculaAtrasoSalida(this IEnumerable<sp_LibroAtrasosResultDTO> lista)
        {
            return lista.Any(x => x.SalidaTeorica.HasValue && x.Salida.HasValue && x.Salida < x.SalidaTeorica) ?
                lista.Where(x => x.SalidaTeorica.HasValue && x.Salida.HasValue && x.Salida < x.SalidaTeorica)
                .Sum(x => x.SalidaTeorica.Value.Subtract(x.Salida.Value).Ticks) : 0;
        }
    }
}