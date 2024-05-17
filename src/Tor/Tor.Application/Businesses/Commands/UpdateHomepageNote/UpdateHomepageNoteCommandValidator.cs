using FluentValidation;

namespace Tor.Application.Businesses.Commands.UpdateHomepageNote;

public class UpdateHomepageNoteCommandValidator : AbstractValidator<UpdateHomepageNoteCommand>
{
    public UpdateHomepageNoteCommandValidator()
    {
        RuleFor(x => x.BusinessId)
            .NotEmpty();
    }
}
