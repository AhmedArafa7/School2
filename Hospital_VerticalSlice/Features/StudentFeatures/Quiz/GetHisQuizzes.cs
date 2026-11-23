using School_VerticalSlice.Infrastructure;
using School_VerticalSlice.Infrastructure.Services;
using School_VerticalSlice.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace School_VerticalSlice.Features.StudentFeatures.Quiz
{
    public record GetHisQuizzesCommand() : IRequest<Result<List<int>>>;

    public class GetHisQuizzesHandler : IRequestHandler<GetHisQuizzesCommand, Result<List<int>>>
    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public GetHisQuizzesHandler(ApplicationDbContext context, ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<Result<List<int>>> Handle(GetHisQuizzesCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUser.UserId;

            var questions = await _context.Quizzes
                .Where(q => q.Classes.Any(s => s.Students.Any(s => s.Id == currentUserId)) && q.IsDeleted != true)
                .Select(q => q.Id).ToListAsync();
            // add status , total degree

            return questions;
        }
    }

}
