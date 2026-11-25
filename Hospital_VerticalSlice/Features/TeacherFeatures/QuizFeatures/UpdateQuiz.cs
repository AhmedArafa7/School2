using FluentValidation;
using School_VerticalSlice.Entities;
using School_VerticalSlice.Entities.Enum;
using School_VerticalSlice.Features.Admin;
using School_VerticalSlice.Infrastructure;
using School_VerticalSlice.Infrastructure.Services;
using School_VerticalSlice.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace School_VerticalSlice.Features.TeacherFeatures.QuizFeatures
{

    public record UpdateQuizCommand(
        int QuizId,
        string? Title,
        Subject? Subject,
        List<int>? AccessedClassIds,
        bool MyClassesOnly,
        DateTime? Starttime,
        DateTime? Endtime,
        int? Duration,
        List<UpdateQuestionComm> QuestionComm
        ) : IRequest<Result<string>>;

    public record UpdateQuestionComm(
        string? Question,
        List<string>? Choices,
        string? CorrectAnswer,
        float? Degree
        );

    public class UpdateQuizValidator : AbstractValidator<UpdateQuizCommand>
    {
        public UpdateQuizValidator()
        {
            // التحقق من الأسئلة في حال تم إرسالها
            When(x => x.QuestionComm != null && x.QuestionComm.Any(), () =>
            {
                RuleForEach(x => x.QuestionComm).ChildRules(q =>
                {
                    q.RuleFor(x => x.Question)
                        .NotEmpty()
                        .WithMessage("Question text cannot be empty if questions are provided.");

                    q.RuleFor(x => x.CorrectAnswer)
                        .NotEmpty()
                        .WithMessage("Correct Answer is required for each question.");

                    q.RuleFor(x => x.Degree)
                        .NotNull()
                        .GreaterThan(0)
                        .WithMessage("Degree must be greater than zero.");
                });
            });
        }
    }

    public class UpdateQuizHandler : IRequestHandler<UpdateQuizCommand, Result<string>>
    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public UpdateQuizHandler(ApplicationDbContext context, ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }
        public async Task<Result<string>> Handle(UpdateQuizCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUser.UserId;
            var quizQuery = _context.Quizzes
                .Where(s => s.TeacherId == currentUserId);

            if (request.QuestionComm != null)
                quizQuery = quizQuery.Include(q => q.Question);
            if (request.AccessedClassIds != null)
                quizQuery = quizQuery.Include(q => q.Classes);

            var quiz = await quizQuery.FirstOrDefaultAsync(s => s.Id == request.QuizId);
            if (quiz == null)
                return Error.Unauthorized("You are not authorized to access this session's attendance."); // look again

            bool isChanged = false;
            if (!string.IsNullOrEmpty(request.Title) && quiz.Title != request.Title)
            {
                quiz.Title = request.Title;
                isChanged = true;
            }
            if (request.Subject.HasValue && quiz.Subject != request.Subject.Value)
            {
                quiz.Subject = request.Subject.Value;
                isChanged = true;
            }
            if (request.Starttime.HasValue && quiz.Starttime != request.Starttime.Value)
            {
                quiz.Starttime = request.Starttime.Value;
                isChanged = true;
            }

            if (request.Endtime.HasValue && quiz.Endtime != request.Endtime.Value)
            {
                quiz.Endtime = request.Endtime.Value;
                isChanged = true;
            }

            if (request.Duration.HasValue && quiz.Duration != request.Duration.Value)
            {
                quiz.Duration = request.Duration.Value;
                isChanged = true;
            }

            if (request.AccessedClassIds != null && request.AccessedClassIds.Any())
            {
                var newClassesQuery = _context.Classes
                    .Where(c => request.AccessedClassIds.Contains(c.Id));

                if (request.MyClassesOnly)
                    newClassesQuery = newClassesQuery
                        .Where(c => c.Schedules.Sessions.Any(se => se.Teacher.Id == currentUserId));

                var newClasses = await newClassesQuery.ToListAsync();
                if (newClasses.Any())
                {
                    quiz.Classes.Clear();
                    foreach (var cls in newClasses)
                    {
                        quiz.Classes.Add(cls);
                    }
                    isChanged = true;
                }
            }

            if (request.QuestionComm != null && request.QuestionComm.Any())
            {
                _context.Questions.RemoveRange(quiz.Question);

                quiz.Question = request.QuestionComm.Select(q => new Question
                {
                    question = q.Question ?? string.Empty,
                    Choices = q.Choices,
                    CorrectAnswer = q.CorrectAnswer ?? string.Empty,
                    Degree = q.Degree ?? 1.00F,
                    QuizId = quiz.Id
                }).ToList();

                isChanged = true;
            }

            if (!isChanged)
            {
                return Error.Validation("There is nothing changed.");
            }

            await _context.SaveChangesAsync();
            return "update done";
        }
    }
}

