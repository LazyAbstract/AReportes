using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aufen.PortalReportes.Web.Models.ReglaValidacionModels
{
    public interface IReglaValidacion
    {
        string Mensaje { get; }
        bool ValidaRegla(object sujeto);
    }
}
