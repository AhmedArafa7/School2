using FluentValidation;
using Hospital_VerticalSlice.Entities;
using Hospital_VerticalSlice.Entities.Enum;
using Hospital_VerticalSlice.Features.Admin;
using Hospital_VerticalSlice.Infrastructure;
using Hospital_VerticalSlice.Infrastructure.Services;
using Hospital_VerticalSlice.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hospital_VerticalSlice.Features.StudentFeatures.Quiz
{

    public record AnswerQuizCommand(
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
            var questionAnswer = request.QuestionAnswers.Select(a=> new StudentAnswers
            {
                StudentAnswer = a.Answer,
                QuestionId = a.QuestionId,
                StudentId = currentUserId ?? 0
            }).ToList();


            _context.AddRange(questionAnswer);
            await _context.SaveChangesAsync();
            return Result.Ok();
        }
    }
}

