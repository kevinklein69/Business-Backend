using MediatR;

namespace Business.Application.Features.Auth.ChangePassword;

public record ChangePasswordCommand(string CurrentPassword, string NewPassword) : IRequest;
