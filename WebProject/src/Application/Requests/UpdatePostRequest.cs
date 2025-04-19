using FluentValidation;
using Infrastructure;

namespace Application.Requests
{
    public class UpdatePostRequest
    {
        public int Id { get; set; }
        public string? Text { get; set; }
    }

    public class UpdatePostRequestValidator : AbstractValidator<UpdatePostRequest>
    {
        public UpdatePostRequestValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Post ID is required")
                .GreaterThanOrEqualTo(0).WithMessage("Post ID must be positive");

            RuleFor(x => x.Text)
                .NotEmpty().WithMessage("Post text is required")
                .MaximumLength(ValidationConstants.MaxTextContentLength).WithMessage("Post text cannot exceed 1000 characters");
        }
    }
}
