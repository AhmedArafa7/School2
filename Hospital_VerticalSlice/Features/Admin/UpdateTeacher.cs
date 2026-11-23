using Azure.Core;
using FluentValidation;
using School_VerticalSlice.Entities;
using School_VerticalSlice.Entities.Enum;
using School_VerticalSlice.Entities.Enum.Flags;
using School_VerticalSlice.Infrastructure;
using School_VerticalSlice.Infrastructure.Services;
using School_VerticalSlice.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace School_VerticalSlice.Features.Admin
{
    public record UpdateTeacherCommand(
        int Id,
        string? Name,
        bool? Gender,
        string? Phone,
        Subjects? Subject,
        Levels? Levels,
        bool? Available
        ) : IRequest<Result<List<TeacherResponce>>>;
    public class UpdateTeacherValidator : AbstractValidator<UpdateTeacherCommand>
    {
        public UpdateTeacherValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("ID is required.");
            RuleFor(x => x.Phone)
                .Matches(@"^01[0125][0-9]{8}$")
                .WithMessage("Invalid phone number.")
                .When(x => x.Phone != null);
            RuleFor(x => x.Levels)
                .IsInEnum()
                .WithMessage("Invalid Level");

        }
    }

    public class UpdateTeacherHandler : IRequestHandler<UpdateTeacherCommand, Result<List<TeacherResponce>>>
    {
        private readonly ApplicationDbContext _context;

        public UpdateTeacherHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<List<TeacherResponce>>> Handle(UpdateTeacherCommand request, CancellationToken cancellationToken)
        {
            var teacherQuery = _context.Teachers.AsQueryable();

            if(request.Name != null || request.Gender.HasValue || request.Phone != null)
            teacherQuery = teacherQuery.Include(t => t.User);

            var teacher = await teacherQuery.FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);
            if (teacher == null)
                return Error.NotFound("Teacher", "Teacher not found.");
            bool IsChanged = false;


            if (request.Name != null && teacher.User.Name != request.Name)
            {
                teacher.User.Name = request.Name;
                IsChanged = true;
            }
            if (request.Phone != null && teacher.User.Phone != request.Phone)
            {
                teacher.User.Phone = request.Phone;
                IsChanged = true;
            }
            if (request.Gender != null && teacher.User.Gender != request.Gender.Value)
            {
                teacher.User.Gender = request.Gender.Value;
                IsChanged = true;
            }
            if (request.Subject != null && teacher.Subjects != request.Subject)
            {
                teacher.Subjects = request.Subject ?? teacher.Subjects;
                IsChanged = true;
            }
            if (request.Levels != null && teacher.Levels != request.Levels)
            {
                teacher.Levels = request.Levels;
                IsChanged = true;
            }
            if (request.Available != null && teacher.Available != request.Available)
            {
                teacher.Available = request.Available ?? teacher.Available;
                IsChanged = true;
            }
            if (!IsChanged)
                return Error.Validation("No changes detected.", "No changes were made to the teacher's information.");

            await _context.SaveChangesAsync();
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
