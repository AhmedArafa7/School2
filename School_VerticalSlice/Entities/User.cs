using School_VerticalSlice.Entities.Enum;

namespace School_VerticalSlice.Entities
{
    public class User : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Email { set; get; } = string.Empty;
        public string PasswordHash { set; get; } = string.Empty;
        public bool? Gender { set; get; }
        public Role Role { set; get; }
        public string? Phone {  set; get; } = string.Empty;
        public bool IsDeleted { set; get; }

        public ICollection<RefreshToken> RefreshTokens { get; set; } = new HashSet<RefreshToken>();
        public Teacher Teacher { set; get; } = null!;
        public Student Student { set; get; } = null!;

    }
}
