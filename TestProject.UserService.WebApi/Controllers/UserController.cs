using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TestProject.UserService.Application.Auth.Login;
using TestProject.UserService.Application.Auth.Register;

namespace TestProject.UserService.WebApi.Controllers;

[ApiController]
[Route("api/user")]
public sealed class UserController : ControllerBase
{
    private readonly IMediator _mediator;

    public UserController(IMediator mediator) => _mediator = mediator;

    public sealed record RegisterRequest(string Name, string Password);
    public sealed record LoginRequest(string Name, string Password);

    [HttpPost("register")]
    public async Task<ActionResult<RegisterUserResult>> Register(RegisterRequest req, CancellationToken ct)
    {
        var result = await _mediator.Send(new RegisterUserCommand(req.Name, req.Password), ct);
        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResult>> Login(LoginRequest req, CancellationToken ct)
    {
        var result = await _mediator.Send(new LoginCommand(req.Name, req.Password), ct);
        return Ok(result);
    }

    [Authorize]
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        // Stateless JWT: logout = клиент удалил токен.
        return NoContent();
    }
}