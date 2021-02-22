using System;
using NServiceBus;

public record OrderSagaData : IContainSagaData
{
    public Guid Id { get; set; }
    public string Originator { get; set; }
    public string OriginalMessageId { get; set; }
    public Guid OrderId { get; set; }
    public OrderData Order { get; init; }
}

public record OrderData
{
    public bool OrderShipped { get; set; }
    public bool OrderStarted { get; set; }
}