using NDomain.CQRS;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NDomain.Helpers
{
   internal static class ReflectionUtils
   {
      public static IEnumerable<Type> FindEventTypes(Type aggregateType)
      {
         var stateType = aggregateType.BaseType.GetGenericArguments()[0];

         var eventTypes = stateType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                                   .Where(m => m.Name.Length > 2 && m.Name.StartsWith("On") && m.GetParameters().Length == 1 && m.ReturnType == typeof(void))
                                   .Select(m => m.GetParameters()[0].ParameterType)
                                   .ToArray();

         return eventTypes;
      }

      public static Dictionary<Type, Func<THandler, ICommand, Task>> FindCommandHandlerMethods<THandler>()
      {
         var methods = typeof(THandler)
                               .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                               .Where(m => m.Name == "Handle"
                                        && m.GetParameters().Length == 1 && typeof(ICommand).IsAssignableFrom(m.GetParameters()[0].ParameterType)
                                        && m.GetParameters()[0].ParameterType.IsGenericType
                                        && m.ReturnType == typeof(Task))
                               .ToArray();

         var handlers = methods.ToDictionary(
                                 m => m.GetParameters()[0].ParameterType.GetGenericArguments()[0],
                                 m => BuildCommandHandler<THandler>(m));
         return handlers;
      }

      public static Dictionary<Type, Func<THandler, IEvent, Task>> FindEventHandlerMethods<THandler>()
      {
         var methods = typeof(THandler)
                               .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                               .Where(m => m.Name == "On"
                                        && m.GetParameters().Length == 1 && typeof(IEvent).IsAssignableFrom(m.GetParameters()[0].ParameterType)
                                        && m.GetParameters()[0].ParameterType.IsGenericType
                                        && m.ReturnType == typeof(Task))
                               .ToArray();

         var handlers = methods.ToDictionary(
                                 m => m.GetParameters()[0].ParameterType.GetGenericArguments()[0],
                                 m => BuildEventHandler<THandler>(m));
         return handlers;
      }

      private static Func<THandler, ICommand, Task> BuildCommandHandler<THandler>(MethodInfo method)
      {
         var instanceParam = Expression.Parameter(typeof(THandler), "instance");

         var commandType = method.GetParameters()[0].ParameterType;

         var commandParam = Expression.Parameter(typeof(ICommand), "cmd");

         // instance.Handle(cmd as ICommand<T>)
         var methodCallExpr = Expression.Call(
                                 instanceParam,
                                 method,
                                 Expression.Convert(commandParam, commandType));

         var lambda = Expression.Lambda<Func<THandler, ICommand, Task>>(methodCallExpr, instanceParam, commandParam);
         return lambda.Compile();
      }

      private static Func<THandler, IEvent, Task> BuildEventHandler<THandler>(MethodInfo method)
      {
         var instanceParam = Expression.Parameter(typeof(THandler), "instance");

         var eventType = method.GetParameters()[0].ParameterType;

         var eventParam = Expression.Parameter(typeof(IEvent), "ev");

         // instance.On(ev as IEvent<T>)
         var methodCallExpr = Expression.Call(
                                 instanceParam,
                                 method,
                                 Expression.Convert(eventParam, eventType));

         var lambda = Expression.Lambda<Func<THandler, IEvent, Task>>(methodCallExpr, instanceParam, eventParam);
         return lambda.Compile();
      }
   }
}
