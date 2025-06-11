using EmployeeManagementSystem.BusinessLogic.Dtos;
using FluentValidation;

namespace EmployeeManagementSystem.BusinessLogic.DtoValidators
{
    public class TokenRefreshRequestDtoValidator:AbstractValidator<TokenRefreshRequestDto>
    {
        public TokenRefreshRequestDtoValidator()
        {
            RuleFor(x => x.RefreshToken)
                .NotEmpty().WithMessage("Refresh token is required.");
        }
    }
}