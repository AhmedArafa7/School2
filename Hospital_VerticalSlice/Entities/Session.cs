using School_VerticalSlice.Entities.Enum;

namespace School_VerticalSlice.Entities
{
    public class Session : BaseEntity
    {
        public SessionEnum SessionNumber { get; set; }
        public DayOfWeek Day { get; set; }
        public Subject Subjects { get; set; }

        public int ScheduleId { get; set; }
        public Schedule Schedule { get; set; } = null!;
        public int TeacherId { get; set; }
        public Teacher Teacher { get; set; } = null!;
        public ICollection<Attendance> Attendances { get; set; } = new HashSet<Attendance>();
    }
}
