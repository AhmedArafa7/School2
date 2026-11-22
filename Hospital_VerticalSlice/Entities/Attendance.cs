using Hospital_VerticalSlice.Entities.Enum;

namespace Hospital_VerticalSlice.Entities
{
    public class Attendance : BaseEntity
    {
        public bool IsAttended { get; set; }

        public int StudentId { get; set; }
        public Student Student { get; set; } = null!;
        public int SessionId { get; set; }
        public Session Session { get; set; } = null!;

    }
}
