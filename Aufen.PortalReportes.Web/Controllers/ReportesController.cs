using Aufen.PortalReportes.Web.Models.ReportesModels;
using Aufen.PortalReportes.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using AutoMapper;
using ICSharpCode.SharpZipLib.Core;

namespace Aufen.PortalReportes.Web.Controllers
{
    public class ReportesController : BaseController
    {
        //[Authorize(Roles = "Admin")]
        public ActionResult ListarReporte(ListarReporteFormModel FORM)
        {
            ListarReporteViewModel model = new ListarReporteViewModel(FORM);
            if (FORM.IdTipoReportes == null || (FORM.IdTipoReportes != null && !FORM.IdTipoReportes.Any()))
            {
                ModelState.AddModelError("FORM.IdTipoReportes", "Debe elegir almenos un tipode reporte.");
            }
            if (ModelState.IsValid)
            {
                // Armamos nuestro archivo de salida por empresa y departamento
                MemoryStream outputMemStream = new MemoryStream();
                ZipOutputStream zipStream = new ZipOutputStream(outputMemStream);
                zipStream.SetLevel(3); //0-9, 9 being the highest level of compression
                using (Stream memOutput = new MemoryStream())
                using (ZipOutputStream zipOutput = new ZipOutputStream(memOutput))
                {
                    zipOutput.SetLevel(3); //0-9, 9 being the highest level of compression
                    foreach (var tipoEmpresa in FORM.Empresa)
                    {
                        var empresa = db.EMPRESAs.SingleOrDefault(x => x.Codigo == tipoEmpresa);
                        IEnumerable<string> tipoDepartamentos = FORM.Departamento.Where(x=> x.Substring(0,2).Trim() == tipoEmpresa.Trim());
                        foreach(var tipoDepartamento in tipoDepartamentos)
                        {
                            string codigoDepartamento = tipoDepartamento.Substring(2,2).Trim();
                            var departamento = db.vw_Ubicaciones.FirstOrDefault(x => x.IdEmpresa == tipoEmpresa && x.Codigo.Trim() == codigoDepartamento);
                            foreach (var tipoReporte in FORM.IdTipoReportes)
                            {
                                ArchivoReporteFactoria archivoReporteFactoria = new ArchivoReporteFactoria();
                                IArchivoReporte archivoReporte = archivoReporteFactoria.CrearArchivoReporteFactoria(
                                    tipoReporte, 
                                    db, 
                                    empresa, 
                                    departamento, 
                                    FORM.FechaDesde.Value, 
                                    FORM.FechaHasta.Value, 
                                    HttpContext.Server.MapPath("~/Content")
                                    , FORM.Rut);
                                if (archivoReporte.GetArchivo() != null)
                                {
                                    zipOutput.PutNextEntry(archivoReporte.GetZipArchivoReporte());
                                    zipOutput.Write(archivoReporte.GetArchivo(), 0, archivoReporte.GetArchivoLength());
                                }
                            }
                        }
                    }
                    zipOutput.Finish();
                    byte[] newBytes = new byte[memOutput.Length];
                    memOutput.Seek(0, SeekOrigin.Begin);
                    memOutput.Read(newBytes, 0, newBytes.Length);
                    zipOutput.Close();
                    return File(newBytes, "application/zip", "Reportes -" + FORM.FechaDesde.Value.ToShortDateString() + " - " + FORM.FechaHasta.Value.ToShortDateString() + ".zip");
                }
            }
            return View(model);
        }
        public PartialViewResult GetDepartamentos(ListarReporteFormModel FORM)
        {
            GetDepartamentosViewModel model = new GetDepartamentosViewModel(FORM.Empresa, FORM.Departamento);
            return PartialView(model);
        }
    }
}
