using Aufen.PortalReportes.Web.Models.ReglaValidacionModels;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Web;
using Aufen.PortalReportes.Core;

namespace Aufen.PortalReportes.Web.Models.CargaArchivoModels
{
    public class CargadorExcelGenerico
    {
        public XSSFWorkbook Libro { get; set; }
        public XSSFWorkbook LibroErrores { get; set; }
        private Dictionary<string, string> Diccionario { get; set; }
        private bool EncabezadoValido { get; set; }
        private IList<string> ErroresEncabezado { get; set; }
        private IList<IReglaValidacion> ReglasValidacion { get; set; }
        private bool FilasValidas { get; set; }

        public CargadorExcelGenerico()
        {
            // No es válido hasta que sea validado
            EncabezadoValido = false;
            // Es válido hasta que se diga lo contrario
            FilasValidas = true;
            ErroresEncabezado = new List<string>();
            ReglasValidacion = new List<IReglaValidacion>();
        }

        public CargadorExcelGenerico(XSSFWorkbook libro,
            Dictionary<string, string> diccionario)
            : this()
        {
            Libro = libro;
            Diccionario = diccionario;
        }

        public CargadorExcelGenerico(XSSFWorkbook libro,
            Dictionary<string, string> diccionario, List<IReglaValidacion> reglasValidacion) :
            this(libro, diccionario)
        {
            ReglasValidacion = reglasValidacion;
        }

        public bool EsEncabezadoValido
        {
            get
            {
                return EncabezadoValido;
            }
        }

        public bool EsArchivoValido
        {
            get
            {
                return FilasValidas && EncabezadoValido;
            }
        }

        public IEnumerable<string> GetErroresEncabezado
        {
            get
            {
                return ErroresEncabezado;
            }
        }

        public void ValidarEncabezado()
        {
            // Asumimos que es válido y buscamos errores
            EncabezadoValido = true;
            // Validamos que se haya construido apropiadamente el lector
            if (Libro == null)
                throw new NotImplementedException();
            if (Diccionario == null || (Diccionario != null && !Diccionario.Any()))
                throw new NotImplementedException();
            try
            {
                XSSFSheet sabana = (XSSFSheet)Libro.GetSheetAt(0);

                if (sabana != null)
                {
                    XSSFRow cabecera = (XSSFRow)sabana.GetRow(0);
                    if (cabecera != null)
                    {
                        List<string> valoresCabecera = new List<string>();
                        for (int j = 0; j < cabecera.LastCellNum; j++)
                        {
                            XSSFCell celdaCabecera = (XSSFCell)cabecera.GetCell(j);
                            if (celdaCabecera != null)
                            {
                                valoresCabecera.Add(celdaCabecera.ToString());
                            }
                        }
                        // Esto implica que son 1 a 1 con el diccionario.. esto se puede modificar 
                        // para tener más de un nombre de columna de la cabecera del excel que corresponda
                        // a un atributo del objeto a mapear
                        foreach (var item in Diccionario)
                        {
                            if (!valoresCabecera.Any(x => x.ToLower().Trim() == item.Key.ToLower().Trim()))
                            {
                                EncabezadoValido = false;
                                ErroresEncabezado.Add(String.Format(
                                    "No se encuentra el término {0} en la cabecera", item.Key));
                            }
                        }
                    }
                    else
                    {
                        EncabezadoValido = false;
                        ErroresEncabezado.Add("No existe cabecera en el libro cargado");
                    }
                }
                else
                {
                    EncabezadoValido = false;
                    ErroresEncabezado.Add("No existen sábanas en el libro cargado");
                }
            }
            catch
            {
                EncabezadoValido = false;
                ErroresEncabezado
                    .Add("Ha habido un problema leyendo el archivo. Contáctese con el administrador del sistema.");
            }

        }

