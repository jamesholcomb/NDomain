using NDomain.Bus;
using NDomain.Bus.Subscriptions;
using NDomain.Bus.Transport;
using NDomain.IoC;
using NDomain.Logging;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace NDomain.Configuration
{
	public class ProcessorConfigurator : Configurator
	{
		readonly Action<ProcessorConfigurator> configurer;

		public event Action<Processor> Configuring;
		public string EndpointName { get; set; }
		public int ConcurrencyLevel { get; set; }

		public ProcessorConfigurator(ContextBuilder builder, Action<ProcessorConfigurator> configurer)
			: base(builder)
		{
			this.configurer = configurer;
			this.EndpointName = Assembly.GetExecutingAssembly().GetName().Name;
			this.ConcurrencyLevel = 10; //default, define constant
		}

		public ProcessorConfigurator Endpoint(string name)
		{
			this.EndpointName = name;
			return this;
		}

		public ProcessorConfigurator WithConcurrencyLevel(int concurrencyLevel)
		{
			this.ConcurrencyLevel = concurrencyLevel;
			return this;
		}

		public ProcessorConfigurator RegisterMessageHandler<TMessage>(string handlerName, Func<TMessage, Task> handlerFunc)
		{
			this.Configuring += processor => processor.RegisterMessageHandler<TMessage>(
															handlerName,
															new MessageHandler<TMessage>(handlerFunc));

			return this;
		}

		public IProcessor Configure(ISubscriptionManager subscriptionManager,
									ITransportFactory messagingFactory,
									ILoggerFactory loggerFactory,
									IDependencyResolver resolver)
		{

			this.configurer(this);

			var processor = new Processor(this.EndpointName, this.ConcurrencyLevel, subscriptionManager, messagingFactory, loggerFactory, resolver);

			// handlers can be registered here
			if (this.Configuring != null)
			{
				this.Configuring(processor);
			}

			return processor;
		}
	}
}
