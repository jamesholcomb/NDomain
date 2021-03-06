﻿using NDomain.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NDomain.CQRS.Projections
{
    public abstract class QueryEventsHandler<T>
        where T : new()
    {
        readonly Dictionary<string, Action<T, IAggregateEvent>> handlers;

        readonly IQueryStore<T> queryStore;
        readonly IEventStore eventStore;

        protected QueryEventsHandler(IQueryStore<T> queryStore, IEventStore eventStore)
        {
            this.queryStore = queryStore;
            this.eventStore = eventStore;

            this.handlers = ReflectionUtils.FindQueryEventHandlerMethods<T>(this);
        }

        private bool TryGetEventHandler(string name, out Action<T, IAggregateEvent> handler)
        {
            return this.handlers.TryGetValue(name, out handler);
        }

        private Action<T, IAggregateEvent> GetEventHandler(string name)
        {
            return this.handlers[name];
        }

        protected async Task OnEvent(IAggregateEvent ev, string queryId = null)
        {
            var eventHandler = this.GetEventHandler(ev.Name);
            queryId = queryId ?? ev.AggregateId;

            var query = await this.queryStore.Get(queryId);

            if (query.Version == 0)
            {
                query.Data = new T();
            }

            var data = query.Data;

            if (query.Version >= ev.SequenceId)
            {
                // event already applied
                return;
            }

            if (query.Version < ev.SequenceId - 1)
            {
                var start = query.Version + 1;
                var end = ev.SequenceId - 1;
                var events = await this.eventStore.LoadRange(ev.AggregateId, start, end);

                foreach (var @event in events)
                {
                    Action<T, IAggregateEvent> evHandler;
                    if (this.TryGetEventHandler(@event.Name, out evHandler))
                    {
                        evHandler(data, @event);
                    }
                }
            }

            eventHandler(data, ev);

            query.Version = ev.SequenceId;
            query.DateUtc = DateTime.UtcNow;

            await this.queryStore.Set(queryId, query);
        }
    }
}
