using Domain.Enums;
using FluentValidation;

namespace Application.Requests
{
    public class UpdateReactionRequest
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int PostId { get; set; }
        public ReactionType Type { get; set; }
    }
    public class UpdateReactionRequestValidator : AbstractValidator<UpdateReactionRequest>
    {
        public UpdateReactionRequestValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Reaction ID is required")
                .GreaterThanOrEqualTo(0).WithMessage("Reaction ID must be positive");

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required")
                .GreaterThanOrEqualTo(0).WithMessage("User ID must be positive");

            RuleFor(x => x.PostId)
                .NotEmpty().WithMessage("Post ID is required")
                .GreaterThanOrEqualTo(0).WithMessage("Post ID must be positive");

            RuleFor(x => x.Type)
                .IsInEnum().WithMessage("Invalid reaction type");
        }
    }
}
