using CSharpFunctionalExtensions;
using Logic.Students;
using System;
using System.Collections.Generic;
using System.Text;

namespace Logic.Decorators
{
    public sealed class AuditLoggingDecorator<TCommand> : ICommandHandler<TCommand>
            where TCommand : ICommand
    {
        private readonly ICommandHandler<TCommand> _handler;
        public AuditLoggingDecorator(ICommandHandler<TCommand> handler)
        {
            _handler = handler ?? throw new ArgumentNullException(nameof(handler));
        }


        public Result Handle(TCommand command)
        {

        }
    }
}
