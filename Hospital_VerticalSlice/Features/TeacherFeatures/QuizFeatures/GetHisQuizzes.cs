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
    public record GetHisQuizzesResponce(
        int QuizId,
        string Title,
        Subject? Subject,
        List<int> AccessedClassIds,
        DateTime Starttime,
        DateTime Endtime,
        int Duration
    );

    public record GetHisQuizzesCommand() : IRequest<Result<List<GetHisQuizzesResponce>>>;

    public class GetHisQuizzesHandler : IRequestHandler<GetHisQuizzesCommand, Result<List<GetHisQuizzesResponce>>>
    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public GetHisQuizzesHandler(ApplicationDbContext context, ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<Result<List<GetHisQuizzesResponce>>> Handle(GetHisQuizzesCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUser.UserId;

            var quiz = await _context.Quizzes
                .Where(q => q.TeacherId == currentUserId && q.IsDeleted == false)
                .Select(q => new GetHisQuizzesResponce(
                    QuizId: q.Id,
                    Title: q.Title,
                    Subject: q.Subject,
                    AccessedClassIds: q.Classes.Select(c => c.Id).ToList(),
                    Starttime: q.Starttime,
                    Endtime: q.Endtime,
                    Duration: q.Duration
                )).ToListAsync();

            if (quiz == null)
                return Error.NotFound("you don't have any quizzes yet");



            return quiz;

        }

    }

}

