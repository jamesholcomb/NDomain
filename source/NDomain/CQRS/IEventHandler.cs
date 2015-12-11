using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NDomain.CQRS
{
   public interface IEventHandler<TEvent>
   {
      Task On(IEvent<TEvent> e);
   }
}