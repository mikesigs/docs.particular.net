namespace Sample
{
    using System.Threading.Tasks;
    using NServiceBus;
    using NServiceBus.Logging;

    public class OrderShippingHandler : IHandleMessages<ShipOrder>
    {
        static ILog log = LogManager.GetLogger<OrderShippingHandler>();

        public Task Handle(ShipOrder message, IMessageHandlerContext context)
        {
            log.Info("Shipping order");
            return context.Reply(new OrderShipped
            {
                OrderId = message.OrderId
            });
        }
    }
}