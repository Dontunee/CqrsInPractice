using CSharpFunctionalExtensions;
using Logic.Attributes;
using Logic.Students;
using Logic.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Logic.Commands
{
    public sealed class TransferCommand : ICommand
    {
        public long Id { get; }

        public int EnrollmentNumber { get; }

        public string Course { get; }

        public string CourseGrade { get; }

        public TransferCommand(long id, int enrollmentNumber, string course, string courseGrade)
        {
            Id = id;
            EnrollmentNumber = enrollmentNumber;
            Course = course ?? throw new ArgumentNullException(nameof(course));
            CourseGrade = courseGrade ?? throw new ArgumentNullException(nameof(courseGrade));
        }


        [DatabaseRetry]
        [AuditLogRetry]
        internal sealed class TransferCommandHandler : ICommandHandler<TransferCommand>
        {
            private SessionFactory _sessionFactory;
            public TransferCommandHandler(SessionFactory sessionFactory)
            {
                _sessionFactory = sessionFactory ?? throw new ArgumentNullException(nameof(sessionFactory));
            }

            public Result Handle(TransferCommand command)
            {
                var _unitOfWork = new UnitOfWork(_sessionFactory);
                var studentRepository = new StudentRepository(_unitOfWork);
                Student student = studentRepository.GetById(command.Id);
                if (student == null)
                    return Result.Fail($"No student found for Id {command.Id}");

                var courseRepository = new CourseRepository(_unitOfWork);
                Course course = courseRepository.GetByName(command.Course);
                if (course == null)
                    return Result.Fail($"Course is incorrect: {command.Course}");


                bool success = Enum.TryParse(command.CourseGrade, out Grade grade);
                if (!success)
                    return Result.Fail($"Grade is incorrect: {command.CourseGrade}");

                Enrollment enrollment = student.GetEnrollment(command.EnrollmentNumber);
                if (enrollment == null)
                    return Result.Fail($"No enrollment found with number {command.EnrollmentNumber}");

                enrollment.Update(course, grade);

                _unitOfWork.Commit();

                return Result.Ok();
            }
        }


    }

}
