namespace PruebaAngular.Api
{
    public class PruebaAngularSettings
    {
        //public const bool EntornoDesarrollo = true;
        public bool UseCustomizationData { get; set; }
        public string ConnectionString { get; set; }
        public string QueryConnectionString { get; set; }

        public string EventBusConnection { get; set; }

        public int CheckUpdateTime { get; set; }

        public string TipoAuthenticacion { get; set; }

        public bool GlobalAuthorize { get; set; }

        public bool UseLoadTest {  get; set; }

        public bool UseGraphQl { get; set; }

        public bool UseHealthCheks { get; set; }

        public bool UseCors { get; set; }

        public string[] CorsOrigin { get; set; }

        public string ApiUrl1 { get; set; }
    }
}
