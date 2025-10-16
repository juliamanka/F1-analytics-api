using F1Analytics.Requests.Series;
using FluentValidation;

namespace F1Analytics.Validators.Series;

public class CreateSeriesRequestValidator : AbstractValidator<CreateSeriesRequest>
{
    public CreateSeriesRequestValidator()
    {
        RuleFor(x => x.SeriesId)
            .NotEmpty()
            .WithMessage("SeriesId is required")
            .Length(3, 50)
            .WithMessage("SeriesId must be between 3 and 50 characters")
            .Matches(@"^[a-zA-Z0-9_-]+$")
            .WithMessage("SeriesId can only contain letters, numbers, underscores, and hyphens");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required")
            .Length(3, 100)
            .WithMessage("Name must be between 3 and 100 characters");

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .WithMessage("Description cannot exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.MinValue)
            .NotEmpty()
            .WithMessage("MinValue is required")
            .InclusiveBetween(-1500, -50)
            .WithMessage("MinValue must be between -1500 and -50 Newtons");

        RuleFor(x => x.MaxValue)
            .NotEmpty()
            .WithMessage("MaxValue is required")
            .InclusiveBetween(-1500, -50)
            .WithMessage("MaxValue must be between -1500 and -50 Newtons");

        RuleFor(x => x.Unit)
            .NotEmpty()
            .WithMessage("Unit is required")
            .MaximumLength(10)
            .WithMessage("Unit cannot exceed 10 characters");

        RuleFor(x => x.Color)
            .Matches(@"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$")
            .WithMessage("Color must be a valid hex color code")
            .When(x => !string.IsNullOrEmpty(x.Color));

        RuleFor(x => x.MeasurementType)
            .NotEmpty()
            .WithMessage("MeasurementType is required")
            .MaximumLength(50)
            .WithMessage("MeasurementType cannot exceed 50 characters");

        // Cross-field validation
        RuleFor(x => x.MaxValue)
            .GreaterThan(x => x.MinValue)
            .WithMessage("MaxValue must be greater than MinValue");
    }
}