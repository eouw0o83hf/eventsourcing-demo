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

    public class ItemAddedToCart
    {
        public Guid ItemId { get; set; }
        public Guid CartEntryId { get; set; }

        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    public class ItemRemovedFromCart
    {
        public Guid CartEntryId { get; set; }
    }
}