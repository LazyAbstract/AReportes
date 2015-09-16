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
                foreach (var reporte in resultado.GroupBy(x => new { 
                    x.Rut, Mes = x.Fecha.Value.ToString("MMMM yyyy"), 
                    x.IdEmpresa, Nombre = x.Nombres + " " + x.Apellidos }).Take(3))
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (PdfReader pdfReader = new PdfReader(path))
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
                                    pdfStamper.AcroFields.SetField(String.Format("Semana{0}Tipo{1}",i,j),dia!=null ? dia.Observacion : String.Empty);
                                    pdfStamper.AcroFields.SetField(String.Format("Semana{0}Dia{1}",i,j),String.Format("{0} {1}",dia!=null ? dia.Fecha.Value.ToString("dd/MM") : string.Empty , diasSemana[j]));
                                }

                                pdfStamper.AcroFields.SetField(String.Format("Semana{0}Jornada", i),CalculaJornada(semana));
                                pdfStamper.AcroFields.SetField(String.Format("Semana{0}Asistencia", i), CalculaAsistencia(semana));
                                pdfStamper.AcroFields.SetField(String.Format("Semana{0}Salidas", i), ""); 
                                pdfStamper.AcroFields.SetField(String.Format("Semana{0}Ausencias", i), "");
                                pdfStamper.AcroFields.SetField(String.Format("Semana{0}AtrasosSalidas", i), "");
                                pdfStamper.AcroFields.SetField(String.Format("Semana{0}NumeroAtrasos", i), "");
                                pdfStamper.AcroFields.SetField(String.Format("Semana{0}NumeroSalidas", i), "");
                                pdfStamper.AcroFields.SetField(String.Format("Semana{0}ExtraConTurno", i), "");
                                pdfStamper.AcroFields.SetField(String.Format("Semana{0}ExtraSinTurno", i), "");
                            }

                                pdfStamper.FormFlattening = true;
                            pdfStamper.Close();
                        }
                        ms.Position = 0;
                        copy.AddDocument(new PdfReader(ms));
                        ms.Dispose();
                    }
                }
                copy.Close();
                _Archivo = finalStream.ToArray();
            }
        }

        private string CalculaJornada(IEnumerable<sp_LibroAtrasosResultDTO> lista)
        {
            return lista.Any(x => x.SalidaTeorica.HasValue && x.EntradaTeorica.HasValue) ?
                        new DateTime(lista.Where(x => x.SalidaTeorica.HasValue && x.EntradaTeorica.HasValue)
                        .Sum(x => x.SalidaTeorica.Value.Subtract(x.EntradaTeorica.Value).Ticks))
                            .ToString("HH:mm") : String.Empty;

        }

        private string CalculaAsistencia(IEnumerable<sp_LibroAtrasosResultDTO> lista)
        {
            return lista.Any(x => x.Salida.HasValue && x.Entrada.HasValue) ?
                new DateTime(lista.Where(x => x.Salida.HasValue && x.Entrada.HasValue)
                .Sum(x => x.Salida.Value.Subtract(x.Entrada.Value).Ticks)).ToString("HH:mm") : String.Empty;
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