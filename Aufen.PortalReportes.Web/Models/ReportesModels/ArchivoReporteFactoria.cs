using Aufen.PortalReportes.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aufen.PortalReportes.Web.Models.ReportesModels
{
    public class ArchivoReporteFactoria
    {
        public ArchivoReporteFactoria()
        {

        }
        public IArchivoReporte CrearArchivoReporteFactoria(int tipoReporte, AufenPortalReportesDataContext db, EMPRESA empresa, vw_Ubicacione departamento, DateTime FechaDesde, DateTime FechaHasta, string path)
        {
            IArchivoReporte archivoReporte = null;
            switch (tipoReporte)
            {
                case TipoReporte.LibroAtrasos:
                    archivoReporte =
                        new LibroAtrasos(db, empresa, departamento, FechaDesde, FechaHasta);
                    break;
                case TipoReporte.AsistenciaLegal:

                    archivoReporte = new AsistenciaLegal(db, empresa, departamento, FechaDesde, FechaHasta, path);
                    break;
                case TipoReporte.LibroSobreTiempo:
                    archivoReporte = new LibroSobreTiempo(db, empresa, departamento, FechaDesde, FechaHasta);
                    break;
                case TipoReporte.AsistenciaPersonal:
                    archivoReporte = new AsistenciaPersonal(db, empresa, departamento, FechaDesde, FechaHasta);
                    break;
            }
            return archivoReporte;
        }
    }
}