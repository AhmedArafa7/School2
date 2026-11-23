using School_VerticalSlice.Entities.Enum;
using School_VerticalSlice.Infrastructure;
using School_VerticalSlice.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace School_VerticalSlice.Features.Admin.ScheduleFeatures
{
    public record GetClassesHasScheruleResponce(
        int ScheduleId,
        int ClassId,
        ClassEnum Class,
        Level Level,
        Building Building
    );
    public record GetClassesHasScheruleCommand() : IRequest<Result<List<GetClassesHasScheruleResponce>>>;

    public class GetClassesHasScheruleHandler : IRequestHandler<GetClassesHasScheruleCommand, Result<List<GetClassesHasScheruleResponce>>>
    {
        private readonly ApplicationDbContext _context;

        public GetClassesHasScheruleHandler(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Result<List<GetClassesHasScheruleResponce>>> Handle(GetClassesHasScheruleCommand request, CancellationToken cancellationToken)
        {
            var classesHasScherule = await _context.Schedules
                .AsNoTracking()
                .Select(c=> new GetClassesHasScheruleResponce(
                    ScheduleId: c.Id,
                    ClassId: c.ClassId,
                    Class: c.Classes.ClassCode,
                    Level: c.Classes.Level,
                    Building: c.Classes.Building
                    ))
                .ToListAsync(cancellationToken);

            return classesHasScherule;
        }
    }
}
