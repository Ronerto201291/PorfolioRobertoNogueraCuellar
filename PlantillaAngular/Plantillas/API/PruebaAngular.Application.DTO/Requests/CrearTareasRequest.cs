using System;

namespace PruebaAngular.Application.DTO.Requests
{

    public class CrearTareasRequest
    {

        #region Propiedades

        public int IdUsuarioTarea { get; set; }
        public string NombreTarea { get; set; }
        public string DescripcionTarea { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        #endregion
    }


}
