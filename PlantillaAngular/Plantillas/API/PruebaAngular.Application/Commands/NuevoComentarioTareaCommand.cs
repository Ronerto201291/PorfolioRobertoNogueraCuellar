using MediatR;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace PruebaAngular.Application.Commands
{
    public class NuevoComentarioTareaCommand : IRequest<int>
    {
        [DataMember]
        [Required]
        public int IdTarea { get; set; }

        [DataMember]
        [Required]
        public string ComentarioTarea { get; set; }


        [DataMember]
        [Required]
        public int IdUsuarioComentario { get; set; }
    }
}
