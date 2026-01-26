using MediatR;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace PruebaAngular.Application.Commands
{
    public class CrearTareaCommand : IRequest<int>
    {



        [DataMember]
        [Required]
        public int IdUsuarioTarea { get; set; }
        [DataMember]
        [Required]
        public string NombreTarea { get; set; }
        [DataMember]
        [Required]
        public string DescripcionTarea { get; set; }

        [DataMember]
        [Required]
        public DateTime? FechaVencimiento { get; set; }
    }
}
