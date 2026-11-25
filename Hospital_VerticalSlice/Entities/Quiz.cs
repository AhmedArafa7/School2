using School_VerticalSlice.Entities.Enum;

namespace School_VerticalSlice.Entities
{
    public class Quiz : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public DateTime Starttime { get; set; }
        public DateTime Endtime { get; set; }
        public int Duration { get; set; }
        public Subject Subject { get; set; }
        public bool IsDeleted { get; set; }

        public ICollection<Class> Classes { get; set; } = new HashSet<Class>();
        public int TeacherId { get; set; }
        public Teacher Teacher { get; set; } = null!;
        public ICollection<Question> Question { get; set; } = new HashSet<Question>();
        public ICollection<StudentQuiz> StudentQuiz { get; set; } = new HashSet<StudentQuiz>();
    }
}
