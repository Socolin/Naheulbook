using FluentValidation;
using Naheulbook.Requests.Requests;

namespace Naheulbook.Requests.Validators
{
    public class CreateCharacterRequestValidator: AbstractValidator<CreateCharacterRequest>
    {
        public CreateCharacterRequestValidator()
        {
        }
    }
}