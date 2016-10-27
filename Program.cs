using System;
using System.Threading.Tasks;
using ConsoleApplication.Aggregate;
using ConsoleApplication.Events;

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

        private static async Task MainAsync(string[] args)
        {
            var aggregate = await GetAggregateAsync();

            Console.WriteLine($"OrderId: {aggregate.OrderId}");   
            Console.WriteLine($"Order status: {aggregate.Status}");
            Console.WriteLine($"Delivery time: {aggregate.DeliveredTimestamp.Value - aggregate.SubmittedTimestamp.Value})");
        }

        public static async Task<OrderAggregate> GetAggregateAsync()
        {
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
