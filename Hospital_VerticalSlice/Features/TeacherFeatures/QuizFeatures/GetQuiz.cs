using Azure.Core;
using FluentValidation;
using Hospital_VerticalSlice.Entities;
using Hospital_VerticalSlice.Entities.Enum;
using Hospital_VerticalSlice.Infrastructure;
using Hospital_VerticalSlice.Infrastructure.Services;
using Hospital_VerticalSlice.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Hospital_VerticalSlice.Features.TeacherFeatures.QuizFeatures
{
    public record GetQuizResponce(
        string Title,
        Subject Subject,
        List<int> AccessedClassIds,
        DateTime Starttime,
        DateTime Endtime,
        int Duration,
        List<GetQuestionResponce> QuestionResponces
        );
    public record GetQuestionResponce(
        string Question,
        List<string>? Choices,
        string Answer,
        float Degree
        );
    public record GetQuizCommand(
        int QuizId
    ) : IRequest<Result<GetQuizResponce>>;

    public class GetQuizValidator : AbstractValidator<GetQuizCommand>
    {
        public GetQuizValidator()
        {
            RuleFor(s => s.QuizId)
                .GreaterThan(0)
                .WithMessage("Invalid QuizId.");
        }
    }

    public class GetQuizHandler : IRequestHandler<GetQuizCommand, Result<GetQuizResponce>>
    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public GetQuizHandler(ApplicationDbContext context, ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<Result<GetQuizResponce>> Handle(GetQuizCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUser.UserId;

            var quiz = await _context.Quizzes
                .AsNoTracking()
                .Where(q => q.Id == request.QuizId && q.TeacherId == currentUserId && q.IsDeleted != true)
                .Select(q => new GetQuizResponce(
                    Title: q.Title,
                    Subject: q.Subject,
                    AccessedClassIds: q.Classes.Select(c => c.Id).ToList(),
                    Starttime: q.Starttime,
                    Endtime: q.Endtime,
                    Duration: q.Duration,
                    QuestionResponces: q.Question.Select(qe => new GetQuestionResponce(
                                            Question: qe.question,
                                            Choices: qe.Choices,
                                            Answer: qe.Answer,
                                            Degree: qe.Degree

                    )).ToList()
                )).FirstOrDefaultAsync();

            if (quiz == null)
                return Error.NotFound("you don't have access on that quiz");



            return quiz;

        }

    }

}

