using F1Analytics.Requests.Series;
using FluentValidation;

namespace F1Analytics.Validators.Series;

public class UpdateSeriesRequestValidator : AbstractValidator<UpdateSeriesRequest>
{
    public UpdateSeriesRequestValidator()
    {
        RuleFor(x => x.Name)
            .Length(3, 100)
            .WithMessage("Name must be between 3 and 100 characters")
            .When(x => !string.IsNullOrEmpty(x.Name));

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .WithMessage("Description cannot exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.MinValue)
            .InclusiveBetween(-1500, -50)
            .WithMessage("MinValue must be between -1500 and -50 Newtons")
            .When(x => x.MinValue.HasValue);

        RuleFor(x => x.MaxValue)
            .InclusiveBetween(-1500, -50)
            .WithMessage("MaxValue must be between -1500 and -50 Newtons")
            .When(x => x.MaxValue.HasValue);

        RuleFor(x => x.Unit)
            .MaximumLength(10)
            .WithMessage("Unit cannot exceed 10 characters")
            .When(x => !string.IsNullOrEmpty(x.Unit));

        RuleFor(x => x.Color)
            .Matches(@"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$")
            .WithMessage("Color must be a valid hex color code")
            .When(x => !string.IsNullOrEmpty(x.Color));

        RuleFor(x => x.MeasurementType)
            .MaximumLength(50)
            .WithMessage("MeasurementType cannot exceed 50 characters")
            .When(x => !string.IsNullOrEmpty(x.MeasurementType));

        // Cross-field validation
        RuleFor(x => x)
            .Must(x => x.MaxValue > x.MinValue)
            .WithMessage("MaxValue must be greater than MinValue")
            .When(x => x.MinValue.HasValue && x.MaxValue.HasValue);
    }
}