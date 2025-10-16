using F1Analytics.Requests.Measurement;
using FluentValidation;

namespace F1Analytics.Validators.Measurements;

public class UpdateMeasurementRequestValidator : AbstractValidator<UpdateMeasurementRequest>
{
    public UpdateMeasurementRequestValidator()
    {
        RuleFor(x => x.Value)
            .InclusiveBetween(-1500, 500)
            .WithMessage("Value must be between -1500 and 500 Newtons")
            .When(x => x.Value.HasValue);

        RuleFor(x => x.Timestamp)
            .LessThanOrEqualTo(DateTime.UtcNow.AddMinutes(5))
            .WithMessage("Timestamp cannot be in the future")
            .GreaterThan(new DateTime(2020, 1, 1))
            .WithMessage("Timestamp must be after January 1, 2020")
            .When(x => x.Timestamp.HasValue);

        RuleFor(x => x.SeriesId)
            .Length(3, 50)
            .WithMessage("SeriesId must be between 3 and 50 characters")
            .When(x => !string.IsNullOrEmpty(x.SeriesId));

        RuleFor(x => x.Notes)
            .MaximumLength(200)
            .WithMessage("Notes cannot exceed 200 characters")
            .When(x => !string.IsNullOrEmpty(x.Notes));
    }
}