using FluentValidation;
using School_VerticalSlice.Entities;
using School_VerticalSlice.Infrastructure;
using School_VerticalSlice.Infrastructure.Services;
using School_VerticalSlice.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace School_VerticalSlice.Features.TeacherFeatures
{
    public record PostStudentsAttendenceCommand(
        int ClassId,
        int SessionId,
        DateTime toDay,
        List<int> studentsIds
        ) : IRequest<Result<string>>;

    public class PostStudentsAttendenceValidator : AbstractValidator<PostStudentsAttendenceCommand>
    {
        public PostStudentsAttendenceValidator()
        {
            RuleFor(s => s.ClassId)
                .GreaterThan(0)
                .WithMessage("Class ID is required.");
            RuleFor(s => s.SessionId)
                .GreaterThan(0)
                .WithMessage("session ID is required.");
            RuleForEach(s=>s.studentsIds)
                .GreaterThan(0)
                .WithMessage("Studets IDs are required.");

            //RuleForEach(s=>s.studentsIds).ChildRules(e=>
            //{
            //    e.RuleFor(s=>s.)
            //})
        }
    }

    public class PostStudentsAttendenceHandler : IRequestHandler<PostStudentsAttendenceCommand, Result<string>>
    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public PostStudentsAttendenceHandler(ApplicationDbContext context, ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<Result<string>> Handle(PostStudentsAttendenceCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUser.UserId;
            var Isautherized = await _context.Sessions
                .AsNoTracking()
                .Where(s => s.Id == request.SessionId
                                            //&& s.Day == request.Day               // pack it after testing
                                            )
                .AnyAsync(s=>s.TeacherId == currentUserId);
            if (!Isautherized)
                return Error.Unauthorized("unauthorize request", $"You are unauthorized on that class");

            var classStudents = await _context.Classes
                .AsNoTracking()
                .Where(c => c.Id == request.ClassId)
                .SelectMany(c => c.Students)
                .Select(s=> s.Id)
                .ToListAsync(cancellationToken);

            foreach (var student in request.studentsIds)
            {
                if(!classStudents.Contains(student))
                {
                    return Error.Validation("Invalid student ID", $"Student with ID {student} is not in the specified class.");
                }
            }

            var IsAttendExist = await _context.Attendances
                .AsNoTracking()
                .Where(s => s.Session.Id == request.SessionId
                                && s.CreatedAt.Date == request.toDay.Date)
                .Select(s=> new {
                    studentId = s.StudentId,
                    sessionId = s.StudentId
                })
                .ToListAsync();

            if (IsAttendExist.Any(s=>s.sessionId == request.SessionId))
                return Error.Conflict("That session already got his attendance");

            Attendance studentAttendence = null;
            bool IsChanged = false;
            foreach (var student in classStudents)
            {
                 studentAttendence = new Attendance
                 {
                     StudentId = student,
                     SessionId = request.SessionId,
                     IsAttended = request.studentsIds.Contains(student)
                 };
  
                _context.Add(studentAttendence);
                IsChanged = true;
            }


            if (!IsChanged)
                return Error.Validation("No changes detected.", "No changes were made to the student's information.");

            await _context.SaveChangesAsync();
            return "Student updated successfully.";

        }

    }

}

