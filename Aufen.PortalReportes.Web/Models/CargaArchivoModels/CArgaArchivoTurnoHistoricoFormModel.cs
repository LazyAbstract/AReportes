using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Aufen.PortalReportes.Web.CustomValidation;

namespace Aufen.PortalReportes.Web.Models.CargaArchivoModels
{
    public class CargaArchivoTurnoHistoricoFormModel
    {
        [Required]
        [AValidFile(Allowed = new string[] { ".xlsx", ".XLSX" }, MaxLength = 1024 * 1024 * 100, ErrorMessage = "El archivo debe tener extensión .xlsx y pesar, cómo máximo, 100MG.")]
        public HttpPostedFileBase Archivo { get; set; }
        [Required]
        public string Periodo { get; set; }

        public string Error
        {
            get { return String.Empty; }
        }

        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case "Archivo":
                        if (Archivo == null || (Archivo != null && Archivo.ContentLength <= 0))
                        {
                            return "Debe subir un archivo válido.";
                        }
                        break;
                }
                return String.Empty;
            }

        }
    }
}