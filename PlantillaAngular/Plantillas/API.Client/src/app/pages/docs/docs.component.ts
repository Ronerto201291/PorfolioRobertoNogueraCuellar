import { Component } from '@angular/core';

@Component({
  selector: 'app-docs',
  templateUrl: './docs.component.html',
  styleUrls: ['./docs.component.scss']
})
export class DocsComponent {

  sections = [
    {
      title: '¿Qué es este proyecto?',
      icon: '💡',
      content: `Esta aplicación es un portfolio full-stack diseñado como una prueba técnica realista.
Refleja cómo se estructura una aplicación orientada a producción, no un simple ejemplo o tutorial.`
    },
    {
      title: '¿Cómo se ejecuta la aplicación?',
      icon: '▶️',
      code: 'dotnet run --project src/AppHost',
      items: []
    },
    {
      title: 'Requisitos de ejecución',
      icon: '⚙️',
      items: [
        '.NET SDK instalado',
        'Docker Desktop en ejecución',
        'PostgreSQL se levanta automáticamente mediante .NET Aspire'
      ]
    },
    {
      title: 'Decisiones técnicas',
      icon: '🎯',
      items: [
        'Clean Architecture para separar responsabilidades',
        'CQRS para aislar operaciones de lectura y escritura',
        'GraphQL para mayor flexibilidad en el frontend',
        'PostgreSQL ejecutándose en contenedor Docker'
      ]
    },
    {
      title: 'Qué demuestra este proyecto',
      icon: '✅',
      items: [
        'Diseño orientado a mantenibilidad',
        'Separación clara de capas',
        'Enfoque realista de producción'
      ]
    }
  ];

  graphqlExamples = {
    title: 'Ejemplos de GraphQL',
    icon: '🔮',
    queries: [
      {
        name: 'Obtener todos los proyectos con sus tareas',
        description: 'Esta query devuelve todos los proyectos junto con sus tareas asociadas.',
        code: `query {
  projects {
    projectId
    name
    description
    createdAt
    tasks {
      taskId
      title
      status
      priority
      dueDate
    }
  }
}`
      },
      {
        name: 'Obtener un proyecto por ID',
        description: 'Busca un proyecto específico usando su identificador único.',
        code: `query GetProject($id: UUID!) {
  projectById(id: $id) {
    projectId
    name
    description
    tasks {
      taskId
      title
      status
    }
  }
}

# Variables:
# { "id": "550e8400-e29b-41d4-a716-446655440000" }`
      },
      {
        name: 'Crear un nuevo proyecto (Mutation)',
        description: 'Crea un proyecto nuevo en la base de datos.',
        code: `mutation CreateProject($input: CreateProjectInput!) {
  createProject(input: $input) {
    projectId
    name
    description
    createdAt
  }
}

# Variables:
# {
#   "input": {
#     "name": "Mi nuevo proyecto",
#     "description": "Descripción del proyecto"
#   }
# }`
      },
      {
        name: 'Actualizar estado de una tarea (Mutation)',
        description: 'Cambia el estado de una tarea (Pending, InProgress, Completed).',
        code: `mutation UpdateTaskStatus($taskId: UUID!, $status: String!) {
  updateTaskStatus(taskId: $taskId, status: $status) {
    taskId
    title
    status
  }
}

# Variables:
# {
#   "taskId": "550e8400-e29b-41d4-a716-446655440001",
#   "status": "Completed"
# }`
      }
    ]
  };

  techStack = [
    { name: '.NET 10', category: 'Backend' },
    { name: 'Angular 17', category: 'Frontend' },
    { name: 'GraphQL', category: 'API' },
    { name: 'PostgreSQL', category: 'Base de datos' },
    { name: 'Docker', category: 'Infraestructura' },
    { name: 'Aspire', category: 'Orquestación' }
  ];
}
