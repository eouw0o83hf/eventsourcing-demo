using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ConsoleApplication.Events;

namespace ConsoleApplication.Aggregate
{
    public class CartItemModel
    {
        public Guid ItemId { get; set; }

        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}