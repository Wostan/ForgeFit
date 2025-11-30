using FluentValidation;
using ForgeFit.Application.DTOs.Food;

namespace ForgeFit.Api.Validations.Food;

public class RecognizeByPhotoRequestValidator : AbstractValidator<RecognizeByPhotoRequest>
{
    public RecognizeByPhotoRequestValidator()
    {
        RuleFor(x => x.ImageBase64)
            .NotEmpty().WithMessage("Image is required.");
    }
}