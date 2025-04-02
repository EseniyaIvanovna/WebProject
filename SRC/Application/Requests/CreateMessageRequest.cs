using FluentValidation;
using Infrastructure;

namespace Application.Requests
{
    public class CreateMessageRequest
    {
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public required string Text { get; set; }
    }
    public class CreateMessageRequestValidator : AbstractValidator<CreateMessageRequest>
    {
        public CreateMessageRequestValidator()
        {
            RuleFor(x => x.SenderId)
                .NotEmpty().WithMessage("Sender ID is required")
                .GreaterThanOrEqualTo(0).WithMessage("Sender ID must be positive");

            RuleFor(x => x.ReceiverId)
                .NotEmpty().WithMessage("Receiver ID is required")
                .GreaterThanOrEqualTo(0).WithMessage("Receiver ID must be positive")
                .NotEqual(x => x.SenderId).WithMessage("Cannot send message to yourself");

            RuleFor(x => x.Text)
                .NotEmpty().WithMessage("Message text is required")
                .MaximumLength(ValidationConstants.MaxTextContentLength).WithMessage("Message text cannot exceed 1000 characters");
        }
    }
}
