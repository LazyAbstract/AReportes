using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aufen.PortalReportes.Core
{
    public partial class vw_Ubicacione
    {
        public string SucursalPlanta
        {
            get
            {
                string buffer = (Descripcion ?? String.Empty).Trim();
                if (buffer == "MOSTAZAL")
                    return "Casa Matriz";
                return buffer;
            }
        }
    }
}
