using F1Analytics.Requests.Measurement;
using FluentValidation;

namespace F1Analytics.Validators.Measurements;

public class CreateMeasurementRequestValidator : AbstractValidator<CreateMeasurementRequest>
{
    public CreateMeasurementRequestValidator()
    {
        RuleFor(x => x.SeriesId)
            .NotEmpty()
            .WithMessage("SeriesId is required")
            .Length(3, 50)
            .WithMessage("SeriesId must be between 3 and 50 characters");

        RuleFor(x => x.Value)
            .NotEmpty()
            .WithMessage("Value is required")
            .InclusiveBetween(-1500, 500)
            .WithMessage("Value must be between -1500 and 500 Newtons");

        RuleFor(x => x.Timestamp)
            .NotEmpty()
            .WithMessage("Timestamp is required")
            .LessThanOrEqualTo(DateTime.UtcNow.AddMinutes(5))
            .WithMessage("Timestamp cannot be in the future")
            .GreaterThan(new DateTime(2020, 1, 1))
            .WithMessage("Timestamp must be after January 1, 2020");
    }
}