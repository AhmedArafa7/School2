namespace School_VerticalSlice.Entities
{
    public class Student
    {
        public int Id { get; set; }

        public int? ClassId { get; set; }
        public Class Class { get; set; } = null!;
        public ICollection<Attendance> Attendances { get; set; } = new HashSet<Attendance>();
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public ICollection<StudentAnswers> StudentAnswers { get; set; } = new HashSet<StudentAnswers>();
        public ICollection<StudentQuiz> StudentQuizzes { get; set; } = new HashSet<StudentQuiz>();
    }
}
