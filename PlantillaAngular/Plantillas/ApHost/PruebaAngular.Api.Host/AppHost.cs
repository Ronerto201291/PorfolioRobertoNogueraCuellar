using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

// ==========================================
// DATABASE (PostgreSQL via Docker)
// ==========================================
var postgres = builder.AddPostgres("postgres")
    .WithImageTag("16-alpine")
    .WithDataVolume("portfolio-postgres-data");

var portfolioDb = postgres.AddDatabase("portfolio", "portfolio");

// ==========================================
// BACKEND API
// ==========================================
var api = builder.AddProject<Projects.PruebaAngular_Api>("PruebaAngularApi")
    .WithReference(portfolioDb)
    .WaitFor(portfolioDb);

// URLs de acceso directo en el Dashboard de Aspire
api.WithUrl($"{api.GetEndpoint("http")}/swagger", "Swagger UI");
api.WithUrl($"{api.GetEndpoint("http")}/graphql/portfolio", "GraphQL Playground");

// ==========================================
// FRONTEND (Angular)
// ==========================================
var frontend = builder.AddNpmApp("frontend", "../../API.Client", "start")
    .WithReference(api)
    .WithHttpEndpoint(env: "PORT")
    .WithExternalHttpEndpoints()
    .WithEnvironment("BROWSER", "none")
    .WithEnvironment("NG_CLI_ANALYTICS", "false");

// URL de acceso directo al frontend
frontend.WithUrl("http://localhost:4200", "Frontend");

builder.Build().Run();