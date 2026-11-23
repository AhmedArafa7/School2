using School_VerticalSlice.Infrastructure;
using School_VerticalSlice.Infrastructure.Services;
using School_VerticalSlice.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace School_VerticalSlice.Features.StudentFeatures.Quiz
{
    public record GetHisQuizResponce(
        string Question,
        List<string>? Choices,
        float Degree
        );
    public record GetHisQuizCommand(int QuizId) : IRequest<Result<List<GetHisQuizResponce>>>;

    public class GetHisQuizHandler : IRequestHandler<GetHisQuizCommand, Result<List<GetHisQuizResponce>>>
    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public GetHisQuizHandler(ApplicationDbContext context, ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<Result<List<GetHisQuizResponce>>> Handle(GetHisQuizCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUser.UserId;

            var questions = await _context.Questions
                .Where(q => q.Quiz.Classes.Any(s => s.Students.Any(s => s.Id == currentUserId)) && q.QuizId == request.QuizId && q.Quiz.IsDeleted != true)
                .Select(q=> new GetHisQuizResponce(
                    Question: q.question,
                    Choices: q.Choices,
                    Degree: q.Degree
                )).ToListAsync();

            if (!questions.Any())
                return Error.NotFound("Questions not found ");

            return questions;
        }
    }

}
