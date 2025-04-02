using FluentValidation;
using Infrastructure;

namespace Application.Requests
{
    public class CreateCommentRequest
    {
        public int PostId { get; set; }
        public int UserId { get; set; }
        public required string Content { get; set; }
    }
    public class CreateCommentRequestValidator : AbstractValidator<CreateCommentRequest>
    {
        public CreateCommentRequestValidator()
        {
            RuleFor(x => x.PostId)
                .NotEmpty().WithMessage("Post ID is required")
                .GreaterThanOrEqualTo(0).WithMessage("Post ID must be positive");

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required")
                .GreaterThanOrEqualTo(0).WithMessage("User ID must be positive");

            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Comment content is required")
                .MaximumLength(ValidationConstants.MaxTextContentLength).WithMessage("Comment cannot exceed 1000 characters");
        }
    }
}
