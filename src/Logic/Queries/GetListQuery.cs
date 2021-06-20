using Dapper;
using Logic.Attributes;
using Logic.Dtos;
using Logic.Students;
using Logic.Utils;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
            private readonly QueriesConnectionString _connectionString;

            public GetListQueryHandler(QueriesConnectionString connectionString)
            {
                _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            }

            public List<StudentDto> Handle(GetListQuery query)
            {
                string sql = @"
                    SELECT s.StudentID Id, s.Name, s.Email,
	                    s.FirstCourseName Course1, s.FirstCourseCredits Course1Credits, s.FirstCourseGrade Course1Grade,
	                    s.SecondCourseName Course2, s.SecondCourseCredits Course2Credits, s.SecondCourseGrade Course2Grade
                    FROM dbo.Student s
                    WHERE (s.FirstCourseName = @Course
		                    OR s.SecondCourseName = @Course
		                    OR @Course IS NULL)
                        AND (s.NumberOfEnrollments = @Number
                            OR @Number IS NULL)
                    ORDER BY s.StudentID ASC";

                using(SqlConnection connection = new SqlConnection(_connectionString.Value))
                {
                    List<StudentDto> students = connection
                                                    .Query<StudentDto>(sql, new
                                                    {
                                                        Course = query.EnrolledIn,
                                                        Number = query.NumberOfCourses
                                                    }).ToList();

                    return students;
                }

                //using (SqlConnection connection = new SqlConnection(_connectionString.Value))
                //{

                //    //Execute the query using filteration parameter from the query object
                //    List<StudentInDb> students = connection
                //                                    .Query<StudentInDb>(sql, new
                //                                    {
                //                                        Course = query.EnrolledIn,
                //                                        Number = query.NumberOfCourses
                //                                    }).ToList();

                //    //Extract all identifiers out of the result 
                //    List<long> ids = students.GroupBy(x => x.StudentID)
                //                       .Select(X => X.Key)
                //                        .ToList();

                //    var result = new List<StudentDto>();

                //    foreach (long id in ids)
                //    {
                //        //select all the data from the collection 
                //        List<StudentInDb> data = students.Where(x => x.StudentID == id).ToList();

                //        //Transform it into a dto

                //        var dto = new StudentDto
                //        {
                //            Id = data[0].StudentID,
                //            Name = data[0].Name,
                //            Email = data[0].Email,
                //            Course1 = data[0].CourseName,
                //            Course1Credits = data[0].Credits,
                //            Course1Grade = data[0].Grade.ToString()
                //        };

                //        if (data.Count > 1)
                //        {
                //            dto.Course2 = data[1].CourseName;
                //            dto.Course2Credits = data[1].Credits;
                //            dto.Course2Grade = data[1].Grade.ToString();
                //        }

                //        //Add dto to the output list
                //        result.Add(dto);
                //    }

                //    return result;

                //}
            }

       
        }


        //private class StudentInDb
        //{
        //    public readonly long StudentID;
        //    public readonly string Name;
        //    public readonly string Email;
        //    public readonly Grade? Grade;
        //    public readonly string CourseName;
        //    public readonly int? Credits;

        //    public StudentInDb(long studentID, string name, string email, 
        //        Grade? grade, string courseName, int? credits)
        //    {
        //        StudentID = studentID;
        //        Name = name ?? throw new ArgumentNullException(nameof(name));
        //        Email = email ?? throw new ArgumentNullException(nameof(email));
        //        Grade = grade;
        //        CourseName = courseName ?? throw new ArgumentNullException(nameof(courseName));
        //        Credits = credits;
        //    }
        //}
    }

}
