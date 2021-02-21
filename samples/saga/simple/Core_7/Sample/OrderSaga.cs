using System;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;
using Sample;

#region thesaga
public class OrderSaga :
    Saga<OrderSagaData>,
    IAmStartedByMessages<StartOrder>,
    IHandleMessages<CompleteOrder>,
    IHandleTimeouts<CancelOrder>,
    IHandleMessages<OrderShipped>
{
    static ILog log = LogManager.GetLogger<OrderSaga>();

    protected override void ConfigureHowToFindSaga(SagaPropertyMapper<OrderSagaData> mapper)
    {
        mapper.ConfigureMapping<StartOrder>(message => message.OrderId)
                .ToSaga(sagaData => sagaData.OrderId);
        mapper.ConfigureMapping<CompleteOrder>(message => message.OrderId)
                .ToSaga(sagaData => sagaData.OrderId);
    }

    public async Task Handle(StartOrder message, IMessageHandlerContext context)
    {
        // Correlation property Data.OrderId is automatically assigned with the value from message.OrderId;
        log.Info($"StartOrder received with OrderId {message.OrderId}");
        var shipOrder = new ShipOrder
        {
            OrderId = message.OrderId
        };

        await context.SendLocal(shipOrder);
        log.Info(@"Sending a ShipOrder message.");

        var timeout = DateTime.UtcNow.AddSeconds(30);
        log.Info(@"Requesting a CancelOrder that will be executed in 30 seconds. Stop the endpoint now to see the timeout data");
        await RequestTimeout<CancelOrder>(context, timeout)
            .ConfigureAwait(false);
    }

    public Task Handle(CompleteOrder message, IMessageHandlerContext context)
    {
        log.Info($"CompleteOrder received with OrderId {message.OrderId}");
        MarkAsComplete();
        return Task.CompletedTask;
    }

    public Task Timeout(CancelOrder state, IMessageHandlerContext context)
    {
        log.Info($"CompleteOrder not received soon enough OrderId {Data.OrderId}. Calling MarkAsComplete");
        MarkAsComplete();
        return Task.CompletedTask;
    }

    public async Task Handle(OrderShipped message, IMessageHandlerContext context)
    {
        log.Info($"OrderShipped was received. Completing OrderId {Data.OrderId}");
        var completeOrder = new CompleteOrder
        {
            OrderId = Data.OrderId
        };
        var sendOptions = new SendOptions();
        sendOptions.DelayDeliveryWith(TimeSpan.FromSeconds(60));
        sendOptions.RouteToThisEndpoint();
        await context.Send(completeOrder, sendOptions)
            .ConfigureAwait(false);
    }
}
#endregion
