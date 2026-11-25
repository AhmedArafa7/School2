using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using School_VerticalSlice.Infrastructure;
using School_VerticalSlice.Infrastructure.Services;
using School_VerticalSlice.Shared;

namespace School_VerticalSlice.Features.TeacherFeatures.QuizFeatures
{
    public record GradeEssayAnswersCommand(
        int QuizId,
        int StudentId,
        List<EssayDegree> EssayDegree
    ) : IRequest<Result<float>>;

    public record EssayDegree(
        int QuestionId,
        float Degree
    );

    public class GradeEssayAnswersValidator : AbstractValidator<GradeEssayAnswersCommand>
    {
        public GradeEssayAnswersValidator()
        {
            RuleFor(x => x.QuizId).GreaterThan(0);
            RuleFor(x => x.StudentId).GreaterThan(0);
            RuleForEach(x => x.EssayDegree).ChildRules(item =>
            {
                item.RuleFor(d => d.QuestionId).GreaterThan(0);
                item.RuleFor(d => d.Degree).GreaterThanOrEqualTo(0);
            });
        }
    }

    public class GradeEssayAnswersHandler : IRequestHandler<GradeEssayAnswersCommand, Result<float>>
    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public GradeEssayAnswersHandler(ApplicationDbContext context, ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<Result<float>> Handle(GradeEssayAnswersCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUser.UserId;
            if (currentUserId is null)
                return Error.Unauthorized("User is not authenticated.");

            var quizExistsForTeacher = await _context.Quizzes
                .AsNoTracking()
                .AnyAsync(q => q.Id == request.QuizId && q.TeacherId == currentUserId, cancellationToken);

            if (!quizExistsForTeacher)
                return Error.Unauthorized("You are not authorized to grade this quiz or quiz was not found.");

            var questions = await _context.Questions
                .AsNoTracking()
                .Where(q => q.QuizId == request.QuizId)
                .Select(q => new
                {
                    QuestionId = q.Id,
                    q.Choices,
                    q.CorrectAnswer,
                    q.Degree
                })
                .ToListAsync(cancellationToken);

            if (questions.Count == 0)
                return Error.NotFound("Questions.NotFound", "No questions found for this quiz.");

            var studentAnswers = await _context.StudentAnswers
                .AsNoTracking()
                .Where(sa => sa.StudentId == request.StudentId
                          && questions.Select(q => q.QuestionId).Contains(sa.QuestionId))
                .Select(sa => new
                {
                    sa.QuestionId,
                    sa.StudentAnswer
                })
                .ToListAsync(cancellationToken);

            var answersByQuestionId = studentAnswers
                .GroupBy(sa => sa.QuestionId)
                .ToDictionary(g => g.Key, g => g.Last().StudentAnswer);

            var essayDegreesMap = (request.EssayDegree ?? [])
                .GroupBy(e => e.QuestionId)
                .ToDictionary(g => g.Key, g => g.Last().Degree);

            float totalDegree = 0f;

            foreach (var q in questions)
            {
                answersByQuestionId.TryGetValue(q.QuestionId, out var studentAns);

                bool isMcq = q.Choices is { Count: > 0 };

                if (isMcq)
                {
                    if (!string.IsNullOrWhiteSpace(q.CorrectAnswer) &&
                        string.Equals(studentAns?.Trim(), q.CorrectAnswer.Trim(), StringComparison.OrdinalIgnoreCase))
                    {
                        totalDegree += q.Degree;
                    }
                }
                else
                {
                    if (essayDegreesMap.TryGetValue(q.QuestionId, out var givenDegree))
                    {
                        if (givenDegree < 0 || givenDegree > q.Degree)
                        {
                            return Error.Validation(
                                "EssayDegree.Invalid",
                                $"Degree ({givenDegree}) for question {q.QuestionId} must be between 0 and {q.Degree}.");
                        }

                        totalDegree += givenDegree;
                    }
                }
            }

            var studentQuiz = await _context.StudentQuiz
                .FirstOrDefaultAsync(sq => sq.QuizId == request.QuizId && sq.StudentId == request.StudentId, cancellationToken);

            if (studentQuiz is null)
                return Error.NotFound("StudentQuiz.NotFound", "Student quiz record not found.");

            studentQuiz.TotalDegree = totalDegree;


            await _context.SaveChangesAsync(cancellationToken);

            return totalDegree;
        }
    }
}