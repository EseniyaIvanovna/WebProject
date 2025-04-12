using FluentValidation;
using Infrastructure;

namespace Application.Requests
{
    public class UpdateCommentRequest
    {
        public int Id { get; set; }
        public string? Content { get; set; }
    }

    public class UpdateCommentRequestValidator : AbstractValidator<UpdateCommentRequest>
    {
        public UpdateCommentRequestValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Comment ID is required")
                .GreaterThan(0).WithMessage("Comment ID must be positive");

            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Comment content is required")
                .MaximumLength(ValidationConstants.MaxTextContentLength).WithMessage("Comment cannot exceed 1000 characters");
        }
    }
}
