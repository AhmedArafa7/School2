using Hospital_VerticalSlice.Entities.Enum;
using Hospital_VerticalSlice.Infrastructure;
using Hospital_VerticalSlice.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hospital_VerticalSlice.Features.Admin
{
    public record StudentResponce(
        int Id,
        string Name,
        string Email,
        Level Level,
        ClassEnum? Class,
        string? Phone,
        bool? Gender
    );

    public class GetStudentsCommand : IRequest<Result<List<StudentResponce>>>
    {

    }

    public class GetStudentsHandler : IRequestHandler<GetStudentsCommand, Result<List<StudentResponce>>>
    {
        private readonly ApplicationDbContext _context;

        public GetStudentsHandler(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Result<List<StudentResponce>>> Handle(GetStudentsCommand request, CancellationToken cancellationToken)
        {
            var students = await _context.Students
                .AsNoTracking()
                .Where(s => s.User.IsDeleted != true)
                .Select(t => new StudentResponce(
                    Id: t.Id,
                    Name:  t.User.Name,
                    Email: t.User.Email,
                    Level: t.Class != null ? t.Class.Level : Level.None,
                    Class: t.Class != null ? t.Class.ClassCode : ClassEnum.None,
                    Phone:  t.User.Phone,
                    Gender:  t.User.Gender
                ))
                .ToListAsync(cancellationToken);


            return students;
        }
    }
}
