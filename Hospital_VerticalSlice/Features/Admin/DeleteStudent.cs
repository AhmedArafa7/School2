using FluentValidation;
using School_VerticalSlice.Entities;
using School_VerticalSlice.Entities.Enum;
using School_VerticalSlice.Infrastructure;
using School_VerticalSlice.Infrastructure.Services;
using School_VerticalSlice.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace School_VerticalSlice.Features.Admin
{
    public record DeleteStudentCommand(
        int id
        ) : IRequest<Result<List<StudentResponce>>>;
    public class DeleteStudentValidator : AbstractValidator<DeleteStudentCommand>
    {
        public DeleteStudentValidator()
        {
            RuleFor(x => x.id)
                .NotEmpty()
                .WithMessage("ID is required.");

        }
    }

    public class DeleteStudentHandler : IRequestHandler<DeleteStudentCommand, Result<List<StudentResponce>>>
    {
        private readonly ApplicationDbContext _context;

        public DeleteStudentHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<List<StudentResponce>>> Handle(DeleteStudentCommand request, CancellationToken cancellationToken)
        {
            var student = await _context.Students
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.Id == request.id);

            if (student == null)
                return Error.NotFound("Student", "Student not found.");

            if (student.User.IsDeleted == true)
                return Error.Conflict("student", "student is already deleted.");

            if (student.User != null)
            {
                student.User.IsDeleted = true;
            }

            await _context.SaveChangesAsync();
            var students = await _context.Students
                .AsNoTracking()
                .Where(s => s.User.IsDeleted != true)
                .Select(t => new StudentResponce(
                    Id: t.Id,
                    Name: t.User.Name,
                    Email: t.User.Email,
                    Level: t.Class != null ? t.Class.Level : Level.None,
                    Building: t.Class != null ? t.Class.Building : Building.None,
                    Class: t.Class != null ? t.Class.ClassCode : ClassEnum.None,
                    Phone: t.User.Phone,
                    Gender: t.User.Gender
                ))
                .ToListAsync(cancellationToken);


            return students;
        }
    }
}
