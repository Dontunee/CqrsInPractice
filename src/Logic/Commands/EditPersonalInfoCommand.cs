using CSharpFunctionalExtensions;
using Logic.Attributes;
using Logic.Students;
using Logic.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Logic.Commands
{
    public sealed class EditPersonalInfoCommand : ICommand
    {
        public long Id { get; }

        public string Name { get; }

        public string Email { get; }

        public EditPersonalInfoCommand(string email, string name, long id)
        {
            Email = email ?? throw new ArgumentNullException(nameof(email));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Id = id;
        }



        [DatabaseRetry]
        [AuditLogRetry]
        internal sealed class EditPersonalInfoCommandHandler : ICommandHandler<EditPersonalInfoCommand>
        {
            private readonly SessionFactory _sessionFactory;

            public EditPersonalInfoCommandHandler(SessionFactory sessionFactory)
            {
                _sessionFactory = sessionFactory ?? throw new ArgumentNullException(nameof(sessionFactory));
            }

            public Result Handle(EditPersonalInfoCommand command)
            {
                var _unitOfWork = new UnitOfWork(_sessionFactory);
                var studentRepository = new StudentRepository(_unitOfWork);
                Student student = studentRepository.GetById(command.Id);
                if (student == null)
                    return Result.Fail($"No student found for Id {command.Id}");

                student.Name = command.Name;
                student.Email = command.Email;



                _unitOfWork.Commit();
                return Result.Ok();
            }
        }
    }
}
