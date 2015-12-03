using Aufen.PortalReportes.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Aufen.PortalReportes.Web.Helpers;

namespace Aufen.PortalReportes.Web.Models.ReportesModels
{
    public class LibroAsistenciaDTO
    {
        public DateTime? Fecha { get; set; }
        public int? NumSemana { get; set; }
        public String Rut { get; set; }
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
        public bool EsPermiso { get; set; }

        public string printHorasPactadas
        {
            get
            {
                TimeSpan buffer = new TimeSpan();
                string output = String.Empty;
                if (this.EntradaTeorica.HasValue && this.SalidaTeorica.HasValue)
                {
                    buffer = this.SalidaTeorica.Value.Subtract(this.EntradaTeorica.Value).Subtract(this.TiempoColacion.Value);
                    output = (int)buffer.TotalHours + buffer.ToString(@"\:mm");
                }
                return output;
            }
        }
        public TimeSpan? HorasPactadas
        {
            get
            {
                TimeSpan buffer = new TimeSpan();
                if (this.EntradaTeorica.HasValue && this.SalidaTeorica.HasValue)
                {
                    buffer = this.SalidaTeorica.Value.Subtract(this.EntradaTeorica.Value).Subtract(this.TiempoColacion.Value);
                }
                return buffer;
            }
        }

        public string printHorasReales
        {
            get
            {
                TimeSpan buffer = new TimeSpan();
                string output = String.Empty;
                if (this.Entrada.HasValue && this.Salida.HasValue)
                {
                    TimeSpan buffer2 = new TimeSpan();
                    buffer = this.Salida.Value.Subtract(this.Entrada.Value);
                    if (this.SalidaColacion.HasValue && this.EntradaColacion.HasValue)
                    {
                        if (this.SalidaColacion.Value < this.EntradaColacion.Value)
                        {
                            buffer2 = this.EntradaColacion.Value.Subtract(this.SalidaColacion.Value);
                            buffer = buffer.Subtract(buffer2);
                        }
                        else
                        {
                            buffer.Subtract(TiempoColacion.Value);
                        }
                    }
                    else
                    {
                        buffer.Subtract(TiempoColacion.Value);
                    }
                    if (this.EntradaTeorica.HasValue && this.SalidaTeorica.HasValue)
                    {
                        buffer2 = this.SalidaTeorica.Value.Subtract(this.EntradaTeorica.Value).Subtract(this.TiempoColacion.Value);
                    }
                    output = (int)buffer.TotalHours + buffer.ToString(@"\:mm");
                }
                return output;
            }
        }
        public TimeSpan? HorasReales
        {
            get
            {
                TimeSpan buffer = new TimeSpan();
                if (this.Entrada.HasValue && this.Salida.HasValue)
                {
                    TimeSpan buffer2 = new TimeSpan();
                    buffer = this.Salida.Value.Subtract(this.Entrada.Value);
                    if (this.SalidaColacion.HasValue && this.EntradaColacion.HasValue)
                    {
                        if (this.SalidaColacion.Value < this.EntradaColacion.Value)
                        {
                            buffer2 = this.EntradaColacion.Value.Subtract(this.SalidaColacion.Value);
                            buffer = buffer.Subtract(buffer2);
                        }
                        else
                        {
                            buffer.Subtract(TiempoColacion.Value);
                        }
                    }
                    else
                    {
                        buffer.Subtract(TiempoColacion.Value);
                    }
                    if (this.EntradaTeorica.HasValue && this.SalidaTeorica.HasValue)
                    {
                        buffer2 = this.SalidaTeorica.Value.Subtract(this.EntradaTeorica.Value).Subtract(this.TiempoColacion.Value);
                    }
                }
                return buffer;
            }
        }

