using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using School_VerticalSlice.Entities;
using School_VerticalSlice.Entities.Enum;
using School_VerticalSlice.Features.Admin;
using School_VerticalSlice.Infrastructure;
using School_VerticalSlice.Infrastructure.Services;
using School_VerticalSlice.Shared;

namespace School_VerticalSlice.Features.StudentFeatures.Quiz
{

    public record EnterQuizCommand(int QuizId) : IRequest<Result>;

    public class EnterQuizValidator : AbstractValidator<EnterQuizCommand>
    {
        public EnterQuizValidator()
        {

        }
    }

    public class EnterQuizHandler : IRequestHandler<EnterQuizCommand, Result>
    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public EnterQuizHandler(ApplicationDbContext context, ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }
        public async Task<Result> Handle(EnterQuizCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUser.UserId;
            var timeNow = DateTime.UtcNow;
            var quizTime = await _context.Quizzes
                .AsNoTracking()
                .Where(q=>q.Classes.Any(c=>c.Students.Any(s=>s.Id == currentUserId))
                          && q.Id == request.QuizId)
                .Select(q=> new
                {
                    q.Starttime,
                    q.Endtime
                })
                .FirstOrDefaultAsync();

            if (quizTime == null)
                return Error.NotFound("That quiz not found");
            if (quizTime.Starttime > timeNow)
                return Error.Conflict("That quiz time didn't start yet");
            if (quizTime.Endtime <= timeNow)
                return Error.Conflict("That quiz is fennished already");



            var enterQuiz = new StudentQuiz {
                StudentId = currentUserId ?? 0,
                QuizId = request.QuizId
            };

            _context.Add(enterQuiz);
            await _context.SaveChangesAsync();
            return Result.Ok();
        }
    }
}

