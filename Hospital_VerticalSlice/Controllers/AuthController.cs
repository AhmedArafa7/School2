using School_VerticalSlice.Features.Auth;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace School_VerticalSlice.Controllers
{
    public class AuthController : ApiBaseController
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<ActionResult> Login([FromBody] LoginCommand command)
        {
            var result = await _mediator.Send(command);
            if(!result.IsSuccess)
                return HandleResult(result);    

            Response.Cookies.Append("refreshToken", result.Value.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true, 
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddDays(7)
            });

            return Ok(new
            {
                accessToken = result.Value.AccessToken,
                role = result.Value.Role,
                Name = result.Value.Name
            });
        }

        [HttpPost("RefreshToken")]
        public async Task<ActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(refreshToken))
            {
                return Unauthorized(new { message = "جلسة العمل منتهية، يرجى تسجيل الدخول مجدداً." });
            }

            var result = await _mediator.Send(new RefreshTokenCommand(refreshToken));
            if (!result.IsSuccess)
                return HandleResult(result);

            Response.Cookies.Append("refreshToken", result.Value.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddDays(7)
            });

            return Ok(new
            {
                accessToken = result.Value.AccessToken,
                role = result.Value.Role,
                Name = result.Value.Name
            });
        }


        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var refreshToken = Request.Cookies["refreshToken"];

            var result = await _mediator.Send(new LogoutCommand(refreshToken));

            Response.Cookies.Delete("refreshToken", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            });

            return Ok(new { message = result.Value });
        }
    }
}
