using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ConsoleApplication.Events;

namespace ConsoleApplication.Aggregate
{
    public class OrderAggregate
    {
        public Guid OrderId { get; set; }
        public OrderStatus Status { get; set; }

        public DateTimeOffset? SubmittedTimestamp { get; set; }
        public DateTimeOffset? DeliveredTimestamp { get; set; }

        public readonly IDictionary<Guid, CartItemModel> Cart = new Dictionary<Guid, CartItemModel>();

        private static readonly IDictionary<Type, MethodInfo> ApplyMethods = 
            typeof(OrderAggregate).GetMethods(BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.Public)
            .Concat(typeof(OrderAggregate).GetMethods(BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.NonPublic))
                .Where(a => a.Name.Equals("Apply", StringComparison.OrdinalIgnoreCase))
                .Select(a => new { Types = a.GetParameters(), Method = a })
                .Where(a => a.Types.Length == 1 && a.Types[0].ParameterType != typeof(object))
                .ToDictionary(a => a.Types[0].ParameterType, a => a.Method);

        public async Task Apply(object @event)
        {
            var eventType = @event.GetType();
            if(!ApplyMethods.ContainsKey(eventType))
            {
                throw new ArgumentException($"No Apply() method exists for type {eventType}");
            }
            var target = ApplyMethods[eventType];

            var result = target.Invoke(this, new[] { @event });
            var task = result as Task;
            if(task != null)
            {
                await task;
            }
        }

        private void Apply(OrderInitialized @event)
        {
            OrderId = @event.OrderId;
            Status = OrderStatus.Started;
        }

        private void Apply(OrderSubmitted @event)
        {
            Status = OrderStatus.Submitted;
            SubmittedTimestamp = @event.Timestamp;
        }

        private void Apply(DeliveryCompleted @event)
        {
            Status = OrderStatus.Delivered;
            DeliveredTimestamp = @event.Timestamp;
        }

        private void Apply(ItemAddedToCart @event)
        {
            Cart[@event.CartEntryId] = new CartItemModel
            {
                ItemId = @event.ItemId,
                Quantity = @event.Quantity,
                Price = @event.Price
            };
        }

        private void Apply(ItemRemovedFromCart @event)
        {
            if(Cart.ContainsKey(@event.CartEntryId))
            {
                Cart.Remove(@event.CartEntryId);
            }
        }
    }

    public enum OrderStatus
    {
        Started,
        Submitted,
        Delivered
    }
}