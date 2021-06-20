using CSharpFunctionalExtensions;
using Logic.Attributes;
using Logic.Students;
using Logic.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Logic.Commands
{
    public sealed class RegisterCommand : ICommand
    {
        public string Name { get; }

        public string Email { get; }

        public string FirstCourse { get; }

        public string FirstCourseGrade { get; }

        public string SecondCourse { get; }

        public string SecondCourseGrade { get; }

        public RegisterCommand(string name, string email, string firstCourse, string firstCourseGrade,
                                string secondCourse, string secondCourseGrade)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Email = email ?? throw new ArgumentNullException(nameof(email));
            FirstCourse = firstCourse ?? throw new ArgumentNullException(nameof(firstCourse));
            FirstCourseGrade = firstCourseGrade ?? throw new ArgumentNullException(nameof(firstCourseGrade));
            SecondCourse = secondCourse ?? throw new ArgumentNullException(nameof(secondCourse));
            SecondCourseGrade = secondCourseGrade ?? throw new ArgumentNullException(nameof(secondCourseGrade));
        }



        [DatabaseRetry]
        [AuditLogRetry]
        internal sealed class RegisterCommandHandler : ICommandHandler<RegisterCommand>
        {
            private readonly SessionFactory _sessionFactory;

            public RegisterCommandHandler(SessionFactory sessionFactory)
            {
                _sessionFactory = sessionFactory ?? throw new ArgumentNullException(nameof(sessionFactory));
            }


            public Result Handle(RegisterCommand command)
            {
                var student = new Student(command.Name, command.Email);

                var _unitOfWork = new UnitOfWork(_sessionFactory);

                if (command.FirstCourse != null && command.FirstCourseGrade != null)
                {
                    var courseRepository = new CourseRepository(_unitOfWork);
                    Course course = courseRepository.GetByName(command.FirstCourse);
                    student.Enroll(course, Enum.Parse<Grade>(command.FirstCourseGrade));
                }

                if (command.SecondCourse != null && command.SecondCourseGrade != null)
                {
                    var courseRepository = new CourseRepository(_unitOfWork);
                    Course course = courseRepository.GetByName(command.SecondCourse);
                    student.Enroll(course, Enum.Parse<Grade>(command.SecondCourseGrade));
                }


                _unitOfWork.Commit();
                return Result.Ok();
            }
        }
    }
}
