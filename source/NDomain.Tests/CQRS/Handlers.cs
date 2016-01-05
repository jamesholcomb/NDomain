using NDomain.CQRS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NDomain.Tests.CQRS
{
    public class TestCommandHandler : ICommandHandler<DoSimple>
    {
        readonly Action<ICommand> onMsg;

        public TestCommandHandler(Action<ICommand> onMsg)
        {
            this.onMsg = onMsg;
        }

        public Task Handle(ICommand<DoSimple> cmd)
        {
            this.onMsg(cmd);
            return Task.FromResult(true);
        }

        public Task Handle(ICommand<DoComplex> cmd)
        {
            this.onMsg(cmd);
            return Task.FromResult(true);
        }

        public Task Handle(ICommand<DoSimpleStuff> cmd)
        {
            this.onMsg(cmd);
            return Task.FromResult(true);
        }

        public Task Handle(ICommand<DoComplexStuff> cmd)
        {
            this.onMsg(cmd);
            return Task.FromResult(true);
        }

        public Task Handle(ICommand<DoGenericStuff<DoComplexStuff>> cmd)
        {
            this.onMsg(cmd);
            return Task.FromResult(true);
        }

        public Task Handle(ICommand<DoNonGenericStuff> cmd)
        {
            this.onMsg(cmd);
            return Task.FromResult(true);
        }
    }

    public class CounterEventsHandler :
		IEventHandler<Sample.CounterIncremented>,
		IEventHandler<Sample.CounterResetEvent>,
		IEventHandler<Sample.CounterMultipliedEvent>
	{
        readonly Action<IEvent> onMsg;

        public CounterEventsHandler(Action<IEvent> onMsg)
        {
            this.onMsg = onMsg;
        }

        public Task On(IEvent<Sample.CounterIncremented> ev)
        {
            this.onMsg(ev);
            return Task.FromResult(true);
        }

        public Task On(IEvent<Sample.CounterMultipliedEvent> ev)
        {
            this.onMsg(ev);
            return Task.FromResult(true);
        }

        public Task On(IEvent<Sample.CounterResetEvent> ev)
        {
            this.onMsg(ev);
            return Task.FromResult(true);
        }
    }
}
