using Vindi.Webhook.Models.Managers;

namespace Vindi.Webhook.Infrastructure.Middlewares
{
    public class TenantMiddleware(RequestDelegate next)
    {
        #region Propriedades

        private readonly RequestDelegate _next = next;

        #endregion

        #region Metodos publicos

        public async Task InvokeAsync(HttpContext context, TenantContext tenantContext)
        {
            var tenant = context.GetRouteValue("tenant")?.ToString();

            if (string.IsNullOrEmpty(tenant))
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsJsonAsync(ApiResponse<object>.Failure("Tenant not informed."));
                return;
            }

            tenantContext.Tenant = tenant;

            await _next(context);
        }
        #endregion
    }
}
