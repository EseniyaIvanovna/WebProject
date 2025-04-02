﻿using FluentValidation;
using Infrastructure;

namespace Application.Requests
{
    public class UpdateUserRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? Info { get; set; }
        public string Email { get; set; }
    }

    public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
    {
        public UpdateUserRequestValidator() 
        {
            RuleFor(x => x.Id)
                .NotNull().WithMessage("Id is required")
                .GreaterThanOrEqualTo(0).WithMessage("Id must be positive");
            
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(ValidationConstants.MaxNameLength).WithMessage("{PropertyName} has length more then 20 ");
            
            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("LastName is required")
                .MaximumLength(ValidationConstants.MaxLastNameLength).WithMessage("{PropertyName} has length more then 20 ");
            
            RuleFor(x => x.Info)
                .MaximumLength(ValidationConstants.MaxUserInfoLength).WithMessage("{PropertyName} has length more then 255");

            RuleFor(x => x.DateOfBirth)
                .NotEmpty().WithMessage("DateOfBirth is required")
                .LessThan(DateTime.Today.AddYears(-14)).WithMessage("User must be at least 14 years old"); 
                             
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .MaximumLength(ValidationConstants.MaxEmailLength).WithMessage("Email has length more then 50")
                .EmailAddress().WithMessage("It does not look like email");
        }
    }
}
