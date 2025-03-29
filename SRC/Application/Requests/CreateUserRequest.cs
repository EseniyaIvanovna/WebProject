using FluentValidation;

namespace Application.Requests
{
    public class CreateUserRequest
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public string Info { get; set; }
        public string Email { get; set; }
    }

    public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
    {
        public CreateUserRequestValidator() 
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is requied")
                .MaximumLength(20).WithMessage("{PropertyName} has length more then 20 ");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("LastName is requied")
                .MaximumLength(20).WithMessage("{PropertyName} has length more then 20 ");
            
            RuleFor(x => x.Info)
                .NotEmpty().WithMessage("Info is requied")
                .MaximumLength(255).WithMessage("{PropertyName} has length more then 255");
            
            RuleFor(x => x.Age)
                .NotEmpty().WithMessage("Age is requied")
                .GreaterThanOrEqualTo(0).WithMessage("Age must be positive");
           
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is requied")
                .MaximumLength(50).WithMessage("Email has length more then 50" )
                .EmailAddress().WithMessage("It does not look like email");
        }
    }
}
