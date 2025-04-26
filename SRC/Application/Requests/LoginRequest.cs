using FluentValidation;

namespace Application.Requests
{
    public class LoginRequest
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
            RuleFor(r => r.Password)
               .NotEmpty().WithMessage("Пароль обязателен")
               .MinimumLength(8).WithMessage("Пароль должен содержать минимум 8 символов")
               .MaximumLength(32).WithMessage("Пароль не должен превышать 32 символа");
        }
    }
}