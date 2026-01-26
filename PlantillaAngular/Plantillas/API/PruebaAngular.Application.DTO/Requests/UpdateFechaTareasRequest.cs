using System;

namespace PruebaAngular.Application.DTO.Requests
{

    public class UpdateFechaTareasRequest
    {

        #region Propiedades

        public int IdTarea { get; set; }

        public string Usuario { get; set; }
        public DateTime Fecha { get; set; }
        #endregion
    }

}