        public List<T> CargarArchivo<T>()
        {
            if (Libro == null)
                throw new NotImplementedException();
            if (Diccionario == null || (Diccionario != null && !Diccionario.Any()))
                throw new NotImplementedException();
            XSSFSheet sabana = (XSSFSheet)Libro.GetSheetAt(0);
            XSSFRow cabecera = (XSSFRow)sabana.GetRow(0);
            int ultimaColumnaCabecera = cabecera.LastCellNum;

            List<T> lista = new List<T>();
            for (int i = 1; i <= sabana.LastRowNum; i++)
            {
                //Nos saltamos posibles filas vacias
                XSSFRow fila = (XSSFRow)sabana.GetRow(i);
                if (fila == null) { continue; }
                // Creamos el objeto que vamos a mapear
                Type tipoT = typeof(T);
                T buffer = (T)Activator.CreateInstance(tipoT);
                List<string> listaErrores = new List<string>();
                for (int j = 0; j < cabecera.LastCellNum; j++)
                {
                    //Asumimos que la fila es valida
                    bool valida = true;
                    //Rescatamos la celda correspondiente a la cabecera
                    //HSSFCell celdaCabecera = (HSSFCell)cabecera.GetCell(j);
                    XSSFCell celdaCabecera = (XSSFCell)cabecera.GetCell(j);

                    XSSFCell celdaFila = (XSSFCell)fila.GetCell(j);
                    if (celdaCabecera != null && !String.IsNullOrEmpty(celdaCabecera.ToString()))
                    {
                        if (Diccionario.Any(x => x.Key.ToLower() == celdaCabecera.ToString().Trim().ToLower()))
                        {
                            // Validaciones de estructura
                            var atributo = Diccionario.SingleOrDefault(x => x.Key.ToLower() ==
                                celdaCabecera.ToString().Trim().ToLower());

                            //string valor = celdaFila != null && celdaFila.CellType != NPOI.SS.UserModel.CellType.Blank ? celdaFila.ToString() : "";
                            string valor = celdaFila != null ? celdaFila.ToString() : "";

                            //// ESTOS TRY CATCH HACEN EL CARGADOR UNAS 5 VECES MÁS LENTO!
                            ////  Pregunta si es tipo fecha y descarta a los tipo string y numerico
                            //try
                            //{
                            //    if (celdaFila != null && celdaFila.CellType != NPOI.SS.UserModel.CellType.String
                            //        && HSSFDateUtil.IsCellDateFormatted(celdaFila))
                            //    {
                            //        valor = celdaFila.StringCellValue != ""
                            //            ? celdaFila.DateCellValue.ToShortDateString() : null;
                            //    }
                            //}
                            //catch (Exception ex) { }

                            //try
                            //{
                            //    if (HSSFDateUtil.IsCellDateFormatted(celdaFila)
                            //        && celdaFila.CellType != NPOI.SS.UserModel.CellType.String)
                            //    {
                            //        valor = celdaFila.StringCellValue != "" ?
                            //            celdaFila.DateCellValue.ToString() : null;
                            //    }
                            //}
                            //catch
                            //{
                            //    if (celdaFila.CellType != NPOI.SS.UserModel.CellType.String)
                            //    {
                            //        valor = celdaFila.DateCellValue.ToString();
                            //    }
                            //}

                            PropertyInfo propInfo = tipoT.GetProperty(atributo.Value);
                            object propValue = null;
                            Type lololo = propInfo.GetType();
                            if (!String.IsNullOrEmpty(valor))
                            {
                                try
                                {   // Los tipos anulables de el objeto se deben generar explicitamente, 
                                    // ya que el typeConverter no los genera de otra forma.
                                    if (propInfo.PropertyType.Name.Contains("Nullable"))
                                    {
                                        NullableConverter nullableConverter =
                                            new NullableConverter(propInfo.PropertyType);
                                        propValue = nullableConverter.ConvertFromString(valor);
                                    }
                                    else
                                    {
                                        TypeConverter typeConverter =
                                            TypeDescriptor.GetConverter(propInfo.PropertyType);
                                        propValue = typeConverter.ConvertFromString(valor);
                                    }
                                    propInfo.SetValue(buffer, propValue, null);
                                }
                                catch (Exception ex)
                                {
                                    valida = false;
                                    FilasValidas = false;
                                    listaErrores.Add(
                                        String.Format("Tipo de dato incorrecto en la columna {0}.",
                                            atributo.Key));
                                }
                            }

                            // El try/catch es porque el programador puede poner el tipo de validación 
                            // incorrecta para el tipo de dato que está en la base de datos.
                            object[] customAttributes = propInfo
                                .GetCustomAttributes(typeof(ValidationAttribute), true);
                            bool esValido = true;
                            if (valida == true)
                            {
                                foreach (var customAttribute in customAttributes)
                                {
                                    //Obtiene los atributos del objeto y luego pregunta si cumple con este
                                    var validationAttribute = (ValidationAttribute)customAttribute;
                                    bool isValid = validationAttribute.IsValid(propInfo.GetValue(buffer,
                                        BindingFlags.GetProperty, null, null, null));
                                    esValido &= isValid;
                                    if (!isValid)
                                    {
                                        //agrega el Mensaje de error en el caso de no cumplir 
                                        //con la validación del atributo
                                        FilasValidas = false;
                                        listaErrores.Add(validationAttribute.ErrorMessage);
                                    }
                                    else
                                    {

                                    }
                                }
                            }
                        }
                        else
                        {
                            // El valor de la cabecera no se encuentra en el diccionario de términos aceptados
                        }
                    }
                }
                //Validaciones de negocio
                foreach (var item in ReglasValidacion)
                {
                    if (!item.ValidaRegla(buffer))
                    {
                        FilasValidas = false;
                        listaErrores.Add(item.Mensaje ?? String.Empty);
                    }
                }

                if (listaErrores.Any())
                {
                    //  Si tiene errores lo escribimos en la última celda teniendo como referencia la cabecera
                    XSSFCell celdaErrores = (XSSFCell)fila.GetCell(ultimaColumnaCabecera + 1);
                    if (celdaErrores == null)
                        celdaErrores = (XSSFCell)fila.CreateCell(ultimaColumnaCabecera + 1);
                    celdaErrores.SetCellValue(listaErrores.Flatten(", "));
                }
                else
                {
                    lista.Add(buffer);
                }
            }
            return lista;
        }

        public XSSFWorkbook GetSalida()
        {
            return Libro;
        }
    }
}