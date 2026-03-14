using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Vindi.Webhook.Infrastructure.Middlewares
{
    public class TenantRoutePrefixConvention : IApplicationModelConvention
    {
        public void Apply(ApplicationModel application)
        {
            foreach (var controller in application.Controllers)
            {
                var template = "api/{tenant}/" + controller.ControllerName.ToLower();

                // Define a rota no controller
                foreach (var selector in controller.Selectors)
                {
                    selector.AttributeRouteModel = new AttributeRouteModel(
                        new RouteAttribute(template)
                    );
                }

                // Define a rota nos actions também
                foreach (var action in controller.Actions)
                {
                    foreach (var selector in action.Selectors)
                    {
                        selector.AttributeRouteModel ??= new AttributeRouteModel();
                    }
                }
            }
        }
    }
}
