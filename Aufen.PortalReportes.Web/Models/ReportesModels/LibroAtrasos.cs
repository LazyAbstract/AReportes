using Aufen.PortalReportes.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iTextSharp;
using iTextSharp.text.pdf;
using System.IO;
using iTextSharp.text;

namespace Aufen.PortalReportes.Web.Models.ReportesModels
{
    public class LibroAtrasos
    {
        private byte[] _Archivo { get; set; }
        private IEnumerable<sp_LibroAtrasosResultDTO> libroAtrasos { get; set; }
        private Font Titulo { set; get; }
        private Font Normal { set; get; }
        private Font Chico { set; get; }

        public LibroAtrasos(IEnumerable<sp_LibroAtrasosResultDTO> resultado, AufenPortalReportesDataContext db)
        {
            Configuracion();
            libroAtrasos = resultado;//.Where(x => x.Salida.Subtract(x.Entrada) < x.SalidaTeorica.Subtract(x.EntradaTeorica));
            Document doc = new Document(iTextSharp.text.PageSize.LETTER, 10, 10, 42, 35);
            using (var ms = new MemoryStream())
            {
                PdfWriter pdfWriter = PdfWriter.GetInstance(doc, ms);
                doc.Open();
                foreach (var reporte in libroAtrasos.GroupBy(x => new { x.Rut.Numero, x.IdDepartamento, x.IdEmpresa }).Take(3))
                {
                    var departamento = db.vw_Ubicaciones.SingleOrDefault(x =>
                        x.Codigo == reporte.Key.IdDepartamento &&
                        x.IdEmpresa == reporte.Key.IdEmpresa);
                    doc.AddAuthor("Aufen");
                    doc.AddCreationDate();
                    doc.AddCreator("Aufen");
                    doc.AddTitle("Libro de Atrasos");

                    // Texto 
                    Paragraph parrafo = new Paragraph();
                    parrafo.Add(new Paragraph(
                        String.Format("Departamento: {1}, Fecha de Reporte: {0}",
                        DateTime.Now.ToShortDateString(),
                        departamento != null ? departamento.Descripcion : "Sin Información"), Normal) { Alignment = Element.ALIGN_CENTER });
                    parrafo.Add(new Paragraph("Informe de Atrasos por Área", Titulo) { Alignment = Element.ALIGN_CENTER });
                    doc.Add(parrafo);
                    doc.Add(new Phrase());
                    // tabla
                    PdfPTable tabla = new PdfPTable(new float[] { 3, 3, 3, 1, 1, 1, 1, 1, 1, 4 });
                    // Primera lìnea cabecera
                    tabla.AddCell(new PdfPCell(new Phrase("Empleado", Chico)) { Colspan = 3 });
                    tabla.AddCell(new PdfPCell(new Phrase("Horario", Chico)) { Colspan = 2 });
                    tabla.AddCell(new PdfPCell(new Phrase("Marcas", Chico)) { Colspan = 2 });
                    tabla.AddCell(new PdfPCell(new Phrase("Horas Trabajadas", Chico)) { Colspan = 2 });
                    tabla.AddCell(new PdfPCell(new Phrase("Autorizaciones", Chico)));
                    // Segunda lìnea cabecera
                    tabla.AddCell(new PdfPCell(new Phrase("Rut", Chico)));
                    tabla.AddCell(new PdfPCell(new Phrase("Apellidos", Chico)));
                    tabla.AddCell(new PdfPCell(new Phrase("Nombres", Chico)));
                    tabla.AddCell(new PdfPCell(new Phrase("Ing.", Chico)));
                    tabla.AddCell(new PdfPCell(new Phrase("Sal.", Chico)));
                    tabla.AddCell(new PdfPCell(new Phrase("Ing.", Chico)));
                    tabla.AddCell(new PdfPCell(new Phrase("Sal.", Chico)));
                    tabla.AddCell(new PdfPCell(new Phrase("Atrasos", Chico)));
                    tabla.AddCell(new PdfPCell(new Phrase("H.T.N.", Chico)));
                    tabla.AddCell(new PdfPCell(new Phrase("Permisos", Chico)));

                    foreach (var atraso in reporte)
                    {
                        TimeSpan tiempoAtraso = (atraso.Salida.Value.Subtract(atraso.Entrada.Value)) - atraso.SalidaTeorica.Value.Subtract(atraso.EntradaTeorica.Value);
                        TimeSpan tiempoNormal = atraso.SalidaTeorica.Value.Subtract(atraso.EntradaTeorica.Value);
                        tabla.AddCell(new PdfPCell(new Phrase(atraso.Fecha.Value.ToString("ddd dd/MM"), Chico)) { HorizontalAlignment = Element.ALIGN_LEFT });
                        tabla.AddCell(new PdfPCell(new Phrase(atraso.Rut.ToStringConGuion(), Chico)));
                        tabla.AddCell(new PdfPCell(new Phrase(atraso.Apellidos, Chico)));
                        tabla.AddCell(new PdfPCell(new Phrase(atraso.Nombres, Chico)));
                        tabla.AddCell(new PdfPCell(new Phrase(atraso.Entrada.HasValue ? atraso.Entrada.Value.ToString("HH:mm") : String.Empty, Chico)));
                        tabla.AddCell(new PdfPCell(new Phrase(atraso.Salida.HasValue ? atraso.Salida.Value.ToString("HH:mm") : String.Empty, Chico)));
                        tabla.AddCell(new PdfPCell(new Phrase(atraso.EntradaTeorica.HasValue ? atraso.EntradaTeorica.Value.ToString("HH:mm") : String.Empty, Chico)));
                        tabla.AddCell(new PdfPCell(new Phrase(atraso.SalidaTeorica.HasValue ? atraso.SalidaTeorica.Value.ToString("HH:mm") : String.Empty, Chico)));
                        tabla.AddCell(new PdfPCell(new Phrase(String.Format("{0}:{1}",
                            Math.Floor(tiempoAtraso.TotalMinutes / 60.0).ToString("00"),
                            (tiempoAtraso.TotalMinutes - Math.Floor(tiempoAtraso.TotalMinutes / 60.0) * 60).ToString("00")
                            ), Chico)));
                        //tabla.AddCell(new PdfPCell(new Phrase("", Chico)));
                        tabla.AddCell(new PdfPCell(new Phrase(atraso.Observacion, Chico)));
                    }
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
            Chico = new Font(bf, 9, Font.NORMAL, BaseColor.BLACK);

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