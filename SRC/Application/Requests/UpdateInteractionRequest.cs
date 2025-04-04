using Domain.Enums;
using FluentValidation;

namespace Application.Requests
{
    public class UpdateInteractionRequest
    {
        public int Id { get; set; }
        public Status Status { get; set; }
    }

    public class UpdateInteractionRequestValidator : AbstractValidator<UpdateInteractionRequest>
    {
        public UpdateInteractionRequestValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Interaction ID is required")
                .GreaterThanOrEqualTo(0).WithMessage("Interaction ID must be positive");

            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("Invalid interaction status");
        }
    }
}
