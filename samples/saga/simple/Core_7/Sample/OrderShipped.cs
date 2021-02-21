namespace Sample
{
    using System;
    using NServiceBus;

    public record OrderShipped : IMessage
    {
        public Guid OrderId { get; init; }
    }
}