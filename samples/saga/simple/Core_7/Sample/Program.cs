using System;
using System.Threading.Tasks;
using NServiceBus;
using System.Data.SqlClient;

class Program
{
    static async Task Main()
    {
        Console.Title = "Samples.SimpleSaga";
        var endpointConfiguration = new EndpointConfiguration("Samples.SimpleSaga");

        endpointConfiguration.EnableInstallers();
        #region config

        var connection = "Data Source=192.168.0.114;User=sa;Password=GQI1qNeq0oEHlL";
        endpointConfiguration.UseTransport<RabbitMQTransport>()
            .ConnectionString("host=192.168.0.114;username=rabbitmq;password=rabbitmq")
            .UseConventionalRoutingTopology();

        var persistence = endpointConfiguration.UsePersistence<SqlPersistence>();

        persistence.SqlDialect<SqlDialect.MsSqlServer>();
        persistence.ConnectionBuilder(
            connectionBuilder: () =>
            {
                return new SqlConnection(connection);
            });

        #endregion

        var endpointInstance = await Endpoint.Start(endpointConfiguration)
            .ConfigureAwait(false);

        Console.WriteLine();
        Console.WriteLine("Press 'Enter' to send a StartOrder message");
        Console.WriteLine("Press any other key to exit");

        while (true)
        {
            Console.WriteLine();
            if (Console.ReadKey().Key != ConsoleKey.Enter)
            {
                break;
            }
            var orderId = Guid.NewGuid();
            var startOrder = new StartOrder
            {
                OrderId = orderId
            };
            await endpointInstance.SendLocal(startOrder)
                .ConfigureAwait(false);
            Console.WriteLine($"Sent StartOrder with OrderId {orderId}.");
        }

        await endpointInstance.Stop()
            .ConfigureAwait(false);
    }
}