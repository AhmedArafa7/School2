using School_VerticalSlice.Entities;
using School_VerticalSlice.Entities.Enum;
using School_VerticalSlice.Entities.Enum.Flags;
using School_VerticalSlice.Infrastructure;
using School_VerticalSlice.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace School_VerticalSlice.Features.Admin
{
    public record TeacherResponce(
        int Id,
        string Name,
        string Email,
        Levels? Levels,
        Subjects? Subjects,
        string? Phone,
        bool? Gender, 
        bool Available
    );

    public class GetTeachersCommand : IRequest<Result<List<TeacherResponce>>>
    {

    }

    public class GetTeachersHandler : IRequestHandler<GetTeachersCommand, Result<List<TeacherResponce>>>
    {
        private readonly ApplicationDbContext _context;

        public GetTeachersHandler(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Result<List<TeacherResponce>>> Handle(GetTeachersCommand request, CancellationToken cancellationToken)
        {
            var teachers = await _context.Users
                            .AsNoTracking()
                            .Where(t => t.Role == Role.Teacher && t.IsDeleted != true)
                            .Select(t => new TeacherResponce(
                                Id: t.Teacher.Id,
                                Name: t.Name,
                                Email: t.Email,
                                Levels: t.Teacher != null ? t.Teacher.Levels : null,
                                Subjects: t.Teacher != null ? t.Teacher.Subjects : null,
                                Phone: t.Phone,
                                Gender: t.Gender,
                                Available: t.Teacher != null ? t.Teacher.Available : false
                            ))
                            .ToListAsync(cancellationToken);


            return teachers;
        }
    }
}
