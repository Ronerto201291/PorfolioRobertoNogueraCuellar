using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace PruebaAngular.Application.DTO.Requests
{
    public class NuevoComentarioTareaRequest
    {

        #region Propiedades

        public int IdTarea { get; set; }

        public string Comentario { get; set; }
        #endregion
    }

}
