namespace School_VerticalSlice.Entities
{
    public class StudentAnswers : BaseEntity
    {
        public string StudentAnswer { get; set; } = string.Empty;
        public float Degree { get; set; }

        public int QuestionId { get; set; }
        public Question Question { get; set; } = null!;
        public int StudentId { get; set; }
        public Student Student { get; set; } = null!;
        //add entity to status and final degree
    }
}
