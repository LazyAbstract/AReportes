using Aufen.PortalReportes.Web.Models.CargaArchivoModels;
using Aufen.PortalReportes.Web.Models.DTOModels;
using Aufen.PortalReportes.Web.Models.ReglaValidacionModels;
using Aufen.PortalReportes.Web.Models.ReglaValidacionModels.ReglaValidacionTurnoHistoricoModels;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Aufen.PortalReportes.Web.Controllers
{
    public class CargaArchivoController : BaseController
    {
        public ActionResult Index()
        {
            return RedirectToAction("CargaArchivoTurnoHistorico");
        }

        public ActionResult CargaArchivoTurnoHistorico()
        {
            CargaArchivoTurnoHistoricoViewModel model = new CargaArchivoTurnoHistoricoViewModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult CargaArchivoTurnoHistorico(CargaArchivoTurnoHistoricoFormModel Form)
        {
            CargaArchivoTurnoHistoricoViewModel model = new CargaArchivoTurnoHistoricoViewModel(Form);
            Dictionary<string, string> diccionario = DiccionarioCabecera.DiccionarioCabeceraHistorico.GetDiccionarioTurnoHistorico();
            if (diccionario == null || (diccionario != null && !diccionario.Any()))
            {
                ModelState.AddModelError("Form.Archivo", "No se puede reconocer la estrucutra del archivo.");
            }

            if (ModelState.IsValid)
            {
                int largo = Form.Archivo.ContentLength;
                byte[] buffer = new byte[largo];
                Form.Archivo.InputStream.Read(buffer, 0, largo);
                using (MemoryStream ms = new MemoryStream(buffer))
                {
                    XSSFWorkbook libro = null;
                    var stream = Form.Archivo.InputStream;
                    try
                    {
                        libro = new XSSFWorkbook(ms);
                    }
                    catch
                    {
                        ModelState.AddModelError(String.Empty, "Formato de achivo incorrecto.");
                        return View(model);
                    }

                    List<IReglaValidacion> reglas = new List<IReglaValidacion>();
                    reglas.Add(new RutRequeridoValidacion());
                    reglas.Add(new CalendarioRequeridoValidacion());
                    reglas.Add(new CalendarioExisteValidacion());
                    reglas.Add(new FechaDesdeFormatoValidacion());
                    reglas.Add(new FechaDesdeRequeridoValidacion());
                    reglas.Add(new FechaHastaFormatoValidacion());
                    reglas.Add(new HorarioExisteValidacion());
                    reglas.Add(new HorarioRequeridoValidacion());
                    CargadorExcelGenerico cargador = new CargadorExcelGenerico(libro, diccionario, reglas);
                    cargador.ValidarEncabezado();

                    if (cargador.EsEncabezadoValido)
                    {
                        IEnumerable<TurnoHistoricoDTO> resultado =
                                cargador.CargarArchivo<TurnoHistoricoDTO>();

                        //  TODO: insertar en tabla turnohistorico

                        if (!cargador.EsArchivoValido)
                        {
                            MemoryStream salidaExcel = new MemoryStream();
                            var salida = cargador.GetSalida();
                            salida.Write(salidaExcel);
                            return File(salidaExcel.ToArray(),
                                "application/vnd.ms-excel",
                                "Errores_" + DateTime.Now.ToString("dd-MM-yyyy") + ".xlsx");
                        }
                    }
                    else
                    {
                        foreach (var item in cargador.GetErroresEncabezado)
                        {
                            ModelState.AddModelError(String.Empty, item);
                        }
                        return View(model);
                    }
                }
                Mensaje = "El proceso de carga ha finalizado exitosamente.";
                return RedirectToAction("ListarReporte", "Reportes");
            }
            Mensaje = "Ha ocurrido un problema con el proceso de carga.";
            return View(model);
        }

        public ActionResult DescargarFormato()
        {
            throw new NotImplementedException();
        }
    }
}
