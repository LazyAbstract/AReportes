using Aufen.PortalReportes.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iTextSharp;
using iTextSharp.text.pdf;
using System.IO;
using iTextSharp.text;
using System.Globalization;
namespace Aufen.PortalReportes.Web.Models.ReportesModels
{
    public class AsistenciaLegal
    {
        private byte[] _Archivo { get; set; }
        private IEnumerable<sp_LibroAtrasosResultDTO> Resultado { get; set; }
        private Font Titulo { set; get; }
        private Font Normal { set; get; }

        public AsistenciaLegal(IEnumerable<sp_LibroAtrasosResultDTO> resultado, AufenPortalReportesDataContext db, string path)
        {
            string[] diasSemana = new[] { "dom", "lun", "mar", "mie", "ju", "vie", "sab" };
            using (MemoryStream finalStream = new MemoryStream())
            {
                PdfCopyFields copy = new PdfCopyFields(finalStream);
                foreach (var reporte in resultado.GroupBy(x => new
                {
                    x.Rut,
                    Mes = x.Fecha.Value.ToString("MMMM yyyy"),
                    x.IdEmpresa,
                    Nombre = x.Nombres + " " + x.Apellidos
                }).Take(3))
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (PdfReader pdfReader = new PdfReader(path + @"\ReporteAsistenciaLegal.pdf"))
                        {
                            DateTime fechaReferencia = reporte.First().Fecha.Value;
                            DateTime primerDiaMes = new DateTime(fechaReferencia.Year, fechaReferencia.Month, 1);
                            DateTime ultimoDiaMes = primerDiaMes.AddMonths(1).AddSeconds(-1);
                            PdfStamper pdfStamper = new PdfStamper(pdfReader, ms);
                            pdfStamper.AcroFields.SetField("Mes", reporte.Key.Mes);
                            pdfStamper.AcroFields.SetField("Nombre", reporte.Key.Nombre);
                            pdfStamper.AcroFields.SetField("Rut", reporte.Key.Rut.ToStringConGuion());
                            pdfStamper.AcroFields.SetField("Fecha", String.Format("{0} - {1}", primerDiaMes.ToShortDateString(), ultimoDiaMes.ToShortDateString()));
                            pdfStamper.AcroFields.SetField("ImpresoPagina1", DateTime.Now.ToShortDateString());
                            pdfStamper.AcroFields.SetField("ImpresoPagina2", DateTime.Now.ToShortDateString());
                            pdfStamper.AcroFields.SetField("UsuarioPagina1", "");
                            pdfStamper.AcroFields.SetField("UsuarioPagina2", "");
                            //Para todas las semanas
                            for (int i = 1; i <= 5; i++)
                            {
                                //Para todos los días de la semana
                                var semana = reporte.Where(x => x.NumSemana == i);
                                for (int j = 0; j <= 6; j++)
                                {
                                    var dia = reporte.SingleOrDefault(x => x.NumSemana == i && (int)x.Fecha.Value.DayOfWeek == j);
                                    pdfStamper.AcroFields.SetField(String.Format("Semana{0}Tipo{1}", i, j), dia != null ? dia.Observacion : String.Empty);
                                    pdfStamper.AcroFields.SetField(String.Format("Semana{0}Dia{1}", i, j), String.Format("{0} {1}", dia != null ? dia.Fecha.Value.ToString("dd/MM") : string.Empty, diasSemana[j]));
                                }
                                // Semana a semana
                                pdfStamper.AcroFields.SetField(String.Format("Semana{0}Jornada", i), CalculaJornada(semana));
                                pdfStamper.AcroFields.SetField(String.Format("Semana{0}Asistencia", i), CalculaAsistencia(semana));
                                pdfStamper.AcroFields.SetField(String.Format("Semana{0}Salidas", i), "");
                                pdfStamper.AcroFields.SetField(String.Format("Semana{0}Ausencias", i), "");
                                pdfStamper.AcroFields.SetField(String.Format("Semana{0}AtrasosSalidas", i), new DateTime(CalculaAtrasoEntrada(semana)+CalculaAtrasoSalida(semana)).ToString("HH:mm"));
                                pdfStamper.AcroFields.SetField(String.Format("Semana{0}NumeroAtrasos", i),
                                    (semana.Count(x => x.EntradaTeorica.HasValue && x.Entrada.HasValue && x.Entrada > x.EntradaTeorica) +
                                    semana.Count(x => x.SalidaTeorica.HasValue && x.Salida.HasValue && x.Salida < x.SalidaTeorica)).ToString("N0"));
                                pdfStamper.AcroFields.SetField(String.Format("Semana{0}NumeroSalidas", i), "");
                                pdfStamper.AcroFields.SetField(String.Format("Semana{0}ExtraConTurno", i), "");
                                pdfStamper.AcroFields.SetField(String.Format("Semana{0}ExtraSinTurno", i), "");
                            }
                            // Resumen de todas las semanas
                            pdfStamper.AcroFields.SetField("ResumenJornada", CalculaJornada(reporte));
                            pdfStamper.AcroFields.SetField("ResumenAsistencia", CalculaAsistencia(reporte));
                            pdfStamper.AcroFields.SetField("ResumenSalidas", "");
                            pdfStamper.AcroFields.SetField("ResumenAusencias", "");
                            pdfStamper.AcroFields.SetField("ResumenAtrasosSalidas", new DateTime(CalculaAtrasoEntrada(reporte) + CalculaAtrasoSalida(reporte)).ToString("HH:mm"));
                            pdfStamper.AcroFields.SetField("ResumenNumeroAtrasos", 
                                (reporte.Count(x => x.EntradaTeorica.HasValue && x.Entrada.HasValue && x.Entrada > x.EntradaTeorica) +
                                reporte.Count(x => x.SalidaTeorica.HasValue && x.Salida.HasValue && x.Salida < x.SalidaTeorica)).ToString("N0"));
                            pdfStamper.AcroFields.SetField("ResumenNumeroSalidas", "");
                            pdfStamper.AcroFields.SetField("ResumenExtraConTurno", "");
                            pdfStamper.AcroFields.SetField("ResumenExtraSinTurno", "");
                            pdfStamper.Writer.CloseStream = false;
                            pdfStamper.FormFlattening = true;
                            pdfStamper.Close();
                            ms.Position = 0;
                            copy.AddDocument(new PdfReader(ms));
                            ms.Dispose();
                        }
                    }
                }
                copy.Close();
                _Archivo = finalStream.ToArray();
            }
        }

        /// <summary>
        ///     A partir de un DTO de resultado del SP calculo la jornada teórica utilizando las filas que tienen los datos de salida y entrada teórica
        /// </summary>
        /// <param name="lista">Enumeración de sp_LibroAtrasosResultDTO para hacer el calculo</param>
        /// <returns>Las Horas y minutos correspondientes al cálculo de la hora jornada teórica en formato de texto HH:mm</returns>
        private string CalculaJornada(IEnumerable<sp_LibroAtrasosResultDTO> lista)
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
        private string CalculaAsistencia(IEnumerable<sp_LibroAtrasosResultDTO> lista)
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
        private long CalculaAtrasoEntrada(IEnumerable<sp_LibroAtrasosResultDTO> lista)
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
        private long CalculaAtrasoSalida(IEnumerable<sp_LibroAtrasosResultDTO> lista)
        {
            return lista.Any(x => x.SalidaTeorica.HasValue && x.Salida.HasValue && x.Salida < x.SalidaTeorica) ?
                lista.Where(x => x.SalidaTeorica.HasValue && x.Salida.HasValue && x.Salida < x.SalidaTeorica)
                .Sum(x => x.SalidaTeorica.Value.Subtract(x.Salida.Value).Ticks) : 0;      
        }

        private void Configuracion()
        {
            BaseFont bf = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, false);
            Titulo = new Font(bf, 16, Font.UNDERLINE, BaseColor.BLACK);
            Normal = new Font(bf, 11, Font.NORMAL, BaseColor.BLACK);

        }

        public byte[] Archivo
        {
            get
            {
                return _Archivo;
            }
        }
    }
}