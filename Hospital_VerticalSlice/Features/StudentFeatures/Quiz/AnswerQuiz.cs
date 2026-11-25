using FluentValidation;
using School_VerticalSlice.Entities;
using School_VerticalSlice.Entities.Enum;
using School_VerticalSlice.Features.Admin;
using School_VerticalSlice.Infrastructure;
using School_VerticalSlice.Infrastructure.Services;
using School_VerticalSlice.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace School_VerticalSlice.Features.StudentFeatures.Quiz
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
                                                              // add chek on each answer and add in if not exist or changed
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

