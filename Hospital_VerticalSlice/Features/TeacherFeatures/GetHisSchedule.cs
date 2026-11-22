using Hospital_VerticalSlice.Entities;
using Hospital_VerticalSlice.Entities.Enum;
using Hospital_VerticalSlice.Features.Admin;
using Hospital_VerticalSlice.Infrastructure;
using Hospital_VerticalSlice.Infrastructure.Services;
using Hospital_VerticalSlice.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hospital_VerticalSlice.Features.TeacherFeatures
{
    public record TeacherScheduleResponse(
        List<TeacherDaySchedule> TeacherSchedule
    );

    public record TeacherDaySchedule(
        DayOfWeek Day,
        List<TeacherSessionDetail> Sessions
    );

    public record TeacherSessionDetail(
        int SessionId,
        int? ClassId,
        Entities.Enum.SessionEnum Session,
        Building Building,
        Level Level,
        ClassEnum Class,
        Subject? Subject
    );

    public record GetHisScheduleCommand() : IRequest<Result<TeacherScheduleResponse>>;

    public class GetHisScheduleHandler : IRequestHandler<GetHisScheduleCommand, Result<TeacherScheduleResponse>>
    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public GetHisScheduleHandler(ApplicationDbContext context, ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }
        public async Task<Result<TeacherScheduleResponse>> Handle(GetHisScheduleCommand request, CancellationToken cancellationToken)
        {
            var RequeredDays = Enum.GetValues<DayOfWeek>().ToList();
            var currentUserId = _currentUser.UserId;
            var schedulesQuery = _context.Schedules
                .AsNoTracking()
                .Where(s => s.Sessions.Any(t => t.Teacher.Id == currentUserId))
                .SelectMany(s => s.Sessions, (schedule, teacher) => new //look to useing (schedule, teacher) again
                {
                    schedule.Sessions,
                    Class = schedule.Classes
                });

            var teacherSchedules = await schedulesQuery.ToListAsync(cancellationToken);

            var teacherScheduleList = RequeredDays.Select(day => new TeacherDaySchedule(
                        Day: day,
                        Sessions: teacherSchedules
                            .SelectMany(s => s.Sessions.Where(sess=> sess.Day == day), (s, sess) => new TeacherSessionDetail(
                                SessionId: sess.Id,
                                ClassId: s.Class?.Id ?? null,
                                Session: sess.SessionNumber,
                                Building: s.Class?.Building ?? Building.None,
                                Level: s.Class?.Level ?? Level.None,
                                Class: s.Class?.ClassCode ?? ClassEnum.None,
                                Subject: sess.Subjects
                            ))
                            .OrderBy(ds => ds.Session)
                            .ToList()
            ))
                .Where(d => d.Sessions.Any())
                .ToList();


            return new TeacherScheduleResponse(teacherScheduleList);
        }
    }
}

