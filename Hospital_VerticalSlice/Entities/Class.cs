using School_VerticalSlice.Entities.Enum;

namespace School_VerticalSlice.Entities
{
    public class Class : BaseEntity
    {
        public ClassEnum ClassCode { get; set; }
        public Level Level { get; set; }
        public Building Building { get; set; }

        public ICollection<Quiz> Quizzes { get; set; } = new HashSet<Quiz>();
        public ICollection<Student> Students { get; set; } = new HashSet<Student>();
        public Schedule Schedules { get; set; } = null!;
    }
}
