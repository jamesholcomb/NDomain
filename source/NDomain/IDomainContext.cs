using NDomain.CQRS;
using NDomain.IoC;
using NDomain.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NDomain
{
    public interface IDomainContext : IDisposable
    {
        IEventBus EventBus { get; }
        ICommandBus CommandBus { get; }

        void StartProcessors();
        void StopProcessors();
    }
}
