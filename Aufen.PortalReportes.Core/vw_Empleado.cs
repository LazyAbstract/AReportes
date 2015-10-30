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

        public string SucursalPlanta
        {
            get
            {
                string buffer = (NombreUbicacion ?? String.Empty).Trim();
                if (buffer == "MOSTAZAL")
                    return "Casa Matriz";
                return buffer;
            }
        }

        public string RutAufen
        {
            get
            {
                if (!String.IsNullOrEmpty(Codigo) && Codigo.Substring(0, 2) == "00")
                {
                    return Codigo.Substring(2, Codigo.Length - 2);
                }
                if (!String.IsNullOrEmpty(Codigo) && Codigo.Substring(0,1) == "0")
                {
                    return Codigo.Substring(1, Codigo.Length-1);
                }
                return Codigo ?? String.Empty;
            }
        }

    }
}
