using CSharpFunctionalExtensions;
using Logic.Attributes;
using Logic.Students;
using Logic.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Logic.Commands
{
    public sealed class EnrollCommand : ICommand
    {
        public long Id { get; }

        public string Course { get; }

        public string CourseGrade { get; }

        public EnrollCommand(long id, string course, string courseGrade)
        {
            Id = id;
            Course = course ?? throw new ArgumentNullException(nameof(course));
            CourseGrade = courseGrade ?? throw new ArgumentNullException(nameof(courseGrade));
        }


        [DatabaseRetry]
        [AuditLogRetry]
        internal sealed class EnrollCommandHandler : ICommandHandler<EnrollCommand>
        {
            private readonly SessionFactory _sessionFactory;

            public EnrollCommandHandler(SessionFactory sessionFactory)
            {
                _sessionFactory = sessionFactory ?? throw new ArgumentNullException(nameof(sessionFactory));
            }


            public Result Handle(EnrollCommand command)
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



                // Student enrolls
                student.Enroll(course, grade);

                _unitOfWork.Commit();

                return Result.Ok();
            }
        }

    }


}
