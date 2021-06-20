﻿using Logic.Attributes;
using Logic.Dtos;
using Logic.Students;
using Logic.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Logic.Queries
{
    public sealed class GetListQuery : IQuery<List<StudentDto>>
    {
        public string EnrolledIn { get; }

        public int? NumberOfCourses { get; }

        public GetListQuery(string enrolledIn, int? numberOfCourses)
        {
            EnrolledIn = enrolledIn ?? throw new ArgumentNullException(nameof(enrolledIn));
            NumberOfCourses = numberOfCourses;
        }


        [DatabaseRetry]
        [AuditLogRetry]
        internal sealed class GetListQueryHandler : IQueryHandler<GetListQuery, List<StudentDto>>
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

}