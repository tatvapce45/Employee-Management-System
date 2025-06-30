using EmployeeManagementSystem.BusinessLogic.Dtos;
using FluentValidation;

namespace EmployeeManagementSystem.BusinessLogic.DtoValidators
{
    public class MarkAttendanceDtoValidator:AbstractValidator<MarkAttendanceDto>
    {
        public MarkAttendanceDtoValidator()
        {
            RuleFor(d => d.EmployeeId)
                .NotEmpty().WithMessage("Employee Id is required.");

            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("Status field must not be null!");
        }
    }

    public class UpdateAttendanceDtoValidator:AbstractValidator<UpdateAttendanceDto>
    {
        public UpdateAttendanceDtoValidator()
        {
            RuleFor(d => d.Id)
                .NotEmpty().WithMessage("Id is required.");

            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("Status field must not be null!");
        }
    }
}