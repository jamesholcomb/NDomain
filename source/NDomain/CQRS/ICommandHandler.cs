using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NDomain.CQRS
{
   public interface ICommandHandler<TCommand>
   {
      Task Handle(ICommand<TCommand> c);
   }
}