using FluentValidation;

namespace Business.Application.Features.Orders.UploadAttachments;

public class UploadOrderAttachmentsCommandValidator : AbstractValidator<UploadOrderAttachmentsCommand>
{
    public const long MaxFileSizeBytes = 10 * 1024 * 1024;

    public static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".pdf", ".docx", ".xlsx"];

    private static readonly string[] AllowedContentTypes =
    [
        "image/jpeg",
        "image/png",
        "application/pdf",
        "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        "application/octet-stream"
    ];

    public UploadOrderAttachmentsCommandValidator()
    {
        RuleFor(x => x.Files)
            .NotEmpty().WithMessage("Mindestens eine Datei ist erforderlich.");

        RuleForEach(x => x.Files).ChildRules(file =>
        {
            file.RuleFor(f => f.FileName)
                .NotEmpty().WithMessage("Der Dateiname ist erforderlich.")
                .MaximumLength(255);

            file.RuleFor(f => f.FileName)
                .Must(name => AllowedExtensions.Contains(Path.GetExtension(name).ToLowerInvariant()))
                .WithMessage("Nur JPG, PNG, PDF, DOCX und XLSX sind erlaubt.");

            file.RuleFor(f => f.ContentType)
                .Must(type => AllowedContentTypes.Contains(type.ToLowerInvariant()))
                .WithMessage("Der Dateityp wird nicht unterstützt.");

            file.RuleFor(f => f.SizeBytes)
                .GreaterThan(0).WithMessage("Die Datei darf nicht leer sein.")
                .LessThanOrEqualTo(MaxFileSizeBytes).WithMessage("Die Datei darf maximal 10 MB groß sein.");
        });
    }
}
