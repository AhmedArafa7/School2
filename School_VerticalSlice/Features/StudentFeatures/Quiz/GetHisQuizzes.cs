using School_VerticalSlice.Infrastructure;
using School_VerticalSlice.Infrastructure.Services;
using School_VerticalSlice.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using School_VerticalSlice.Entities.Enum;

namespace School_VerticalSlice.Features.StudentFeatures.Quiz
{
    public record GetHisQuizzesResponse(
        int QuizId,
        string Title,
        DateTime StartTime,
        DateTime EndTime,
        int Duration,
        Subject Subject
        );
    public record GetHisQuizzesCommand() : IRequest<Result<List<GetHisQuizzesResponse>>>;

    public class GetHisQuizzesHandler : IRequestHandler<GetHisQuizzesCommand, Result<List<GetHisQuizzesResponse>>>
    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public GetHisQuizzesHandler(ApplicationDbContext context, ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<Result<List<GetHisQuizzesResponse>>> Handle(GetHisQuizzesCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUser.UserId;

            var questions = await _context.Quizzes
                .Where(q => q.Classes.Any(s => s.Students.Any(s => s.Id == currentUserId)) && q.IsDeleted != true)
                .Select(q => new GetHisQuizzesResponse(
                    q.Id,
                    q.Title,
                    q.Starttime,
                    q.Endtime,
                    q.Duration,
                    q.Subject
                )).ToListAsync();
            // add status , total degree

            return questions;
        }
    }

}
