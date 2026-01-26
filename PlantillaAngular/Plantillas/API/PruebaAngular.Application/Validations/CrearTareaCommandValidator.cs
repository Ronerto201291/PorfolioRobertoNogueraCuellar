using PruebaAngular.Application.Commands;
using FluentValidation;
using System;

namespace PruebaAngular.Application.Validations
{
    public class CrearTareaCommandValidator : AbstractValidator<CrearTareaCommand>
    {
        public CrearTareaCommandValidator()
        {

            RuleFor(command => command.IdUsuarioTarea).NotEqual(0);
            RuleFor(command => command.NombreTarea).NotNull();
            RuleFor(command => command.NombreTarea).NotEmpty();
        }

        private bool BeValidExpirationDate(DateTime dateTime)
        {
            return dateTime >= DateTime.UtcNow;
        }


    }
}
