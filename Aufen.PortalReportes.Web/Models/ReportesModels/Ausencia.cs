using Aufen.PortalReportes.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iTextSharp;
using iTextSharp.text.pdf;
using System.IO;
using iTextSharp.text;
using ICSharpCode.SharpZipLib.Zip;
using AutoMapper;

namespace Aufen.PortalReportes.Web.Models.ReportesModels
{
    public class Ausencia : IArchivoReporte
    {
        private byte[] _Archivo { get; set; }
        private string NombreArchivo { get; set; }
        private Font Titulo { set; get; }
        private Font Normal { set; get; }
        private Font Chico { set; get; }

        public Ausencia(AufenPortalReportesDataContext db, EMPRESA empresa, vw_Ubicacione departamento, DateTime FechaDesde, DateTime FechaHasta, string path, Rut rut)
        {
            //Nombre del archivo y ubiación en el árbol de carpetas
            NombreArchivo = String.Format("{0}/{1}/PersonalAusente.pdf", empresa.Descripcion, departamento.Descripcion);
            // Vamos a buscar los datos que nos permitirtán armar elreporte
            string buff = null;
            if (rut != null)
            {
                buff = rut.ToString();
            }
            IEnumerable<sp_LibroInasistenciaResult> resultado = db.sp_LibroInasistencia(
                FechaDesde.ToString("yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture)
                , FechaHasta.ToString("yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture)
                , int.Parse(empresa.Codigo).ToString()
                , departamento.Codigo
                , buff).OrderBy(x => x.Fecha).ToList();
            IEnumerable<LibroInasistenciaDTO> inasistencias =
                Mapper.Map<IEnumerable<sp_LibroInasistenciaResult>, IEnumerable<LibroInasistenciaDTO>>(resultado);
            if (inasistencias.Any())
            {
                Configuracion();
                Document doc = new Document(iTextSharp.text.PageSize.LETTER, 10, 10, 50, 35);
                using (var ms = new MemoryStream())
                {
                    PdfWriter pdfWriter = PdfWriter.GetInstance(doc, ms);
                    pdfWriter.PageEvent = new Header(empresa, path);                    
                    doc.Open();
                    foreach (var reporte in inasistencias.Where(x=>x.Rut!=null).GroupBy(x => new { x.Rut.Numero, x.IdDepartamento, x.IdEmpresa }).Take(3))
                    {
                        doc.AddAuthor("Aufen");
                        doc.AddCreationDate();
                        doc.AddCreator("Aufen");
                        doc.AddTitle("Informe de Personal Ausente (sin marcas)");

                        Paragraph parrafo = new Paragraph();

                        parrafo.Add(new Paragraph("Informe de Personal Ausente (sin marcas)", Titulo) { Alignment = Element.ALIGN_CENTER });
                        parrafo.Add(new Paragraph(String.Format("Período: {0} a {1}", FechaDesde.ToShortDateString(), FechaHasta.ToShortDateString()), Normal) { Alignment = Element.ALIGN_CENTER });
                        doc.Add(parrafo);
                        doc.Add(new Phrase());

                        PdfPTable tabla = new PdfPTable(new float[] {2, 1, 2, 2, 1, 1, 4 });
                        // Encabezado
                        tabla.AddCell(new PdfPCell(new Phrase("Empleado", Normal)) { Colspan = 4});
                        tabla.AddCell(new PdfPCell(new Phrase("Horario", Normal)) { Colspan = 2 });
                        tabla.AddCell(new PdfPCell(new Phrase("Autorizaciones", Normal)));
                        // 2 encabezado
                        tabla.AddCell(new PdfPCell(new Phrase("Fecha", Chico)));
                        tabla.AddCell(new PdfPCell(new Phrase("Código", Chico)));
                        tabla.AddCell(new PdfPCell(new Phrase("Apellidos", Chico)));
                        tabla.AddCell(new PdfPCell(new Phrase("Nombres", Chico)));
                        tabla.AddCell(new PdfPCell(new Phrase("Ing.", Chico)));
                        tabla.AddCell(new PdfPCell(new Phrase("Sal.", Chico)));
                        tabla.AddCell(new PdfPCell(new Phrase("Autorizaciones", Chico)));

                        string codigoEmpleado = reporte.Key.Numero.ToString("00000000") + new Rut(reporte.Key.Numero).DV;
                        var empleado = db.vw_Empleados.FirstOrDefault(x => x.IdEmpresa == empresa.Codigo &&
                            x.IdUbicacion == reporte.Key.IdDepartamento &&
                                 x.Codigo == codigoEmpleado);
                        foreach(var ausencia in reporte)
                        {
                            //Fecha
                            tabla.AddCell(new PdfPCell(new Phrase(ausencia.Fecha.HasValue ? ausencia.Fecha.Value.ToString("ddd dd/MM") : String.Empty, Chico)));
                            //Código
                            tabla.AddCell(new PdfPCell(new Phrase("", Chico)));
                            //Apellidos
                            tabla.AddCell(new PdfPCell(new Phrase((ausencia.Apellidos ?? string.Empty).Trim(), Chico)));
                            //Nombres
                            tabla.AddCell(new PdfPCell(new Phrase((ausencia.Nombres ?? string.Empty).Trim(), Chico)));
                            //Ing.
                            tabla.AddCell(new PdfPCell(new Phrase(ausencia.EntradaTeorica.HasValue ? ausencia.EntradaTeorica.Value.ToString("HH:mm") : String.Empty, Chico)));
                            //Sal.
                            tabla.AddCell(new PdfPCell(new Phrase(ausencia.SalidaTeorica.HasValue ? ausencia.SalidaTeorica.Value.ToString("HH:mm") : String.Empty, Chico)));
                            //Autorizaciones
                            tabla.AddCell(new PdfPCell(new Phrase(ausencia.Observacion, Chico)));
                        }
                        doc.Add(tabla);
                        doc.NewPage();
                    }
                    doc.Close();
                    _Archivo = ms.ToArray();
                }
            }
        }

        private void Configuracion()
        {
            BaseFont bf = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, false);
            Titulo = new Font(bf, 18, Font.UNDERLINE, BaseColor.BLACK);
            Normal = new Font(bf, 11, Font.NORMAL, BaseColor.BLACK);
            Chico = new Font(bf, 9, Font.NORMAL, BaseColor.BLACK);

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