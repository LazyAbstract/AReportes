using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Aufen.PortalReportes.Core
{
    public partial class AufenPortalReportesDataContext
    {
        const string CONNECTIONSTRING_KEY = "AppDb";
        public AufenPortalReportesDataContext WithConnectionStringFromConfiguration(string key)
        {
            var settingsCollection = ConfigurationManager.ConnectionStrings[key];
            if (settingsCollection == null) throw new ConfigurationErrorsException(String.Format("No existe el ConnectionString con la llave \"{0}\" en el archivo de configuracion.", key));

            string connectionString = settingsCollection.ConnectionString;
            if (connectionString == null && String.IsNullOrEmpty(connectionString)) throw new ConfigurationErrorsException(String.Format("El ConnectionString especificado para la llave \"{0}\" está vacío.", key));

            this.Connection.ConnectionString = settingsCollection.ConnectionString;
            this.CommandTimeout = 6 * 60; // 6 minutos
            return this;
        }

        public AufenPortalReportesDataContext WithConnectionStringFromConfiguration()
        {
            return WithConnectionStringFromConfiguration(CONNECTIONSTRING_KEY);
        }
    }
}
