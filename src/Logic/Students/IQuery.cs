using System;
using System.Collections.Generic;
using System.Text;

namespace Logic.Students
{

    public interface IQuery<TResult>
    {

    }

    public interface IQueryHandler<TQuery, TResult> where TQuery : IQuery<TResult>
    {
        TResult Handle(TQuery query);
    }
}
