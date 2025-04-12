using FluentValidation;
using Infrastructure;

namespace Application.Requests
{
    public class CreateUserRequest
    {
        public required string Name { get; set; }
        public required string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? Info { get; set; }
        public string? Email { get; set; }
    }

    public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
    {
        public CreateUserRequestValidator() 
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(ValidationConstants.MaxNameLength).WithMessage("{PropertyName} has length more then 20 ");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("LastName is required")
                .MaximumLength(ValidationConstants.MaxLastNameLength).WithMessage("{PropertyName} has length more then 20 ");
            
            RuleFor(x => x.Info)
                .NotEmpty().WithMessage("Info is required")
                .MaximumLength(ValidationConstants.MaxUserInfoLength).WithMessage("{PropertyName} has length more then 255");

            RuleFor(x => x.DateOfBirth)
                .NotEmpty().WithMessage("DateOfBirth is required")
                .LessThan(DateTime.Today.AddYears(-14)).WithMessage("User must be at least 14 years old"); 
           
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .MaximumLength(ValidationConstants.MaxEmailLength).WithMessage("Email has length more then 50" )
                .EmailAddress().WithMessage("It does not look like email");
        }
    }
}
