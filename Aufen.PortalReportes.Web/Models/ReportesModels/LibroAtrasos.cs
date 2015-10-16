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
    public class LibroAtrasos : IArchivoReporte
    {
        private byte[] _Archivo { get; set; }
        private string NombreArchivo { get; set; }
        private Font Titulo { set; get; }
        private Font Normal { set; get; }
        private Font Chico { set; get; }

        public LibroAtrasos(AufenPortalReportesDataContext db, EMPRESA empresa, vw_Ubicacione departamento, DateTime FechaDesde, DateTime FechaHasta, string path, string rut)
        {
             //Nombre del archivo y ubiación en el árbol de carpetas
            NombreArchivo = String.Format("{0}/{1}/LibroAtrasos.pdf", empresa.Descripcion, departamento.Descripcion);
            // Vamos a buscar los datos que nos permitirtán armar elreporte
            IEnumerable<sp_LibroAsistenciaResult> resultadoLibroAtrasos =
                                           db.sp_LibroAsistencia(
                                           FechaDesde.ToString("yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture),
                                           FechaHasta.ToString("yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture),
                                           int.Parse(empresa.Codigo).ToString(),
                                           departamento.Codigo,
                                           rut).ToList(); 
            IEnumerable<LibroAsistenciaDTO> libroAtrasos = Mapper.Map<IEnumerable<sp_LibroAsistenciaResult>,
                IEnumerable<LibroAsistenciaDTO>>(resultadoLibroAtrasos)
            .Where(x =>x.Entrada.HasValue && x.Salida.HasValue && x.SalidaTeorica.HasValue && x.EntradaTeorica.HasValue &&
                x.Salida.Value.Subtract(x.Entrada.Value) < x.SalidaTeorica.Value.Subtract(x.EntradaTeorica.Value));
            // Comenzaremos a crear el reporte
            if (libroAtrasos.Any())
            {
                Configuracion();
                Document doc = new Document(iTextSharp.text.PageSize.LETTER, 10, 10, 50, 35);
                using (var ms = new MemoryStream())
                {
                    PdfWriter pdfWriter = PdfWriter.GetInstance(doc, ms);
                    pdfWriter.PageEvent = new Header(empresa, path); 
                    doc.Open();
                    foreach (var reporte in libroAtrasos.GroupBy(x => new { x.Rut.Numero, x.IdDepartamento, x.IdEmpresa }))
                    {
                        var empleado = db.vw_Empleados.FirstOrDefault(x => x.IdEmpresa == empresa.Codigo &&
                            x.IdUbicacion == reporte.Key.IdDepartamento &&
                                 x.Codigo.HasValue && x.Codigo.Value == reporte.Key.Numero);
                        if (empleado == null)
                        {
                            empleado = new vw_Empleado();
                        }

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
                        doc.Add(new Phrase());

                        PdfPTable informacionPersonal = new PdfPTable(new float[] { 1,5});
                        informacionPersonal.AddCell(new PdfPCell(new Phrase("Rut:", Normal)));
                        informacionPersonal.AddCell(new PdfPCell(new Phrase(new Rut(reporte.Key.Numero).ToStringSinFormato(), Normal)));
                        informacionPersonal.AddCell(new PdfPCell(new Phrase("Nombre:", Normal)));
                        informacionPersonal.AddCell(new PdfPCell(new Phrase(empleado.NombreCompleto, Normal)));
                        doc.Add(new Phrase());

                        doc.Add(informacionPersonal);
                        // tabla
                        PdfPTable tabla = new PdfPTable(new float[] {2, 2, 2, 2, 2, 2, 2, 4 });
                        // Primera lìnea cabecera
                        tabla.AddCell(new PdfPCell(new Phrase("Fecha", Chico)) { Rowspan = 2 });
                        //tabla.AddCell(new PdfPCell(new Phrase("Empleado", Chico)) { Colspan = 3 });
                        tabla.AddCell(new PdfPCell(new Phrase("Horario", Chico)) { Colspan = 2 });
                        tabla.AddCell(new PdfPCell(new Phrase("Marcas", Chico)) { Colspan = 2 });
                        tabla.AddCell(new PdfPCell(new Phrase("Horas Trabajadas", Chico)) { Colspan = 2 });
                        tabla.AddCell(new PdfPCell(new Phrase("Autorizaciones", Chico)));
                        // Segunda lìnea cabecera
                        //tabla.AddCell(new PdfPCell(new Phrase("Rut", Chico)));
                        //tabla.AddCell(new PdfPCell(new Phrase("Apellidos", Chico)));
                        //tabla.AddCell(new PdfPCell(new Phrase("Nombres", Chico)));
                        tabla.AddCell(new PdfPCell(new Phrase("Ing.", Chico)));
                        tabla.AddCell(new PdfPCell(new Phrase("Sal.", Chico)));
                        tabla.AddCell(new PdfPCell(new Phrase("Ing.", Chico)));
                        tabla.AddCell(new PdfPCell(new Phrase("Sal.", Chico)));
                        tabla.AddCell(new PdfPCell(new Phrase("Atrasos", Chico)));
                        tabla.AddCell(new PdfPCell(new Phrase("H.T.N.", Chico)));
                        tabla.AddCell(new PdfPCell(new Phrase("Permisos", Chico)));

                        foreach (var atraso in reporte)
                        {
                            TimeSpan tiempoAtraso =
                                (atraso.Salida.HasValue && atraso.Entrada.HasValue ? atraso.Salida.Value.Subtract(atraso.Entrada.Value) : new TimeSpan(0)) -
                                (atraso.SalidaTeorica.HasValue && atraso.EntradaTeorica.HasValue ? atraso.SalidaTeorica.Value.Subtract(atraso.EntradaTeorica.Value) : new TimeSpan(0));
                            TimeSpan tiempoNormal = atraso.SalidaTeorica.HasValue && atraso.EntradaTeorica.HasValue ? atraso.SalidaTeorica.Value.Subtract(atraso.EntradaTeorica.Value) : new TimeSpan(0);
                            tabla.AddCell(new PdfPCell(new Phrase(atraso.Fecha.Value.ToString("ddd dd/MM"), Chico)) { HorizontalAlignment = Element.ALIGN_LEFT });
                            //tabla.AddCell(new PdfPCell(new Phrase(atraso.Rut.ToStringConGuion(), Chico)));
                            //tabla.AddCell(new PdfPCell(new Phrase(atraso.Apellidos, Chico)));
                            //tabla.AddCell(new PdfPCell(new Phrase(atraso.Nombres, Chico)));
                            tabla.AddCell(new PdfPCell(new Phrase(atraso.Entrada.HasValue ? atraso.Entrada.Value.ToString("HH:mm") : String.Empty, Chico)));
                            tabla.AddCell(new PdfPCell(new Phrase(atraso.Salida.HasValue ? atraso.Salida.Value.ToString("HH:mm") : String.Empty, Chico)));
                            tabla.AddCell(new PdfPCell(new Phrase(atraso.EntradaTeorica.HasValue ? atraso.EntradaTeorica.Value.ToString("HH:mm") : String.Empty, Chico)));
                            tabla.AddCell(new PdfPCell(new Phrase(atraso.SalidaTeorica.HasValue ? atraso.SalidaTeorica.Value.ToString("HH:mm") : String.Empty, Chico)));
                            tabla.AddCell(new PdfPCell(new Phrase(atraso.printAtraso, Chico)));
                            //tabla.AddCell(new PdfPCell(new Phrase(String.Format("{0}:{1}",
                            //    Math.Floor(tiempoAtraso.TotalMinutes / 60.0).ToString("00"),
                            //    (tiempoAtraso.TotalMinutes - Math.Floor(tiempoAtraso.TotalMinutes / 60.0) * 60).ToString("00")
                            //    ), Chico)));
                            tabla.AddCell(new PdfPCell(new Phrase("", Chico)));
                            tabla.AddCell(new PdfPCell(new Phrase(atraso.Observacion, Chico)));
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