        public string printHorasExtra
        {
            get
            {
                TimeSpan buffer = new TimeSpan();
                string output = String.Empty;
                if (this.HorasReales.HasValue && this.HorasPactadas.HasValue && this.SalidaTeorica.HasValue 
                    && this.EntradaTeorica.HasValue && this.Entrada.HasValue && this.Salida.HasValue)
                {
                    if (this.HorasReales > this.HorasPactadas)
                    {
                        buffer = this.Salida.Value.Subtract(this.Entrada.Value) - this.SalidaTeorica.Value.Subtract(this.EntradaTeorica.Value);
                        output = (int)buffer.TotalHours + buffer.ToString(@"\:mm");
                    }
                }
                return output;
            }
        }
        public TimeSpan HorasExtra
        {
            get
            {
                TimeSpan buffer = new TimeSpan();
                if (this.HorasReales.HasValue && this.HorasPactadas.HasValue && this.SalidaTeorica.HasValue
                    && this.EntradaTeorica.HasValue && this.Entrada.HasValue && this.Salida.HasValue)
                {
                    if (this.HorasReales > this.HorasPactadas)
                    {
                        buffer = this.Salida.Value.Subtract(this.Entrada.Value) - this.SalidaTeorica.Value.Subtract(this.EntradaTeorica.Value);
                    }
                }
                return buffer;
            }
        }

        public string printAtraso
        {
            get
            {
                TimeSpan buffer = new TimeSpan();
                string output = String.Empty;
                if (this.Entrada.HasValue && this.EntradaTeorica.HasValue)
                {
                    if (this.Entrada.Value > this.EntradaTeorica.Value && String.IsNullOrWhiteSpace(this.Observacion))
                    {
                        buffer = this.Entrada.Value.Subtract(this.EntradaTeorica.Value);
                        output = (int)buffer.TotalHours + buffer.ToString(@"\:mm");
                    }
                }
                return ("0" + output);
            }
        }
        public TimeSpan Atraso
        {
            get
            {
                TimeSpan buffer = new TimeSpan();
                if (this.Entrada.HasValue && this.EntradaTeorica.HasValue)
                {
                    if (this.Entrada.Value > this.EntradaTeorica.Value && String.IsNullOrWhiteSpace(this.Observacion))
                    {
                        buffer = this.Entrada.Value.Subtract(this.EntradaTeorica.Value);
                    }
                }
                return buffer;
            }
        }

        public string printSalidaAdelantada
        {
            get
            {
                TimeSpan buffer = new TimeSpan();
                string output = String.Empty;
                if (this.Salida.HasValue && this.SalidaTeorica.HasValue && String.IsNullOrWhiteSpace(this.Observacion))
                {
                    if (this.Salida.Value < this.SalidaTeorica.Value)
                    {
                        buffer = this.SalidaTeorica.Value.Subtract(this.Salida.Value);
                        output = (int)buffer.TotalHours + buffer.ToString(@"\:mm");
                    }
                }
                return output;
            }
        }
        public TimeSpan SalidaAdelantada
        {
            get
            {
                TimeSpan buffer = new TimeSpan();
                if (this.Salida.HasValue && this.SalidaTeorica.HasValue && String.IsNullOrWhiteSpace(this.Observacion))
                {
                    if (this.Salida.Value < this.SalidaTeorica.Value)
                    {
                        buffer = this.SalidaTeorica.Value.Subtract(this.Salida.Value);
                    }
                }
                return buffer;
            }
        }

        public string printTiempoColacionReal
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
        public TimeSpan TiempoColacionReal
        {
            get
            {
                TimeSpan buffer = new TimeSpan();
                if (this.SalidaColacion.HasValue && this.EntradaColacion.HasValue)
                {
                    if (this.SalidaColacion.Value < this.EntradaColacion.Value)
                    {
                        buffer = this.EntradaColacion.Value.Subtract(this.SalidaColacion.Value);
                    }
                    //else { buffer = this.TiempoColacion.Value; }
                }
                //else { buffer = this.TiempoColacion.Value; }
                return buffer;
            }
        }

