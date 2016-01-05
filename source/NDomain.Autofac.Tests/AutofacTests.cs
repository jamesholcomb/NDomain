using Autofac;
using NDomain.Configuration;
using NDomain.CQRS;
using NUnit.Framework;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace NDomain.Tests.CQRS
{
	/// <summary>
	/// Covers aggregate event handlers, since its the most relevant difference between EventBus and CommandBus
	/// and there's already a good set of tests for CommandBus
	/// </summary>
	public class AutofacTests
	{
		[Test]
		public async Task CanRegisterAndResolveComponents()
		{
			// arrange
			var sync = new CountdownEvent(3);

			var builder = new ContainerBuilder();
			builder.Register(c => new Action<IEvent>((x) => sync.AddCount())).AsSelf();
			builder.Register(c => new Action<ICommand>((x) => sync.AddCount())).AsSelf();
			var assembly = Assembly.GetExecutingAssembly();

			var handlers = assembly
				.GetTypes()
				.Where(type => type
					.GetInterfaces()
					.Where(t => t.IsGenericType)
					.Where(t =>
						t.GetGenericTypeDefinition() == typeof(IEventHandler<>) ||
						t.GetGenericTypeDefinition() == typeof(ICommandHandler<>))
					.Any());

			builder
				.RegisterAssemblyTypes(assembly)
				.Where(handlers.Contains)
				.AsSelf()
				.AsImplementedInterfaces()
				.PreserveExistingDefaults();

			var container = builder.Build();

			var ctx = DomainContext.Configure()
				.Logging(c => c.LoggerFactory = new Logging.TraceLoggerFactory())
				.IoC(c => c.WithAutofac(container))
				.Bus(b =>
					b.WithProcessor(p =>
						p.RegisterHandlers(Assembly.GetExecutingAssembly())))
				.Start();

			Assert.IsNotNull(container.Resolve<CounterEventsHandler>());
			Assert.IsNotNull(container.Resolve<TestCommandHandler>());
		}
	}
}
