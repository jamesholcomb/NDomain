﻿using NDomain.Bus;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NDomain.CQRS
{
    public class EventBus : IEventBus
    {
        readonly IMessageBus messageBus;

        public EventBus(IMessageBus messageBus)
        {
            this.messageBus = messageBus;
        }

        public Task Publish<T>(IEvent<T> @event)
        {
            var message = BuildMessage(@event);
            return this.messageBus.Send(message);
        }

        public Task Publish(IEnumerable<IEvent> events)
        {
            var messages = events.Select(e => BuildMessage(e)).ToArray();
            return this.messageBus.Send(messages);
        }

        private Message BuildMessage(IEvent ev)
        {
            var headers = new Dictionary<string, string> 
            {
                { CqrsMessageHeaders.DateUtc, ev.DateUtc.ToBinary().ToString() }
            };

            var message = new Message(ev.Payload, ev.Name, headers);
            return message;
        }
    }
}
