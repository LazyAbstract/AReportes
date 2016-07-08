using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aufen.PortalReportes.Web.Models.CargaArchivoModels
{
    public class CargaArchivoIncidenciaHistoricoViewModel
    {
        public CargaArchivoIncidenciaHistoricoFormModel Form { get; set; }
        public CargaArchivoIncidenciaHistoricoViewModel()
        {
            Form = new CargaArchivoIncidenciaHistoricoFormModel();
        }

        public CargaArchivoIncidenciaHistoricoViewModel(CargaArchivoIncidenciaHistoricoFormModel F) : this()
        {
            Form = F;
        }
    }
}