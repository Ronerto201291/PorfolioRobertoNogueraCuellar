namespace PruebaAngular.Infrastructure.Data.Queries
{
    /// <summary>
    /// Contiene las queries SQL para la inicialización y configuración de la base de datos.
    /// Centraliza todas las consultas SQL para mejor mantenibilidad.
    /// </summary>
    public static class DatabaseSetupQueries
    {
        /// <summary>
        /// Query para crear las tablas del portfolio (projects y tasks)
        /// </summary>
        public const string CreateTables = @"
            CREATE TABLE IF NOT EXISTS public.projects (
                id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
                name VARCHAR(200) NOT NULL,
                description VARCHAR(1000),
                created_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
            );

            CREATE TABLE IF NOT EXISTS public.tasks (
                id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
                project_id UUID NOT NULL REFERENCES public.projects(id) ON DELETE CASCADE,
                title VARCHAR(200) NOT NULL,
                description VARCHAR(1000),
                status VARCHAR(20) NOT NULL DEFAULT 'Pending',
                priority VARCHAR(20) NOT NULL DEFAULT 'Medium',
                due_date TIMESTAMPTZ,
                created_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
            );

            CREATE INDEX IF NOT EXISTS ix_tasks_project_id ON public.tasks(project_id);
            CREATE INDEX IF NOT EXISTS ix_tasks_status ON public.tasks(status);
        ";

        /// <summary>
        /// Query para verificar si existen datos en la tabla de proyectos
        /// </summary>
        public const string CheckProjectsExist = "SELECT 1 FROM public.projects LIMIT 1";

        /// <summary>
        /// Genera la query para insertar el proyecto semilla
        /// </summary>
        public static string GetInsertProjectQuery(string projectId) => $@"
            INSERT INTO public.projects (id, name, description, created_at)
            VALUES ('{projectId}', 'Aplicación Portfolio Full-Stack', 
                    'Aplicación de portfolio completa que demuestra Clean Architecture, CQRS, GraphQL y desarrollo frontend moderno con Angular.',
                    NOW())
        ";

        /// <summary>
        /// Genera la query para insertar las tareas semilla
        /// </summary>
        public static string GetInsertTasksQuery(string projectId) => $@"
            INSERT INTO public.tasks (id, project_id, title, description, status, priority, created_at)
            VALUES 
                (gen_random_uuid(), '{projectId}', 'Configurar estructura Clean Architecture', 
                 'Implementar las capas Domain, Application, Infrastructure y API siguiendo los principios de Clean Architecture.',
                 'Completed', 'High', NOW()),
                (gen_random_uuid(), '{projectId}', 'Implementar API GraphQL', 
                 'Crear queries y mutations de GraphQL usando HotChocolate para acceso flexible a datos.',
                 'InProgress', 'High', NOW()),
                (gen_random_uuid(), '{projectId}', 'Construir frontend Angular', 
                 'Desarrollar frontend responsive en Angular para consumir la API GraphQL y mostrar el contenido del portfolio.',
                 'Pending', 'Medium', NOW())
        ";

        /// <summary>
        /// Genera las queries completas para sembrar datos iniciales
        /// </summary>
        public static string GetSeedDataQuery(string projectId) => 
            GetInsertProjectQuery(projectId) + ";" + GetInsertTasksQuery(projectId);
    }
}
