using Azure.Core;
using FluentValidation;
using Hospital_VerticalSlice.Entities;
using Hospital_VerticalSlice.Entities.Enum;
using Hospital_VerticalSlice.Entities.Enum.Flags;
using Hospital_VerticalSlice.Infrastructure;
using Hospital_VerticalSlice.Infrastructure.Services;
using Hospital_VerticalSlice.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hospital_VerticalSlice.Features.Admin
{
    public record AddTeacherCommand(
        string name,
        string Email,
        string Password,
        bool? Gender,
        string? Phone,
        Subjects Subject,
        Levels? Levels,
        bool? Available
        ) : IRequest<Result<List<TeacherResponce>>>;
    public class AddTeacherValidator : AbstractValidator<AddTeacherCommand>
    {
        public AddTeacherValidator()
        {
            RuleFor(x => x.name)
                .NotEmpty().WithMessage("Name is required.");
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");
            RuleFor(x => x.Phone)
                .Matches(@"^01[0125][0-9]{8}$")
                .WithMessage("Invalid phone number format.");
            RuleFor(x => x.Levels)
                .IsInEnum()
                .WithMessage("That level not exist")
                .When(x => x.Levels == null);

        }
    }

    public class AddTeacherHandler : IRequestHandler<AddTeacherCommand,Result<List<TeacherResponce>>>
    {
        private readonly ApplicationDbContext _context;

        public AddTeacherHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<List<TeacherResponce>>> Handle(AddTeacherCommand request, CancellationToken cancellationToken)
        {
            var IsEmailExist = await _context.Users
                .AsNoTracking().AnyAsync(t => t.Email == request.Email);

            if (IsEmailExist)
                return Error.Conflict("User.Email", "that email is already exist !");

            var PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var teacher = new User
            {
                Name = request.name,
                Email = request.Email,
                PasswordHash = PasswordHash,
                Gender = request.Gender,
                Role = Role.Teacher,
                Phone = request.Phone,
                Teacher = new Teacher
                {
                    Subjects = request.Subject,
                    Levels = request.Levels,
                    Available = request.Available ?? true
                }
            };
                _context.Users.Add(teacher);
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
