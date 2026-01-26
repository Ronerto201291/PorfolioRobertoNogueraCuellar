using System;

namespace PruebaAngular.Application.DTO.Response
{
    public class TareasComentariosPorUsuarioResponse
    {
        public int? IdTarea { get; set; }

        public string NombreTarea { get; set; }

        public string Comentario { get; set; }
        public DateTime FechaComentario { get; set; }
        

    }
}
