using Azure.Messaging.ServiceBus;
using PedidoGestao.Infrastructure.Persistence;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using PedidoGestao.Infrastructure.Persistence;

namespace PedidoGestaoAPI.BackgroundServices
{ 
    public class OrderProcessor : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;
        private ServiceBusProcessor _processor;

        public OrderProcessor(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _configuration = configuration;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            var client = new ServiceBusClient(_configuration["ServiceBus:ConnectionString"]);
            _processor = client.CreateProcessor(_configuration["ServiceBus:QueueName"], new ServiceBusProcessorOptions());

            _processor.ProcessMessageAsync += ProcessMessageHandler;
            _processor.ProcessErrorAsync += ErrorHandler;

            await _processor.StartProcessingAsync(cancellationToken);
            await base.StartAsync(cancellationToken);
        }

        private async Task ProcessMessageHandler(ProcessMessageEventArgs args)
        {
            var body = args.Message.Body.ToString();
            var data = System.Text.Json.JsonSerializer.Deserialize<OrderMessage>(body);

            if (data?.Id != null)
            {
                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var order = await context.Orders.FirstOrDefaultAsync(o => o.Id == data.Id);
                if (order != null)
                {
                    order.Status = "Processando";
                    await context.SaveChangesAsync();

                    // Simula um tempo de processamento
                    await Task.Delay(5000);

                    order.Status = "Finalizado";
                    await context.SaveChangesAsync();
                }
            }

            await args.CompleteMessageAsync(args.Message);
        }

        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine($"Erro no Service Bus: {args.Exception.Message}");
            return Task.CompletedTask;
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await _processor.StopProcessingAsync(cancellationToken);
            await _processor.DisposeAsync();
            await base.StopAsync(cancellationToken);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.CompletedTask;
        }

        private class OrderMessage
        {
            public Guid Id { get; set; }
        }
    }

}

