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
    public record ClassesAttendanceResponse(
        int StudentId,
        string StudentName,
        bool? Status
    );

    public record GetClassesAttendanceCommand(int ClassId, int sessionId) : IRequest<Result<List<ClassesAttendanceResponse>>>;

    public class GetClassesAttendanceHandler : IRequestHandler<GetClassesAttendanceCommand, Result<List<ClassesAttendanceResponse>>>
    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public GetClassesAttendanceHandler(ApplicationDbContext context, ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }
        public async Task<Result<List<ClassesAttendanceResponse>>> Handle(GetClassesAttendanceCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUser.UserId;
            var autherized = await _context.Schedules
                .AsNoTracking()
                .AnyAsync(s => s.Classes.Id == request.ClassId
                            && s.Sessions.Any(e=>e.Id == request.sessionId
                                              && e.Teacher.Id == currentUserId));

            if (!autherized)
                return Error.Unauthorized("You are not authorized to access this session's attendance.");

            var schedulesQuery = _context.Attendances
                .AsNoTracking()
                .Select(s => new ClassesAttendanceResponse(
                    StudentId: s.Id,
                    StudentName: s.Student.User.Name,
                    Status: s.IsAttended? s.IsAttended : null
                ));

            var teacherSchedules = await schedulesQuery.ToListAsync(cancellationToken);


            return teacherSchedules;
        }
    }
}

