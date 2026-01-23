using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using TestProject.UserService.Application.Auth.Login;
using TestProject.UserService.Application.Auth.Register;
using TestProject.UserService.Application.Currency;

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
    [HttpGet("check-token")]
    public IActionResult CheckToken()
    {
        // Если токен валиден, этот метод будет выполнен, иначе вернётся 401 Unauthorized
        var userId = User.FindFirstValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");

        // Для демонстрации просто вернём userId из токена
        return Ok(new { UserId = userId });
    }

    [Authorize]
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        // Stateless JWT: logout = клиент удалил токен.
        return NoContent();
    }
    
    private bool TryGetUserId(out long userId)
    {
        userId = 0;
        var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return !string.IsNullOrWhiteSpace(raw) && long.TryParse(raw, out userId);
    }

    [Authorize]
    [HttpPost("favorites/{currencyId:long}")]
    public async Task<IActionResult> AddFavorite(long currencyId, CancellationToken ct)
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();

        await _mediator.Send(new AddCurrencyToFavoritesCommand(userId, currencyId), ct);
        return NoContent();
    }

    [Authorize]
    [HttpDelete("favorites/{currencyId:long}")]
    public async Task<IActionResult> RemoveFavorite(long currencyId, CancellationToken ct)
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();

        await _mediator.Send(new RemoveCurrencyFromFavoritesCommand(userId, currencyId), ct);
        return NoContent();
    }

    [Authorize]
    [HttpGet("favorites")]
    public async Task<ActionResult<IReadOnlyList<FavoriteCurrencyDto>>> GetFavorites(CancellationToken ct)
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();

        var result = await _mediator.Send(new GetMyFavoritesQuery(userId), ct);
        return Ok(result);
    }
}