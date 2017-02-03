using System;
using System.Threading.Tasks;
using ConsoleApplication.Aggregate;
using ConsoleApplication.Events;
using ConsoleApplication.EventStore;

namespace ConsoleApplication
{
    public class Program
    {         
        public static void Main(string[] args)
        {
            var main = MainAsync(args);
            try
            {
                main.Wait();
            }
            catch(Exception ex)
            {
                Console.Error.WriteLine("Something screwed up: {0}", ex);
            }
        }

        private static NumberOfOrdersProjection NumberOfOrders = new NumberOfOrdersProjection();
        private static AverageDeliveryTimeProjection AverageDeliveryTime = new AverageDeliveryTimeProjection();

        private static async Task MainAsync(string[] args)
        {
            var aggregate = await GetAggregateAsync();

            Console.WriteLine($"OrderId: {aggregate.OrderId}");   
            Console.WriteLine($"Order status: {aggregate.Status}");
            Console.WriteLine($"Delivery time: {aggregate.DeliveredTimestamp.Value - aggregate.SubmittedTimestamp.Value}");

            Console.WriteLine($"Items in cart: {aggregate.Cart.Count}");
            foreach(var cartItem in aggregate.Cart)
            {
                Console.WriteLine($"Ordered {cartItem.Value.Quantity} of {cartItem.Value.ItemId} for ${cartItem.Value.Price}");
            }

            Console.WriteLine($"Number of orders from projection: {NumberOfOrders.NumberOfOrders}");
            Console.WriteLine($"AverageDeliveryTime: {AverageDeliveryTime.AverageDeliveryMilliseconds}");
        }

        public static async Task<OrderAggregate> GetAggregateAsync()
        {
            EventStore.EventStore.RegisterProjection(NumberOfOrders);
            EventStore.EventStore.RegisterProjection(AverageDeliveryTime);

            foreach(var @event in EventStore.EventStore.GetEventPayloads(Guid.NewGuid()))
            {
                EventStore.EventStore.CommitEvent(@event);
            }

            var messages = EventStore.EventStore.GetEvents();   
            var aggregate = new OrderAggregate();

            foreach(var m in messages)
            {
                await aggregate.Apply(m.Payload);
            }

            return aggregate;
        }
    }
}
