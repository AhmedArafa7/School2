using School_VerticalSlice.Entities;
using School_VerticalSlice.Entities.Enum;
using School_VerticalSlice.Features.Admin;
using School_VerticalSlice.Infrastructure;
using School_VerticalSlice.Infrastructure.Services;
using School_VerticalSlice.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace School_VerticalSlice.Features.StudentFeatures
{
    public record StudentScheduleResponse(
        List<StudentDaySchedule> StudentSchedule
    );

    public record StudentDaySchedule(
        DayOfWeek Day,
        List<StudentSessionDetail> Sessions
    );

    public record StudentSessionDetail(
        int SessionId,
        int TeacherId,
        int? ClassId,
        string TeacherName,
        SessionEnum Session,
        Building Building,
        ClassEnum Class,
        Subject? Subject
    );

    public record GetHisScheduleCommand() : IRequest<Result<StudentScheduleResponse>>;

    public class GetHisScheduleHandler : IRequestHandler<GetHisScheduleCommand, Result<StudentScheduleResponse>>
    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public GetHisScheduleHandler(ApplicationDbContext context, ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }
        public async Task<Result<StudentScheduleResponse>> Handle(GetHisScheduleCommand request, CancellationToken cancellationToken)
        {
            var RequeredDays = Enum.GetValues<DayOfWeek>().ToList();
            var currentUserId = _currentUser.UserId;
            var schedulesQuery = _context.Schedules
                .AsNoTracking()
                .Where(s => s.Classes.Students.Any(s=>s.Id == currentUserId))
                .Select(s => new //look to useing (schedule, teacher) again
                {
                    s.Sessions,
                    Class = s.Classes,
                    Name = s.Sessions.Select(t=>t.Teacher.User.Name).FirstOrDefault()
                });

            var StudentSchedules = await schedulesQuery.ToListAsync(cancellationToken);

            var StudentScheduleList = RequeredDays.Select(day => new StudentDaySchedule(
                        Day: day,
                        Sessions: StudentSchedules
                            .SelectMany(s => s.Sessions.Where(sess => sess.Day == day), (s, sess) => new StudentSessionDetail(
                                SessionId: sess.Id,
                                TeacherId: sess.TeacherId,
                                TeacherName: s.Name ?? "faild to git the name",
                                ClassId: s.Class?.Id ?? null,
                                Session: sess.SessionNumber,
                                Building: s.Class?.Building ?? Building.None,
                                Class: s.Class?.ClassCode ?? ClassEnum.None,
                                Subject: sess.Subjects
                            ))
                            .OrderBy(ds => ds.Session)
                            .ToList()
            ))
                .Where(d => d.Sessions.Any())
                .ToList();


            return new StudentScheduleResponse(StudentScheduleList);
        }
    }
}

