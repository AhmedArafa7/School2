using Azure.Core;
using FluentValidation;
using School_VerticalSlice.Entities;
using School_VerticalSlice.Entities.Enum;
using School_VerticalSlice.Entities.Enum.Flags;
using School_VerticalSlice.Infrastructure;
using School_VerticalSlice.Infrastructure.Services;
using School_VerticalSlice.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace School_VerticalSlice.Features.TeacherFeatures.QuizFeatures
{
    public record CreateQuizCommand(
        string Title,
        Subject Subject,
        List<int> AccessedClassIds,
        DateTime Starttime,
        DateTime Endtime,
        int Duration,
        List<QuestionComm> QuestionComm
        ) : IRequest<Result<int>>;

    public record QuestionComm(
        string Question,
        List<string>? Choices,
        string Answer,
        float? Degree
        );

    public class CreateQuizValidator : AbstractValidator<CreateQuizCommand>
    {
        public CreateQuizValidator()
        {
            RuleFor(s => s.Subject)
                .IsInEnum()
                .WithMessage("Invalid Subject.");
            RuleForEach(s => s.AccessedClassIds)
                .GreaterThan(0)
                .WithMessage("requir class.");
            RuleForEach(q => q.QuestionComm).ChildRules(q =>
            {
                q.RuleFor(q => q.Question)
                    .NotEmpty()
                    .WithMessage("Question required !");
                q.RuleFor(q => q.Answer)
                    .NotEmpty()
                    .WithMessage("Answer required !");
                q.RuleFor(q => q.Degree)
                    .NotEmpty()
                    .WithMessage("Degree required !");
                q.RuleForEach(q => q.Choices)
                    .NotEmpty()
                    .WithMessage("every choices shoild have value !");
            });
        }
    }

    public class CreateQuizHandler : IRequestHandler<CreateQuizCommand, Result<int>>
    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public CreateQuizHandler(ApplicationDbContext context, ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }
        private static Subjects ToSubjectFlag(Subject subject) => subject switch
        {
            Subject.Arabic => Subjects.Arabic,
            Subject.Eslamic => Subjects.Eslamic,
            Subject.English => Subjects.English,
            Subject.Mathematics => Subjects.Mathematics,
            Subject.Science => Subjects.Science,
            Subject.Art => Subjects.Art,
            Subject.play => Subjects.play,
            Subject.Lunch => Subjects.Lunch,
            _ => Subjects.None
        };

        public async Task<Result<int>> Handle(CreateQuizCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUser.UserId;
            var targetFlag = ToSubjectFlag(request.Subject);
            if (targetFlag == Subjects.None)
            {
                return Error.Validation("Quiz.InvalidSubject", "Invalid subject.");
            }
            var teacherSubjects = await _context.Teachers
                .Where(t=>t.Id == currentUserId)
                .Select(t => t.Subjects)
                .FirstOrDefaultAsync();
            if (!teacherSubjects.HasFlag(targetFlag))
            {
                return Error.Forbidden("Quiz.SubjectNotAllowed", "You are not allowed to create a quiz for this subject.");
            }

            bool quizExists = await _context.Quizzes
                .AnyAsync(q => q.TeacherId == currentUserId && q.Title == request.Title, cancellationToken);

            if (quizExists)
            {
                return Error.Conflict("Quiz", "A quiz with this title already exists."); // أو Error مخصص للتكرار
            }

            var Classes = await _context.Classes
                .Where(c => request.AccessedClassIds.Contains(c.Id))
                .ToListAsync();


            var quiz = new Quiz
            {
                Title = request.Title,
                Classes = Classes,
                Starttime = request.Starttime,
                Endtime = request.Endtime,
                Duration = request.Duration,
                Subject = request.Subject,
                TeacherId = currentUserId ?? 0,
                Question = request.QuestionComm.Select(q => new Question
                {
                    question = q.Question,
                    Choices = q.Choices,
                    Answer = q.Answer,
                    Degree = q.Degree ?? 1.00F
                }).ToList()
            };



            _context.Quizzes.Add(quiz);
            await _context.SaveChangesAsync();
            return quiz.Id;

        }

    }

}

