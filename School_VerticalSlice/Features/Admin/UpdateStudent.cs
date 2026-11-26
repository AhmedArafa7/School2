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
    public record UpdateStudentCommand(
        int Studentid,
        string? Name,
        bool? Gender,
        string? Phone,
        ClassEnum? Class,
        Building? Building,
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
                .When(x=> !string.IsNullOrEmpty(x.Name));
            RuleFor(x => x.Phone)
                .Matches(@"^01[0125][0-9]{8}$")
                .WithMessage("Invalid phone number.")
                .When(x => x.Phone != null);
            RuleFor(x => x.Class)
                .IsInEnum()
                .WithMessage("Invalid class.");

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
            var studentQuery = _context.Students.AsQueryable();

            if (request.Name != null || request.Gender.HasValue || request.Phone != null)
                studentQuery = studentQuery.Include(s => s.User);

            Class? RequestedClass = null;
            bool isAnyClassField = request.Building.HasValue || request.Class.HasValue || request.Level.HasValue;
            bool isClassEntered = request.Level.HasValue && request.Building.HasValue && request.Class.HasValue;
            if (isAnyClassField)
            {
                if (!isClassEntered)
                    return Error.Validation("Class.Incomplete", "You should enter building, class, and level together.");
                RequestedClass = await _context.Classes
                        .FirstOrDefaultAsync(c => c.Level == request.Level
                                               && c.Building == request.Building
                                               && c.ClassCode == request.Class);
                    if (RequestedClass == null)
                        return Error.NotFound("Class", "That class not found.");
                    studentQuery = studentQuery.Include(s => s.Class);
                
            }



            var student = await studentQuery.FirstOrDefaultAsync(s => s.Id == request.Studentid);

            if (student == null)
                return Error.NotFound("Student", "That student not found.");


            bool IsChanged = false;

            if (isClassEntered && student.ClassId != RequestedClass?.Id)
            {
                student.ClassId = RequestedClass?.Id;
                IsChanged = true;
            }

            if (request.Name != default && request.Name != student.User.Name)
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

