using MediatR;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace PruebaAngular.Application.Commands
{
    public class DeleteTareaCommand : IRequest<int>
    {
        [DataMember]
        [Required]
        public int IdTarea { get; set; }

    }
}
