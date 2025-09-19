using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Auth.Users.Register;
using FluentValidation;

namespace Application.Game.Characters.Create;

internal sealed class CreateCharacterCommandValidator : AbstractValidator<CreateCharacterCommand>
{
    public CreateCharacterCommandValidator()
    {
        RuleFor(c => c.Name).NotEmpty();
        RuleFor(c => c.Name).MaximumLength(32);
    }
}
