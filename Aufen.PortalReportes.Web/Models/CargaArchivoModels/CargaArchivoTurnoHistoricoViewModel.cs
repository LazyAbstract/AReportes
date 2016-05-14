using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aufen.PortalReportes.Web.Models.CargaArchivoModels
{
    public class CargaArchivoTurnoHistoricoViewModel
    {
        public CargaArchivoTurnoHistoricoFormModel Form { get; set; }

        public CargaArchivoTurnoHistoricoViewModel()
        {
            Form = new CargaArchivoTurnoHistoricoFormModel();
        }

        public CargaArchivoTurnoHistoricoViewModel(CargaArchivoTurnoHistoricoFormModel f) : this()
        {
            Form = f;
        }
    }
}