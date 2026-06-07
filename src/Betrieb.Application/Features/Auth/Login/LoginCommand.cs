using MediatR;

namespace Betrieb.Application.Features.Auth.Login;

public record LoginCommand(string Email, string Password) : IRequest<LoginResult>;

public record LoginResult(string Token);
