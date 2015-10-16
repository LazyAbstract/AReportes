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
        public IArchivoReporte CrearArchivoReporteFactoria(int tipoReporte, AufenPortalReportesDataContext db, EMPRESA empresa, vw_Ubicacione departamento, DateTime FechaDesde, DateTime FechaHasta, string path, string rut)
        {
            IArchivoReporte archivoReporte = null;
            string buff = null;
            if (!String.IsNullOrEmpty(rut))
            {
                buff = ("000000000" + rut.Trim()).Substring(("000000000" + rut.Trim()).Length - 9, 9);
            }
            switch (tipoReporte)
            {
                case TipoReporte.LibroAtrasos:
                    archivoReporte =
                        new LibroAtrasos(db, empresa, departamento, FechaDesde, FechaHasta, path, buff);
                    break;
                case TipoReporte.AsistenciaLegal:

                    archivoReporte = new AsistenciaLegal(db, empresa, departamento, FechaDesde, FechaHasta, path, buff);
                    break;
                case TipoReporte.LibroSobreTiempo:
                    archivoReporte = new LibroSobreTiempo(db, empresa, departamento, FechaDesde, FechaHasta, path, buff);
                    break;
                case TipoReporte.AsistenciaPersonal:
                    archivoReporte = new AsistenciaPersonal(db, empresa, departamento, FechaDesde, FechaHasta, path, buff);
                    break;
                case TipoReporte.Ausencia:
                    archivoReporte = new Ausencia(db, empresa, departamento, FechaDesde, FechaHasta, path, buff);
                    break;
            }
            return archivoReporte;
        }
    }
}