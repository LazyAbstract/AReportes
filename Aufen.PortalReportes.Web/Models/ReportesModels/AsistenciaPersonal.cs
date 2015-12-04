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
    public class AsistenciaPersonal : IArchivoReporte
    {
        private byte[] _Archivo { get; set; }
        private string NombreArchivo { get; set; }
        private Font Titulo { set; get; }
        private Font Normal { set; get; }
        private Font NormalNegrita { set; get; }
        private Font ChicoNegrita { set; get; }
        private Font Chico { set; get; }
        private Font MuyChico { set; get; }

        public AsistenciaPersonal(AufenPortalReportesDataContext db, EMPRESA empresa, vw_Ubicacione departamento, DateTime FechaDesde, DateTime FechaHasta, string path, string rut)
        {
            //Nombre del archivo y ubiación en el árbol de carpetas
            NombreArchivo = String.Format("{0}/{1}/AsistenciaPersonal.pdf", empresa.Descripcion, departamento.Descripcion);
            // Vamos a buscar los datos que nos permitirtán armar elreporte
            IEnumerable<sp_LibroAsistenciaResult> resultado =
                                           db.sp_LibroAsistencia(
                                           FechaDesde.ToString("yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture),
                                           FechaHasta.ToString("yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture),
                                           int.Parse(empresa.Codigo).ToString(),
                                           departamento.Codigo,
                                           rut).ToList();
            IEnumerable<LibroAsistenciaDTO> libroAsistencia = Mapper.Map<IEnumerable<sp_LibroAsistenciaResult>,
                IEnumerable<LibroAsistenciaDTO>>(resultado);
            if (libroAsistencia.Any())
            {
                Configuracion();
                Document doc = new Document(iTextSharp.text.PageSize.LETTER, 10, 10, 50, 35);
                using (var ms = new MemoryStream())
                {
                    PdfWriter pdfWriter = PdfWriter.GetInstance(doc, ms);
                    pdfWriter.PageEvent = new Header(empresa, path);
                    doc.Open();
                    foreach (var reporte in libroAsistencia.Where(x => x.Rut != null).GroupBy(x => new { x.Rut, x.IdDepartamento, x.IdEmpresa }))
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
                        doc.AddTitle("Libro de Asistencia Personal");

                        Paragraph parrafo = new Paragraph();
                        parrafo.Add(new Paragraph("Asistencia Personal", Titulo) { Alignment = Element.ALIGN_CENTER });
                        doc.Add(parrafo);
                        // Texto 
                        PdfPTable tablaEncabezado = new PdfPTable(new float[] { 1, 5, 1, 5 });

                        tablaEncabezado.AddCell(new PdfPCell(new Phrase("Nombre:", Chico)) { Border = Rectangle.NO_BORDER });
                        tablaEncabezado.AddCell(new PdfPCell(new Phrase(empleado.NombreCompleto, Normal)) { Border = Rectangle.NO_BORDER, Colspan=3 });


                        tablaEncabezado.AddCell(new PdfPCell(new Phrase("Empresa:", Chico)) { Border = Rectangle.NO_BORDER });
                        tablaEncabezado.AddCell(new PdfPCell(new Phrase(empresa != null ? empresa.Descripcion.Trim() : String.Empty, Normal)) { Border = Rectangle.NO_BORDER });
                        tablaEncabezado.AddCell(new PdfPCell(new Phrase("Rut:", Chico)) { Border = Rectangle.NO_BORDER });
                        tablaEncabezado.AddCell(new PdfPCell(new Phrase(empleado.RutAufen, Normal)) { Border = Rectangle.NO_BORDER });

                        tablaEncabezado.AddCell(new PdfPCell(new Phrase("Sucursal o Planta:", Chico)) { Border = Rectangle.NO_BORDER });
                        tablaEncabezado.AddCell(new PdfPCell(new Phrase(empleado.SucursalPlanta, Normal)) { Border = Rectangle.NO_BORDER });
                        tablaEncabezado.AddCell(new PdfPCell(new Phrase("Cargo:", Chico)) { Border = Rectangle.NO_BORDER });
                        //tablaEncabezado.AddCell(new PdfPCell(new Phrase(empleado.Cargo, Normal)) { Border = Rectangle.NO_BORDER });
                        tablaEncabezado.AddCell(new PdfPCell(new Phrase(String.Empty, Normal)) { Border = Rectangle.NO_BORDER });

                        tablaEncabezado.AddCell(new PdfPCell(new Phrase(String.Format("PERIODO: {0} a {1}", FechaDesde.ToShortDateString(), FechaHasta.ToShortDateString()), Normal)) { Colspan = 4, Border = Rectangle.NO_BORDER });
                        doc.Add(new Paragraph(" "));
                        tablaEncabezado.AddCell(new PdfPCell(GetNomenclatura()) { Colspan = 4 });
                        doc.Add(tablaEncabezado);
                        doc.Add(new Phrase());
                        doc.Add(new Phrase());
                        // tabla
                        PdfPTable tabla = new PdfPTable(new float[] { 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 4 });

                        // Primera lìnea cabecera
                        tabla.AddCell(new PdfPCell(new Phrase("Fecha", Chico)));
                        tabla.AddCell(new PdfPCell(new Phrase("HI", Chico)) { });
                        tabla.AddCell(new PdfPCell(new Phrase("HS", Chico)));
                        tabla.AddCell(new PdfPCell(new Phrase("HCol", Chico)));
                        tabla.AddCell(new PdfPCell(new Phrase("MI", Chico)));
                        tabla.AddCell(new PdfPCell(new Phrase("MS", Chico)));
                        tabla.AddCell(new PdfPCell(new Phrase("HTH", Chico)));
                        tabla.AddCell(new PdfPCell(new Phrase("HTN", Chico)));
                        tabla.AddCell(new PdfPCell(new Phrase("HE", Chico)));
                        tabla.AddCell(new PdfPCell(new Phrase("ATR", Chico)));
                        tabla.AddCell(new PdfPCell(new Phrase("ADL", Chico)));
                        tabla.AddCell(new PdfPCell(new Phrase("Col.", Chico)));
                        tabla.AddCell(new PdfPCell(new Phrase("S.Ent", Chico)));
                        tabla.AddCell(new PdfPCell(new Phrase("Permisos", Chico)));

                        foreach (var atraso in reporte)
                        {
                            //Fecha
                            tabla.AddCell(new PdfPCell(new Phrase(atraso.Fecha.Value.ToString("ddd dd/MM"), Chico)));
                            //Hora Ingreso
                            tabla.AddCell(new PdfPCell(new Phrase(atraso.EntradaTeorica.HasValue ? atraso.EntradaTeorica.Value.ToShortTimeString() : null, Chico)));
                            // Hora Salida
                            tabla.AddCell(new PdfPCell(new Phrase(atraso.SalidaTeorica.HasValue ? atraso.SalidaTeorica.Value.ToShortTimeString() : null, Chico)));
                            // Colaciòn por turno
                            tabla.AddCell(new PdfPCell(new Phrase((int)atraso.TiempoColacion.Value.TotalHours + atraso.TiempoColacion.Value.ToString(@"\:mm"), Chico)));
                            // Marca de entrada
                            tabla.AddCell(new PdfPCell(new Phrase(atraso.Entrada.HasValue ? atraso.Entrada.Value.ToShortTimeString() : null, Chico)));
                            // Marca de salida
                            tabla.AddCell(new PdfPCell(new Phrase(atraso.Salida.HasValue ? atraso.Salida.Value.ToShortTimeString() : null, Chico)));
                            //Horas pactadas po hombre
                            tabla.AddCell(new PdfPCell(new Phrase(atraso.printHorasPactadas, Chico)));
                            //Horas realizadas
                            tabla.AddCell(new PdfPCell(new Phrase(atraso.printHorasReales.ToString(), Chico)));
                            //Horas extra
                            tabla.AddCell(new PdfPCell(new Phrase(atraso.printHorasExtra, Chico)));
                            // Atraso
                            tabla.AddCell(new PdfPCell(new Phrase(atraso.printAtraso, Chico)));
                            // Salida adelantada
                            tabla.AddCell(new PdfPCell(new Phrase(atraso.printSalidaAdelantada, Chico)));
                            // Colación
                            tabla.AddCell(new PdfPCell(new Phrase(atraso.printTiempoColacionReal, Chico)));
                            // S.Ent
                            tabla.AddCell(new PdfPCell(new Phrase(atraso.printSobreEntrada, Chico)));
                            // Permisos
                            tabla.AddCell(new PdfPCell(new Phrase(atraso.Observacion, Chico)));
                        }
                        //Subtotal
                        tabla.AddCell(new PdfPCell(new Phrase("Sub Total", Chico)) { Colspan = 6 });
                        //Horas pactadas po hombre
                        tabla.AddCell(new PdfPCell(new Phrase(LibroAsistenciaDTOHelpers.CalculaJornada(reporte), Chico)));
                        //Horas realizadas
                        tabla.AddCell(new PdfPCell(new Phrase(LibroAsistenciaDTOHelpers.CalculaAsistencia(reporte), Chico)));
                        //Horas extra
                        tabla.AddCell(new PdfPCell(new Phrase(LibroAsistenciaDTOHelpers.CalculaHorasExtra(reporte), Chico)));
                        // Atraso
                        tabla.AddCell(new PdfPCell(new Phrase(LibroAsistenciaDTOHelpers.CalculaAtrasoEntrada(reporte), Chico)));
                        // Salida adelantada
                        tabla.AddCell(new PdfPCell(new Phrase(LibroAsistenciaDTOHelpers.CalculaSalidaAdelantada(reporte), Chico)));
                        // Colacón
                        tabla.AddCell(new PdfPCell(new Phrase(LibroAsistenciaDTOHelpers.CalculaColacion(reporte), Chico)));
                        // S.Ent
                        tabla.AddCell(new PdfPCell(new Phrase(LibroAsistenciaDTOHelpers.CalculaSobreEntrada(reporte), Chico)));
                        // Permisos
                        tabla.AddCell(new PdfPCell(new Phrase("", Chico)));
                        doc.Add(tabla);
                        doc.Add(new Phrase());

                        // Resumen
                        PdfPTable resumen = new PdfPTable(new float[] { 2, 1, 2, 1, 2, 1, 2, 1 });
                        //Días Trabajados
                        resumen.AddCell(new PdfPCell(new Phrase("Días Trabajados", Chico)) { Border = Rectangle.NO_BORDER });
                        resumen.AddCell(new PdfPCell(new Phrase(LibroAsistenciaDTOHelpers.CalculaDiasTrabajdos(reporte), Chico)) { Border = Rectangle.NO_BORDER });
                        //
                        resumen.AddCell(new PdfPCell(new Phrase("Horas Pactadas", Chico)) { Border = Rectangle.NO_BORDER });
                        resumen.AddCell(new PdfPCell(new Phrase(LibroAsistenciaDTOHelpers.CalculaAsistencia(reporte), Chico)) { Border = Rectangle.NO_BORDER });
                        //
                        resumen.AddCell(new PdfPCell(new Phrase("Horas extras", Chico)) { Border = Rectangle.NO_BORDER });
                        resumen.AddCell(new PdfPCell(new Phrase(LibroAsistenciaDTOHelpers.CalculaHorasExtra(reporte), Chico)) { Border = Rectangle.NO_BORDER });
                        //
                        resumen.AddCell(new PdfPCell(new Phrase("Errores de marca", Chico)) { Border = Rectangle.NO_BORDER });
                        resumen.AddCell(new PdfPCell(new Phrase(LibroAsistenciaDTOHelpers.CalculaErroresMarcaje(reporte), Chico)) { Border = Rectangle.NO_BORDER });
                        //
                        resumen.AddCell(new PdfPCell(new Phrase("Días ausentes", Chico)) { Border = Rectangle.NO_BORDER });
                        resumen.AddCell(new PdfPCell(new Phrase(LibroAsistenciaDTOHelpers.CalculaDiasInasistencias(reporte), Chico)) { Border = Rectangle.NO_BORDER });
                        //
                        resumen.AddCell(new PdfPCell(new Phrase("Horas Trabajadas", Chico)) { Border = Rectangle.NO_BORDER });
                        resumen.AddCell(new PdfPCell(new Phrase(LibroAsistenciaDTOHelpers.CalculaAsistencia(reporte), Chico)) { Border = Rectangle.NO_BORDER });
                        //
                        resumen.AddCell(new PdfPCell(new Phrase("Vacaciones", Chico)) { Border = Rectangle.NO_BORDER });
                        resumen.AddCell(new PdfPCell(new Phrase("", Chico)) { Border = Rectangle.NO_BORDER });
             
                        // Aquí va una tabla con el espacio para la firma
                        resumen.AddCell(new PdfPCell(new Phrase("", Chico)) { VerticalAlignment = Rectangle.ALIGN_BOTTOM, HorizontalAlignment = Rectangle.ALIGN_CENTER, Colspan = 2, Rowspan = 3, Border = Rectangle.NO_BORDER });
                        //
                        resumen.AddCell(new PdfPCell(new Phrase("Días Atraso", Chico)) { Border = Rectangle.NO_BORDER });
                        resumen.AddCell(new PdfPCell(new Phrase(LibroAsistenciaDTOHelpers.CalculaDiasAtraso(reporte), Chico)) { Border = Rectangle.NO_BORDER });
                        //
                        resumen.AddCell(new PdfPCell(new Phrase("Total Atraso", Chico)) { Border = Rectangle.NO_BORDER });
                        resumen.AddCell(new PdfPCell(new Phrase(LibroAsistenciaDTOHelpers.CalculaAtrasoEntrada(reporte), Chico)) { Border = Rectangle.NO_BORDER });
                        //
                        resumen.AddCell(new PdfPCell(new Phrase("Licencias Médicas", Chico)) { Border = Rectangle.NO_BORDER });
                        resumen.AddCell(new PdfPCell(new Phrase("", Chico)) { Border = Rectangle.NO_BORDER });
                        //
                        resumen.AddCell(new PdfPCell(new Phrase("Salidas", Chico)) { Border = Rectangle.NO_BORDER });
                        resumen.AddCell(new PdfPCell(new Phrase(LibroAsistenciaDTOHelpers.CalculaDiasSalidaAdelantada(reporte), Chico)) { Border = Rectangle.NO_BORDER });
                        //
                        resumen.AddCell(new PdfPCell(new Phrase("Total Salidas", Chico)) { Border = Rectangle.NO_BORDER });
                        resumen.AddCell(new PdfPCell(new Phrase(LibroAsistenciaDTOHelpers.CalculaSalidaAdelantada(reporte), Chico)) { Border = Rectangle.NO_BORDER });

                        resumen.AddCell(new PdfPCell(new Phrase("", Chico)) { Border = Rectangle.NO_BORDER });
                        resumen.AddCell(new PdfPCell(new Phrase("", Chico)) { Border = Rectangle.NO_BORDER });

                        resumen.AddCell(new PdfPCell(new Phrase(" ", Chico)) { Border = Rectangle.NO_BORDER, Colspan = 8, Rowspan = 5 });
                        resumen.AddCell(new PdfPCell(new Phrase(" ", Chico)) { Border = Rectangle.NO_BORDER, Colspan = 8 });

                        resumen.AddCell(new PdfPCell(new Phrase("", Chico)) { Border = Rectangle.NO_BORDER, Colspan = 6 });
                        resumen.AddCell(new PdfPCell(new Phrase("______________________ Firma Empleado", Chico)) { VerticalAlignment = Rectangle.ALIGN_BOTTOM, HorizontalAlignment = Rectangle.ALIGN_CENTER, Colspan = 2, Border = Rectangle.NO_BORDER });

                        doc.Add(resumen);

                        //PdfPTable firma = new PdfPTable(new float[] { 2, 1, 2, 1, 2, 1, 2, 1 });

                        //firma.AddCell(new PdfPCell(new Phrase(" ", Chico)) { Border = Rectangle.NO_BORDER, Colspan = 8, Rowspan = 2 });
                        ////firma.AddCell(new PdfPCell(new Phrase(" ", Chico)) { Border = Rectangle.NO_BORDER, Colspan = 8 });
                       

                        //firma.AddCell(new PdfPCell(new Phrase("", Chico)) { Border = Rectangle.NO_BORDER, Colspan = 6 });
                        //firma.AddCell(new PdfPCell(new Phrase("______________________ Firma Empleado", Chico)) { VerticalAlignment = Rectangle.ALIGN_BOTTOM, HorizontalAlignment = Rectangle.ALIGN_CENTER, Colspan = 2, Border = Rectangle.NO_BORDER, FixedHeight = 1.7f });
                        //doc.Add(new Phrase());
                        //doc.Add(new Phrase());
                        //doc.Add(new Phrase());
                        //doc.Add(new Phrase());
                        //doc.Add(firma);

                        doc.NewPage();
                    }
                    doc.Close();
                    _Archivo = ms.ToArray();
                }
            }
        }

        private PdfPTable GetNomenclatura()
        {
            PdfPTable tabla = new PdfPTable(new float[] { 1,1,1});

            tabla.AddCell(new PdfPCell(new Phrase("Nomenclatura Básica", NormalNegrita)) { Colspan = 6, Border = Rectangle.NO_BORDER });

            tabla.AddCell(new PdfPCell(new Phrase("HI: Hora de Ingreso", Chico)) { Border = Rectangle.NO_BORDER });
            tabla.AddCell(new PdfPCell(new Phrase("HCol: Colación por turno", Chico)) { Border = Rectangle.NO_BORDER });
            tabla.AddCell(new PdfPCell(new Phrase("ATR: Atraso", Chico)) { Border = Rectangle.NO_BORDER });

            tabla.AddCell(new PdfPCell(new Phrase("HS: Hora de salida", Chico)) { Border = Rectangle.NO_BORDER });
            tabla.AddCell(new PdfPCell(new Phrase("MI: Marca de Entrada", Chico)) { Border = Rectangle.NO_BORDER });
            tabla.AddCell(new PdfPCell(new Phrase("ADL: Salida Adelantada", Chico)) { Border = Rectangle.NO_BORDER });

            tabla.AddCell(new PdfPCell(new Phrase("MS: Marca de salida", Chico)) { Border = Rectangle.NO_BORDER });
            tabla.AddCell(new PdfPCell(new Phrase("HTN: Horas realizadas", Chico)) { Border = Rectangle.NO_BORDER });
            tabla.AddCell(new PdfPCell(new Phrase("HTH: Pactadas por hombre", Chico)) { Border = Rectangle.NO_BORDER });
            return tabla;
        }

        private void Configuracion()
        {
            BaseFont bf = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, false);
            Titulo = new Font(bf, 18, Font.UNDERLINE, BaseColor.BLACK);
            Normal = new Font(bf, 11, Font.NORMAL, BaseColor.BLACK);
            NormalNegrita = new Font(bf, 11, Font.BOLD, BaseColor.BLACK);
            Chico = new Font(bf, 9, Font.NORMAL, BaseColor.BLACK);
            ChicoNegrita = new Font(bf, 9, Font.BOLD, BaseColor.BLACK);
            MuyChico = new Font(bf, 7, Font.NORMAL, BaseColor.BLACK);

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