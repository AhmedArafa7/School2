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

    public record PutSpecificAttendanceCommand(int StudentId, int SessionId, bool status) : IRequest<Result<string>>;

    public class PutSpecificAttendanceHandler : IRequestHandler<PutSpecificAttendanceCommand, Result<string>>
    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public PutSpecificAttendanceHandler(ApplicationDbContext context, ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }
        public async Task<Result<string>> Handle(PutSpecificAttendanceCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUser.UserId;
            var autherized = await _context.Sessions
                .AsNoTracking()
                .Where(s=>s.TeacherId == currentUserId)
                .AnyAsync(s => s.Attendances.Any(a=>a.StudentId == request.StudentId));
            if (!autherized)
                return Error.Unauthorized("You are not authorized to access this session's attendance."); // look again

            //var status = await _context.Sessions
            //    .AsNoTracking()
            //    .Where(s => s.TeacherId == currentUserId 
            //             && s.Attendances.Any(a => a.StudentId == request.StudentId))
            //    .SelectMany(s=>s.Attendances)
            //    .FirstOrDefaultAsync();
            var status = await _context.Attendances
                .Where(a => a.SessionId == request.SessionId)
                .FirstOrDefaultAsync();

            if (status == null)
                return Error.NotFound("There is nothing ggggggggchanged");


            if (status?.IsAttended != request.status)
            {
                status?.IsAttended = request.status;
            }
            else
            {
                return Error.NotFound("There is nothing changed");
            }

            await _context.SaveChangesAsync();
            return "update done";
        }
    }
}

