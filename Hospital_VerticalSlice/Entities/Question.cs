namespace Hospital_VerticalSlice.Entities
{
    public class Question : BaseEntity
    {
        public string question { get; set; } = string.Empty;
        public List<string>? Choices { get; set; }
        public string Answer { get; set; } = string.Empty;
        public float Degree { get; set; } = 1.00F;

        public int QuizId { get; set; }
        public Quiz Quiz { get; set; } = null!;
        public ICollection<StudentAnswers> StudentAnswers { get; set; } = new HashSet<StudentAnswers>();
    }
}
