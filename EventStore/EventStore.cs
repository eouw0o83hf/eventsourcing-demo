using System;
using System.Collections.Generic;
using ConsoleApplication.Events;

namespace ConsoleApplication.EventStore
{
    public class EventStore
    {
        public static IEnumerable<EventContext> GetEvents(DateTimeOffset? asOf = null)
        {
            var orderId = Guid.NewGuid();

            yield return new EventContext
            {
                StreamId = orderId,
                Sequence = 1,
                Payload = new OrderInitialized
                {
                    OrderId = orderId
                }
            };

            yield return new EventContext
            {
                StreamId = orderId,
                Sequence = 2,
                Payload = new OrderSubmitted
                {
                    Timestamp = DateTimeOffset.MinValue
                }
            };

            yield return new EventContext
            {
                StreamId = orderId,
                Sequence = 3,
                Payload = new DeliveryCompleted
                {
                    Timestamp = DateTimeOffset.MinValue.AddHours(1)
                }
            };
        }
    }
}