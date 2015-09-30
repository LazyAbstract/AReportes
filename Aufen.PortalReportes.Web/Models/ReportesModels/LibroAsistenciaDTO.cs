using Aufen.PortalReportes.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aufen.PortalReportes.Web.Models.ReportesModels
{
    public class LibroAsistenciaDTO
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
        public DateTime? SalidaColacion { get; set; }
        public DateTime? EntradaColacion { get; set; }
        public TimeSpan? TiempoColacion { get; set; }
        public string Observacion { get; set; }

        public string HorasPactadas
        {
            get
            {
                TimeSpan buffer = new TimeSpan();
                string output = String.Empty;
                if (this.EntradaTeorica.HasValue && this.SalidaTeorica.HasValue)
                {
                    buffer = this.SalidaTeorica.Value.Subtract(this.EntradaTeorica.Value);
                    output = (int)buffer.TotalHours + buffer.ToString(@"\:mm");
                }
                return output;
            }
        }

        public string HorasReales
        {
            get
            {
                TimeSpan buffer = new TimeSpan();
                string output = String.Empty;
                if (this.Entrada.HasValue && this.Salida.HasValue)
                {
                    buffer = this.Salida.Value.Subtract(this.Entrada.Value);
                    output = (int)buffer.TotalHours + buffer.ToString(@"\:mm");
                }
                return output;
            }
        }

        public string HorasExtra
        {
            get
            {
                TimeSpan buffer = new TimeSpan();
                string output = String.Empty;
                if (this.EntradaTeorica.HasValue
                    && this.Entrada.HasValue
                    && this.SalidaTeorica.HasValue
                    && this.Salida.HasValue
                    && TimeSpan.Compare(this.Salida.Value.Subtract(this.Entrada.Value), this.SalidaTeorica.Value.Subtract(this.EntradaTeorica.Value)) == 1
                    )
                {

                    buffer = this.Salida.Value.Subtract(this.Entrada.Value) - this.SalidaTeorica.Value.Subtract(this.EntradaTeorica.Value);
                    output = (int)buffer.TotalHours + buffer.ToString(@"\:mm");
                }
                return output;
            }
        }

        public string Atraso
        {
            get 
            {
                TimeSpan buffer = new TimeSpan();
                string output = String.Empty;
                if(this.Entrada.HasValue && this.EntradaTeorica.HasValue)
                {
                    if (this.Entrada.Value > this.EntradaTeorica.Value)
                    {
                        buffer = this.EntradaTeorica.Value.Subtract(this.Entrada.Value);
                        output = (int)buffer.TotalHours + buffer.ToString(@"\:mm");
                    }
                }
                return output;
            }
        }

        public string SalidaAdelantada
        {
            get
            {
                TimeSpan buffer = new TimeSpan();
                string output = String.Empty;
                if (this.Salida.HasValue && this.SalidaTeorica.HasValue)
                {
                    if (this.Salida.Value < this.SalidaTeorica.Value)
                    {
                        buffer = this.EntradaTeorica.Value.Subtract(this.Entrada.Value);
                        output = (int)buffer.TotalHours + buffer.ToString(@"\:mm");
                    }
                }
                return output;
            }
        }

        public string TiempoColacionReal
        {
            get
            {
                TimeSpan buffer = new TimeSpan();
                string output = String.Empty;
                if (this.SalidaColacion.HasValue && this.EntradaColacion.HasValue)
                {
                    if (this.SalidaColacion.Value < this.EntradaColacion.Value)
                    {
                        buffer = this.EntradaColacion.Value.Subtract(this.SalidaColacion.Value);
                        output = (int)buffer.TotalHours + buffer.ToString(@"\:mm");
                    }
                }
                return output;
            }
        }
    }

    public static class LibroAsistenciaDTOHelpers
    {
        /// <summary>
        ///     A partir de un DTO de resultado del SP calculo la jornada teórica utilizando las filas que tienen los datos de salida y entrada teórica
        /// </summary>
        /// <param name="lista">Enumeración de sp_LibroAtrasosResultDTO para hacer el calculo</param>
        /// <returns>Las Horas y minutos correspondientes al cálculo de la hora jornada teórica en formato de texto HH:mm</returns>
        public static string CalculaJornada(this IEnumerable<LibroAsistenciaDTO> lista)
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
        public static string CalculaAsistencia(this IEnumerable<LibroAsistenciaDTO> lista)
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
        public static long CalculaAtrasoEntrada(this IEnumerable<LibroAsistenciaDTO> lista)
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
        public static long CalculaAtrasoSalida(this IEnumerable<LibroAsistenciaDTO> lista)
        {
            return lista.Any(x => x.SalidaTeorica.HasValue && x.Salida.HasValue && x.Salida < x.SalidaTeorica) ?
                lista.Where(x => x.SalidaTeorica.HasValue && x.Salida.HasValue && x.Salida < x.SalidaTeorica)
                .Sum(x => x.SalidaTeorica.Value.Subtract(x.Salida.Value).Ticks) : 0;
        }

        /// <summary>
        ///     A partir de un DTO de resultado del SP calcula las horas para filas que tengan valores para la Entrada real
        ///     , entrada teorica, Salida real y Salida teórica
        /// </summary>
        /// <param name="lista">Enumeración de sp_LibroAtrasosResultDTO para hacer el calculo</param>
        /// <returns>Ticks correspondientes al cálculo de las horas extra</returns>
        //public static long CalculaHorasExtra(this IEnumerable<LibroAsistenciaDTO> lista)
        //{
        //    return lista.Any(x => x.HorasReales && x.HorasPactadas.HasValue && x.HorasPactadas < x.HorasReales) ?
        //        lista.Where(x => x.HorasReales.HasValue && x.HorasPactadas.HasValue && x.HorasPactadas < x.HorasReales)
        //        .Sum(x => x.HorasReales.Value.Subtract(x.HorasPactadas.Value).Ticks) : 0;
        //}


    }
}