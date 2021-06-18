namespace Logic.Dtos

{
    public sealed class StudentEnrollmentDto
    {
        public string Course { get; set; }
        public string CourseGrade { get; set; }

    }

    public sealed class StudentTransferDto
    {

        public string Course { get; set; }
        public string CourseGrade { get; set; }

    }

    public sealed class StudentDisenrollmentDto
    {
        public string Comment { get; set; }

    }

    public sealed class StudentPersonalInfoDto
    {
        public string Name { get; set; }

        public string Email { get; set; }


    }

    public sealed class NewStudentDto
    {
        public string Name { get; set; }

        public string Email { get; set; }

        public string FirstCourse { get; set; }

        public string FirstCourseGrade { get; set; }

        public string SecondCourse { get; set; }

        public string SecondCourseGrade { get; set; }





    }

    public sealed class StudentDto
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Course1 { get; set; }


        public string Course1Grade { get; set; }

        public int? Course1Credits { get; set; }


        public string Course2 { get; set; }

        public string Course2Grade { get; set; }

        public int? Course2Credits { get; set; }






    }
}
