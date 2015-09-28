using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aufen.PortalReportes.Web.Models.ReportesModels
{
    public interface IArchivoReporte
    {
        ZipEntry GetZipArchivoReporte();
        byte[] GetArchivo();
        int GetArchivoLength();
    }
}
