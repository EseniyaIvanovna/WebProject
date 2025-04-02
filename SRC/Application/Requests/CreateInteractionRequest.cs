using Domain.Enums;
using FluentValidation;

namespace Application.Requests
{
    public class CreateInteractionRequest
    {
        public int User1Id { get; set; }
        public int User2Id { get; set; }
        public Status Status { get; set; }
    }
    public class CreateInteractionRequestValidator : AbstractValidator<CreateInteractionRequest>
    {
        public CreateInteractionRequestValidator()
        {
            RuleFor(x => x.User1Id)
                .NotEmpty().WithMessage("User1 ID is required")
                .GreaterThanOrEqualTo(0).WithMessage("User1 ID must be positive")
                .NotEqual(x => x.User2Id).WithMessage("Users must be different");

            RuleFor(x => x.User2Id)
                .NotEmpty().WithMessage("User2 ID is required")
                .GreaterThanOrEqualTo(0).WithMessage("User2 ID must be positive");

            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("Invalid interaction status");
        }
    }
}
