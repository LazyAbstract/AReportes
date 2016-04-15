using Aufen.PortalReportes.Core;
using Aufen.PortalReportes.Web.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Aufen.PortalReportes.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {

        
        protected void Application_Start()
        {
            //if (DateTime.Today > new DateTime(2016, 4, 30))
            //{
            //    throw new NotImplementedException("Usted no paga pendejo!!!!");
            //}
            
            AreaRegistration.RegisterAllAreas();
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();

            ModelBinders.Binders.Add(typeof(Rut), new RutModelBinder());
            ModelBinders.Binders.Add(typeof(DateTime?), new CurrentCultureDateTimeBinder());
            ModelBinders.Binders.Add(typeof(DateTime), new CurrentCultureDateTimeBinder());

            IList<IConfigurable> configurations = new List<IConfigurable>();
            configurations.Add(new MapperConfiguration());
            foreach (var configuration in configurations)
            {
                configuration.Configure();
            } 
        }
    }
}