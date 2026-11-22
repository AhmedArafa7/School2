using Hospital_VerticalSlice.Entities;
using Hospital_VerticalSlice.Entities.Enum;
using Hospital_VerticalSlice.Features.Admin;
using Hospital_VerticalSlice.Infrastructure;
using Hospital_VerticalSlice.Infrastructure.Services;
using Hospital_VerticalSlice.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hospital_VerticalSlice.Features.TeacherFeatures
{
    public record GetStudentAttendanceResponse(
        int SessionId,
        SessionEnum SessionName,
        DayOfWeek Day,
        bool? Status
    );

    public record GetStudentAttendanceCommand(int StudentId) : IRequest<Result<List<GetStudentAttendanceResponse>>>;

    public class GetStudentAttendanceHandler : IRequestHandler<GetStudentAttendanceCommand, Result<List<GetStudentAttendanceResponse>>>
    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public GetStudentAttendanceHandler(ApplicationDbContext context, ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }
        public async Task<Result<List<GetStudentAttendanceResponse>>> Handle(GetStudentAttendanceCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUser.UserId;
            var autherized = await _context.Sessions
                .AsNoTracking()
                .AnyAsync(s => s.TeacherId == currentUserId
                            && s.Attendances.Any(a=>a.StudentId == request.StudentId));
            if (!autherized)
                return Error.Forbidden("Attendance.Forbidden", "You are not allowed to view this student's attendance.");


            var schedulesQuery = _context.Sessions
                .AsNoTracking()
                .Where(s => s.TeacherId == currentUserId && s.Attendances.Any(a => a.StudentId == request.StudentId))
                .Select(s => new GetStudentAttendanceResponse(
                    SessionId: s.Id,
                    SessionName: s.SessionNumber,
                    Day: s.Day,
                    Status: s.Attendances
                        .Where(a=>a.StudentId == request.StudentId)
                        .Select(a=>a.IsAttended)
                        .FirstOrDefault()
                ));

            var teacherSchedules = await schedulesQuery.ToListAsync(cancellationToken);


            return teacherSchedules;
        }
    }
}