        public string printSobreEntrada
        {
            get
            {
                TimeSpan buffer = new TimeSpan();
                string output = String.Empty;
                if (this.Entrada.HasValue && this.EntradaTeorica.HasValue)
                {
                    if (this.Entrada.Value < this.EntradaTeorica.Value)
                    {
                        buffer = this.EntradaTeorica.Value.Subtract(this.Entrada.Value);
                        output = (int)buffer.TotalHours + buffer.ToString(@"\:mm");
                    }
                }
                return output;
            }
        }
        public TimeSpan SobreEntrada
        {
            get
            {
                TimeSpan buffer = new TimeSpan();
                if (this.Entrada.HasValue && this.EntradaTeorica.HasValue)
                {
                    if (this.Entrada.Value < this.EntradaTeorica.Value)
                    {
                        buffer = this.EntradaTeorica.Value.Subtract(this.Entrada.Value);
                    }
                }
                return buffer;
            }
        }

        public string printSobreSalida
        {
            get
            {
                TimeSpan buffer = new TimeSpan();
                string output = String.Empty;
                if (this.Salida.HasValue && this.SalidaTeorica.HasValue)
                {
                    if (this.Salida.Value > this.SalidaTeorica.Value)
                    {
                        buffer = this.Salida.Value.Subtract(this.SalidaTeorica.Value);
                        output = (int)buffer.TotalHours + buffer.ToString(@"\:mm");
                    }
                }
                return output;
            }
        }
        public TimeSpan SobreSalida
        {
            get
            {
                TimeSpan buffer = new TimeSpan();
                if (this.Salida.HasValue && this.SalidaTeorica.HasValue)
                {
                    if (this.Salida.Value > this.SalidaTeorica.Value)
                    {
                        buffer = this.Salida.Value.Subtract(this.SalidaTeorica.Value);
                    }
                }
                return buffer;
            }
        }

