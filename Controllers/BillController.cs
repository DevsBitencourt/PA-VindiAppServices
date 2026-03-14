using Microsoft.AspNetCore.Mvc;
using Vindi.Webhook.Infrastructure.Azure;
using Vindi.Webhook.Models.Bills;
using Vindi.Webhook.Models.Managers;

namespace Vindi.Webhook.Controllers
{
    [Controller]
    public class BillController : ControllerBase
    {
        private readonly TenantContext _tenantContext;
        private readonly ServiceBusService _busService;

        public BillController(TenantContext tenantContext, ServiceBusService busService)
        {
            _tenantContext = tenantContext;
            _busService = busService;
        }

        [HttpPost]
        public async Task<IActionResult> WebhookAsync([FromBody] WebhookRequest request)
        {
            var response = ApiResponse<WebhookRequest>.Success(request);
            response.Message = $"Tenant informado: {_tenantContext.Tenant}";

            if (request.@event.type.Equals("test", StringComparison.InvariantCultureIgnoreCase))
            {
                return new OkObjectResult(null);
            }

            await _busService.EnqueueAsync(_tenantContext.Tenant, response);

            return new OkObjectResult(response);
        }

        [HttpGet]
        public async Task<IActionResult> Dequeue()
        {
            var request = await _busService.DequeueAsync<ApiResponse<WebhookRequest>>(_tenantContext.Tenant);

            if (request == null)
                return new NotFoundObjectResult(ApiResponse<object>.Failure("Empty queue"));

            return new OkObjectResult(request);
        }

    }
}
