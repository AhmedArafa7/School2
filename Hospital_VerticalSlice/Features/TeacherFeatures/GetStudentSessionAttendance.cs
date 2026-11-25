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
    public record GetStudentSessionAttendanceResponse(
        int Week,
        bool? Status
    );

    public record GetStudentSessionAttendanceCommand(int StudentId,int sessionId) : IRequest<Result<List<GetStudentSessionAttendanceResponse>>>;

    public class GetStudentSessionAttendanceHandler : IRequestHandler<GetStudentSessionAttendanceCommand, Result<List<GetStudentSessionAttendanceResponse>>>
    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public GetStudentSessionAttendanceHandler(ApplicationDbContext context, ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }
        public async Task<Result<List<GetStudentSessionAttendanceResponse>>> Handle(GetStudentSessionAttendanceCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUser.UserId;

            var isAuthorized = await _context.Sessions
                    .AsNoTracking()
                    .AnyAsync(s => s.Id == request.sessionId
                    && s.TeacherId == currentUserId,
                    cancellationToken);

            if (!isAuthorized)
                return Error.Forbidden("Attendance.Forbidden", "You are not allowed to view this student's attendance.");

            // 2. جلب تاريخ بداية الترم (قيمة واحدة ثابتة للعملية)
            var termStartDate = await _context.AcademicSettings
                .AsNoTracking()
                .Select(ac => (DateOnly?)ac.TermStartDate)
                .FirstOrDefaultAsync(cancellationToken);

            if (termStartDate is null || termStartDate == default)
                return Error.Validation("AcademicSettings.MissingTermStartDate", "Term start date is not configured.");

            var startDate = termStartDate.Value.ToDateTime(TimeOnly.MinValue);

            // 3. حساب رقم الأسبوع داخل الداتابيز مباشرة
            var studentSessionAttendance = await _context.Attendances
                .AsNoTracking()
                .Where(a => a.SessionId == request.sessionId
                         && a.StudentId == request.StudentId)
                .OrderBy(a => a.CreatedAt)
                .Select(a => new GetStudentSessionAttendanceResponse(
                    Week: EF.Functions.DateDiffDay(startDate, a.CreatedAt) < 0
                        ? 1
                        : (EF.Functions.DateDiffDay(startDate, a.CreatedAt) / 7) + 1,
                    Status: a.IsAttended
                ))
                .ToListAsync(cancellationToken);

            return studentSessionAttendance;
        }
    }
}

