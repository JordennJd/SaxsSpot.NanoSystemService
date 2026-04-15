using FluentValidation;

namespace SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Commands.UpdateSeriesComment;

public class UpdateSeriesCommentValidator : AbstractValidator<UpdateSeriesCommentCommand>
{
    public const int MaxCommentLength = 8000;

    public UpdateSeriesCommentValidator()
    {
        RuleFor(x => x.SeriesId).NotEmpty();
        RuleFor(x => x.Comment)
            .MaximumLength(MaxCommentLength)
            .When(x => x.Comment is not null);
    }
}
