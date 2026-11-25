using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using School_VerticalSlice.Entities;
using School_VerticalSlice.Infrastructure;
using School_VerticalSlice.Infrastructure.Services;
using School_VerticalSlice.Shared;

namespace School_VerticalSlice.Features.StudentFeatures.Quiz
{
    public record AnswerQuizCommand(
        int QuizId,
        List<QuestionAnswer> QuestionAnswers
    ) : IRequest<Result>;

    public record QuestionAnswer(
        int QuestionId,
        string Answer
    );

    public class AnswerQuizValidator : AbstractValidator<AnswerQuizCommand>
    {
        public AnswerQuizValidator()
        {
            RuleFor(x => x.QuizId).GreaterThan(0);

            RuleFor(x => x.QuestionAnswers)
                .NotNull()
                .NotEmpty();

            RuleForEach(x => x.QuestionAnswers).ChildRules(item =>
            {
                item.RuleFor(q => q.QuestionId).GreaterThan(0);
                item.RuleFor(q => q.Answer).NotNull();
            });
        }
    }

    public class AnswerQuizHandler : IRequestHandler<AnswerQuizCommand, Result>
    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public AnswerQuizHandler(ApplicationDbContext context, ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<Result> Handle(AnswerQuizCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUser.UserId;
            if (currentUserId is null)
                return Error.Unauthorized("User is not authenticated.");

            var studentId = currentUserId.Value;

            // 1. تأكد أن الطالب دخل هذا الكويز فعلاً
            var hasEnteredQuiz = await _context.StudentQuiz
                .AsNoTracking()
                .AnyAsync(sq => sq.QuizId == request.QuizId && sq.StudentId == studentId, cancellationToken);

            if (!hasEnteredQuiz)
                return Error.Validation("Quiz.NotEntered", "You have not entered this quiz.");

            // 2. جلب أرقام الأسئلة الحقيقية التابعة لهذا الكويز فقط
            var validQuizQuestionIds = await _context.Questions
                .AsNoTracking()
                .Where(q => q.QuizId == request.QuizId)
                .Select(q => q.Id)
                .ToListAsync(cancellationToken);

            if (validQuizQuestionIds.Count == 0)
                return Error.NotFound("Quiz.QuestionsNotFound", "No questions found for this quiz.");

            var validQuestionSet = validQuizQuestionIds.ToHashSet();

            // تجميع إجابات الطلب حسب السؤال، واستبعاد أي سؤال لا يتبع هذا الكويز
            var answersByQuestionId = request.QuestionAnswers
                .Where(a => validQuestionSet.Contains(a.QuestionId))
                .GroupBy(a => a.QuestionId)
                .ToDictionary(g => g.Key, g => g.Last().Answer);

            if (answersByQuestionId.Count == 0)
                return Error.Validation("Answers.InvalidQuestions", "None of the submitted questions belong to this quiz.");

            var questionIds = answersByQuestionId.Keys.ToList();

            // 3. فحص الإجابات السابقة للطالب لنفس أسئلة هذا الكويز
            var existingAnswers = await _context.StudentAnswers
                .Where(sa => sa.StudentId == studentId && questionIds.Contains(sa.QuestionId))
                .ToListAsync(cancellationToken);

            var existingByQuestionId = existingAnswers
                .GroupBy(sa => sa.QuestionId)
                .ToDictionary(g => g.Key, g => g.Last());

            var newAnswers = new List<StudentAnswers>();

            foreach (var (questionId, submittedAnswer) in answersByQuestionId)
            {
                if (existingByQuestionId.TryGetValue(questionId, out var existingAnswer))
                {
                    if (!string.Equals(existingAnswer.StudentAnswer, submittedAnswer, StringComparison.Ordinal))
                    {
                        existingAnswer.StudentAnswer = submittedAnswer;
                    }
                }
                else
                {
                    newAnswers.Add(new StudentAnswers
                    {
                        StudentId = studentId,
                        QuestionId = questionId,
                        StudentAnswer = submittedAnswer
                    });
                }
            }

            if (newAnswers.Count > 0)
                await _context.StudentAnswers.AddRangeAsync(newAnswers, cancellationToken);

            if (_context.ChangeTracker.HasChanges())
                await _context.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
    }
}