using PruebaAngular.Application.Commands;
using FluentValidation;
using System;

namespace PruebaAngular.Application.Validations
{
    public class UpdateTareaCommandValidator : AbstractValidator<UpdateTareaCommand>
    {
        public UpdateTareaCommandValidator()
        {

            RuleFor(command => command.IdTarea).NotEqual(0);
            RuleFor(command => command.FechaVencimiento).NotNull();
            RuleFor(command => command.FechaVencimiento);

        }

        private bool BeValidExpirationDate(DateTime dateTime)
        {
            return dateTime >= DateTime.UtcNow;
        }


    }
}
