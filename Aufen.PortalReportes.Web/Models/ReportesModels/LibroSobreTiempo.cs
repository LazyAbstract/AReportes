using Aufen.PortalReportes.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iTextSharp;
using iTextSharp.text.pdf;
using System.IO;
using iTextSharp.text;
using AutoMapper;
using ICSharpCode.SharpZipLib.Zip;

namespace Aufen.PortalReportes.Web.Models.ReportesModels
{
    public class LibroSobreTiempo : IArchivoReporte
    {
        private byte[] _Archivo { get; set; }
        private string NombreArchivo { get; set; }
        private Font Titulo { set; get; }
        private Font Normal { set; get; }
        private Font Chico { set; get; }

        public LibroSobreTiempo(AufenPortalReportesDataContext db, EMPRESA empresa, vw_Ubicacione departamento, DateTime fechaDesde, DateTime fechaHasta)
        {
            // Nombre del archivo y ubiación en el árbol de carpetas
            NombreArchivo = String.Format("{0}/{1}/LibroAtrasos.pdf", empresa.Descripcion, departamento.Descripcion);
            // Vamos a buscar los datos que nos permitirtán armar elreporte
            IEnumerable<sp_LibroAtrasosResult> resultadolibroSobretiempo =
                                           db.sp_LibroAtrasos(
                                           fechaDesde.ToString("yyyyMMdd"),
                                           fechaHasta.ToString("yyyyMMdd"),
                                           int.Parse(empresa.Codigo).ToString(), null).ToList();
            IEnumerable<sp_LibroAtrasosResultDTO> libroSobretiempo = Mapper.Map<IEnumerable<sp_LibroAtrasosResult>,
                IEnumerable<sp_LibroAtrasosResultDTO>>(resultadolibroSobretiempo);
            Configuracion();
            Document doc = new Document(iTextSharp.text.PageSize.LETTER, 10, 10, 42, 35);
            using (var ms = new MemoryStream())
            {
                PdfWriter pdfWriter = PdfWriter.GetInstance(doc, ms);
                doc.Open();
                foreach (var reporte in libroSobretiempo.GroupBy(x => new { x.Rut, x.IdDepartamento, x.IdEmpresa, NombreCompleto = x.Nombres+" "+x.Apellidos }).Take(3))
                {
                    doc.AddAuthor("Aufen");
                    doc.AddCreationDate();
                    doc.AddCreator("Aufen");
                    doc.AddTitle("Libro de Asistencia Personal - Sobretiempos");

                    // Agregamos el título y la bajada del reporte
                    Paragraph parrafo = new Paragraph();
                    parrafo.Add(new Paragraph("Libro de Asistencia Personal - Sobretiempos", Titulo) { Alignment = Element.ALIGN_CENTER });
                    parrafo.Add(new Paragraph(
                        String.Format("Período: {1} al {0}",
                        fechaDesde.ToShortDateString(),
                        fechaHasta.ToShortDateString()), Normal) { Alignment = Element.ALIGN_CENTER });
                    doc.Add(parrafo);
                    doc.Add(new Phrase());
                    //Agregamos el mes y tabla con siglas para el reporte
                    PdfPTable bajada = new PdfPTable(new float[] { 1,1});
                    bajada.AddCell(new PdfPCell(new Phrase(String.Format("Mes de {0}", fechaHasta.ToString("MMMM")), Normal)) { Rowspan = 2, Border = Rectangle.NO_BORDER });
                    PdfPTable bajadaSiglas = new PdfPTable(new float[] { 1, 1 });
                    bajadaSiglas.AddCell(new PdfPCell(new Phrase("HE: Horario Entrada", Normal)) { Border = Rectangle.NO_BORDER });
                    bajadaSiglas.AddCell(new PdfPCell(new Phrase("ME: Marca Entrada", Normal)) { Border = Rectangle.NO_BORDER });
                    bajadaSiglas.AddCell(new PdfPCell(new Phrase("HS: Horario de Salida", Normal)) { Border = Rectangle.NO_BORDER });
                    bajadaSiglas.AddCell(new PdfPCell(new Phrase("MS: Marca Salida", Normal)) { Border = Rectangle.NO_BORDER });
                    bajada.AddCell(new PdfPCell(bajadaSiglas) { });
                    doc.Add(bajada);
                    doc.Add(new Phrase());
                    // tabla
                    PdfPTable tabla = new PdfPTable(new float[] {2,1,1,1,1,1,1,1,1,2});
                    // Primera lìnea cabecera
                    tabla.AddCell(new PdfPCell(new Phrase(String.Format("Departamento: {0}", departamento.Descripcion), Normal)) { Colspan = 5 });
                    tabla.AddCell(new PdfPCell(new Phrase(String.Format("Rut: {0}", reporte.Key.Rut.ToStringConGuion()), Normal)) { Colspan = 2 });
                    tabla.AddCell(new PdfPCell(new Phrase(String.Format("Nombre: {0}", reporte.Key.NombreCompleto), Normal)) { Colspan = 3 });
                    // Segunda lìnea cabecera
                    tabla.AddCell(new PdfPCell(new Phrase("Fecha", Chico)));
                    tabla.AddCell(new PdfPCell(new Phrase("HE", Chico)));
                    tabla.AddCell(new PdfPCell(new Phrase("HS", Chico)));
                    tabla.AddCell(new PdfPCell(new Phrase("ME", Chico)));
                    tabla.AddCell(new PdfPCell(new Phrase("MS", Chico)));
                    tabla.AddCell(new PdfPCell(new Phrase("Tiempo Atrasos", Chico)));
                    tabla.AddCell(new PdfPCell(new Phrase("Salida Temprana", Chico)));
                    tabla.AddCell(new PdfPCell(new Phrase("Tiempo Extra", Chico)));
                    tabla.AddCell(new PdfPCell(new Phrase("Horas Laborales", Chico)));
                    tabla.AddCell(new PdfPCell(new Phrase("Observaciones", Chico)));
                    // Filas con datos
                    foreach (var sobretiempo in reporte)
                    {
                        TimeSpan tiempoAtraso = (sobretiempo.Salida.Value.Subtract(sobretiempo.Entrada.Value)) - sobretiempo.SalidaTeorica.Value.Subtract(sobretiempo.EntradaTeorica.Value);
                        TimeSpan tiempoNormal = sobretiempo.SalidaTeorica.Value.Subtract(sobretiempo.EntradaTeorica.Value);
                        tabla.AddCell(new PdfPCell(new Phrase(sobretiempo.Fecha.Value.ToString("dd/MM/yyyy ddd"), Chico)) { HorizontalAlignment = Element.ALIGN_LEFT });
                        tabla.AddCell(new PdfPCell(new Phrase(sobretiempo.EntradaTeorica.HasValue ? sobretiempo.EntradaTeorica.Value.ToString("HH:mm") : String.Empty, Chico)));
                        tabla.AddCell(new PdfPCell(new Phrase(sobretiempo.SalidaTeorica.HasValue ? sobretiempo.SalidaTeorica.Value.ToString("HH:mm") : String.Empty, Chico)));
                        tabla.AddCell(new PdfPCell(new Phrase(sobretiempo.Entrada.HasValue ? sobretiempo.Entrada.Value.ToString("HH:mm") : String.Empty, Chico)));
                        tabla.AddCell(new PdfPCell(new Phrase(sobretiempo.Salida.HasValue ? sobretiempo.Salida.Value.ToString("HH:mm") : String.Empty, Chico)));
                        tabla.AddCell(new PdfPCell(new Phrase(new DateTime(tiempoAtraso.Ticks).ToString("HH:mm"), Chico)));
                        tabla.AddCell(new PdfPCell(new Phrase("", Chico)));
                        tabla.AddCell(new PdfPCell(new Phrase("", Chico)));
                        tabla.AddCell(new PdfPCell(new Phrase(new DateTime(tiempoNormal.Ticks).ToString("HH:mm"), Chico)));
                        tabla.AddCell(new PdfPCell(new Phrase(sobretiempo.Observacion, Chico)));
                    }
                    // Fila con total
                    tabla.AddCell(new PdfPCell(new Phrase("Totales", Normal)) { Colspan = 5 });
                    tabla.AddCell(new PdfPCell(new Phrase("", Normal)));
                    tabla.AddCell(new PdfPCell(new Phrase("", Normal)));
                    tabla.AddCell(new PdfPCell(new Phrase("", Normal)));
                    tabla.AddCell(new PdfPCell(new Phrase("", Normal)));
                    tabla.AddCell(new PdfPCell(new Phrase("", Normal)));
                   doc.Add(tabla);
                   doc.NewPage();
                }
                doc.Close();
                _Archivo = ms.ToArray();
            }
        }

        private void Configuracion()
        {
            BaseFont bf = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, false);
            Titulo = new Font(bf, 18, Font.UNDERLINE, BaseColor.BLACK);
            Normal = new Font(bf, 11, Font.NORMAL, BaseColor.BLACK);
            Chico = new Font(bf, 8, Font.NORMAL, BaseColor.BLACK);

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