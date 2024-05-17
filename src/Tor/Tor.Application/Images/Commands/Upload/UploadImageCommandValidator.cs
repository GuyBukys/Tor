using FluentValidation;

namespace Tor.Application.Images.Commands.Upload;

public sealed class UploadImageCommandValidator : AbstractValidator<UploadImageCommand>
{
    public UploadImageCommandValidator()
    {
        RuleFor(x => x.ImageType)
            .IsInEnum();

        RuleFor(x => x.EntityType)
            .IsInEnum();

        RuleFor(x => x.ContentAsBase64)
            .NotEmpty()
            .Must(x => IsBase64String(x))
            .WithMessage("content is not valid base64 string");
    }

    private static bool IsBase64String(string base64)
    {
        Span<byte> buffer = new(new byte[base64.Length]);

        return Convert.TryFromBase64String(base64, buffer, out int bytesParsed);
    }
}
