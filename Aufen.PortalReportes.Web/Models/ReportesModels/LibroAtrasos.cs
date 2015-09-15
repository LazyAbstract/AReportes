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

        public LibroAtrasos(IEnumerable<sp_LibroAtrasosResultDTO> resultado, AufenPortalReportesDataContext db)
        {
            Configuracion();
            libroAtrasos = resultado;//.Where(x => x.Salida.Subtract(x.Entrada) < x.SalidaTeorica.Subtract(x.EntradaTeorica));
            Document doc = new Document(iTextSharp.text.PageSize.LETTER, 10, 10, 42, 35);
            using (var ms = new MemoryStream())
            {
                PdfWriter pdfWriter = PdfWriter.GetInstance(doc, ms);
                doc.Open();
                foreach (var reporte in libroAtrasos.GroupBy(x => new { x.Rut.Numero, x.IdDepartamento, x.IdEmpresa }))
                {
                    var departamento = db.vw_Ubicaciones.SingleOrDefault(x =>
                        x.Codigo == reporte.Key.IdDepartamento &&
                        x.IdEmpresa == reporte.Key.IdEmpresa);
                    doc.AddAuthor("Aufen");
                    doc.AddCreationDate();
                    doc.AddCreator("Aufen");
                    doc.AddTitle("Libro de Atrasos");

                    // Texto 
                    doc.Add(new Paragraph(new Chunk(
                        String.Format("Departamento: {1}, Fecha de Reporte: {0}",
                        DateTime.Now.ToShortDateString(),
                        departamento != null ? departamento.Descripcion : "Sin Información"), Normal)));
                    doc.Add(new Paragraph(new Chunk("Informe de Atrasos por Área",Titulo)) { Alignment = Element.ALIGN_CENTER });
                    // tabla
                    PdfPTable tabla = new PdfPTable(new float[] { 3, 3, 3, 1, 1, 1, 1, 1, 1, 4 });
                    // Primera lìnea cabecera
                    tabla.AddCell(new PdfPCell(new Phrase("Empleado")) { Colspan = 3 });
                    tabla.AddCell(new PdfPCell(new Phrase("Horario")) { Colspan = 2 });
                    tabla.AddCell(new PdfPCell(new Phrase("Marcas")) { Colspan = 2 });
                    tabla.AddCell(new PdfPCell(new Phrase("Horas Trabajadas")) { Colspan = 2 });
                    tabla.AddCell(new PdfPCell(new Phrase("Autorizaciones")));
                    // Segunda lìnea cabecera
                    tabla.AddCell(new PdfPCell(new Phrase("Rut")));
                    tabla.AddCell(new PdfPCell(new Phrase("Apellidos")));
                    tabla.AddCell(new PdfPCell(new Phrase("Nombres")));
                    tabla.AddCell(new PdfPCell(new Phrase("Ing.")));
                    tabla.AddCell(new PdfPCell(new Phrase("Sal.")));
                    tabla.AddCell(new PdfPCell(new Phrase("Ing.")));
                    tabla.AddCell(new PdfPCell(new Phrase("Sal.")));
                    tabla.AddCell(new PdfPCell(new Phrase("Atrasos")));
                    tabla.AddCell(new PdfPCell(new Phrase("H.T.N.")));
                    tabla.AddCell(new PdfPCell(new Phrase("Permisos")));

                    foreach (var atraso in reporte)
                    {
                        TimeSpan tiempoAtraso = atraso.Salida.Subtract(atraso.Entrada) - atraso.SalidaTeorica.Subtract(atraso.EntradaTeorica);
                        TimeSpan tiempoNormal = atraso.SalidaTeorica.Subtract(atraso.EntradaTeorica);
                        tabla.AddCell(new PdfPCell(new Phrase(atraso.Fecha.Value.ToString("ddd dd/MM"))) { Colspan = 10, HorizontalAlignment = Element.ALIGN_LEFT });
                        tabla.AddCell(new PdfPCell(new Phrase(atraso.Rut.ToStringConGuion())));
                        tabla.AddCell(new PdfPCell(new Phrase(atraso.Apellidos)));
                        tabla.AddCell(new PdfPCell(new Phrase(atraso.Nombres)));
                        tabla.AddCell(new PdfPCell(new Phrase(atraso.Entrada.ToString("HH:mm"))));
                        tabla.AddCell(new PdfPCell(new Phrase(atraso.Salida.ToString("HH:mm"))));
                        tabla.AddCell(new PdfPCell(new Phrase(atraso.EntradaTeorica.ToString("HH:mm"))));
                        tabla.AddCell(new PdfPCell(new Phrase(atraso.SalidaTeorica.ToString("HH:mm"))));
                        tabla.AddCell(new PdfPCell(new Phrase(String.Format("{0}:{1}",
                            Math.Floor(tiempoAtraso.TotalMinutes / 60.0).ToString("00"),
                            (tiempoAtraso.TotalMinutes - Math.Floor(tiempoAtraso.TotalMinutes / 60.0) * 60).ToString("00")
                            ))));
                        tabla.AddCell(new PdfPCell(new Phrase("")));
                        tabla.AddCell(new PdfPCell(new Phrase(atraso.Observacion)));
                    }
                   doc.Add(tabla);
                }
                doc.Close();
                _Archivo = ms.ToArray();
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
    }
}