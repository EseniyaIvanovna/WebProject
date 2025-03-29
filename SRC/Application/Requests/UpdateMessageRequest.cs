using FluentValidation;

namespace Application.Requests
{
    public class UpdateMessageRequest
    {
        public int Id { get; set; }
        public string Text { get; set; }
    }
    public class UpdateMessageRequestValidator : AbstractValidator<UpdateMessageRequest>
    {
        public UpdateMessageRequestValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Message ID is required")
                .GreaterThanOrEqualTo(0).WithMessage("Message ID must be positive");

            RuleFor(x => x.Text)
                .NotEmpty().WithMessage("Message text is required")
                .MaximumLength(1000).WithMessage("Message text cannot exceed 1000 characters");
        }
    }
}
