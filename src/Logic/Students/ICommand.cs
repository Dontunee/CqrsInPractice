using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Logic.Students
{
    public interface ICommand
    {

    }

    public interface ICommandHandler<TCommand> where TCommand : ICommand
    {
        Result Handle(TCommand command);
    }


}
