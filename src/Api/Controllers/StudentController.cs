using System;
using System.Collections.Generic;
using System.Linq;
using CSharpFunctionalExtensions;
using Logic.Commands;
using Logic.Dtos;
using Logic.Queries;
using Logic.Students;
using Logic.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/students")]
    public sealed class StudentController : BaseController
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly StudentRepository _studentRepository;
        private readonly CourseRepository _courseRepository;
        private readonly Messages _messages;

        public StudentController(UnitOfWork unitOfWork, Messages messages)
        {
            _unitOfWork = unitOfWork;
            _studentRepository = new StudentRepository(unitOfWork);
            _courseRepository = new CourseRepository(unitOfWork);
            _messages = messages ?? throw new ArgumentNullException(nameof(messages));
        }

        //Query
        [HttpGet]
        public IActionResult GetList(string enrolled, int? number)
        {
            var query = new GetListQuery(enrolled, number);
            var result = _messages.DispatchMethod(query);

            return result != null ? Ok(result) : Error("Empty list of students");
        }


        //Command
        [HttpPut("{id}")]
        public IActionResult EditPersonalInfo(long id, [FromBody] StudentPersonalInfoDto dto)
        {
            var command = new EditPersonalInfoCommand(dto.Email, dto.Name, id);

            Result result = _messages.DispatchMethod(command);

            return FromResult(result);
        }


        //Command
        [HttpPost]
        public IActionResult Register([FromBody] NewStudentDto dto)
        {
          
            var command = new RegisterCommand(dto.Name, dto.Email, dto.FirstCourse,
                                dto.FirstCourseGrade, dto.SecondCourse, dto.SecondCourseGrade);
            Result result = _messages.DispatchMethod(command);


            return FromResult(result);
        }


        //Command
        [HttpDelete("{id}")]
        public IActionResult UnRegister(long id)
        {

            var command = new UnRegisterCommand(id);

            Result result = _messages.DispatchMethod(command);


            return FromResult(result);
        }


     


        //Command
        [HttpPost("{id}")]
        public IActionResult Enroll(long id, [FromBody] StudentEnrollmentDto dto)
        {
            var command = new EnrollCommand(id, dto.Course, dto.CourseGrade);

            Result result = _messages.DispatchMethod(command);


            return FromResult(result);

        }

        //Command
        [HttpPut("{id}/enrollments/{enrollmentNumber}")]
        public IActionResult Transfer(long id, int enrollmentNumber , [FromBody] StudentTransferDto dto)
        {
            var command = new TransferCommand(id, enrollmentNumber, dto.Course, dto.CourseGrade);

            Result result = _messages.DispatchMethod(command);


            return FromResult(result);
        }


        //Command
        [HttpPut("{id}/enrollments/{enrollmentNumber}/deletion")]
        public IActionResult Disenroll(long id, int enrollmentNumber, [FromBody] StudentDisenrollmentDto dto)
        {
            var command = new DisenrollCommand(id, enrollmentNumber, dto.Comment);

            Result result = _messages.DispatchMethod(command);


            return FromResult(result);

        }

    }
}
