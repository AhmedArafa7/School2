using School_VerticalSlice.Features.Auth;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace School_VerticalSlice.Controllers
{
    public class AuthController : ApiBaseController
    {
        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<ActionResult> Login([FromBody] LoginCommand command, CancellationToken ct)
        {
            var result = await Sender.Send(command, ct);
            if (!result.IsSuccess)
                return HandleResult(result);

            SetRefreshTokenCookie(result.Value.RefreshToken);

            return Ok(new
            {
                accessToken = result.Value.AccessToken,
                role = result.Value.Role,
                Name = result.Value.Name
            });
        }

        [HttpPost("RefreshToken")]
        public async Task<ActionResult> RefreshToken(CancellationToken ct)
        {
            var refreshToken = Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(refreshToken))
                return Unauthorized(new { message = "جلسة العمل منتهية، يرجى تسجيل الدخول مجدداً." });

            var result = await Sender.Send(new RefreshTokenCommand(refreshToken), ct);
            if (!result.IsSuccess)
                return HandleResult(result);

            SetRefreshTokenCookie(result.Value.RefreshToken);

            return Ok(new
            {
                accessToken = result.Value.AccessToken,
                role = result.Value.Role,
                Name = result.Value.Name
            });
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout(CancellationToken ct)
        {
            var refreshToken = Request.Cookies["refreshToken"];

            var result = await Sender.Send(new LogoutCommand(refreshToken), ct);

            Response.Cookies.Delete("refreshToken", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            });

            if (!result.IsSuccess)
                return HandleResult(result);

            return Ok(new { message = result.Value });
        }

        private void SetRefreshTokenCookie(string refreshToken)
        {
            Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddDays(7)
            });
        }
    }
}
