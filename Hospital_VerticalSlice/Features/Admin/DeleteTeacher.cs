using Azure.Core;
using FluentValidation;
using Hospital_VerticalSlice.Entities;
using Hospital_VerticalSlice.Entities.Enum;
using Hospital_VerticalSlice.Infrastructure;
using Hospital_VerticalSlice.Infrastructure.Services;
using Hospital_VerticalSlice.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hospital_VerticalSlice.Features.Admin
{
    public record DeleteTeacherCommand(
        int id
        ) : IRequest<Result<List<TeacherResponce>>>;
    public class DeleteTeacherValidator : AbstractValidator<DeleteTeacherCommand>
    {
        public DeleteTeacherValidator()
        {
            RuleFor(x => x.id)
                .NotEmpty()
                .WithMessage("ID is required.");

        }
    }

    public class DeleteTeacherHandler : IRequestHandler<DeleteTeacherCommand, Result<List<TeacherResponce>>>
    {
        private readonly ApplicationDbContext _context;

        public DeleteTeacherHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<List<TeacherResponce>>> Handle(DeleteTeacherCommand request, CancellationToken cancellationToken)
        {
            var teacher = await _context.Teachers
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Id == request.id);

            if (teacher == null)
                return Error.NotFound("Teacher", "Teacher not found.");

            if (teacher.User.IsDeleted == true)
                return Error.Conflict("Teacher", "Teacher is already deleted.");

            if (teacher.User != null)
            {
                teacher.User.IsDeleted = true;
            }

            await _context.SaveChangesAsync();
            var teachers = await _context.Users
                .AsNoTracking()
                .Where(t => t.Role == Role.Teacher)
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
