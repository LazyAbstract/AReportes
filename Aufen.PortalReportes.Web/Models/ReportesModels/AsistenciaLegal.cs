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

        public AsistenciaLegal(AufenPortalReportesDataContext db, EMPRESA empresa, vw_Ubicacione departamento, DateTime FechaDesde, DateTime FechaHasta, string path, string rut)
        {
            // Nombre del archivo y ubiación en el árbol de carpetas
             NombreArchivo = String.Format("{0}/{1}/AsistenciaLegal.pdf", empresa.Descripcion, departamento.Descripcion);
            // Vamos a buscar los datos que nos permitirtán armar elreporte
            //Resultado de marcas
             IEnumerable<sp_LibroAsistenciaResult> resultadoLibroAtrasos =
                                           db.sp_LibroAsistencia(
                                           FechaDesde.ToString("yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture),
                                           FechaHasta.ToString("yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture),
                                           int.Parse(empresa.Codigo).ToString(),
                                           departamento.Codigo,
                                           rut).ToList();
             IEnumerable<LibroAsistenciaDTO> resultado = Mapper.Map<IEnumerable<sp_LibroAsistenciaResult>,
             IEnumerable<LibroAsistenciaDTO>>(resultadoLibroAtrasos);
            // Resumen de inasistencias
             IEnumerable<sp_LibroInasistenciaResult> resultadoLibroInasistencias =
                 db.sp_LibroInasistencia(FechaDesde.ToString("yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture)
                    , FechaHasta.ToString("yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture)
                    , int.Parse(empresa.Codigo).ToString()
                    , departamento.Codigo
                    , rut);
             IEnumerable<LibroInasistenciaDTO> resultadoInasistencia = Mapper.Map<IEnumerable<sp_LibroInasistenciaResult>,
                 IEnumerable<LibroInasistenciaDTO>>(resultadoLibroInasistencias);
             if (resultadoLibroAtrasos.Any())
             {
                 string[] diasSemana = new[] { "dom", "lun", "mar", "mie", "ju", "vie", "sab" };
                 using (MemoryStream finalStream = new MemoryStream())
                 {
                     PdfCopyFields copy = new PdfCopyFields(finalStream);
                     foreach (var reporte in resultado.Where(x => x.Rut != null).GroupBy(x => new
                     {
                         x.Rut,
                         x.IdEmpresa,
                         x.IdDepartamento,
                         Mes = x.Fecha.Value.Month,
                         Anio = x.Fecha.Value.Year
                     }))
                     {
                         var inasistencias = resultadoInasistencia.Where(x => x.Rut!= null &&
                             x.Rut == reporte.Key.Rut &&
                             x.IdEmpresa == reporte.Key.IdEmpresa &&
                             x.IdDepartamento == reporte.Key.IdDepartamento &&
                            reporte.Key.Mes == x.Fecha.Value.Month &&
                            reporte.Key.Anio == x.Fecha.Value.Year);
                         var empleado = db.vw_Empleados.FirstOrDefault(x => x.IdEmpresa == empresa.Codigo &&
                             x.IdUbicacion == reporte.Key.IdDepartamento &&
                                  x.Codigo == reporte.Key.Rut);
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
                                 pdfStamper.AcroFields.SetField("Rut", empleado.RutAufen);
                                 pdfStamper.AcroFields.SetField("Departamento", String.Format("{0} ({1})",departamento!= null ? departamento.SucursalPlanta : String.Empty, empresa!=null ? empresa.Descripcion.Trim() : String.Empty));
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
                                         //
                                         pdfStamper.AcroFields.SetField(String.Format("Semana{0}Tipo{1}", i, correccionDia),
                                             String.Format("{0}\n{1}", 
                                                String.Format("{0}-{1}", dia!= null && dia.Entrada.HasValue ? dia.Entrada.Value.ToString("HH:mm") : String.Empty
                                                    , dia!=null && dia.Salida.HasValue ? dia.Salida.Value.ToString("HH:mm") : String.Empty),
                                                dia != null ? dia.Observacion : String.Empty));
                                        pdfStamper.AcroFields.SetField(String.Format("Semana{0}Dia{1}", i, correccionDia), String.Format("{0} {1}", dia != null ? dia.Fecha.Value.ToString("dd/MM") : string.Empty, diasSemana[j]));
                                     }
                                     // Semana a semana
                                     pdfStamper.AcroFields.SetField(String.Format("Semana{0}Jornada", i), semana.CalculaJornada());
                                     pdfStamper.AcroFields.SetField(String.Format("Semana{0}Asistencia", i), semana.CalculaAsistencia());
                                     pdfStamper.AcroFields.SetField(String.Format("Semana{0}Salidas", i), semana.CalculaSalidaAdelantada());
                                     
                                     var inasistenciaSemanal = inasistencias.Where(x => x.Fecha.HasValue && x.Fecha.Value.Day >= (i - 1) * 7 && x.Fecha.Value.Day <= i * 7);
                                     pdfStamper.AcroFields.SetField(String.Format("Semana{0}Ausencias", i), inasistenciaSemanal.CalculaJornada());
                                     pdfStamper.AcroFields.SetField(String.Format("Semana{0}AtrasosSalidas", i), semana.CalculaAtrasoSalida());
                                     pdfStamper.AcroFields.SetField(String.Format("Semana{0}NumeroAtrasos", i), reporte.CalculaDiasAtraso());
                                     pdfStamper.AcroFields.SetField(String.Format("Semana{0}NumeroSalidas", i), reporte.CalculaDiasSalidaAdelantada());
                                     pdfStamper.AcroFields.SetField(String.Format("Semana{0}ExtraConTurno", i), "");
                                     pdfStamper.AcroFields.SetField(String.Format("Semana{0}ExtraSinTurno", i), "");
                                 }
                                 // Resumen de todas las semanas
                                 pdfStamper.AcroFields.SetField("ResumenJornada", reporte.CalculaJornada());
                                 pdfStamper.AcroFields.SetField("ResumenAsistencia", reporte.CalculaAsistencia());
                                 pdfStamper.AcroFields.SetField("ResumenSalidas", reporte.CalculaSalidaAdelantada());
                                 pdfStamper.AcroFields.SetField("ResumenAusencias", inasistencias.CalculaJornada());
                                 pdfStamper.AcroFields.SetField("ResumenAtrasosSalidas", reporte.CalculaAtrasoSalida());
                                 pdfStamper.AcroFields.SetField("ResumenNumeroAtrasos", reporte.CalculaDiasAtraso());
                                 pdfStamper.AcroFields.SetField("ResumenNumeroSalidas", reporte.CalculaDiasSalidaAdelantada());
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