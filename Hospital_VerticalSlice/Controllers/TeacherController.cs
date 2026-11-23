using School_VerticalSlice.Entities;
using School_VerticalSlice.Features.Admin.ScheduleFeatures;
using School_VerticalSlice.Features.TeacherFeatures;
using School_VerticalSlice.Features.TeacherFeatures.QuizFeatures;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace School_VerticalSlice.Controllers
{
    [Authorize(Roles = "Teacher")]
    public class TeacherController : ApiBaseController
    {
        private readonly IMediator _mediator;
        public TeacherController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet("GetHisSchedule")]
        public async Task<ActionResult> GetHisSchedule()
        {
            var result = await _mediator.Send(new GetHisScheduleCommand());
            return HandleResult(result);
        }
        [HttpGet("GetHisClassStudents/{ClassId}")]
        public async Task<ActionResult> GetHisClassStudents(int ClassId)
        {
            var result = await _mediator.Send(new GetHisClassStudentsCommand(ClassId));
            return HandleResult(result);
        }
        [HttpPost("PostStudentsAttendence")]
        public async Task<ActionResult> PostStudentsAttendence([FromBody] PostStudentsAttendenceCommand command)
        {
            var result = await _mediator.Send(command);
            return HandleResult(result);
        }
        [HttpGet("GetSessionAttendance/{SessionId}")]
        public async Task<ActionResult> GetSessionAttendance(int SessionId)
        {
            var result = await _mediator.Send(new GetSessionAttendanceCommand(SessionId));
            return HandleResult(result);
        }
        [HttpGet("GetStudentAttendance/{StudentId}")]
        public async Task<ActionResult> GetStudentAttendance(int StudentId)
        {
            var result = await _mediator.Send(new GetStudentAttendanceCommand(StudentId));
            return HandleResult(result);
        }
        [HttpPut("PutSpecificAttendance")]
        public async Task<ActionResult> PutSpecificAttendance([FromBody] PutSpecificAttendanceCommand command)
        {
            var result = await _mediator.Send(command);
            return HandleResult(result);
        }
        [HttpPost("CreateQuiz")]
        public async Task<ActionResult> CreateQuiz([FromBody] CreateQuizCommand command)
        {
            var result = await _mediator.Send(command);
            return HandleResult(result);
        }
        [HttpGet("GetHisQuizzes")]
        public async Task<ActionResult> GetHisQuizzes(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetHisQuizzesCommand(),cancellationToken);
            return HandleResult(result);
        }
        [HttpGet("GetQuiz/{QuizId}")]
        public async Task<ActionResult> GetQuiz(int QuizId)
        {
            var result = await _mediator.Send(new GetQuizCommand(QuizId));
            return HandleResult(result);
        }
        [HttpPut("UpdateQuiz/{QuizId}")]
        public async Task<ActionResult> UpdateQuiz(int QuizId,[FromBody] UpdateQuizCommand request)
        {
            var command = new UpdateQuizCommand(
                QuizId: QuizId,
                Title: request.Title,
                Subject: request.Subject,
                AccessedClassIds: request.AccessedClassIds,
                MyClassesOnly: request.MyClassesOnly,
                Starttime: request.Starttime,
                Endtime: request.Endtime,
                Duration: request.Duration,
                QuestionComm: request.QuestionComm
            );
            var result = await _mediator.Send(command);
            return HandleResult(result);
        }

        [HttpDelete("DeleteQuiz/{QuizId}")]
        public async Task<ActionResult> DeleteQuiz(int QuizId)
        {
            var command = new DeleteQuizCommand(QuizId);
            var result = await _mediator.Send(command);
            return HandleResult(result);
        }
    }
}
