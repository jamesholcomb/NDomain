using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NDomain.Tests.Configuration
{
   public class DomainContextTests
   {
      [Test]
      public void CanConfigureJustEventSourcing()
      {
         // act
         var context = DomainContext.Configure()
                                    .Start();

         // assert
         Assert.NotNull(context);
         Assert.NotNull(context.CommandBus);
         Assert.NotNull(context.EventBus);
         var ctx = context as DomainContext;
         Assert.NotNull(ctx.LoggerFactory);
         Assert.NotNull(ctx.Resolver);
      }
   }
}
