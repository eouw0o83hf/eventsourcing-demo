using System;
using System.Collections.Generic;
using System.Linq;
using ConsoleApplication.Events;

namespace ConsoleApplication.EventStore
{
    public class EventStore
    {
        public static IEnumerable<EventContext> GetEvents(DateTimeOffset? asOf = null)
        {
            var orderId = Guid.NewGuid();

            return GetEventPayloads(orderId)
                    .Select((payload, i) => WrapPayload(orderId, i, payload))
                    .Where(a => asOf == null || a.Timestamp <= asOf.Value)
                    .OrderBy(a => a.Sequence);
        }

        private static readonly DateTimeOffset BaseTime = new DateTimeOffset(2010, 6, 7, 10, 34, 18, 0, TimeSpan.Zero);

        private static IEnumerable<object> GetEventPayloads(Guid orderId)
        {                        
            yield return new OrderInitialized
            {
                OrderId = orderId
            };

            var itemId = Guid.NewGuid();
            var cartEntryId = Guid.NewGuid();

            yield return new ItemAddedToCart
            {
                ItemId = itemId,
                CartEntryId = cartEntryId,
                Quantity = 1,
                Price = 18.20m
            };

            // yield return new ItemRemovedFromCart
            // {
            //     CartEntryId = cartEntryId
            // };

            itemId = Guid.NewGuid();
            cartEntryId = Guid.NewGuid();

            yield return new ItemAddedToCart
            {
                ItemId = itemId,
                CartEntryId = cartEntryId,
                Quantity = 1000000,
                Price = 18471284.27m
            };

            yield return new OrderSubmitted
            {
                Timestamp = BaseTime.AddSeconds(1)
            };

            yield return new DeliveryCompleted
            {
                Timestamp = BaseTime.AddSeconds(2)
            };
        }

        private static EventContext WrapPayload(Guid streamId, int sequence, object payload)
        {
            return new EventContext
            {
                StreamId = streamId,
                Sequence = sequence,
                Timestamp = BaseTime.AddSeconds(sequence),
                Payload = payload
            };
        }
    }
}