using FluentValidation;
using Hospital_VerticalSlice.Entities;
using Hospital_VerticalSlice.Entities.Enum;
using Hospital_VerticalSlice.Infrastructure;
using Hospital_VerticalSlice.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hospital_VerticalSlice.Features.Admin.ScheduleFeatures
{
    public record AddClassScheduleResponce(
        int ClassId,
        List<int> SessionIds
        );
    public record AddClassScheduleCommand(
        Level Level,
        Building Building,
        ClassEnum Class,
        List<SessionDTO> Schedule
        ) : IRequest<Result<AddClassScheduleResponce>>;
    public record SessionDTO(
        DayOfWeek Day,
        List<DaySessionsDTO> DaySessionsDTOs
        );
    public record DaySessionsDTO(
        SessionEnum Session,
        int TeacherId,
        Subject? Subject
        );
    public class AddClassScheduleValidator : AbstractValidator<AddClassScheduleCommand>
    {
        public AddClassScheduleValidator()
        {
            RuleFor(x => x.Level)
                .IsInEnum().WithMessage("Invalid level.");
            RuleFor(x => x.Building)
                .IsInEnum().WithMessage("Invalid building.");
            RuleFor(x => x.Class)
                .IsInEnum().WithMessage("Invalid class.");
            RuleFor(s => s.Schedule)
                .NotEmpty().WithMessage("Schedules cannot be empty.");

            RuleForEach(s => s.Schedule).ChildRules(d =>
            {
                d.RuleFor(x => x.Day)
                    .IsInEnum().WithMessage("Invalid day.");
                d.RuleForEach(x => x.DaySessionsDTOs).ChildRules(sessions =>
                {
                    sessions.RuleFor(x => x.Session)
                        .IsInEnum().WithMessage("Invalid session.");
                    sessions.RuleFor(x => x.TeacherId)
                        .GreaterThan(0).WithMessage("Invalid teacher ID.");
                    sessions.RuleFor(x => x.Subject)
                    .IsInEnum().WithMessage("Invalid subject.").When(x => x.Subject != null);
                });
            });

        }
    }

    public class AddClassScheduleHandler : IRequestHandler<AddClassScheduleCommand, Result<AddClassScheduleResponce>>
    {
        private readonly ApplicationDbContext _context;

        public AddClassScheduleHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<AddClassScheduleResponce>> Handle(AddClassScheduleCommand request, CancellationToken cancellationToken)
        {
            var IsClassExist = await _context.Schedules
                .AsNoTracking()
                .AnyAsync(c => c.Classes.ClassCode == request.Class
                            && c.Classes.Level == request.Level
                            && c.Classes.Building == request.Building);
            if (IsClassExist)
                return Error.Conflict("That class is already exist");

            var TeacherIdList = request.Schedule
                            .SelectMany(s => s.DaySessionsDTOs.Select(d => new
                            {
                                Id = d.TeacherId,
                                Subject = d.Subject
                            }))
                            .Distinct().ToList();
            var existTeachers = await _context.Teachers
                .AsNoTracking()
                .Where(u=> TeacherIdList.Select(t=>t.Id).Contains(u.Id))
                .Select(u=>new { u.Id, u.Subjects })
                .ToListAsync();

            var missingTeacherIds = TeacherIdList.Select(t => t.Id).Except(existTeachers.Select(t=>t.Id)).ToList();

            if (missingTeacherIds.Count > 0)
                return Error.NotFound($"Teachers not found: {string.Join(", ", missingTeacherIds)}");

            foreach(var teacher in existTeachers)
            {
                if (TeacherIdList.Where(t => t.Id == teacher.Id).FirstOrDefault()?.Subject != (Subject)teacher.Subjects)
                    return Error.Conflict($"teacher {teacher.Id} can't get {teacher.Subjects}");
            }



            var ClassRequest = new Class
            {
                ClassCode = request.Class,
                Level = request.Level,
                Building = request.Building
            };
            _context.Add(ClassRequest);

            var ClassSchedule = new Schedule
            {
                Classes = ClassRequest,
                Sessions = request.Schedule
                    .SelectMany(day => day.DaySessionsDTOs.Select(s => new Session
                    {
                        Day = day.Day,
                        SessionNumber = s.Session,
                        Subjects = s.Subject ?? Subject.None,
                        TeacherId = s.TeacherId,
                    }))
                    .ToList()
            };




            _context.Add(ClassSchedule);
            await _context.SaveChangesAsync();
            var response = new AddClassScheduleResponce(
                ClassId: ClassSchedule.Classes.Id,
                SessionIds: ClassSchedule.Sessions.Select(s=>s.Id).ToList()
                );
            return response;
        }
    }
}
