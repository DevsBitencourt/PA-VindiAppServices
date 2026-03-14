using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using System.Text.Json;

namespace Vindi.Webhook.Infrastructure.Azure
{
    public class ServiceBusService
    {
        #region Propriedades

        private readonly ServiceBusClient _client;
        private readonly ServiceBusAdministrationClient _adminClient;

        #endregion

        #region Construtores

        public ServiceBusService(IConfiguration configuration)
        {
            var connectionString = configuration["connectionstring"];

            _client = new ServiceBusClient(connectionString);
            _adminClient = new ServiceBusAdministrationClient(connectionString);
        }

        #endregion

        #region Metodos private

        private static string GetQueueName(string tenant) => $"client-{tenant}".ToLower();

        private async Task EnsureQueueAsync(string tenant)
        {
            var queueName = GetQueueName(tenant);
           
            if (!await _adminClient.QueueExistsAsync(queueName))
            {
                var options = new CreateQueueOptions(queueName)
                {
                    // Tempo máximo que a mensagem fica na fila sem ser processada
                    DefaultMessageTimeToLive = TimeSpan.FromMinutes(5),

                    // Tempo que a mensagem fica bloqueada para outro consumidor
                    LockDuration = TimeSpan.FromSeconds(30),

                    // Tamanho máximo da fila em MB (1024 = 1GB)
                    MaxSizeInMegabytes = 1024,

                    // Máximo de tentativas antes de ir para Dead Letter
                    MaxDeliveryCount = 5,

                    // Fila de mensagens mortas (falhou todas as tentativas)
                    DeadLetteringOnMessageExpiration = true,

                    // Detecta mensagens duplicadas em um intervalo de tempo
                    RequiresDuplicateDetection = false,

                    // Garante ordem de entrega (FIFO)
                    RequiresSession = false,

                    // Deleta a fila automaticamente se ficar inativa
                    AutoDeleteOnIdle = TimeSpan.FromDays(30),
                };

                await _adminClient.CreateQueueAsync(options);
            }
        }

        #endregion

        #region Metodos publicos

        public async Task EnqueueAsync<T>(string tenant, T item)
        {
            await EnsureQueueAsync(tenant);
            var sender = _client.CreateSender(GetQueueName(tenant));
            var json = JsonSerializer.Serialize(item);
            await sender.SendMessageAsync(new ServiceBusMessage(json));
        }

        public async Task<T?> DequeueAsync<T>(string tenant)
        {
            await EnsureQueueAsync(tenant);
            var receiver = _client.CreateReceiver(GetQueueName(tenant));
            var message = await receiver.ReceiveMessageAsync(maxWaitTime: TimeSpan.FromSeconds(10));

            if (message == null) return default;

            await receiver.CompleteMessageAsync(message);
            return JsonSerializer.Deserialize<T>(message.Body.ToString());
        }

        #endregion
    }
}
