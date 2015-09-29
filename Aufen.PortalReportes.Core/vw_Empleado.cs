using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aufen.PortalReportes.Core
{
    public partial class vw_Empleado
    {
        public string NombreCompleto
        {
            get
            {
                return (Nombre ?? String.Empty).Trim() + " " + (Apellidos ?? String.Empty).Trim();
            }
        }
            
    }
}
