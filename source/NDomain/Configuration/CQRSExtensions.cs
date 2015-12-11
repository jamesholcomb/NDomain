using NDomain.Configuration;
using NDomain.Bus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NDomain.CQRS;
using NDomain.CQRS.Handlers;
using NDomain.Helpers;

namespace NDomain.Configuration
{
	public static class CQRSExtensions
	{
		/// <summary>
		/// Looks for methods with the signatures:
		/// On(ICommand<typeparamref name="T"/>)
		/// On(IEvent<typeparamref name="T"/>)
		/// On(IAggregateEvent<typeparamref name="T"/>) 
		/// and susbscribes as message handler for the specific message Type
		/// </summary>
		/// <typeparam name="THandler">Type of the message handler</typeparam>
		/// <param name="processorConfigurator">processorConfigurator used to register handler</param>
		/// <param name="instance">optional instance</param>
		/// <remarks>When instance is not provided, an instance of THandler will be resolved with the IoC container
		/// <returns>List of message names subscribed by this handler</returns>
		public static ProcessorConfigurator RegisterHandler<THandler>(this ProcessorConfigurator processorConfigurator,
																						  THandler instance = null)
			 where THandler : class
		{
			processorConfigurator.Configuring += processor => RegisterCQRSHandler(processor, instance);

			// add this type to IoC known types, so that it can be resolved
			// not pretty, but it's a compromise..
			processorConfigurator.Builder
				.IoCConfigurator
				.KnownTypes.Add(typeof(THandler));

			return processorConfigurator;
		}

		/// <summary>Registers all classes implementing <see cref="IEventHandler{TEvent}"/> or <see cref="ICommandHandler{TCommand}"/> in an assembly.</summary>
		/// <param name="processorConfigurator">The processor configurator.</param>
		/// <param name="assembly">The assembly.</param>
		/// <returns></returns>
		public static ProcessorConfigurator RegisterHandlers(this ProcessorConfigurator processorConfigurator,
			Assembly assembly)
		{
			var handlers = assembly
				.GetTypes()
				.Where(type => type
					.GetInterfaces()
					.Where(t => t.IsGenericType)
					.Where(t => t.GetGenericTypeDefinition() == typeof(IEventHandler<>) || t.GetGenericTypeDefinition() == typeof(ICommandHandler<>))
					.Any());

			foreach (var handler in handlers)
			{
				typeof(CQRSExtensions).GetMethod("RegisterHandler", BindingFlags.Static | BindingFlags.Public)
					.MakeGenericMethod(handler)
					.Invoke(null, new object[] { processorConfigurator, null });
			}

			return processorConfigurator;
		}

		public static ProcessorConfigurator RegisterCommandHandler<TMessage>(this ProcessorConfigurator processorConfigurator,
																									string handlerName,
																									Func<ICommand<TMessage>, Task> handlerFunc)
		{
			processorConfigurator.Configuring += processor => SubscribeCommand<TMessage, object>(
																				 processor,
																				 handlerName,
																				 (_, msg) => handlerFunc(msg),
																				 new object()); //dummy instance

			return processorConfigurator;
		}

		public static ProcessorConfigurator RegisterEventHandler<TMessage>(this ProcessorConfigurator processorConfigurator,
																									string handlerName,
																									Func<IEvent<TMessage>, Task> handlerFunc)
		{
			processorConfigurator.Configuring += processor => SubscribeEvent<TMessage, object>(
																				 processor,
																				 handlerName,
																				 (_, msg) => handlerFunc(msg),
																				 new object()); //dummy instance

			return processorConfigurator;
		}

		private static void RegisterCQRSHandler<THandler>(Processor processor, THandler instance = null)
			 where THandler : class
		{
			var commandHandlers = ReflectionUtils.FindCommandHandlerMethods<THandler>();
			var eventHandlers = ReflectionUtils.FindEventHandlerMethods<THandler>();

			var handlerName = typeof(THandler).Name;

			foreach (var handler in commandHandlers)
			{
				var messageType = handler.Key;
				typeof(CQRSExtensions).GetMethod("SubscribeCommand", BindingFlags.Static | BindingFlags.NonPublic)
												.MakeGenericMethod(messageType, typeof(THandler))
												.Invoke(null, new object[] { processor, handlerName, handler.Value, instance });
			}

			foreach (var handler in eventHandlers)
			{
				var messageType = handler.Key;
				typeof(CQRSExtensions).GetMethod("SubscribeEvent", BindingFlags.Static | BindingFlags.NonPublic)
												.MakeGenericMethod(messageType, typeof(THandler))
												.Invoke(null, new object[] { processor, handlerName, handler.Value, instance });
			}
		}

		private static void SubscribeCommand<TMessage, THandler>(Processor processor,
																					string handlerName,
																					Func<THandler, ICommand<TMessage>, Task> handlerFunc,
																					THandler instance = null)
			 where THandler : class
		{
			var handler = new CommandMessageHandler<TMessage, THandler>(handlerFunc, instance);

			processor.RegisterMessageHandler<TMessage>(handlerName, handler);
		}

		private static void SubscribeEvent<TMessage, THandler>(Processor processor,
																				 string handlerName,
																				 Func<THandler, IEvent<TMessage>, Task> handlerFunc,
																				 THandler instance = null)
			 where THandler : class
		{
			var handler = new EventMessageHandler<TMessage, THandler>(handlerFunc, instance);
			processor.RegisterMessageHandler<TMessage>(handlerName, handler);
		}
	}
}
