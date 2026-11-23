using School_VerticalSlice.Entities;
using School_VerticalSlice.Entities.Enum;
using School_VerticalSlice.Features.Admin;
using School_VerticalSlice.Infrastructure;
using School_VerticalSlice.Infrastructure.Services;
using School_VerticalSlice.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static System.Collections.Specialized.BitVector32;

namespace School_VerticalSlice.Features.TeacherFeatures
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
        int ClassId,
        SessionEnum Session,
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
            var schedulesQuery = _context.Sessions
                .AsNoTracking()
                .Where(s => s.TeacherId == currentUserId)
                .Select(session => new 
                {
                    session,
                    Class = session.Schedule.Classes
                });

            var teacherSchedules = await schedulesQuery.ToListAsync(cancellationToken);

            var uniqueSessions = teacherSchedules
                  .GroupBy(x => x.session.Id)
                  .Select(g => g.First())
                  .ToList();

            var teacherScheduleList = RequeredDays.Select(day => new TeacherDaySchedule(
                        Day: day,
                        Sessions: uniqueSessions
                            .Where(s=>s.session.Day == day)
                            .Select(s => new TeacherSessionDetail(
                                SessionId: s.session.Id,
                                ClassId: s.Class.Id,
                                Session: s.session.SessionNumber,
                                Building: s.Class?.Building ?? Building.None,
                                Level: s.Class?.Level ?? Level.None,
                                Class: s.Class?.ClassCode ?? ClassEnum.None,
                                Subject: s.session.Subjects
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

