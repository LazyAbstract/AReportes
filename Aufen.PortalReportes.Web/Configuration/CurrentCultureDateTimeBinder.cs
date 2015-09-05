using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Aufen.PortalReportes.Web.Configuration
{
    public class CurrentCultureDateTimeBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            ValueProviderResult valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            if (valueProviderResult == null) return null;

            var date = bindingContext.ValueProvider.GetValue(bindingContext.ModelName).AttemptedValue;

            if (String.IsNullOrEmpty(date))
                return null;

            bindingContext.ModelState.SetModelValue(bindingContext.ModelName, bindingContext.ValueProvider.GetValue(bindingContext.ModelName));

            try
            {
                return DateTime.Parse(date);
            }
            catch (Exception)
            {
                bindingContext.ModelState.AddModelError(bindingContext.ModelName, String.Format("\"{0}\" no es una fecha válida", bindingContext.ModelName));
                return null;
            }
        }
    }
}