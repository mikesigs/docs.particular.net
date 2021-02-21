namespace Sample
{
    using System;
    using NServiceBus;

    public record ShipOrder : IMessage
    {
        public Guid OrderId { get; init; }
    }
}