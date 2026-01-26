using MediatR;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace PruebaAngular.Application.Commands
{
    public class UpdateTareaCommand : IRequest<int>
    {
        [DataMember]
        [Required]
        public int IdTarea { get; set; }
        [DataMember]
        [Required]
        public DateTime? FechaVencimiento { get; set; }

        [DataMember]
        [Required]
        public int IdUsuario { get; set; }
    }
}
