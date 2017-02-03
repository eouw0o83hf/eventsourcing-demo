using System;
using System.Collections.Generic;
using System.Linq;
using ConsoleApplication.Events;

namespace ConsoleApplication.EventStore
{
    public interface IProjection
    {
        void Consume(object @event);
    }

    public class NumberOfOrdersProjection : IProjection
    {
        public int NumberOfOrders = 0;

        public void Consume(object @event)
        {
            var orderedEvent = @event as OrderSubmitted;
            if(orderedEvent != null)
            {
                ++NumberOfOrders;
            }
        }
    }

    public class AverageDeliveryTimeProjection : IProjection
    {
        private readonly Dictionary<Guid, DateTimeOffset> _orderedDates = new Dictionary<Guid, DateTimeOffset>();
        private readonly Dictionary<Guid, TimeSpan> _completedTimes = new Dictionary<Guid, TimeSpan>();
        public double AverageDeliveryMilliseconds;

        public void Consume(object @event)
        {
            var orderedEvent = @event as OrderSubmitted;
            if(orderedEvent != null)
            {
                _orderedDates[Guid.Empty] = orderedEvent.Timestamp;
                return;
            }

            var completedEvent = @event as DeliveryCompleted;
            if(completedEvent != null)
            {
                _completedTimes[Guid.Empty] = completedEvent.Timestamp - _orderedDates[Guid.Empty];
                AverageDeliveryMilliseconds = _completedTimes.Values.Average(a => a.TotalMilliseconds);
            }
        }
    }

    public class EventStore
    {
        private static readonly List<object> Events = new List<object>();
        private static readonly List<IProjection> Projections = new List<IProjection>();

        public static void CommitEvent(object @event)
        {
            Events.Add(@event);
            foreach(var projection in Projections)
            {
                projection.Consume(@event);
            }
        }

        public static void RegisterProjection(IProjection projection)
        {
            Projections.Add(projection);
        }

        public static IEnumerable<EventContext> GetEvents(DateTimeOffset? asOf = null)
        {
            var orderId = Guid.NewGuid();

            return Events
                    .Select((payload, i) => WrapPayload(orderId, i, payload))
                    .Where(a => asOf == null || a.Timestamp <= asOf.Value)
                    .OrderBy(a => a.Sequence);
        }

        private static readonly DateTimeOffset BaseTime = new DateTimeOffset(2010, 6, 7, 10, 34, 18, 0, TimeSpan.Zero);

        public static IEnumerable<object> GetEventPayloads(Guid orderId)
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

            yield return new ItemRemovedFromCart
            {
                CartEntryId = cartEntryId
            };

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