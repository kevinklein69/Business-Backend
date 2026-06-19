using Business.Application.Features.Auth.ChangePassword;
using Business.Application.Features.Auth.Login;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Business.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(ISender sender) : ControllerBase
{
    public record LoginRequest(string Email, string Password);

    public record ChangePasswordRequest(string CurrentPassword, string NewPassword);

    [HttpPost("login")]
    public async Task<ActionResult<LoginResult>> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await sender.Send(new LoginCommand(request.Email, request.Password), cancellationToken);
            return Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
    }

    [Authorize]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequest request, CancellationToken cancellationToken)
    {
        await sender.Send(new ChangePasswordCommand(request.CurrentPassword, request.NewPassword), cancellationToken);
        return NoContent();
    }
}
