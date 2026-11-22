using Hospital_VerticalSlice.Entities.Enum;
using Hospital_VerticalSlice.Entities.Enum.Flags;

namespace Hospital_VerticalSlice.Entities
{
    public class Teacher
    {
        public int Id { get; set; }
        public Levels? Levels { get; set; }
        public Subjects Subjects { get; set; }
        public bool Available { get; set; } = true;

        public ICollection<Session> Sessions { get; set; } = new HashSet<Session>();
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public ICollection<Quiz> Quizzes { get; set; } = new HashSet<Quiz>();
    }
}
