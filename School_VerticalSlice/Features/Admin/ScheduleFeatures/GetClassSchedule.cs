using School_VerticalSlice.Entities;
using School_VerticalSlice.Entities.Enum;
using School_VerticalSlice.Infrastructure;
using School_VerticalSlice.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace School_VerticalSlice.Features.Admin.ScheduleFeatures
{
    public record ClassesScheduleResponse(
        List<ClassSessionsDay> DaysClassSessions
    );

    public record ClassSessionsDay(
        DayOfWeek Day,
        List<ClassSessionDetailDTO> Sessions
    );

    public record ClassSessionDetailDTO(
        int SessionId,
        SessionEnum SessionNumber,
        int TeacherId,
        string TeacherName,
        Subject Subject 
);



    public record GetClassScheduleCommand(int ClassId) : IRequest<Result<ClassesScheduleResponse>>;

    public class GetClassScheduleHandler : IRequestHandler<GetClassScheduleCommand, Result<ClassesScheduleResponse>>
    {
        private readonly ApplicationDbContext _context;

        public GetClassScheduleHandler(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Result<ClassesScheduleResponse>> Handle(GetClassScheduleCommand request, CancellationToken cancellationToken)
        {
            var RequeredDays = Enum.GetValues<DayOfWeek>().ToList();
            var SessionsList = Enum.GetValues<SessionEnum>().ToList();

            var schedulesQuery = _context.Schedules
                .AsNoTracking()
                .Where(s => s.Classes.Id == request.ClassId)
                .SelectMany(s => s.Sessions.Select(sess => new
                {
                    sess.Day,
                    sess.SessionNumber,
                    sess.Subjects,
                    sess.Id,
                    TeacherId = sess.TeacherId,
                    TeacherName = sess.Teacher != null && sess.Teacher.User != null
                        ? sess.Teacher.User.Name
                        : "Unknown"
                }));

            var classSchedules = await schedulesQuery.ToListAsync(cancellationToken);


            var classScheduleList = classSchedules
                .GroupBy(x => x.Day)
                .OrderBy(g => g.Key)
                .Select(dayGroup => new ClassSessionsDay(
                    Day: dayGroup.Key,
                    Sessions: dayGroup
                        .OrderBy(x => x.SessionNumber)
                        .Select(x => new ClassSessionDetailDTO(
                            SessionId: x.Id,
                            SessionNumber: x.SessionNumber,
                            TeacherId: x.TeacherId,
                            TeacherName: x.TeacherName ?? "Unknown",
                            Subject: x.Subjects
                        ))
                        .ToList()
                ))
                .ToList();

            return (new ClassesScheduleResponse(classScheduleList));
        }
    }
}

