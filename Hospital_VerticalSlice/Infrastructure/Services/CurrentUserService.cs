using System.Security.Claims;

namespace Hospital_VerticalSlice.Infrastructure.Services
{

        public interface ICurrentUserService
        {
            int? UserId { get; }
            string? Role { get; }
            bool IsAdmin { get; }
            bool IsTeacher { get; }
            bool IsStudent { get; }
        }

        public class CurrentUserService : ICurrentUserService
        {
            private readonly IHttpContextAccessor _httpContextAccessor;

            public CurrentUserService(IHttpContextAccessor httpContextAccessor)
            {
                _httpContextAccessor = httpContextAccessor;
            }

            private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

            public int? UserId =>
                int.TryParse(User?.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : null;

            public string? Role =>
                        User?.FindFirstValue(ClaimTypes.Role);

            public bool IsAdmin =>
                User?.IsInRole("Admin") ?? false;

            public bool IsTeacher =>
                User?.IsInRole("Teacher") ?? false;
            public bool IsStudent =>
                User?.IsInRole("Student") ?? false;


    }
}
