using CSharpFunctionalExtensions;
using Logic.Attributes;
using Logic.Students;
using Logic.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Logic.Commands
{
    public sealed class UnRegisterCommand : ICommand
    {
        public long Id { get; }

        public UnRegisterCommand(long id)
        {
            Id = id;
        }



        [DatabaseRetry]
        [AuditLogRetry]
        internal sealed class UnRegisterCommandHandler : ICommandHandler<UnRegisterCommand>
        {
            private readonly SessionFactory _sessionFactory;

            public UnRegisterCommandHandler(SessionFactory sessionFactory)
            {
                _sessionFactory = sessionFactory ?? throw new ArgumentNullException(nameof(sessionFactory));
            }


            public Result Handle(UnRegisterCommand command)
            {
                var _unitOfWork = new UnitOfWork(_sessionFactory);
                var studentRepository = new StudentRepository(_unitOfWork);
                Student student = studentRepository.GetById(command.Id);
                if (student == null)
                    return Result.Fail($"No student found for Id {command.Id}");

                studentRepository.Delete(student);
                _unitOfWork.Commit();

                return Result.Ok();
            }
        }

    }

}
