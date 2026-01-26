namespace PruebaAngular.Infrastructure.Data
{
    public class IDbContextSchema
    {
      public string Schema { get; }
        public IDbContextSchema(string schema)
        {
            Schema = schema;
        }
    }
}
