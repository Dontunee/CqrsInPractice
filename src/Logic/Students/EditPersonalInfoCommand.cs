using CSharpFunctionalExtensions;
using Logic.Attributes;
using Logic.Dtos;
using Logic.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
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
    public interface ICommand
    {

    }

    public interface ICommandHandler<TCommand>  where TCommand : ICommand
    {
        Result Handle(TCommand command);
    }

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
    }

    [DatabaseRetry]
    [AuditLogRetry]
    public sealed class EditPersonalInfoCommandHandler : ICommandHandler<EditPersonalInfoCommand>
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

    public sealed class RegisterCommand: ICommand
    {
        public string Name { get;}

        public string Email { get;}

        public string FirstCourse { get;}

        public string FirstCourseGrade { get;}

        public string SecondCourse { get; }

        public string SecondCourseGrade { get;}

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
    }

    [DatabaseRetry]
    [AuditLogRetry]
    public sealed class RegisterCommandHandler : ICommandHandler<RegisterCommand>
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


    public sealed class UnRegisterCommand : ICommand
    {
        public long  Id { get; }

        public UnRegisterCommand(long id)
        {
            Id = id;
        }

    }

    [DatabaseRetry]
    [AuditLogRetry]
    public sealed class UnRegisterCommandHandler : ICommandHandler<UnRegisterCommand>
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

    }

    [DatabaseRetry]
    [AuditLogRetry]
    public sealed class EnrollCommandHandler : ICommandHandler<EnrollCommand>
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
    }

    [DatabaseRetry]
    [AuditLogRetry]
    public sealed class TransferCommandHandler : ICommandHandler<TransferCommand>
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

    public sealed class DisenrollCommand : ICommand
    {
        public long Id { get; }

        public int EnrollmentNumber { get; }

        public string Comment { get; }

        public DisenrollCommand(long id, int enrollmentNumber , string comment)
        {
            Id = id;
            EnrollmentNumber = enrollmentNumber;
            Comment = comment ?? throw new ArgumentNullException(nameof(comment));
        }

    }

    [DatabaseRetry]
    [AuditLogRetry]
    public sealed class DisenrollCommandHandler : ICommandHandler<DisenrollCommand>
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


    public sealed class GetListQuery : IQuery<List<StudentDto>>
    {
        public string EnrolledIn { get; }

        public int? NumberOfCourses { get; }

        public GetListQuery(string enrolledIn, int? numberOfCourses)
        {
            EnrolledIn = enrolledIn ?? throw new ArgumentNullException(nameof(enrolledIn));
            NumberOfCourses = numberOfCourses;
        }
    }

    [DatabaseRetry]
    [AuditLogRetry]
    public sealed class GetListQueryHandler : IQueryHandler<GetListQuery, List<StudentDto>>
    {
        private readonly SessionFactory _sessionFactory;

        public GetListQueryHandler(SessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory ?? throw new ArgumentNullException(nameof(sessionFactory));
        }

        public List<StudentDto> Handle(GetListQuery query)
        {
            var _unitOfWork = new UnitOfWork(_sessionFactory);
            return new StudentRepository(_unitOfWork).
                    GetList(query.EnrolledIn, query.NumberOfCourses)
                        .Select(x => ConvertToDto(x)).ToList();
        }

        private StudentDto ConvertToDto(Student student)
        {
            return new StudentDto
            {
                Id = student.Id,
                Name = student.Name,
                Email = student.Email,
                Course1 = student.FirstEnrollment?.Course?.Name,
                Course1Grade = student.FirstEnrollment?.Grade.ToString(),
                Course1Credits = student.FirstEnrollment?.Course?.Credits,
                Course2 = student.SecondEnrollment?.Course?.Name,
                Course2Grade = student.SecondEnrollment?.Grade.ToString(),
                Course2Credits = student.SecondEnrollment?.Course?.Credits,
            };
        }
    }



}
