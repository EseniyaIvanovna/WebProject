using FluentValidation;
using Infrastructure;

namespace Application.Requests
{
    public class CreatePostRequest
    {
        public int UserId { get; set; }
        public required string Text { get; set; }
    }
    public class CreatePostRequestValidator : AbstractValidator<CreatePostRequest>
    {
        public CreatePostRequestValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required")
                .GreaterThanOrEqualTo(0).WithMessage("User ID must be positive");

            RuleFor(x => x.Text)
                .NotEmpty().WithMessage("Post text is required")
                .MaximumLength(ValidationConstants.MaxTextContentLength).WithMessage("Post text cannot exceed 1000 characters");
        }
    }
}
