using School_VerticalSlice.Entities.Enum;
using School_VerticalSlice.Features.StudentFeatures;
using School_VerticalSlice.Features.StudentFeatures.Quiz;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace School_VerticalSlice.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : ApiBaseController
    {
        [HttpGet("GetHisSchedule")]
        public Task<ActionResult> GetHisSchedule(CancellationToken ct)
            => SendAsync(new GetHisScheduleCommand(), ct);

        [HttpGet("GetHisQuizzes")]
        public Task<ActionResult> GetHisQuizzes(CancellationToken ct)
            => SendAsync(new GetHisQuizzesCommand(), ct);

        [HttpGet("GetHisQuiz/{QuizId}")]
        public Task<ActionResult> GetHisQuiz([FromRoute] int QuizId, CancellationToken ct)
            => SendAsync(new GetHisQuizCommand(QuizId), ct);

        [HttpPost("AnswerQuiz")]
        public Task<ActionResult> EnterQuiz([FromBody] AnswerQuizCommand command, CancellationToken ct)
            => SendAsync(command, ct);

        [HttpGet("GetQuizReview/{QuizId}")]
        public Task<ActionResult> GetQuizReview([FromRoute] int QuizId, CancellationToken ct)
            => SendAsync(new GetQuizReviewQuery(QuizId), ct);
    }
}
