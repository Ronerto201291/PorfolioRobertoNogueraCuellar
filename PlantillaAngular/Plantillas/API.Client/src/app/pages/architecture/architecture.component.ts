import { Component } from '@angular/core';

@Component({
  selector: 'app-architecture',
  templateUrl: './architecture.component.html',
  styleUrls: ['./architecture.component.scss']
})
export class ArchitectureComponent {

  layers = [
    { 
      name: 'Frontend Angular', 
      icon: '🅰️', 
      color: '#dd0031',
      description: 'Componentes, Servicios, Cliente GraphQL'
    },
    { 
      name: 'API .NET', 
      icon: '⚡', 
      color: '#512bd4',
      description: 'Servidor GraphQL, Controladores REST'
    },
    { 
      name: 'Capa de Aplicación', 
      icon: '📦', 
      color: '#667eea',
      description: 'Commands, Queries, Handlers (CQRS)'
    },
    { 
      name: 'Capa de Dominio', 
      icon: '💎', 
      color: '#764ba2',
      description: 'Entidades, Agregados, Eventos de Dominio'
    },
    { 
      name: 'Infraestructura', 
      icon: '🔧', 
      color: '#f5a623',
      description: 'EF Core, Repositorios, Servicios Externos'
    },
    { 
      name: 'PostgreSQL', 
      icon: '🐘', 
      color: '#336791',
      description: 'Base de datos vía Docker + Aspire'
    }
  ];

  requestFlow = [
    'UI',
    'GraphQL',
    'Aplicación',
    'Infraestructura',
    'PostgreSQL'
  ];

  benefits = [
    {
      title: 'Separación clara de responsabilidades',
      description: 'Cada capa tiene una función específica'
    },
    {
      title: 'Fácil de testear y evolucionar',
      description: 'El bajo acoplamiento permite pruebas independientes'
    },
    {
      title: 'Cercano a entornos de producción reales',
      description: 'Patrones usados en aplicaciones empresariales'
    }
  ];
}
