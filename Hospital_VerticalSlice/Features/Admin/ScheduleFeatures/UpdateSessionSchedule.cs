using FluentValidation;
using School_VerticalSlice.Entities;
using School_VerticalSlice.Entities.Enum;
using School_VerticalSlice.Infrastructure;
using School_VerticalSlice.Shared;
using School_VerticalSlice.Entities.Enum.Flags;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace School_VerticalSlice.Features.Admin.ScheduleFeatures
{
    public record UpdateSessionScheduleCommand(
        int ScheduleId,
        int SessionId,
        int TeacherId,
        Subject? Subject
        ) : IRequest<Result>;

    public class UpdateSessionScheduleValidator : AbstractValidator<UpdateSessionScheduleCommand>
    {
        public UpdateSessionScheduleValidator()
        {


        }
    }

    public class UpdateSessionScheduleHandler : IRequestHandler<UpdateSessionScheduleCommand, Result>
    {
        private readonly ApplicationDbContext _context;

        public UpdateSessionScheduleHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(UpdateSessionScheduleCommand request, CancellationToken cancellationToken)
        {
            var existTeacher = await _context.Teachers
                .AsNoTracking()
                .Where(t => t.Id == request.TeacherId)
                .Select(t => new
                {
                    t.Id,
                    t.Subjects,
                    t.Available
                })
                .FirstOrDefaultAsync();

            if (existTeacher == default)
                return Error.NotFound($"That teacher not found");

            if (existTeacher.Available == false)
                return Error.Conflict("That teacher not available");

            if (request.Subject != null)
            {
                if (!Enum.TryParse<Subjects>(request.Subject.Value.ToString(), out var requiredSubjectFlag)
                    || requiredSubjectFlag == Subjects.None)
                {
                    return Error.Validation("Invalid subject.");
                }

                if (!existTeacher.Subjects.HasFlag(requiredSubjectFlag))
                    return Error.Conflict("That teacher don't get that subject");

            }

            var ClassSchedule = await _context.Sessions
                .FirstOrDefaultAsync(s => s.Id == request.SessionId
                         && s.ScheduleId == request.ScheduleId);
            if (ClassSchedule == null)
                return Error.NotFound($"That session not found");

            bool isBusy = await _context.Sessions
                .AsNoTracking()
                .AnyAsync(se => se.TeacherId == request.TeacherId
                             && se.Id != request.SessionId
                             && se.Day == ClassSchedule.Day
                             && se.SessionNumber == ClassSchedule.SessionNumber,
                        cancellationToken);

            if (isBusy)
                return Error.Conflict(
                    $"Teacher {request.TeacherId} is already assigned on {ClassSchedule.Day} during session {ClassSchedule.SessionNumber}."
                );

            bool IsChanged = false;
            if(ClassSchedule.TeacherId != request.TeacherId)
            {
                ClassSchedule.TeacherId = request.TeacherId;
                IsChanged = true;
            }

            if (ClassSchedule.Subjects != request.Subject)
            {
                ClassSchedule.Subjects = request.Subject ?? ClassSchedule.Subjects;
                IsChanged = true;
            }

            if (!IsChanged)
                return Error.Conflict("nothing changed");

            await _context.SaveChangesAsync();
            return Result.Ok();
        }
    }
}

