using System;

namespace ConsoleApplication.Events
{
    public class OrderInitialized
    {        
        public Guid OrderId { get; set; }
    }

    public class OrderSubmitted
    {
        public DateTimeOffset Timestamp { get; set; }
    }

    public class DeliveryCompleted
    {
        public DateTimeOffset Timestamp { get; set; }
    }
}