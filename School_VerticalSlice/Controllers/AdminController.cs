using School_VerticalSlice.Entities;
using School_VerticalSlice.Features.Admin;
using School_VerticalSlice.Features.Admin.ScheduleFeatures;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using School_VerticalSlice.Features.Admin.TermStartDate;

namespace School_VerticalSlice.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : ApiBaseController
    {
        [HttpPost("AddTeacher")]
        public Task<ActionResult> AddTeacher([FromBody] AddTeacherCommand command, CancellationToken ct)
             => SendAsync(command, ct);

        [HttpGet("GetTeachers")]
        public Task<ActionResult> GetTeachers(CancellationToken ct)
            => SendAsync(new GetTeachersCommand(), ct);

        [HttpDelete("DeleteTeacher/{TeacherId}")]
        public Task<ActionResult> DeleteTeacher(int TeacherId, CancellationToken ct)
            => SendAsync(new DeleteTeacherCommand(TeacherId), ct);

        [HttpPut("UpdateTeacher/{TeacherId}")]
        public Task<ActionResult> UpdateTeacher(int TeacherId, [FromBody] UpdateTeacherCommand request, CancellationToken ct)
            => SendAsync(request with { Id = TeacherId }, ct);

        [HttpPost("AddStudent")]
        public Task<ActionResult> AddStudent([FromBody] AddStudentCommand command, CancellationToken ct)
            => SendAsync(command, ct);

        [HttpGet("GetStudents")]
        public Task<ActionResult> GetStudents(CancellationToken ct)
            => SendAsync(new GetStudentsCommand(), ct);

        [HttpDelete("DeleteStudent/{StudentId}")]
        public Task<ActionResult> DeleteStudent(int StudentId, CancellationToken ct)
            => SendAsync(new DeleteStudentCommand(id: StudentId), ct);

        [HttpPut("UpdateStudent/{StudentId}")]
        public Task<ActionResult> UpdateStudent(int StudentId, [FromBody] UpdateStudentCommand request, CancellationToken ct)
            => SendAsync(request with { Studentid = StudentId }, ct);

        [HttpGet("GetClassesHasScherule")]
        public Task<ActionResult> GetClassesHasScherule(CancellationToken ct)
            => SendAsync(new GetClassesHasScheruleCommand(), ct);

        [HttpPost("AddFullSchedule")]
        public Task<ActionResult> AddFullSchedule([FromBody] AddClassScheduleCommand command, CancellationToken ct)
            => SendAsync(command, ct);

        [HttpGet("GetTeachersSchedules/{TeacherId}")]
        public Task<ActionResult> GetTeacherSchedule(int TeacherId, CancellationToken ct)
            => SendAsync(new GetTeacherScheduleCommand(TeacherId), ct);

        [HttpGet("GetClassSchedule/{ClassId}")]
        public Task<ActionResult> GetClassSchedule(int ClassId, CancellationToken ct)
            => SendAsync(new GetClassScheduleCommand(ClassId), ct);

        [HttpPut("UpdateSessionSchedule/{SessionId}")]
        public Task<ActionResult> UpdateSessionSchedule(int SessionId, [FromBody] UpdateSessionScheduleCommand request, CancellationToken ct)
            => SendAsync(request with { SessionId = SessionId }, ct);



        [HttpGet("GetTermStartDate")]
        public Task<ActionResult> GetTermStartDate(CancellationToken ct)
            => SendAsync(new GetTermStartDateCommand(), ct);

        [HttpPut("PutTermStartDate")]
        public Task<ActionResult> PutTermStartDate([FromBody] PutTermStartDateCommand command, CancellationToken ct)
            => SendAsync(command, ct);
    }
}
