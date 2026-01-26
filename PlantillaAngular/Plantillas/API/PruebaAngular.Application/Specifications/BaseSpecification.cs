using PruebaAngular.Infrastructure.Data.Specifications;


//Ejemplo de hacer una query con BaseSpecification

//public class PersonaFirmantePorIdSpecification : BaseSpecification<PerDato>
//{
//    public PersonaFirmantePorIdSpecification(int idDato)
//        : base(p => p.IdDato == idDato)
//    {
//        // Incluimos las relaciones (Joins) de forma fuertemente tipada
//        AddInclude(p => p.Empleado);
//        AddInclude("Empleado.Cargo"); // Para el LEFT JOIN con Cargo
//    }
//}