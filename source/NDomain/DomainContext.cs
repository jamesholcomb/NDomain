using NDomain.Configuration;
using NDomain.IoC;
using NDomain.Logging;
using NDomain.Bus.Transport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NDomain.Bus;
using NDomain.CQRS;

namespace NDomain
{
	public class DomainContext : IDomainContext
	{
		readonly IEventBus eventBus;
		readonly ICommandBus commandBus;
		readonly IEnumerable<IProcessor> processors;
		readonly ILoggerFactory loggerFactory;
		readonly IDependencyResolver resolver;


		public DomainContext(
							 IEventBus eventBus,
							 ICommandBus commandBus,
							 IEnumerable<IProcessor> processors,
							 ILoggerFactory loggerFactory,
							 IDependencyResolver resolver)
		{
			this.eventBus = eventBus;
			this.commandBus = commandBus;
			this.processors = processors;
			this.loggerFactory = loggerFactory;
			this.resolver = resolver;
		}

        public IEventBus EventBus { get { return this.eventBus; } }
        public ICommandBus CommandBus { get { return this.commandBus; } }
        public ILoggerFactory LoggerFactory { get { return this.loggerFactory; } }
        public IDependencyResolver Resolver { get { return this.resolver; } }

        public void StartProcessors()
        {
            foreach (var processor in this.processors)
            {
                processor.Start();
            }
        }

        public void StopProcessors()
        {
            foreach (var processor in this.processors)
            {
                processor.Stop();
            }
        }

        public void Dispose()
        {
            foreach (var processor in processors)
            {
                processor.Dispose();
            }
        }

        public static ContextBuilder Configure() 
        {
            return new ContextBuilder();
        }
    }
}
