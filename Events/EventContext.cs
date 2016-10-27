using System;
using System.Collections.Generic;

namespace ConsoleApplication.Events
{
    public class EventContext
    {
        public Guid StreamId { get; set; }

        public int Sequence { get; set; }
        public DateTimeOffset Timestamp { get; set; }

        public IDictionary<string, string> Headers { get; set; }

        public object Payload { get; set; }
    }
}