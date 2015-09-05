using Aufen.PortalReportes.Web.Models.ReportesModels;
using Aufen.PortalReportes.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace Aufen.PortalReportes.Web.Controllers
{
    public class ReportesController : BaseController
    {
        public ActionResult ListarReporte(ListarReporteFormModel FORM)
        {
            ListarReporteViewModel model = new ListarReporteViewModel(FORM);
            if (FORM.IdTipoReportes == null || (FORM.IdTipoReportes != null && !FORM.IdTipoReportes.Any()))
            {
                ModelState.AddModelError("FORM.IdTipoReportes", "Debe elegir almenos un tipode reporte.");
            }
            if (ModelState.IsValid)
            {
                MemoryStream outputMemStream = new MemoryStream();
                ZipOutputStream zipStream = new ZipOutputStream(outputMemStream);
                zipStream.SetLevel(3); //0-9, 9 being the highest level of compression

                using (Stream memOutput = new MemoryStream())
                using (ZipOutputStream zipOutput = new ZipOutputStream(memOutput))
                {
                    zipOutput.SetLevel(9);
                    foreach (var empresa in FORM.Empresa)
                    {
                        //foreach (var departamento in FORM.Departamento)
                        //{
                        foreach (var tipoReporte in FORM.IdTipoReportes)
                        {
                            ZipEntry zipEntry = new ZipEntry(String.Format("{0}/LibroAtrasos.pdf", empresa));
                            zipEntry.DateTime = DateTime.Now;
                            byte[] buffer = new byte[] { };
                            switch (tipoReporte)
                            {
                                case TipoReporte.LibroAtrasos:
                                    IEnumerable<sp_LibroAtrasosResult> resultadoLibroAtrasos =
                                        db.sp_LibroAtrasos(
                                        FORM.FechaDesde.Value.ToString("yyyyMMdd"),
                                        FORM.FechaHasta.Value.ToString("yyyyMMdd"),
                                        int.Parse(empresa).ToString()).AsEnumerable();
                                    LibroAtrasos libroAtrasos = new LibroAtrasos(resultadoLibroAtrasos);
                                    break;
                            }
                            zipOutput.PutNextEntry(zipEntry);
                            zipOutput.Write(buffer, 0, buffer.Length);
                        }
                        //}
                    }
                    zipOutput.Finish();
                    byte[] newBytes = new byte[memOutput.Length];
                    memOutput.Seek(0, SeekOrigin.Begin);
                    memOutput.Read(newBytes, 0, newBytes.Length);
                    zipOutput.Close();
                    return File(newBytes, "application/zip", "reportes.zip");
                }
            }
            return View(model);
        }
    }
}
