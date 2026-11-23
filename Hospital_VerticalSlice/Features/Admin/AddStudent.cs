using Azure.Core;
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
    public record AddStudentCommand(
        string Name,
        string Email,
        string Password,
        bool? Gender,
        string? Phone,
        Level? Level,
        Building? Building,
        ClassEnum? Class
        ) : IRequest<Result<List<StudentResponce>>>;
    public class AddStudentValidator : AbstractValidator<AddStudentCommand>
    {
        public AddStudentValidator()
        {
            RuleFor(x => x.Name)
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
        }
    }

    public class AddStudentHandler : IRequestHandler<AddStudentCommand, Result<List<StudentResponce>>>
    {
        private readonly ApplicationDbContext _context;

        public AddStudentHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<List<StudentResponce>>> Handle(AddStudentCommand request, CancellationToken cancellationToken)
        {
            bool anyClassField = request.Building.HasValue || request.Class.HasValue || request.Level.HasValue;
            bool allClassFields = request.Building.HasValue && request.Class.HasValue && request.Level.HasValue;

            if (anyClassField && !allClassFields)
                return Error.Validation("Class.Incomplete", "You should enter building, class, and level together.");

            var IsEmailExist = await _context.Users
                .AsNoTracking().AnyAsync(t => t.Email == request.Email);

            if (IsEmailExist)
                return Error.Conflict("User.Email", "that email is already exist !");

            var PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            var Class = await _context.Classes
                .FirstOrDefaultAsync(c => c.Level == request.Level
                                     && c.Building == request.Building
                                     && c.ClassCode == request.Class);
            if (Class == null)
                return Error.NotFound("That class not found");

            var student = new Student
            {
                Class = Class,
                User = new User
                {
                    Name = request.Name,
                    Email = request.Email,
                    PasswordHash = PasswordHash,
                    Gender = request.Gender,
                    Role = Role.Student,
                    Phone = request.Phone
                },
            };

            _context.Students.Add(student);
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
