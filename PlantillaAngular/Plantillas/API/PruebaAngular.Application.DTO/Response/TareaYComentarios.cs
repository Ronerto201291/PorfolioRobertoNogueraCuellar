using System;

namespace PruebaAngular.Application.DTO.Response
{
    public class TareaYComentarios
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Comentario { get; set; }

        public DateTime? FechaComentario { get; set; }
    }

}
