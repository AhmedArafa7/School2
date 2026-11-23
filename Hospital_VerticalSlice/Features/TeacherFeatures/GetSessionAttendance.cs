using School_VerticalSlice.Entities;
using School_VerticalSlice.Entities.Enum;
using School_VerticalSlice.Features.Admin;
using School_VerticalSlice.Infrastructure;
using School_VerticalSlice.Infrastructure.Services;
using School_VerticalSlice.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace School_VerticalSlice.Features.TeacherFeatures
{
    public record SessionAttendanceResponse(
        int StudentId,
        string StudentName,
        bool? Status
    );

    public record GetSessionAttendanceCommand(int sessionId) : IRequest<Result<List<SessionAttendanceResponse>>>;

    public class GetSessionAttendanceHandler : IRequestHandler<GetSessionAttendanceCommand, Result<List<SessionAttendanceResponse>>>
    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public GetSessionAttendanceHandler(ApplicationDbContext context, ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }
        public async Task<Result<List<SessionAttendanceResponse>>> Handle(GetSessionAttendanceCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUser.UserId;
            var autherized = await _context.Schedules
                .AsNoTracking()
                .AnyAsync(s => s.Sessions.Any(e=>e.Id == request.sessionId
                                              && e.Teacher.Id == currentUserId));

            if (!autherized)
                return Error.Unauthorized("You are not authorized to access this session's attendance.");

            var schedulesQuery = _context.Attendances
                .AsNoTracking()
                .Where(a=>a.SessionId == request.sessionId)
                .Select(s => new SessionAttendanceResponse(
                    StudentId: s.StudentId,
                    StudentName: s.Student.User.Name,
                    Status: s.IsAttended
                ));

            var teacherSchedules = await schedulesQuery.ToListAsync(cancellationToken);


            return teacherSchedules;
        }
    }
}

