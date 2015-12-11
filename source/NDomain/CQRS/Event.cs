using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NDomain.CQRS
{
	// added jch
	public class Event : IEvent
	{
		public Event(object payload)
		{
			this.dateUtc = DateTime.UtcNow;
			this.name = payload.GetType().Name;
			this.payload = payload;
		}

		readonly DateTime dateUtc;
		readonly string name;
		readonly object payload;

		object IEvent.Payload
		{
			get { return this.payload; }
		}

		string IEvent.Name { get { return this.name; } }

		DateTime IEvent.DateUtc
		{
			get { return this.dateUtc; }
		}
	}

	public class Event<T> : IEvent<T>
	{
		readonly DateTime dateUtc;
		readonly string name;
		readonly T payload;

		public Event(T payload)
		   : this(DateTime.UtcNow, typeof(T).Name, payload)
		{

		}

		public Event(DateTime dateUtc, T payload)
			: this(dateUtc, typeof(T).Name, payload)
		{

		}

		public Event(DateTime dateUtc, string name, T payload)
		{
			this.dateUtc = dateUtc;
			this.name = name;
			this.payload = payload;
		}

		public DateTime DateUtc { get { return this.dateUtc; } }
		public string Name { get { return this.name; } }
		public T Payload { get { return this.payload; } }

		object IEvent.Payload
		{
			get { return this.payload; }
		}
	}
}