using Hospital_VerticalSlice.Entities.Enum;

namespace Hospital_VerticalSlice.Entities
{
    public class Session : BaseEntity
    {
        public Enum.SessionEnum SessionNumber { get; set; }
        public DayOfWeek Day { get; set; }
        public Subject Subjects { get; set; }

        public int ScheduleId { get; set; }
        public Schedule Schedule { get; set; } = null!;
        public int TeacherId { get; set; }
        public Teacher Teacher { get; set; } = null!;
        public ICollection<Attendance> Attendances { get; set; } = new HashSet<Attendance>();
    }
}
