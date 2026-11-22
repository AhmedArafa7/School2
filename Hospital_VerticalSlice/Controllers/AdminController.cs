using Azure.Core;
using Hospital_VerticalSlice.Entities;
using Hospital_VerticalSlice.Features.Admin;
using Hospital_VerticalSlice.Features.Admin.ScheduleFeatures;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hospital_VerticalSlice.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : ApiBaseController
    {
        private readonly IMediator _mediator;

        public AdminController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("AddTeacher")]
        public async Task<ActionResult> AddTeacher([FromBody] AddTeacherCommand command)
        {
            var result = await _mediator.Send(command);
            return HandleResult(result);
        }

        [HttpGet("GetTeachers")]
        public async Task<ActionResult> GetTeachers()
        {
            var result = await _mediator.Send(new GetTeachersCommand());
            return HandleResult(result);
        }

        [HttpDelete("DeleteTeacher/{TeacherId}")]
        public async Task<ActionResult> DeleteTeacher(int TeacherId)
        {
            var command = new DeleteTeacherCommand(TeacherId);
            var result = await _mediator.Send(command);
            return HandleResult(result);
        }
        [HttpPut("UpdateTeacher/{TeacherId}")]
        public async Task<ActionResult> UpdateTeacher(int TeacherId,[FromBody] UpdateTeacherCommand request)
        {
            var command = new UpdateTeacherCommand(
                    Id: TeacherId,
                    Name: request.Name,
                    Gender: request.Gender,
                    Phone: request.Phone,
                    Subject: request.Subject,
                    Levels: request.Levels,
                    Available: request.Available
            );
            var result = await _mediator.Send(command);
            return HandleResult(result);
        }
        [HttpPost("AddStudent")]
        public async Task<ActionResult> AddStudent([FromBody] AddStudentCommand command)
        {
            var result = await _mediator.Send(command);
            return HandleResult(result);
        }
        [HttpGet("GetStudents")]
        public async Task<ActionResult> GetStudents()
        {
            var result = await _mediator.Send(new GetStudentsCommand());
            return HandleResult(result);
        }
        [HttpDelete("DeleteStudent/{StudentId}")]
        public async Task<ActionResult> DeleteStudent(int StudentId)
        {
            var command = new DeleteStudentCommand(id: StudentId);
            var result = await _mediator.Send(command);
            return HandleResult(result);
        }
        [HttpPut("UpdateStudent/{StudentId}")]
        public async Task<ActionResult> UpdateStudent(int StudentId,[FromBody] UpdateStudentCommand request)
        {
            var command = new UpdateStudentCommand(
                    Studentid: StudentId,
                    Name: request.Name,
                    Gender: request.Gender,
                    Phone: request.Phone,
                    Class: request.Class,
                    Level: request.Level
            );
            var result = await _mediator.Send(command);
            return HandleResult(result);
        }
        [HttpGet("GetClassesHasScherule")]
        public async Task<ActionResult> GetClassesHasScherule([FromQuery] GetClassesHasScheruleCommand command)
        {
            var result = await _mediator.Send(command);
            return HandleResult(result);
        }
        [HttpPost("AddFullSchedule")]
        public async Task<ActionResult> AddFullSchedule([FromBody] AddClassScheduleCommand command)
        {
            var result = await _mediator.Send(command);
            return HandleResult(result);
        }
        [HttpGet("GetTeachersSchedules/{TeacherId}")]
        public async Task<ActionResult> GetTeacherSchedule(int TeacherId)
        {
            var result = await _mediator.Send(new GetTeacherScheduleCommand(TeacherId));
            return HandleResult(result);
        }
        [HttpGet("GetClassSchedule/{ClassId}")]
        public async Task<ActionResult> GetClassSchedule(int ClassId,[FromQuery] GetClassScheduleCommand request)
        {
            var command = new GetClassScheduleCommand(ClassId);
            var result = await _mediator.Send(command);
            return HandleResult(result);
        }
        [HttpPut("UpdateSessionSchedule/{SessionId}")]
        public async Task<ActionResult> UpdateSessionSchedule(int SessionId, [FromBody] UpdateSessionScheduleCommand request)
        {
            var command = new UpdateSessionScheduleCommand(
                ScheduleId: request.ScheduleId,
                SessionId: SessionId,
                TeacherId: request.TeacherId,
                Subject: request.Subject
            );
            var result = await _mediator.Send(command);
            return HandleResult(result);
        }
    }
}
