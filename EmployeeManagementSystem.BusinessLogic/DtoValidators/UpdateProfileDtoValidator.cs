using FluentValidation;
using EmployeeManagementSystem.BusinessLogic.Dtos;

namespace EmployeeManagementSystem.BusinessLogic.DtoValidators
{
    public class UpdateProfileDtoValidator : AbstractValidator<UpdateProfileDto>
    {
        public UpdateProfileDtoValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First Name field must not be null!")
                .MaximumLength(30).WithMessage("First Name field must not be longer than 30 characters!");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last Name field must not be null!")
                .MaximumLength(30).WithMessage("Last Name field must not be longer than 30 characters!");

            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("Username field must not be null!")
                .MaximumLength(50).WithMessage("Username field must not be longer than 50 characters!");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email field must not be null!")
                .EmailAddress().WithMessage("Please enter valid email address!")
                .MaximumLength(50).WithMessage("Email field must not be longer than 50 characters!");

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("Address field must not be null!")
                .MaximumLength(1000).WithMessage("Address field must not be longer than 1000 characters!");

            RuleFor(x => x.Zipcode)
                .NotEmpty().WithMessage("Zipcode field must not be null!")
                .Matches(@"^\d{6}$").WithMessage("Zipcode must be exactly 6 digits with no characters.");

            RuleFor(x => x.MobileNo)
                .NotEmpty().WithMessage("Phone number is required.")
                .Matches(@"^\d{10}$").WithMessage("Phone number must be exactly 10 digits with no characters.");

            RuleFor(x => x.CountryId)
                .NotEmpty().WithMessage("Country field must not be null!")
                .GreaterThan(0).WithMessage("Country field must be greater than 0!");

            RuleFor(x => x.StateId)
                .NotEmpty().WithMessage("State field must not be null!")
                .GreaterThan(0).WithMessage("State field must be greater than 0!");

            RuleFor(x => x.CityId)
                .NotEmpty().WithMessage("City field must not be null!")
                .GreaterThan(0).WithMessage("City field must be greater than 0!");

            RuleFor(x => x.RoleId)
                .NotEmpty().WithMessage("Role field must not be null!")
                .GreaterThan(0).WithMessage("Role field must be greater than 0!");

            RuleFor(x => x.Age)
                .NotNull().WithMessage("Employee age can not be null!")
                .GreaterThan(0).WithMessage("Employee age must be greater than 18!");

            RuleFor(x => x.Salary)
                .NotNull().WithMessage("Employee salary can not be null!")
                .GreaterThan(0).WithMessage("Employee salary must be greater than 0!")
                .LessThanOrEqualTo(10000000).WithMessage("Employee salary must be less than or equal to 10,000,000!");
        }
    }
}
