﻿using Aufen.PortalReportes.Core;
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
        private Font Chico { set; get; }
        private Font MuyChico { set; get; }

        public AsistenciaPersonal(AufenPortalReportesDataContext db, EMPRESA empresa, vw_Ubicacione departamento, DateTime FechaDesde, DateTime FechaHasta)
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
                                           null).ToList();
            IEnumerable<LibroAsistenciaDTO> libroAsistencia = Mapper.Map<IEnumerable<sp_LibroAsistenciaResult>,
                IEnumerable<LibroAsistenciaDTO>>(resultado);
            if (libroAsistencia.Any())
            {
                Configuracion();
                Document doc = new Document(iTextSharp.text.PageSize.LETTER, 10, 10, 42, 35);
                using (var ms = new MemoryStream())
                {
                    PdfWriter pdfWriter = PdfWriter.GetInstance(doc, ms);
                    doc.Open();
                    foreach (var reporte in libroAsistencia.Where(x => x.Rut != null).GroupBy(x => new { x.Rut.Numero, x.IdDepartamento, x.IdEmpresa }))
                    {
                        var empleado = db.vw_Empleados.FirstOrDefault(x => x.IdEmpresa == reporte.Key.IdEmpresa &&
                            x.IdUbicacion == reporte.Key.IdDepartamento);
                        if (empleado == null)
                        {
                            empleado = new vw_Empleado();
                        }
                        doc.AddAuthor("Aufen");
                        doc.AddCreationDate();
                        doc.AddCreator("Aufen");
                        doc.AddTitle("Libro de Asistencia Personal");

                        Paragraph parrafo = new Paragraph();
                        parrafo.Add(new Paragraph("Libro de Asistencia Personal", Titulo) { Alignment = Element.ALIGN_CENTER });
                        // Texto 
                        PdfPTable tablaEncabezado = new PdfPTable(new float[] { 1, 5, 1, 5 });

                        tablaEncabezado.AddCell(new PdfPCell(new Phrase("Nombre:", Chico)) { Border = Rectangle.NO_BORDER });
                        tablaEncabezado.AddCell(new PdfPCell(new Phrase((empleado.Nombre ?? String.Empty).Trim() + " " + (empleado.Apellidos ?? String.Empty).Trim(), Normal)) { Border = Rectangle.NO_BORDER });
                        tablaEncabezado.AddCell(new PdfPCell(new Phrase("Código:", Chico)) { Border = Rectangle.NO_BORDER });
                        tablaEncabezado.AddCell(new PdfPCell(new Phrase(empleado.Codigo, Normal)) { Border = Rectangle.NO_BORDER });

                        tablaEncabezado.AddCell(new PdfPCell(new Phrase("C.Costo:", Chico)) { Border = Rectangle.NO_BORDER });
                        tablaEncabezado.AddCell(new PdfPCell(new Phrase(empleado.NombreCentro, Normal)) { Border = Rectangle.NO_BORDER });
                        tablaEncabezado.AddCell(new PdfPCell(new Phrase("Rut:", Chico)) { Border = Rectangle.NO_BORDER });
                        tablaEncabezado.AddCell(new PdfPCell(new Phrase(new Rut(reporte.Key.Numero).ToStringConGuion(), Normal)) { Border = Rectangle.NO_BORDER });

                        tablaEncabezado.AddCell(new PdfPCell(new Phrase("Area:", Chico)) { Border = Rectangle.NO_BORDER });
                        tablaEncabezado.AddCell(new PdfPCell(new Phrase(empleado.NombreUbicacion, Normal)) { Border = Rectangle.NO_BORDER });
                        tablaEncabezado.AddCell(new PdfPCell(new Phrase("Cargo:", Chico)) { Border = Rectangle.NO_BORDER });
                        tablaEncabezado.AddCell(new PdfPCell(new Phrase(String.Empty, Normal)) { Border = Rectangle.NO_BORDER });

                        tablaEncabezado.AddCell(new PdfPCell(new Phrase(String.Format("PERIODO: {0} a {1}", FechaDesde.ToShortDateString(), FechaHasta.ToShortDateString()), Normal)) { Colspan = 4, Border = Rectangle.NO_BORDER });
                        doc.Add(new Paragraph(" "));
                        tablaEncabezado.AddCell(new PdfPCell(GetNomenclatura()) { Colspan = 4 });
                        doc.Add(tablaEncabezado);
                        doc.Add(new Phrase());
                        doc.Add(new Phrase());
                        // tabla
                        PdfPTable tabla = new PdfPTable(new float[] { 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 3 });

                        // Primera lìnea cabecera
                        tabla.AddCell(new PdfPCell(new Phrase("Fecha", Chico)));
                        tabla.AddCell(new PdfPCell(new Phrase("HI", Chico)));
                        tabla.AddCell(new PdfPCell(new Phrase("HS", Chico)));
                        tabla.AddCell(new PdfPCell(new Phrase("HCol", Chico)));
                        tabla.AddCell(new PdfPCell(new Phrase("MI", Chico)));
                        tabla.AddCell(new PdfPCell(new Phrase("MS", Chico)));
                        tabla.AddCell(new PdfPCell(new Phrase("HTH", Chico)));
                        tabla.AddCell(new PdfPCell(new Phrase("HTN", Chico)));
                        tabla.AddCell(new PdfPCell(new Phrase("H.Extra", Chico)));
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
                            tabla.AddCell(new PdfPCell(new Phrase(atraso.HorasPactadas, Chico)));
                            //Horas realizadas
                            tabla.AddCell(new PdfPCell(new Phrase(atraso.HorasReales.ToString(), Chico)));
                            //Horas extra
                            tabla.AddCell(new PdfPCell(new Phrase(atraso.HorasExtra, Chico)));
                            // Atraso
                            tabla.AddCell(new PdfPCell(new Phrase(atraso.Atraso, Chico)));
                            // Salida adelantada
                            tabla.AddCell(new PdfPCell(new Phrase(atraso.SalidaAdelantada, Chico)));
                            // Colación
                            tabla.AddCell(new PdfPCell(new Phrase(atraso.TiempoColacionReal, Chico)));
                            // S.Ent
                            tabla.AddCell(new PdfPCell(new Phrase(atraso.SobreEntrada, Chico)));
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
                        tabla.AddCell(new PdfPCell(new Phrase("", Chico)));//LibroAsistenciaDTOHelpers.CalculaHorasExtra(reporte), Chico)));
                        // Atraso
                        tabla.AddCell(new PdfPCell(new Phrase("", Chico)));
                        // Salida adelantada
                        tabla.AddCell(new PdfPCell(new Phrase("", Chico)));
                        // Colacón
                        tabla.AddCell(new PdfPCell(new Phrase("", Chico)));
                        // S.Ent
                        tabla.AddCell(new PdfPCell(new Phrase("", Chico)));
                        // Permisos
                        tabla.AddCell(new PdfPCell(new Phrase("", Chico)));
                        doc.Add(tabla);
                        doc.Add(new Phrase());

                        // Resumen
                        PdfPTable resumen = new PdfPTable(new float[] { 2, 1, 2, 1, 2, 1, 2, 1 });
                        //Días Trabajados
                        resumen.AddCell(new PdfPCell(new Phrase("Días Trabajados", Chico)) { Border = Rectangle.NO_BORDER });
                        resumen.AddCell(new PdfPCell(new Phrase("", Chico)) { Border = Rectangle.NO_BORDER });
                        //
                        resumen.AddCell(new PdfPCell(new Phrase("Horas Pactadas", Chico)) { Border = Rectangle.NO_BORDER });
                        resumen.AddCell(new PdfPCell(new Phrase("", Chico)) { Border = Rectangle.NO_BORDER });
                        //
                        resumen.AddCell(new PdfPCell(new Phrase("Horas extras", Chico)) { Border = Rectangle.NO_BORDER });
                        resumen.AddCell(new PdfPCell(new Phrase("", Chico)) { Border = Rectangle.NO_BORDER });
                        //
                        resumen.AddCell(new PdfPCell(new Phrase("Errores de marca", Chico)) { Border = Rectangle.NO_BORDER });
                        resumen.AddCell(new PdfPCell(new Phrase("", Chico)) { Border = Rectangle.NO_BORDER });
                        //
                        resumen.AddCell(new PdfPCell(new Phrase("Días ausentes", Chico)) { Border = Rectangle.NO_BORDER });
                        resumen.AddCell(new PdfPCell(new Phrase("", Chico)) { Border = Rectangle.NO_BORDER });
                        //
                        resumen.AddCell(new PdfPCell(new Phrase("Horas Trabajadas", Chico)) { Border = Rectangle.NO_BORDER });
                        resumen.AddCell(new PdfPCell(new Phrase("", Chico)) { Border = Rectangle.NO_BORDER });
                        //
                        resumen.AddCell(new PdfPCell(new Phrase("Vacaciones", Chico)) { Border = Rectangle.NO_BORDER });
                        resumen.AddCell(new PdfPCell(new Phrase("", Chico)) { Border = Rectangle.NO_BORDER });
                        // Aquí va una tabla con el espacio para la firma
                        resumen.AddCell(new PdfPCell(new Phrase("______________________ Firma Empleado", Chico)) { VerticalAlignment = Rectangle.ALIGN_BOTTOM, HorizontalAlignment = Rectangle.ALIGN_CENTER, Colspan = 2, Rowspan = 3, Border = Rectangle.NO_BORDER });
                        //
                        resumen.AddCell(new PdfPCell(new Phrase("Días Atraso", Chico)) { Border = Rectangle.NO_BORDER });
                        resumen.AddCell(new PdfPCell(new Phrase("", Chico)) { Border = Rectangle.NO_BORDER });
                        //
                        resumen.AddCell(new PdfPCell(new Phrase("Total Atraso", Chico)) { Border = Rectangle.NO_BORDER });
                        resumen.AddCell(new PdfPCell(new Phrase("", Chico)) { Border = Rectangle.NO_BORDER });
                        //
                        resumen.AddCell(new PdfPCell(new Phrase("Licencias Médicas", Chico)) { Border = Rectangle.NO_BORDER });
                        resumen.AddCell(new PdfPCell(new Phrase("", Chico)) { Border = Rectangle.NO_BORDER });
                        //
                        resumen.AddCell(new PdfPCell(new Phrase("Salidas", Chico)) { Border = Rectangle.NO_BORDER });
                        resumen.AddCell(new PdfPCell(new Phrase("", Chico)) { Border = Rectangle.NO_BORDER });
                        //
                        resumen.AddCell(new PdfPCell(new Phrase("Total Salidas", Chico)) { Border = Rectangle.NO_BORDER });
                        resumen.AddCell(new PdfPCell(new Phrase("", Chico)) { Border = Rectangle.NO_BORDER });

                        resumen.AddCell(new PdfPCell(new Phrase("", Chico)) { Border = Rectangle.NO_BORDER });
                        resumen.AddCell(new PdfPCell(new Phrase("", Chico)) { Border = Rectangle.NO_BORDER });
                        doc.Add(resumen);
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