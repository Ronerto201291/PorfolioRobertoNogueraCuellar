using PruebaAngular.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PruebaAngular.Domain.DomainEventHandlers.UpdateTareasStateChanged
{
    public class UpddateTareaFechaChangedDomainEventHandler : INotificationHandler<UpdateTareaChangedDomainEvent>
    {
        private readonly ILoggerFactory _logger;

        public UpddateTareaFechaChangedDomainEventHandler(ILoggerFactory logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(UpdateTareaChangedDomainEvent updateTareaStateChangedDomainEvent, CancellationToken cancellationToken)
        {
            bool validado = updateTareaStateChangedDomainEvent.ValidarEstado();
            _logger.CreateLogger<UpdateTareaChangedDomainEvent>()
                .LogTrace("Tarea Id: {IdTarea} asignado correctamente la nueva fecha {NuevaFechaVencimiento}",
                    updateTareaStateChangedDomainEvent.IdTarea,
                    updateTareaStateChangedDomainEvent.NuevaFechaVencimiento);

        }
    }
}
