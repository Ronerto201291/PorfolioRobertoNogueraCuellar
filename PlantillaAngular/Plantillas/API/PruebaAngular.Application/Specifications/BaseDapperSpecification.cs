using Microsoft.Data.SqlClient;
using PruebaAngular.Domain.AggregateModels.Portfolio;
using PruebaAngular.Domain.Core;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

//Ejemplo de como hariamos una query Dapper con especificacion


//public class PersonaFirmantePorIdSpecification
//{

//    private const string SqlExpediente = @"
//				SELECT a.Id_Dato as IdDato
//					,a.Nombre
//					,a.Apellido1
//					,a.Apellido2
//					,b.Ruta_imagen_Firma as RutaImagenFirma
//					,c.Cargo AS Firma
//				FROM dbo.PER_Dato a
//				INNER JOIN dbo.PER_Empleado b ON (a.Id_Dato = b.Id_dato)
//				LEFT JOIN dbo.per_ms_Cargo c ON b.id_Cargo = c.id_cargo
//				WHERE (a.Id_Dato = @IdDato)";

//    private StringBuilder _sql = null;

//    public PersonaFirmantePorIdSpecification()
//    {
//        _sql = new StringBuilder();
//        _sql.AppendLine(SqlExpediente);
//    }

//    public override string ToString()
//    {
//        return _sql.ToString();
//    }
//}