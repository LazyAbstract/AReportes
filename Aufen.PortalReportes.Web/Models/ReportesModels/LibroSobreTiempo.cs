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
        private Font NormalNegrita { set; get; }
        private Font Chico { set; get; }

        public LibroSobreTiempo(AufenPortalReportesDataContext db, EMPRESA empresa, vw_Ubicacione departamento, DateTime FechaDesde, DateTime FechaHasta, string path, string rut)
        {
            // Nombre del archivo y ubiación en el árbol de carpetas
            NombreArchivo = String.Format("{0}/{1}/SobreTiempos.pdf", empresa.Descripcion, departamento.Descripcion);
            // Vamos a buscar los datos que nos permitirtán armar elreporte
            IEnumerable<sp_LibroAsistenciaResult> resultadolibroSobretiempo =
                                           db.sp_LibroAsistencia(
                                           FechaDesde.ToString("yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture),
                                           FechaHasta.ToString("yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture),
                                           int.Parse(empresa.Codigo).ToString(),
                                           departamento.Codigo,
                                           rut).ToList(); 
            IEnumerable<LibroAsistenciaDTO> libroSobretiempo = Mapper.Map<IEnumerable<sp_LibroAsistenciaResult>,
                IEnumerable<LibroAsistenciaDTO>>(resultadolibroSobretiempo);
            // Filtramos los casos que nos interesan
            libroSobretiempo = libroSobretiempo
                .Where(x => (x.Entrada.HasValue && x.EntradaTeorica.HasValue && x.Entrada < x.EntradaTeorica) ||
                    (x.Salida.HasValue && x.SalidaTeorica.HasValue && x.Salida > x.SalidaTeorica));

            if (libroSobretiempo.Any())
            {
                Configuracion();
                Document doc = new Document(iTextSharp.text.PageSize.LETTER, 10, 10, 50, 35);
                using (var ms = new MemoryStream())
                {
                    PdfWriter pdfWriter = PdfWriter.GetInstance(doc, ms);
                    pdfWriter.PageEvent = new Header(empresa, path); 
                    doc.Open();
                    foreach (var reporte in libroSobretiempo.Where(x=>x.Rut!=null)
                        .GroupBy(x => new { x.Rut, x.IdDepartamento, x.IdEmpresa }))
                    {
                        var empleado = db.vw_Empleados.FirstOrDefault(x => x.IdEmpresa == empresa.Codigo &&
                            x.IdUbicacion == reporte.Key.IdDepartamento && x.Codigo == reporte.Key.Rut);
                        if (empleado == null)
                        {
                            empleado = new vw_Empleado();
                        }
                        doc.AddAuthor("Aufen");
                        doc.AddCreationDate();
                        doc.AddCreator("Aufen");
                        doc.AddTitle("Libro de Asistencia Personal - Sobretiempos");

                        // Agregamos el título y la bajada del reporte
                        Paragraph parrafo = new Paragraph();
                        parrafo.Add(new Paragraph("Libro de Asistencia Personal", Titulo) { Alignment = Element.ALIGN_CENTER });
                        parrafo.Add(new Paragraph("Sobretiempos", Titulo) { Alignment = Element.ALIGN_CENTER });
                        doc.Add(parrafo);
                        doc.Add(new Phrase());

                        PdfPTable tablaEncabezado = new PdfPTable(new float[] { 1, 5, 1, 5 });

                        tablaEncabezado.AddCell(new PdfPCell(new Phrase("Nombre:", Chico)) { Border = Rectangle.NO_BORDER });
                        tablaEncabezado.AddCell(new PdfPCell(new Phrase(empleado.NombreCompleto, Normal)) { Border = Rectangle.NO_BORDER, Colspan = 3 });
                        //tablaEncabezado.AddCell(new PdfPCell(new Phrase("Código:", Chico)) { Border = Rectangle.NO_BORDER });
                        //tablaEncabezado.AddCell(new PdfPCell(new Phrase(empleado.Codigo, Normal)) { Border = Rectangle.NO_BORDER });

                        tablaEncabezado.AddCell(new PdfPCell(new Phrase("Empresa:", Chico)) { Border = Rectangle.NO_BORDER });
                        tablaEncabezado.AddCell(new PdfPCell(new Phrase((empresa.Descripcion ?? String.Empty).Trim(), Normal)) { Border = Rectangle.NO_BORDER });
                        tablaEncabezado.AddCell(new PdfPCell(new Phrase("Rut:", Chico)) { Border = Rectangle.NO_BORDER });
                        tablaEncabezado.AddCell(new PdfPCell(new Phrase(empleado.RutAufen, Normal)) { Border = Rectangle.NO_BORDER });

                        tablaEncabezado.AddCell(new PdfPCell(new Phrase("Sucursal o Planta:", Chico)) { Border = Rectangle.NO_BORDER });
                        tablaEncabezado.AddCell(new PdfPCell(new Phrase(empleado.SucursalPlanta, Normal)) { Border = Rectangle.NO_BORDER });
                        tablaEncabezado.AddCell(new PdfPCell(new Phrase("Cargo:", Chico)) { Border = Rectangle.NO_BORDER });
                        tablaEncabezado.AddCell(new PdfPCell(new Phrase(String.Empty, Normal)) { Border = Rectangle.NO_BORDER });

                        tablaEncabezado.AddCell(new PdfPCell(new Phrase(String.Format("PERIODO: {0} a {1}", FechaDesde.ToShortDateString(), FechaHasta.ToShortDateString()), Normal)) { Colspan = 4, Border = Rectangle.NO_BORDER });
                        tablaEncabezado.AddCell(new PdfPCell(GetNomenclatura()) { Colspan = 4 });
                        doc.Add(tablaEncabezado);
                        doc.Add(new Phrase());
                        // tabla
                        PdfPTable tabla = new PdfPTable(new float[] { 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 4 });
                        tabla.AddCell(new PdfPCell(new Phrase("Fecha", Chico)));
                        tabla.AddCell(new PdfPCell(new Phrase("HI", Chico)));
                        tabla.AddCell(new PdfPCell(new Phrase("HS", Chico)));
                        tabla.AddCell(new PdfPCell(new Phrase("HCol", Chico)));
                        tabla.AddCell(new PdfPCell(new Phrase("MI", Chico)));
                        tabla.AddCell(new PdfPCell(new Phrase("MS", Chico)));
                        tabla.AddCell(new PdfPCell(new Phrase("HTH", Chico)));
                        tabla.AddCell(new PdfPCell(new Phrase("HTN", Chico)));                       
                        tabla.AddCell(new PdfPCell(new Phrase("Sob.Ingreso", Chico)));
                        tabla.AddCell(new PdfPCell(new Phrase("Sob.Salida", Chico)));
                        tabla.AddCell(new PdfPCell(new Phrase("P.Todo", Chico)));
                        tabla.AddCell(new PdfPCell(new Phrase("Cantidad", Chico)));
                        tabla.AddCell(new PdfPCell(new Phrase("Motivo", Chico)));
                        // Filas con datos
                        foreach (var sobretiempo in reporte)
                        {
                            //Fecha
                            tabla.AddCell(new PdfPCell(new Phrase(sobretiempo.Fecha.Value.ToString("ddd dd/MM"), Chico)));
                            //Hora Ingreso
                            tabla.AddCell(new PdfPCell(new Phrase(sobretiempo.EntradaTeorica.GetValueOrDefault(new DateTime()).ToString("HH:mm"), Chico)));
                            //Hora Salida
                            tabla.AddCell(new PdfPCell(new Phrase(sobretiempo.SalidaTeorica.GetValueOrDefault(new DateTime()).ToString("HH:mm"), Chico)));
                            //Hora C0lación
                            tabla.AddCell(new PdfPCell(new Phrase(new DateTime(sobretiempo.TiempoColacion.GetValueOrDefault(new TimeSpan()).Ticks).ToString("HH:mm"), Chico)));
                            //Marca Ingreso
                            tabla.AddCell(new PdfPCell(new Phrase(sobretiempo.Entrada.GetValueOrDefault(new DateTime()).ToString("HH:mm"), Chico)));
                            //Marca Salida
                            tabla.AddCell(new PdfPCell(new Phrase(sobretiempo.Salida.GetValueOrDefault(new DateTime()).ToString("HH:mm"), Chico)));
                            //Hora pactada por horario
                            tabla.AddCell(new PdfPCell(new Phrase(sobretiempo.printHorasPactadas, Chico)));
                            //horas realizadas
                            tabla.AddCell(new PdfPCell(new Phrase(sobretiempo.printHorasReales, Chico)));
                            //Sobrante Ingreso
                            tabla.AddCell(new PdfPCell(new Phrase(sobretiempo.printSobreEntrada, Chico)));
                            //Sobrante Salida
                            tabla.AddCell(new PdfPCell(new Phrase(sobretiempo.printSobreSalida, Chico)));
                            //P.Todo
                            tabla.AddCell(new PdfPCell(new Phrase(sobretiempo.printHorasExtra, Chico)));
                            //Cantidad
                            tabla.AddCell(new PdfPCell(new Phrase("", Chico)));
                            //Motivo
                            tabla.AddCell(new PdfPCell(new Phrase(sobretiempo.Observacion, Chico)));
                        }


                        tabla.AddCell(new PdfPCell(new Phrase(" ", Chico)) { Colspan = 8});
                        // Total SobreSalida
                        tabla.AddCell(new PdfPCell(new Phrase(reporte.CalculaSobreEntrada(), Chico)) { Colspan = 1 });
                        // Total Sobre Entrada
                        tabla.AddCell(new PdfPCell(new Phrase(reporte.CalculaSobreSalida(), Chico)) { Colspan = 1 });
                        // Total Horas Extra
                        tabla.AddCell(new PdfPCell(new Phrase(reporte.CalculaHorasExtra(), Chico)) { Colspan = 1 });

                        tabla.AddCell(new PdfPCell(new Phrase(" ", Normal)) { Colspan = 2, Border = Rectangle.NO_BORDER });
                        tabla.AddCell(new PdfPCell(new Phrase(" ", Normal)) { Colspan = 13, Border = Rectangle.NO_BORDER });
                        tabla.AddCell(new PdfPCell(GetFirma()) { Colspan=13});
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
            Chico = new Font(bf, 8, Font.NORMAL, BaseColor.BLACK);
            NormalNegrita = new Font(bf, 11, Font.BOLD, BaseColor.BLACK);
        }

        private PdfPTable GetNomenclatura()
        {
            PdfPTable tabla = new PdfPTable(new float[] { 1, 1, 1 });

            tabla.AddCell(new PdfPCell(new Phrase("Nomenclatura Básica", NormalNegrita)) { Colspan = 6, Border = Rectangle.NO_BORDER });

            tabla.AddCell(new PdfPCell(new Phrase("HI: Hora de Ingreso", Chico)) { Border = Rectangle.NO_BORDER });
            tabla.AddCell(new PdfPCell(new Phrase("HCol: Colación por turno", Chico)) { Border = Rectangle.NO_BORDER });
            tabla.AddCell(new PdfPCell(new Phrase("50%: Horas Extras 50", Chico)) { Border = Rectangle.NO_BORDER });


            tabla.AddCell(new PdfPCell(new Phrase("HS: Hora de salida", Chico)) { Border = Rectangle.NO_BORDER });
            tabla.AddCell(new PdfPCell(new Phrase("MI: Marca de Entrada", Chico)) { Border = Rectangle.NO_BORDER });
            tabla.AddCell(new PdfPCell(new Phrase("100%: Horas Extras 100", Chico)) { Border = Rectangle.NO_BORDER });


            tabla.AddCell(new PdfPCell(new Phrase("MS: Marca de salida", Chico)) { Border = Rectangle.NO_BORDER });
            tabla.AddCell(new PdfPCell(new Phrase("HTN: Horas realizadas", Chico)) { Border = Rectangle.NO_BORDER });
            tabla.AddCell(new PdfPCell(new Phrase("HTH: Pactadas por hombre", Chico)) { Border = Rectangle.NO_BORDER });

            tabla.AddCell(new PdfPCell(new Phrase("ATR: Atraso", Chico)) { Border = Rectangle.NO_BORDER });
            tabla.AddCell(new PdfPCell(new Phrase("ADL: Salida Adelantada", Chico)) { Border = Rectangle.NO_BORDER });
            tabla.AddCell(new PdfPCell(new Phrase("", Chico)) { Border = Rectangle.NO_BORDER });
            return tabla;
        }

        private PdfPTable GetFirma()
        {
            PdfPTable tabla = new PdfPTable(new float[] { 1, 1, 1 });

            tabla.AddCell(new PdfPCell(new Phrase("Observaciones:", Normal)) { Colspan = 3, Border = Rectangle.BOTTOM_BORDER });
            tabla.AddCell(new PdfPCell(new Phrase(" ", Normal)) { Colspan = 3, Border = Rectangle.BOTTOM_BORDER });
            tabla.AddCell(new PdfPCell(new Phrase(" ", Normal)) { Colspan = 3, Border = Rectangle.BOTTOM_BORDER });
            tabla.AddCell(new PdfPCell(new Phrase(" ", Normal)) { Colspan = 3, Border = Rectangle.NO_BORDER });
            tabla.AddCell(new PdfPCell(new Phrase(" ", Normal)) { Colspan = 3, Border = Rectangle.NO_BORDER });
            tabla.AddCell(new PdfPCell(new Phrase(" ", Normal)) { Colspan = 3, Border = Rectangle.NO_BORDER });
            tabla.AddCell(new PdfPCell(new Phrase(" ", Normal)) { Colspan = 3, Border = Rectangle.NO_BORDER });

            tabla.AddCell(new PdfPCell(new Phrase("Firma Empleado", Chico)) { Border = Rectangle.TOP_BORDER , HorizontalAlignment=Rectangle.ALIGN_CENTER});
            tabla.AddCell(new PdfPCell(new Phrase("V°B° Jefatura ", Chico)) { Border = Rectangle.TOP_BORDER, HorizontalAlignment = Rectangle.ALIGN_CENTER });
            tabla.AddCell(new PdfPCell(new Phrase("Autorización", Chico)) { Border = Rectangle.TOP_BORDER, HorizontalAlignment = Rectangle.ALIGN_CENTER });
            return tabla;
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