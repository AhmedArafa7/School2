using Hospital_VerticalSlice.Entities.Enum;

namespace Hospital_VerticalSlice.Entities
{
    public class Schedule : BaseEntity
    {
        public int SessionDuration { get; set; }


        public ICollection<Session> Sessions { get; set; } = new HashSet<Session>();
        public int ClassId { get; set; }
        public Class Classes { get; set; } = null!;
    }
}
