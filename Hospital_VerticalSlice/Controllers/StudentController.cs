using Hospital_VerticalSlice.Entities.Enum;
using Hospital_VerticalSlice.Features.StudentFeatures;
using Hospital_VerticalSlice.Features.StudentFeatures.Quiz;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hospital_VerticalSlice.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : ApiBaseController
    {
        private readonly IMediator _mediator;

        public StudentController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet("GetHisSchedule")]
        public async Task<ActionResult> GetHisSchedule()
        {
            var result = await _mediator.Send(new GetHisScheduleCommand());
            return HandleResult(result);
        }
        [HttpGet("GetHisQuizzes")]
        public async Task<ActionResult> GetHisQuizzes()
        {
            var result = await _mediator.Send(new GetHisQuizzesCommand());
            return HandleResult(result);
        }
        [HttpGet("GetHisQuiz/{QuizId}")]
        public async Task<ActionResult> GetHisQuiz([FromRoute] int QuizId)
        {
            var result = await _mediator.Send(new GetHisQuizCommand(QuizId));
            return HandleResult(result);
        }

        [HttpPost("AnswerQuiz")]
        public async Task<ActionResult> AnswerQuiz([FromRoute] AnswerQuizCommand command)
        {
            var result = await _mediator.Send(command);
            return HandleResult(result);
        }
    }
}
