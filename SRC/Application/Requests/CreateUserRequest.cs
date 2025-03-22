using FluentValidation;

namespace Application.Requests
{
    public class CreateUserRequest
    {
        public string? Name { get; set; }
        public string? LastName { get; set; }
        public int Age { get; set; }
        public string Info { get; set; }
        public string Email { get; set; }
    }

    public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
    {
        public CreateUserRequestValidator() //update для каждого метода своё название, где дто в контроллерах
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(20).WithMessage("{PropertyName} has length more then 20 ");
            RuleFor(x => x.LastName).NotEmpty().MaximumLength(20).WithMessage("{PropertyName} has length more then 20 ");
            RuleFor(x => x.Info).NotEmpty().MaximumLength(200).WithMessage("{PropertyName} has length more then 200 ");
            RuleFor(x => x.Age).NotEmpty().GreaterThan(0).WithMessage("Age must be positive");

        }
    }
}
