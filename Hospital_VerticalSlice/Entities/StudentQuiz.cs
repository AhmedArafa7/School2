using School_VerticalSlice.Entities.Enum;

namespace School_VerticalSlice.Entities
{
    public class StudentQuiz : BaseEntity
    {
        public float? TotalDegree { get; set; }

        public int StudentId { get; set; }
        public Student Student { get; set; } = null!;
        public int QuizId { get; set; }
        public Quiz Quiz { get; set; } = null!;
    }
}