        public string printSobreTiempo
        {
            get
            {
                TimeSpan buffer = new TimeSpan();
                string output = String.Empty;
                if ((this.Entrada.HasValue && this.EntradaTeorica.HasValue && this.Entrada < this.EntradaTeorica))
                {
                    if (this.Entrada.Value < this.EntradaTeorica.Value)
                    {
                        buffer = this.EntradaTeorica.Value.Subtract(this.Entrada.Value);
                    }
                    else
                    {
                        buffer = TimeSpan.Zero;
                    }
                }
                if((this.Salida.HasValue && this.SalidaTeorica.HasValue && this.Salida > this.SalidaTeorica))
                {
                    if (this.Salida.Value > this.SalidaTeorica.Value)
                    {
                        buffer = buffer + this.Salida.Value.Subtract(this.SalidaTeorica.Value);
                    }
                                       
                }
                output = (int)buffer.TotalHours + buffer.ToString(@"\:mm"); 
                return output;
            }
        }
        public TimeSpan SobreTiempo
        {
            get
            {
                TimeSpan buffer = new TimeSpan();
                string output = String.Empty;
                if ((this.Entrada.HasValue && this.EntradaTeorica.HasValue))
                {
                    if (this.Entrada.Value < this.EntradaTeorica.Value)
                    {
                        buffer = this.EntradaTeorica.Value.Subtract(this.Entrada.Value);
                    }
                    else
                    {
                        buffer = TimeSpan.Zero;
                    }
                }
                if ((this.Salida.HasValue && this.SalidaTeorica.HasValue))
                {
                    if (this.Salida.Value > this.SalidaTeorica.Value)
                    {
                        buffer = buffer + this.Salida.Value.Subtract(this.SalidaTeorica.Value);
                    }

                }
                return buffer;
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
            if (lista == null)
                return "00:00";
            //  solo los días que fue... lo que está mal pero insisten
            if (lista.Any(x => x.SalidaTeorica.HasValue && x.EntradaTeorica.HasValue && x.Entrada.HasValue && x.Salida.HasValue))
            {
                long ticks = lista.Where(x => x.SalidaTeorica.HasValue && x.EntradaTeorica.HasValue && x.Entrada.HasValue && x.Salida.HasValue)
                    .Sum(x => x.SalidaTeorica.Value.Subtract(x.EntradaTeorica.Value).Subtract(x.TiempoColacion.GetValueOrDefault()).Ticks);
                return ticks.ImprimeFormatoHoraAufen();
            }
            else return "00:00";

        }

        /// <summary>
        ///     A partir de un DTO de resultado del SP calculo la jornada real para filas que tienen los datos de salida y entrada reales
        /// </summary>
        /// <param name="lista">Enumeración de sp_LibroAtrasosResultDTO para hacer el calculo</param>
        /// <returns>Las Horas y minutos correspondientes al cálculo de la hora jornada  realen formato de texto HH:mm</returns>
        public static string CalculaAsistencia(this IEnumerable<LibroAsistenciaDTO> lista)
        {
            if (lista == null)
                return "00:00";
            if (lista.Any(x => x.Salida.HasValue && x.Entrada.HasValue))
            {
                long ticks = lista.Where(x => x.Salida.HasValue && x.Entrada.HasValue && x.Salida > x.Entrada)
                    .Sum(x => x.Salida.Value.Subtract(x.Entrada.Value).Subtract(x.TiempoColacionReal).Ticks);
                return ticks.ImprimeFormatoHoraAufen();
            }
            else return "00:00";
        }

        /// <summary>
        ///     A partir de un DTO de resultado del SP calcula el atraso de entrada para filas que tengan valores para la entrada real y entrada teórica
        /// </summary>
        /// <param name="lista">Enumeración de sp_LibroAtrasosResultDTO para hacer el calculo</param>
        /// <returns>Ticks correspondientes al cálculo de los atrasos/returns>
        public static string CalculaAtrasoEntrada(this IEnumerable<LibroAsistenciaDTO> lista)
        {
            if (lista == null)
                return "00:00";
            if( lista.Any(x => x.EntradaTeorica.HasValue && x.Entrada.HasValue && x.Entrada > x.EntradaTeorica && String.IsNullOrWhiteSpace(x.Observacion)) )
            {                
                long ticks = lista.Where(x => x.EntradaTeorica.HasValue && x.Entrada.HasValue && x.Entrada > x.EntradaTeorica && String.IsNullOrWhiteSpace(x.Observacion))
                .Sum(x => x.Entrada.Value.Subtract(x.EntradaTeorica.Value).Ticks);
                return ticks.ImprimeFormatoHoraAufen();
            }
            return "00:00";
        }

        /// <summary>
        ///     A partir de un DTO de resultado del SP calcula el atraso de Salida para filas que tengan valores para la Salida real y Salida teórica
        /// </summary>
        /// <param name="lista">Enumeración de sp_LibroAtrasosResultDTO para hacer el calculo</param>
        /// <returns>Ticks correspondientes al cálculo de los atrasos</returns>
        public static string CalculaSalidaAdelantada(this IEnumerable<LibroAsistenciaDTO> lista)
        {
            if (lista == null)
                return "00:00";
            return lista.Any(x => x.SalidaTeorica.HasValue && x.Salida.HasValue && x.Salida < x.SalidaTeorica && String.IsNullOrWhiteSpace(x.Observacion)) ?
                new DateTime(lista.Where(x => x.SalidaTeorica.HasValue && x.Salida.HasValue && x.Salida < x.SalidaTeorica && String.IsNullOrWhiteSpace(x.Observacion))
                .Sum(x => x.SalidaTeorica.Value.Subtract(x.Salida.Value).Ticks)).ToString("HH:mm") : "00:00";
        }

        public static string CalculaAtrasoSalida(this IEnumerable<LibroAsistenciaDTO> lista)
        {
            if (lista == null)
                return "00:00";
            long salida = lista.Any(x => x.SalidaTeorica.HasValue && x.Salida.HasValue && x.Salida < x.SalidaTeorica && String.IsNullOrWhiteSpace(x.Observacion)) ?
                lista.Where(x => x.SalidaTeorica.HasValue && x.Salida.HasValue && x.Salida < x.SalidaTeorica && String.IsNullOrWhiteSpace(x.Observacion))
                .Sum(x => x.SalidaTeorica.Value.Subtract(x.Salida.Value).Ticks) : 0;
            long atraso = lista.Any(x => x.EntradaTeorica.HasValue && x.Entrada.HasValue && x.Entrada > x.EntradaTeorica && String.IsNullOrWhiteSpace(x.Observacion)) ?
                lista.Where(x => x.EntradaTeorica.HasValue && x.Entrada.HasValue && x.Entrada > x.EntradaTeorica && String.IsNullOrWhiteSpace(x.Observacion))
                .Sum(x => x.Entrada.Value.Subtract(x.EntradaTeorica.Value).Ticks) : 0;
            var ticks = salida + atraso;
            return ticks.ImprimeFormatoHoraAufen();
        }

        /// <summary>
        ///     A partir de un DTO de resultado del SP calcula las horas para filas que tengan valores para la Entrada real
        ///     , entrada teorica, Salida real y Salida teórica
        /// </summary>
        /// <param name="lista">Enumeración de sp_LibroAtrasosResultDTO para hacer el calculo</param>
        /// <returns>Ticks correspondientes al cálculo de las horas extra</returns>
        public static string CalculaHorasExtra(this IEnumerable<LibroAsistenciaDTO> lista)
        {
            if (lista == null) return "00:00";
            if (lista.Any(x => x.HorasReales.HasValue && x.HorasPactadas.HasValue && x.HorasPactadas < x.HorasReales && x.EntradaTeorica.HasValue && x.SalidaTeorica.HasValue))
            {
                long ticks = lista.Where(x => x.HorasReales.HasValue && x.HorasPactadas.HasValue && x.HorasPactadas < x.HorasReales && x.EntradaTeorica.HasValue && x.SalidaTeorica.HasValue)
                    .Sum(x => x.HorasExtra.Ticks);
                return ticks.ImprimeFormatoHoraAufen();
            }
            return "00:00";
          
        }

        public static string CalculaSobreTiempo(this IEnumerable<LibroAsistenciaDTO> lista)
        {
            if (lista == null) return "00:00";
            if (lista.Any(x => x.SobreTiempo > TimeSpan.Zero))
            {
                long ticks = lista.Where(x => x.SobreTiempo > TimeSpan.Zero)
                    .Sum(x => x.SobreTiempo.Ticks);
                return ticks.ImprimeFormatoHoraAufen();
            }
            return "00:00";

        }

        public static string CalculaColacion(this IEnumerable<LibroAsistenciaDTO> lista)
        {
            if (lista == null)
                return "00:00";
            if (lista.Any(x => x.TiempoColacionReal != TimeSpan.Zero))
            {
                long ticks = lista.Where(x => x.TiempoColacionReal != TimeSpan.Zero)
                    .Sum(x => x.TiempoColacionReal.Ticks);
                return ticks.ImprimeFormatoHoraAufen();
            }
            return "00:00";
        }

        public static string CalculaSobreEntrada(this IEnumerable<LibroAsistenciaDTO> lista)
        {
            if (lista == null) return "00:00";
            long ticks = lista.Where(x => x.SobreEntrada != TimeSpan.Zero)
               .Sum(x => x.SobreEntrada.Ticks);
            return ticks.ImprimeFormatoHoraAufen();
        }

        public static string CalculaSobreSalida(this IEnumerable<LibroAsistenciaDTO> lista)
        {
            if (lista == null) return "00:00";
            long ticks = lista.Where(x => x.SobreSalida != TimeSpan.Zero)
               .Sum(x => x.SobreSalida.Ticks);
            return ticks.ImprimeFormatoHoraAufen();
        }

        public static string CalculaDiasTrabajdos(this IEnumerable<LibroAsistenciaDTO> lista)
        {
            return lista.Any(x => x.Entrada.HasValue && x.Salida.HasValue) ?
               lista.Where(x => x.Entrada.HasValue && x.Salida.HasValue)
               .Count().ToString() : "0";
        }

        public static string CalculaDiasInasistencias(this IEnumerable<LibroAsistenciaDTO> lista)
        {
            return lista.Any(x => !x.Entrada.HasValue && !x.Salida.HasValue) ?
               lista.Where(x => !x.Entrada.HasValue || !x.Salida.HasValue)
               .Count().ToString() : "0";
        }

        public static string CalculaErroresMarcaje(this IEnumerable<LibroAsistenciaDTO> lista)
        {
            return lista.Any(x => x.Observacion == "Fichajes no cuadran") ?
               lista.Where(x => x.Observacion == "Fichajes no cuadran")
               .Count().ToString() : String.Empty;
        }

        public static string CalculaDiasAtraso(this IEnumerable<LibroAsistenciaDTO> lista)
        {
            return lista.Any(x => x.Atraso != TimeSpan.Zero && String.IsNullOrWhiteSpace(x.Observacion)) ?
               lista.Where(x => x.Atraso != TimeSpan.Zero && String.IsNullOrWhiteSpace(x.Observacion))
               .Count().ToString() : "0";
        }

        public static string CalculaDiasSalidaAdelantada(this IEnumerable<LibroAsistenciaDTO> lista)
        {
            return lista.Any(x => x.SalidaAdelantada != TimeSpan.Zero && String.IsNullOrWhiteSpace(x.Observacion)) ?
               lista.Where(x => x.SalidaAdelantada != TimeSpan.Zero && String.IsNullOrWhiteSpace(x.Observacion))
               .Count().ToString() : "0";
        }

        // T. de Salida para asistencia legal - Justificado
        public static string CalculaInasistenciaJustificadaLegal(this IEnumerable<LibroAsistenciaDTO> lista)
        {
            if (lista == null)
                return "00:00";
            if (lista.Any(x => x.Observacion != "Inasistencia Injustificada" && x.Observacion != "Festivo" && !String.IsNullOrWhiteSpace(x.Observacion)
                && x.SalidaTeorica.HasValue && x.EntradaTeorica.HasValue))
            {
                lista = lista.Where(x => x.Observacion != "Inasistencia Injustificada" && x.Observacion != "Festivo" && !String.IsNullOrWhiteSpace(x.Observacion)
                && x.SalidaTeorica.HasValue && x.EntradaTeorica.HasValue);
                long ticks = 0;
                long tiempoTrabajdo = 0;
                long tiempoInasistencia = 0;
                if (lista.Any(x => x.Entrada.HasValue && x.Salida.HasValue && x.Observacion != "Inasistencia Injustificada" && x.Observacion != "Festivo" && !String.IsNullOrWhiteSpace(x.Observacion)))
                {
                    tiempoTrabajdo = lista.Where(x => x.Entrada.HasValue && x.Salida.HasValue && x.Observacion != "Inasistencia Injustificada" && x.Observacion != "Festivo" && !String.IsNullOrWhiteSpace(x.Observacion)).Sum(x => x.Salida.Value.Subtract(x.Entrada.Value).Ticks);
                }

                tiempoInasistencia = lista.Where(x => x.Observacion != "Inasistencia Injustificada" && x.Observacion != "Festivo" && !String.IsNullOrWhiteSpace(x.Observacion)
                    && x.SalidaTeorica.HasValue && x.EntradaTeorica.HasValue)
                    .Sum(x => x.SalidaTeorica.Value.Subtract(x.EntradaTeorica.Value).Subtract(x.TiempoColacionReal).Ticks);

                ticks = tiempoInasistencia - tiempoTrabajdo;

                //ticks = lista.Where(x => x.Observacion == "Inasistencia Injustificada")
                //   .Sum(x => x.SalidaTeorica.Value.Subtract(x.EntradaTeorica.Value).Subtract(x.TiempoColacionReal).Ticks);
                return ticks.ImprimeFormatoHoraAufen();
            }
            else return "00:00";
        }

        // tiempo de ausencia para asistencia legal - injustificado
        public static string CalculaInasistenciaInjustificadaLegal(this IEnumerable<LibroAsistenciaDTO> lista)
        {
            if (lista == null)
                return "00:00";
            if (lista.Any(x => x.Observacion == "Inasistencia Injustificada" && x.Observacion != "Festivo" && x.EntradaTeorica.HasValue && x.SalidaTeorica.HasValue))
            {
                long ticks = lista.Where(x => x.Observacion == "Inasistencia Injustificada" && x.Observacion != "Festivo" && x.EntradaTeorica.HasValue && x.SalidaTeorica.HasValue)
                    .Sum(x => x.SalidaTeorica.Value.Subtract(x.EntradaTeorica.Value).Subtract(x.TiempoColacionReal).Ticks);
                return ticks.ImprimeFormatoHoraAufen();
            }
            else return "00:00";
        }
    }
}