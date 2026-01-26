using MediatR;
using System;

namespace PruebaAngular.Domain.Events
{
    public class UpdateTareaChangedDomainEvent : INotification
    {
        public DateTime? NuevaFechaVencimiento { get; set; }

        public DateTime? FechaAnteriorVencimiento { get; set; }
        public int IdTarea { get; set; }

        public bool ValidarEstado()
        {
            return NuevaFechaVencimiento != FechaAnteriorVencimiento
                && IdTarea != 0;
        }
    }
}
