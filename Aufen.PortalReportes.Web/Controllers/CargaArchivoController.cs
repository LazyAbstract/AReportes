using Aufen.PortalReportes.Core;
using Aufen.PortalReportes.Web.Models.CargaArchivoModels;
using Aufen.PortalReportes.Web.Models.DTOModels;
using Aufen.PortalReportes.Web.Models.ReglaValidacionModels;
using Aufen.PortalReportes.Web.Models.ReglaValidacionModels.ReglaValidacionIncidenciaHistoricoModels;
using Aufen.PortalReportes.Web.Models.ReglaValidacionModels.ReglaValidacionTurnoHistoricoModels;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
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
                    reglas.Add(new Aufen.PortalReportes.Web.Models.ReglaValidacionModels.ReglaValidacionTurnoHistoricoModels.RutRequeridoValidacion());
                    reglas.Add(new CalendarioRequeridoValidacion());
                    reglas.Add(new CalendarioExisteValidacion());
                    reglas.Add(new FechaDesdeFormatoValidacion());
                    reglas.Add(new FechaDesdeRequeridoValidacion());
                    reglas.Add(new FechaHastaFormatoValidacion());
                    reglas.Add(new HorarioExisteValidacion());
                    reglas.Add(new HorarioRequeridoValidacion());
                    reglas.Add(new FechaDesdeMenorFechaHastaValidacion());
                    CargadorExcelGenerico cargador = new CargadorExcelGenerico(libro, diccionario, reglas);
                    cargador.ValidarEncabezado();

                    if (cargador.EsEncabezadoValido)
                    {
                        IEnumerable<TurnoHistoricoDTO> resultados =
                                cargador.CargarArchivo<TurnoHistoricoDTO>();

                        foreach (var resultado in resultados)
                        {
                            var empleado = db.Fn_DatosEmpleado(resultado.IdHorario, resultado.Rut, resultado.IdCalendario)
                                .FirstOrDefault();
                            if (empleado != null && resultado.FechaHastaAsDateTime.HasValue && resultado.FechaDesdeAsDateTime.HasValue)
                            {
                                var empleadoCalendarioHorariosHistorico01s = db.EmpleadoCalendarioHorariosHistorico01s
                                    .Where(x => x.CodigoEmpleado == empleado.CodigoEmpleado &&
                                    x.FechaDesde < resultado.FechaHastaAsDateTime.Value &&
                                    (!x.FechaHasta.HasValue || 
                                        (x.FechaHasta.HasValue && x.FechaHasta.Value > resultado.FechaDesdeAsDateTime))
                                    ).ToList();// si existe el empleado deberia al menos tener un registro con ultima fecha null
                                // Elimino las antiguas pera no tener problemas
                                db.EmpleadoCalendarioHorariosHistorico01s.DeleteAllOnSubmit(empleadoCalendarioHorariosHistorico01s);
                                // Ahora  omito las que quedan dentro del intervalo que quiero insertar desde el excel, es >= y <= ya que podria querer agregar el mismo registro y lo omito tambien para evitar duplicados
                                List<EmpleadoCalendarioHorariosHistorico01> insercion = empleadoCalendarioHorariosHistorico01s.Where(x=>!(
                                    x.FechaDesde >= resultado.FechaDesdeAsDateTime && 
                                    x.FechaHasta <= resultado.FechaHastaAsDateTime.Value))
                                    .ToList();
                                // Agrego el que quiero insertar desde el excel
                                insercion.Add(new EmpleadoCalendarioHorariosHistorico01()
                                {
                                    IdEmpleadoCalendarioHorariosHistorico01 = Guid.NewGuid(),
                                    CodigoEmpleado = empleado.CodigoEmpleado,
                                    CodigoHorario = ("0000" + resultado.IdHorario).Right(4),
                                    IdCalendario = resultado.IdCalendario,
                                    FechaCreacion = DateTime.Now,
                                    FechaDesde= resultado.FechaDesdeAsDateTime.Value,
                                    FechaHasta = resultado.FechaHastaAsDateTime.Value,
                                });
                                //Ordeno por Fecha desde
                                insercion = insercion.OrderByDescending(x=> x.FechaDesde).ToList();
                                // agrego el que tiene fecha hasta null
                                EmpleadoCalendarioHorariosHistorico01 bufferInsercion = insercion.First();
                                db.EmpleadoCalendarioHorariosHistorico01s.InsertOnSubmit(new EmpleadoCalendarioHorariosHistorico01() { 
                                    IdEmpleadoCalendarioHorariosHistorico01 = Guid.NewGuid(),
                                    FechaHasta = null,
                                    FechaDesde = bufferInsercion.FechaDesde,
                                    FechaCreacion = bufferInsercion.FechaCreacion,
                                    CodigoEmpleado = bufferInsercion.CodigoEmpleado,
                                    IdCalendario = bufferInsercion.IdCalendario,
                                    CodigoHorario = ("0000" + bufferInsercion.CodigoHorario).Right(4),
                                    Donde = ""
                                });
                                foreach (var insertar in insercion.Skip(1))
                                {
                                    EmpleadoCalendarioHorariosHistorico01 bufferInsertar = new EmpleadoCalendarioHorariosHistorico01()
                                    {
                                        IdEmpleadoCalendarioHorariosHistorico01 = Guid.NewGuid(),
                                        FechaDesde = insertar.FechaDesde,
                                        FechaHasta = insertar.FechaHasta,
                                        FechaCreacion = insertar.FechaCreacion,
                                         CodigoEmpleado = insertar.CodigoEmpleado, 
                                         IdCalendario = insertar.IdCalendario, 
                                         CodigoHorario = ("0000" + insertar.CodigoHorario).Right(4), 
                                         Donde = ""
                                    };
                                    // Me aseguro de conservar el intervalo proeniente del excel como corresponde
                                    if (bufferInsertar.FechaDesde > resultado.FechaDesdeAsDateTime.Value &&
                                        bufferInsertar.FechaDesde < resultado.FechaHastaAsDateTime.Value)
                                    {
                                        bufferInsertar.FechaDesde = resultado.FechaHastaAsDateTime.Value.AddDays(1);
                                    }
                                    // seteo la fecha hasta hasta el intervalo que viene adelante
                                    bufferInsertar.FechaHasta = bufferInsercion.FechaDesde.AddDays(-1);
                                    if (bufferInsertar.FechaHasta >= insertar.FechaDesde)
                                    {
                                        // porsiaca, para no insertar intervalos invalidos
                                        db.EmpleadoCalendarioHorariosHistorico01s.InsertOnSubmit(bufferInsertar);
                                    }
                                    bufferInsercion = insertar;
                                }
                            }
                            db.SubmitChanges();
                        }
                        
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
            String path = HttpContext.Server.MapPath("~/Content/Estructura Carga Historico.xlsx");
            return File(path, MediaTypeNames.Text.Plain, "Estructura Carga Historico.xlsx");
        }

        public ActionResult CargaArchivoIncidenciaHistorico()
        {
            CargaArchivoIncidenciaHistoricoViewModel model =
                new CargaArchivoIncidenciaHistoricoViewModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult CargaArchivoIncidenciaHistorico(CargaArchivoIncidenciaHistoricoFormModel Form)
        {
            CargaArchivoIncidenciaHistoricoViewModel model = new CargaArchivoIncidenciaHistoricoViewModel(Form);
            Dictionary<string, string> diccionario = DiccionarioCabecera.DiccionarioCabeceraIncidencia.GetDiccionarioIncidenciaHistorico();
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
                    reglas.Add(new FechaCorrectaValidacion());
                    reglas.Add(new IncidenciaExisteValidacion());
                    reglas.Add(new Aufen.PortalReportes.Web.Models.ReglaValidacionModels.ReglaValidacionIncidenciaHistoricoModels.RutRequeridoValidacion());
                    reglas.Add(new RutExisteValidacion());
                    reglas.Add(new TipoDiaExisteValidacion());
                    reglas.Add(new LlaveUnicaValidacion());
                    CargadorExcelGenerico cargador = new CargadorExcelGenerico(libro, diccionario, reglas);
                    cargador.ValidarEncabezado();

                    if (cargador.EsEncabezadoValido)
                    {
                        IEnumerable<IncidenciaHistoricoDTO> resultados =
                                cargador.CargarArchivo<IncidenciaHistoricoDTO>();
                        foreach (var resultado in resultados)
                        {
                            DateTime _fecha = DateTime.Parse(resultado.Fecha);
                            CALENDARIO01 calendario = new CALENDARIO01()
                            {
                                Fecha = _fecha.Year.ToString() + ("0" + _fecha.Month.ToString()).Right(2) + ("0" + _fecha.Day.ToString()).Right(2),
                                Publico = 0,
                                IdTipoDia = Convert.ToChar(resultado.IdTipoDia),
                                IdIncidencia = ("0000" + resultado.IdIncidencia).Right(4),
                                IdCalendario = ("000000000" + resultado.Rut).Right(9),
                            };
                        }

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

        public ActionResult DescargarFormatoIncidencia()
        {
            String path = HttpContext.Server.MapPath("~/Content/Estructura Carga Incidencia.xlsx");
            return File(path, MediaTypeNames.Text.Plain, "Estructura Carga Incidencia.xlsx");
        }
    }
}
