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
    public record StudentsNamesResponse(
        int StudentId,
        string StudentName
    );

    public record GetHisClassStudentsCommand(Building Building, Level Level, ClassEnum Class) : IRequest<Result<List<StudentsNamesResponse>>>;

    public class GetHisClassStudentsHandler : IRequestHandler<GetHisClassStudentsCommand, Result<List<StudentsNamesResponse>>>
    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public GetHisClassStudentsHandler(ApplicationDbContext context, ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }
        public async Task<Result<List<StudentsNamesResponse>>> Handle(GetHisClassStudentsCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUser.UserId;
            var autherized = await _context.Schedules
                .AsNoTracking()
                .Where(u => u.Sessions.Any(t=> t.Teacher.Id == currentUserId))
                .Select(u => u.Classes)
                .AnyAsync(c=> c.Building == request.Building && c.Level == request.Level && c.ClassCode == request.Class);

            if(!autherized)
                return Error.Unauthorized("You are not authorized to access this class's students.");

            var schedulesQuery = _context.Classes
                .AsNoTracking()
                .Where(c => c.Building == request.Building && c.Level == request.Level && c.ClassCode == request.Class)
                .SelectMany(c => c.Students)
                .Select(s => new StudentsNamesResponse(
                    StudentId: s.Id,
                    StudentName: s.User.Name
                ));

            var teacherSchedules = await schedulesQuery.ToListAsync(cancellationToken);


            return teacherSchedules;
        }
    }
}

