using FluentValidation;

namespace Application.Requests
{
    public class RegistrationRequest
    {
        public string Name { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public DateTime DateOfBirth { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
    public class RegistrationRequestValidator : AbstractValidator<RegistrationRequest>
    {
        public RegistrationRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.LastName).NotEmpty();
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).NotEmpty().MinimumLength(8);

            RuleFor(x => x.DateOfBirth)
                .NotEmpty().WithMessage("DateOfBirth is required")
                .LessThan(DateTime.Today.AddYears(-14)).WithMessage("User must be at least 14 years old");
        }
    }
}