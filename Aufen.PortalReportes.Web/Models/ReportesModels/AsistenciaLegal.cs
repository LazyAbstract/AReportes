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
using ICSharpCode.SharpZipLib.Zip;
using AutoMapper;
namespace Aufen.PortalReportes.Web.Models.ReportesModels
{
    public class AsistenciaLegal : IArchivoReporte
    {
        private byte[] _Archivo { get; set; }
        private string NombreArchivo { get; set; }
        private Font Titulo { set; get; }
        private Font Normal { set; get; }

        public AsistenciaLegal(AufenPortalReportesDataContext db, EMPRESA empresa, vw_Ubicacione departamento, DateTime FechaDesde, DateTime FechaHasta, string path, Rut rut)
        {
            // Nombre del archivo y ubiación en el árbol de carpetas
             NombreArchivo = String.Format("{0}/{1}/AsistenciaLegal.pdf", empresa.Descripcion, departamento.Descripcion);
            // Vamos a buscar los datos que nos permitirtán armar elreporte
            string buff = null;
            if (rut != null)
                 buff = rut.ToString();
             IEnumerable<sp_LibroAsistenciaResult> resultadoLibroAtrasos =
                                           db.sp_LibroAsistencia(
                                           FechaDesde.ToString("yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture),
                                           FechaHasta.ToString("yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture),
                                           int.Parse(empresa.Codigo).ToString(),
                                           departamento.Codigo,
                                           buff).ToList();
             IEnumerable<LibroAsistenciaDTO> resultado = Mapper.Map<IEnumerable<sp_LibroAsistenciaResult>,
             IEnumerable<LibroAsistenciaDTO>>(resultadoLibroAtrasos);
             if (resultadoLibroAtrasos.Any())
             {
                 string[] diasSemana = new[] { "dom", "lun", "mar", "mie", "ju", "vie", "sab" };
                 using (MemoryStream finalStream = new MemoryStream())
                 {
                     PdfCopyFields copy = new PdfCopyFields(finalStream);
                     foreach (var reporte in resultado.Where(x => x.Rut != null).GroupBy(x => new
                     {
                         x.Rut.Numero,
                         x.IdEmpresa,
                         x.IdDepartamento,
                         Mes = x.Fecha.Value.Month,
                         Anio = x.Fecha.Value.Year
                     }))
                     {
                         string codigoEmpleado = reporte.Key.Numero.ToString("00000000") + new Rut(reporte.Key.Numero).DV;
                         var empleado = db.vw_Empleados.FirstOrDefault(x => x.IdEmpresa == empresa.Codigo &&
                             x.IdUbicacion == reporte.Key.IdDepartamento &&
                                  x.Codigo == codigoEmpleado);
                         using (MemoryStream ms = new MemoryStream())
                         {
                             using (PdfReader pdfReader = new PdfReader(path + @"\ReporteAsistenciaLegal.pdf"))
                             {
                                 DateTime fechaReferencia = reporte.First().Fecha.Value;
                                 DateTime primerDiaMes = new DateTime(fechaReferencia.Year, fechaReferencia.Month, 1);
                                 DateTime ultimoDiaMes = primerDiaMes.AddMonths(1).AddSeconds(-1);
                                 PdfStamper pdfStamper = new PdfStamper(pdfReader, ms);
                                 
                                 int PageCount = pdfReader.NumberOfPages;
                                 for (int x = 1; x <= PageCount; x++)
                                 {
                                     PdfContentByte cb = pdfStamper.GetOverContent(x);
                                     Image imagen = Image.GetInstance(String.Format(@"{0}\imagenes\LogosEmpresas\logo{1}.jpg", path, empresa.Codigo.Trim()));
                                    imagen.ScaleToFit(100, 200);
                                     imagen.SetAbsolutePosition(450, 750);
                                     cb.AddImage(imagen);
                                 }

                                 pdfStamper.AcroFields.SetField("Mes", new DateTime(reporte.Key.Anio, reporte.Key.Mes, 1).ToString("yyyy MMM"));
                                 pdfStamper.AcroFields.SetField("Nombre", empleado != null ? empleado.NombreCompleto : String.Empty);
                                 pdfStamper.AcroFields.SetField("Rut", new Rut(reporte.Key.Numero).ToStringConGuion());
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
                                        // Si se elimina esto el domingo va aquedar al final
                                        int correccionDia = j;//j == 6 ? 0 : j + 1;
                                         var dia = reporte.FirstOrDefault(x => x.NumSemana == i && (int)x.Fecha.Value.DayOfWeek == j);
                                        if (dia!=null && !String.IsNullOrEmpty(dia.Observacion))
                                        {
                                            pdfStamper.AcroFields.SetField(String.Format("Semana{0}Tipo{1}", i, correccionDia), dia != null ? dia.Observacion : String.Empty);
                                        }
                                        else if(dia!=null)
                                        {
                                            pdfStamper.AcroFields.SetField(String.Format("Semana{0}Tipo{1}", i, correccionDia), 
                                                String.Format("{0}-{1}", dia.Entrada.HasValue ? dia.Entrada.Value.ToString("HH:mm") : String.Empty
                                                , dia.Salida.HasValue ? dia.Salida.Value.ToString("HH:mm") : String.Empty
                                                ));
                                        }
                                        pdfStamper.AcroFields.SetField(String.Format("Semana{0}Dia{1}", i, correccionDia), String.Format("{0} {1}", dia != null ? dia.Fecha.Value.ToString("dd/MM") : string.Empty, diasSemana[j]));
                                     }
                                     // Semana a semana
                                     pdfStamper.AcroFields.SetField(String.Format("Semana{0}Jornada", i), semana.CalculaJornada());
                                     pdfStamper.AcroFields.SetField(String.Format("Semana{0}Asistencia", i), semana.CalculaAsistencia());
                                     pdfStamper.AcroFields.SetField(String.Format("Semana{0}Salidas", i), "");
                                     pdfStamper.AcroFields.SetField(String.Format("Semana{0}Ausencias", i), "");
                                     pdfStamper.AcroFields.SetField(String.Format("Semana{0}AtrasosSalidas", i), semana.CalculaSalidaAdelantada());
                                     pdfStamper.AcroFields.SetField(String.Format("Semana{0}NumeroAtrasos", i),
                                         (semana.Count(x => x.EntradaTeorica.HasValue && x.Entrada.HasValue && x.Entrada > x.EntradaTeorica) +
                                         semana.Count(x => x.SalidaTeorica.HasValue && x.Salida.HasValue && x.Salida < x.SalidaTeorica)).ToString("N0"));
                                     pdfStamper.AcroFields.SetField(String.Format("Semana{0}NumeroSalidas", i), "");
                                     pdfStamper.AcroFields.SetField(String.Format("Semana{0}ExtraConTurno", i), "");
                                     pdfStamper.AcroFields.SetField(String.Format("Semana{0}ExtraSinTurno", i), "");
                                 }
                                 // Resumen de todas las semanas
                                 pdfStamper.AcroFields.SetField("ResumenJornada", reporte.CalculaJornada());
                                 pdfStamper.AcroFields.SetField("ResumenAsistencia", reporte.CalculaAsistencia());
                                 pdfStamper.AcroFields.SetField("ResumenSalidas", "");
                                 pdfStamper.AcroFields.SetField("ResumenAusencias", "");
                                 pdfStamper.AcroFields.SetField("ResumenAtrasosSalidas", reporte.CalculaSalidaAdelantada());
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

        public ZipEntry GetZipArchivoReporte()
        {
            ZipEntry zipEntry = new ZipEntry(NombreArchivo);
            zipEntry.DateTime = DateTime.Now;
            return zipEntry;
        }

        public byte[] GetArchivo()
        {
            return _Archivo;
        }

        public int GetArchivoLength()
        {
            return _Archivo.Length;
        }
    }
}