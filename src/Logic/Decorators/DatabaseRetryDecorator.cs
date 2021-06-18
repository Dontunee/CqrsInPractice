using CSharpFunctionalExtensions;
using Logic.Students;
using Logic.Utils;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace Logic.Decorators
{
    public sealed class DatabaseRetryDecorator<TCommand> : ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        private readonly ICommandHandler<TCommand> _handler;

        private Config _config { get; }
        public DatabaseRetryDecorator(ICommandHandler<TCommand> handler, Config config)
        {
            _handler = handler ?? throw new ArgumentNullException(nameof(handler));
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }


        public Result Handle(TCommand command)
        {
            for(int i = 0; ; i++)
            {
                try
                {
                    Result result = _handler.Handle(command);
                    return result;
                }
                catch (SqlException ex)
                {
                    if(i >=  _config.NumberOfRetries)
                    throw;
                }
            }
        }
    }
}
