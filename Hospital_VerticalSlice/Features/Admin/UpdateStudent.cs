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
    public record UpdateStudentCommand(
        int Studentid,
        string? Name,
        bool? Gender,
        string? Phone,
        ClassEnum? Class,
        Level? Level
        ) : IRequest<Result<List<StudentResponce>>>;
    public class UpdateStudentValidator : AbstractValidator<UpdateStudentCommand>
    {
        public UpdateStudentValidator()
        {
            RuleFor(x => x.Studentid)
                .GreaterThan(0)
                .WithMessage("Student ID should be greater that 0.");
            RuleFor(x => x.Name)
                .MinimumLength(3)
                .MaximumLength(50)
                .WithMessage("Invalid name !.")
                .When(x=>x.Name != "");
            RuleFor(x => x.Phone)
                .Matches(@"^01[0125][0-9]{8}$")
                .WithMessage("Invalid phone number.")
                .When(x => x.Phone != null);
            RuleFor(x => x.Class)
                .IsInEnum()
                .WithMessage("Student ID should be greater that 0.");

        }
    }

    public class UpdateStudentHandler : IRequestHandler<UpdateStudentCommand, Result<List<StudentResponce>>>
    {
        private readonly ApplicationDbContext _context;

        public UpdateStudentHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<List<StudentResponce>>> Handle(UpdateStudentCommand request, CancellationToken cancellationToken)
        {
            var student = await _context.Students
                .Include(s => s.User)
                .Include(s=>s.Class)
                .FirstOrDefaultAsync(s => s.Id == request.Studentid);

            var IsClassExist = await _context.Classes
                .AnyAsync(c=>c.ClassCode == request.Class);

            if (student == null)
                return Error.NotFound("Student", "That student not found.");
            if (!IsClassExist)
                return Error.NotFound("Class", "That class not found.");

            bool IsChanged = false;

            if(request.Name != default && request.Name != student.User.Name)
            {
                student.User.Name = request.Name;
                IsChanged = true;
            }
            if(request.Phone != default && request.Phone != student.User.Phone)
            {
                student.User.Phone = request.Phone;
                IsChanged = true;
            }
            if(request.Gender != null && request.Gender != student.User.Gender)
            {
                student.User.Gender = request.Gender.Value;
                IsChanged = true;
            }
            if(request.Class != null && request.Class != student.Class.ClassCode)
            {
                student.Class.ClassCode = request.Class.Value;
                IsChanged = true;
            }
            if (request.Level != null && request.Level != student.Class.Level)
            {
                student.Class.Level = request.Level.Value;
                IsChanged = true;
            }


            if (!IsChanged)
                return Error.Validation("No changes detected.", "No changes were made to the student's information.");

            await _context.SaveChangesAsync();
            var students = await _context.Students
                .AsNoTracking()
                .Where(s => s.User.IsDeleted != true)
                .Select(t => new StudentResponce(
                    Id: t.Id,
                    Name: t.User.Name,
                    Email: t.User.Email,
                    Level: t.Class != null ? t.Class.Level : Level.None,
                    Class: t.Class != null ? t.Class.ClassCode : ClassEnum.None,
                    Phone: t.User.Phone,
                    Gender: t.User.Gender
                ))
                .ToListAsync(cancellationToken);


            return students;

        }

    }

}

