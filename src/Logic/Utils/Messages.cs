using CSharpFunctionalExtensions;
using Logic.Students;
using System;
using System.Collections.Generic;
using System.Text;

namespace Logic.Utils
{
    public sealed class Messages
    {
        private readonly IServiceProvider _provider;
        public Messages(IServiceProvider provider)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        public Result DispatchMethod(ICommand command)
        {
            Type type = typeof(ICommandHandler<>);
            Type[] typeArgs = { command.GetType() };
            Type handlerType = type.MakeGenericType(typeArgs);

            dynamic handler = _provider.GetService(handlerType);
            dynamic result = handler.Handle((dynamic)command);

            return result;
        }

        public T DispatchMethod<T>(IQuery<T> query)
        {
            Type type = typeof(IQueryHandler<,>);
            Type[] typeArgs = { query.GetType(), typeof(T) };
            Type handlerType = type.MakeGenericType(typeArgs);
            dynamic handler = _provider.GetService(handlerType);

            T result = handler.Handle((dynamic)query);

            return result;
        }
    }
}
