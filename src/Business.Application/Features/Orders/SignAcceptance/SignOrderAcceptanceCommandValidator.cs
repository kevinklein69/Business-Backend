using FluentValidation;

namespace Business.Application.Features.Orders.SignAcceptance;

public class SignOrderAcceptanceCommandValidator : AbstractValidator<SignOrderAcceptanceCommand>
{
    public SignOrderAcceptanceCommandValidator()
    {
        RuleFor(x => x.SignerName)
            .NotEmpty().WithMessage("Der Name des Unterzeichners ist erforderlich.")
            .MaximumLength(200);

        RuleFor(x => x.SignatureImageBase64)
            .NotEmpty().WithMessage("Die Unterschrift darf nicht leer sein.")
            .Must(BeValidBase64Image).WithMessage("Die Unterschrift ist ungültig.");
    }

    private static bool BeValidBase64Image(string value)
    {
        var commaIndex = value.IndexOf(',');
        var base64 = commaIndex >= 0 ? value[(commaIndex + 1)..] : value;

        if (string.IsNullOrWhiteSpace(base64))
        {
            return false;
        }

        return Convert.TryFromBase64String(base64, new byte[base64.Length], out var bytesWritten) && bytesWritten > 0;
    }
}
