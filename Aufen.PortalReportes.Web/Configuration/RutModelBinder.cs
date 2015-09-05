using Aufen.PortalReportes.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Aufen.PortalReportes.Web.Configuration
{
    public class RutModelBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            string modelName = bindingContext.ModelName;
            var valueProvider = bindingContext.ValueProvider.GetValue(modelName);
            bindingContext.ModelState.SetModelValue(modelName, valueProvider);

            if (valueProvider == null)
            {
                return null;
            }

            if (bindingContext.ModelType != typeof(Rut) ||
                String.IsNullOrEmpty(valueProvider.AttemptedValue))
            {
                return base.BindModel(controllerContext, bindingContext);
            }

            Rut model;

            if (Rut.TryParse(valueProvider.AttemptedValue, out model) && model.EsValido)
            {
                return model;
            }
            else
            {
                string displayName = bindingContext.ModelMetadata.DisplayName;
                string mensajeError = String.Format("El campo {0} no es válido. Verifique que el formato y el dígito verificador sean correctos.", displayName);
                bindingContext.ModelState.AddModelError(modelName, mensajeError);
                return null;
            }
        }
    }
}