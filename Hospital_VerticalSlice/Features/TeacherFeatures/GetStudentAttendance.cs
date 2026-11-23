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




            var schedulesQuery = _context.Sessions
                .AsNoTracking()
                .Where(s => s.TeacherId == currentUserId
                         && s.Schedule.Classes.Students.Any(a => a.Id == request.StudentId))
                .Select(s => new GetStudentAttendanceResponse(
                    SessionId: s.Id,
                    SessionName: s.SessionNumber,
                    Day: s.Day,
                    Status: s.Attendances
                        .Where(a=>a.StudentId == request.StudentId)
                        .Select(a=>(bool?)a.IsAttended)
                        .FirstOrDefault()
                ));

            var teacherSchedules = await schedulesQuery.ToListAsync(cancellationToken);

            if (!teacherSchedules.Any())
            {
                var isStudentInTeacherClass = await _context.Attendances
                    .AsNoTracking()
                    .AnyAsync(s => s.Session.TeacherId == currentUserId
                                && s.StudentId == request.StudentId,
                        cancellationToken);

                if (!isStudentInTeacherClass)
                    return Error.NotFound("Attendance.Forbidden", "This student's attendance didn't created yet.");
            }
            if (!teacherSchedules.Any())
                return Error.Forbidden("Attendance.Forbidden", "You are not allowed to view this student's attendance.");

            return teacherSchedules;
        }
    }
}

