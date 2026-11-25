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
        [HttpGet("GetHisSchedule")]
        public Task<ActionResult> GetHisSchedule(CancellationToken ct)
            => SendAsync(new GetHisScheduleCommand(), ct);

        [HttpGet("GetHisClassStudents/{ClassId}")]
        public Task<ActionResult> GetHisClassStudents(int ClassId, CancellationToken ct)
            => SendAsync(new GetHisClassStudentsCommand(ClassId), ct);

        [HttpPost("PostStudentsAttendence")]
        public Task<ActionResult> PostStudentsAttendence([FromBody] PostStudentsAttendenceCommand command, CancellationToken ct)
            => SendAsync(command, ct);

        [HttpGet("GetSessionAttendance/{SessionId}")]
        public Task<ActionResult> GetSessionAttendance(int SessionId, CancellationToken ct)
            => SendAsync(new GetSessionAttendanceCommand(SessionId), ct);

        [HttpGet("GetStudentSessionAttendance/{StudentId}")]
        public Task<ActionResult> GetStudentSessionAttendance(int StudentId, [FromQuery] int SessionId, CancellationToken ct)
            => SendAsync(new GetStudentSessionAttendanceCommand(StudentId, SessionId), ct);

        [HttpPut("PutSpecificAttendance")]
        public Task<ActionResult> PutSpecificAttendance([FromBody] PutSpecificAttendanceCommand command, CancellationToken ct)
            => SendAsync(command, ct);

        [HttpPost("CreateQuiz")]
        public Task<ActionResult> CreateQuiz([FromBody] CreateQuizCommand command, CancellationToken ct)
            => SendAsync(command, ct);

        [HttpGet("GetHisQuizzes")]
        public Task<ActionResult> GetHisQuizzes(CancellationToken ct)
            => SendAsync(new GetHisQuizzesCommand(), ct);

        [HttpGet("GetQuiz/{QuizId}")]
        public Task<ActionResult> GetQuiz(int QuizId, CancellationToken ct)
            => SendAsync(new GetQuizCommand(QuizId), ct);

        [HttpPut("UpdateQuiz/{QuizId}")]
        public Task<ActionResult> UpdateQuiz(int QuizId, [FromBody] UpdateQuizCommand request, CancellationToken ct)
            => SendAsync(request with { QuizId = QuizId }, ct);

        [HttpDelete("DeleteQuiz/{QuizId}")]
        public Task<ActionResult> DeleteQuiz(int QuizId, CancellationToken ct)
            => SendAsync(new DeleteQuizCommand(QuizId), ct);
        [HttpPost("GradeEssayAnswers")]
        public Task<ActionResult> GradeEssayAnswers([FromBody] GradeEssayAnswersCommand command, CancellationToken ct)
            => SendAsync(command, ct);
    }
}
