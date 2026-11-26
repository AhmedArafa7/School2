using School_VerticalSlice.Entities;
using School_VerticalSlice.Entities.Enum;
using School_VerticalSlice.Infrastructure;
using School_VerticalSlice.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace School_VerticalSlice.Features.Admin.ScheduleFeatures
{
    public record TeachersScheduleResponse(
        List<TeacherSchedule> TeacherSchedule
    );

    public record TeacherSchedule(
    int TeacherId,
    string TeacherName, // لو محتاج اسم المدرس
    List<TeacherDayScheduleDTO> DaysSchedule
);

    public record TeacherDayScheduleDTO(
        DayOfWeek Day,
        List<TeacherSessionDetailDTO> Sessions
    );

    public record TeacherSessionDetailDTO(
        Entities.Enum.SessionEnum Session,
        Building Building,
        Level Level,
        ClassEnum Class,
        Subject? Subject
    );

    public record GetTeacherScheduleCommand(int? TeacherId) : IRequest<Result<TeachersScheduleResponse>>;

    public class GetTeacherScheduleHandler : IRequestHandler<GetTeacherScheduleCommand, Result<TeachersScheduleResponse>>
    {
        private readonly ApplicationDbContext _context;

        public GetTeacherScheduleHandler(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Result<TeachersScheduleResponse>> Handle(GetTeacherScheduleCommand request, CancellationToken cancellationToken)
        {
            var RequeredDays = Enum.GetValues<DayOfWeek>().ToList();

            var schedulesQuery = _context.Schedules
                .AsNoTracking()
                .Where(s=> !request.TeacherId.HasValue || s.Sessions.Any(t => t.Teacher.Id == request.TeacherId.Value))
                .SelectMany(s => s.Sessions, (schedule, session)=> new
                {
                    TeacherId = session.Teacher.Id,
                    TeacherName = session.Teacher.User.Name ?? "Unknown",
                    schedule.Sessions,
                    Class = schedule.Classes
                });

            var teacherSchedules = await schedulesQuery.ToListAsync(cancellationToken);


            var teacherScheduleList = teacherSchedules
                .GroupBy(x=> new {x.TeacherId,x.TeacherName})
                .Select(teacher => new TeacherSchedule(
                    TeacherId: teacher.Key.TeacherId,
                    TeacherName: teacher.Key.TeacherName,
                    DaysSchedule: RequeredDays.Select(day => new TeacherDayScheduleDTO(
                        Day: day,
                        Sessions: teacher
                            .SelectMany(s => s.Sessions)
                            .Where(sess => sess.Day == day)
                            .Select(sess => new TeacherSessionDetailDTO(
                                Session: sess.SessionNumber,
                                Building: teacher.Select(t=> t.Class?.Building).FirstOrDefault() ?? Building.None,
                                Level: teacher.Select(t => t.Class?.Level).FirstOrDefault() ?? Level.None,
                                Class: teacher.Select(t => t.Class?.ClassCode).FirstOrDefault() ?? ClassEnum.None,
                                Subject: sess.Subjects
                            ))
                            .OrderBy(ds => ds.Session)
                            .ToList()
                    )).ToList()
                )).ToList();


            return new TeachersScheduleResponse(teacherScheduleList);
        }
    }
}

