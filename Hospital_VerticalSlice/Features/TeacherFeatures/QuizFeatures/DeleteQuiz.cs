using FluentValidation;
using School_VerticalSlice.Entities;
using School_VerticalSlice.Infrastructure;
using School_VerticalSlice.Infrastructure.Services;
using School_VerticalSlice.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace School_VerticalSlice.Features.TeacherFeatures.QuizFeatures
{
    public record DeleteQuizCommand(
        int QuizId
        ) : IRequest<Result<string>>;
    public class DeleteQuizValidator : AbstractValidator<DeleteQuizCommand>
    {
        public DeleteQuizValidator()
        {
            RuleFor(x => x.QuizId)
                .NotEmpty()
                .WithMessage("ID is required.");

        }
    }

    public class DeleteQuizHandler : IRequestHandler<DeleteQuizCommand, Result<string>>
    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public DeleteQuizHandler(ApplicationDbContext context,ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<Result<string>> Handle(DeleteQuizCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUser.UserId;
            var quiz = await _context.Quizzes
                .Include(s => s.Question)
                .FirstOrDefaultAsync(s => s.Id == request.QuizId);

            if (quiz == null)
                return Error.NotFound("Quiz", "Quiz not found.");

            if (quiz.IsDeleted == true)
                return Error.Conflict("quiz", "quiz is already deleted.");

            quiz.IsDeleted = true;


            await _context.SaveChangesAsync();
            return "Quiz deleted successfully.";
        }
    }
}
