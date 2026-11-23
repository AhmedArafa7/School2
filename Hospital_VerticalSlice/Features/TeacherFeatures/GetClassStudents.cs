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
    public record StudentsNamesResponse(
        int StudentId,
        string StudentName
    );

    public record GetHisClassStudentsCommand(int ClassId) : IRequest<Result<List<StudentsNamesResponse>>>;

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
                .AnyAsync(c=> c.Id == request.ClassId);

            if(!autherized)
                return Error.Unauthorized("You are not authorized to access this class's students.");

            var schedulesQuery = _context.Classes
                .AsNoTracking()
                .Where(c => c.Id == request.ClassId)
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

