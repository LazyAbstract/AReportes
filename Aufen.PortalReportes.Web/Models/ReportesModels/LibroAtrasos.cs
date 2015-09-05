using Aufen.PortalReportes.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aufen.PortalReportes.Web.Models.ReportesModels
{
    public class LibroAtrasos
    {
        private byte[] _Archivo { get; set; }
        public LibroAtrasos(IEnumerable<sp_LibroAtrasosResult> libroAtrasos)
        {

        }

        public byte[] Archivo
        {
            get
            {
                return _Archivo;
            }
        }
    }
}