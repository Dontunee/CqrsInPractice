using CSharpFunctionalExtensions;
using Logic.Attributes;
using Logic.Students;
using Logic.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Logic.Commands
{
    public sealed class DisenrollCommand : ICommand
    {
        public long Id { get; }

        public int EnrollmentNumber { get; }

        public string Comment { get; }

        public DisenrollCommand(long id, int enrollmentNumber, string comment)
        {
            Id = id;
            EnrollmentNumber = enrollmentNumber;
            Comment = comment ?? throw new ArgumentNullException(nameof(comment));
        }


        [DatabaseRetry]
        [AuditLogRetry]
        internal sealed class DisenrollCommandHandler : ICommandHandler<DisenrollCommand>
        {

            private readonly SessionFactory _sessionFactory;

            public DisenrollCommandHandler(SessionFactory sessionFactory)
            {
                _sessionFactory = sessionFactory ?? throw new ArgumentNullException(nameof(sessionFactory));
            }

            public Result Handle(DisenrollCommand command)
            {
                var _unitOfWork = new UnitOfWork(_sessionFactory);
                var studentRepository = new StudentRepository(_unitOfWork);
                Student student = studentRepository.GetById(command.Id);
                if (student == null)
                    return Result.Fail($"No student found for Id {command.Id}");

                if (string.IsNullOrWhiteSpace(command.Comment))
                    return Result.Fail("Disenrollment comment is required");


                Enrollment enrollment = student.GetEnrollment(command.EnrollmentNumber);
                if (enrollment == null)
                    return Result.Fail($"No enrollment found with number {command.EnrollmentNumber}");

                student.RemoveEnrollment(enrollment, command.Comment);

                _unitOfWork.Commit();

                return Result.Ok();
            }
        }
    }
}
