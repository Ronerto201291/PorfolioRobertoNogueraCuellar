import { Component } from '@angular/core';

@Component({
  selector: 'app-about',
  templateUrl: './about.component.html',
  styleUrls: ['./about.component.scss']
})
export class AboutComponent {

  profile = {
    name: 'Roberto Noguera Cuellar',
    title: '.NET Full Stack Developer | Team Leader',
    description: `Apasionado de la tecnología y la programación. Trabajo principalmente en la creación de soluciones 
eficientes, destacables y sostenibles. Combino habilidades técnicas con la gestión de equipos para garantizar resultados.`,
    location: 'Madrid, España',
    experience: '8+ años',
    email: 'blanco.cuellar.r@gmail.com',
    phone: '695 590 754',
    linkedin: 'https://www.linkedin.com/in/rnogueracuellar'
  };

  skills = [
    { 
      category: 'Backend', 
      items: ['C#', 'VB.NET', '.NET Framework', 'ASP.NET MVC', '.NET 5/6/8', 'Swagger', 'GraphQL'] 
    },
    { 
      category: 'Azure & Cloud', 
      items: ['Azure Functions', 'Service Bus', 'Azure DevOps'] 
    },
    { 
      category: 'ORM & Herramientas', 
      items: ['Entity Framework', 'Dapper', 'AutoMapper', 'MediatR'] 
    },
    { 
      category: 'Frontend', 
      items: ['HTML5', 'CSS', 'JavaScript', 'jQuery', 'Bootstrap', 'Razor Pages', 'Blazor', 'Angular'] 
    },
    { 
      category: 'Bases de Datos', 
      items: ['SQL Server', 'Oracle', 'PostgreSQL'] 
    },
    { 
      category: 'DevOps & Control', 
      items: ['GIT', 'TFS', 'Azure DevOps', 'Jira', 'Fork'] 
    },
    { 
      category: 'Arquitectura', 
      items: ['DDD', 'CQRS', 'Clean Architecture', 'Microservicios'] 
    },
    { 
      category: 'Otras Herramientas', 
      items: ['Visual Studio', 'Elasticsearch', 'Docker'] 
    }
  ];

  experience = [
    {
      role: 'Senior .NET Full Stack Developer | Team Leader',
      company: 'Mutualidad de la Abogacía',
      period: 'Septiembre 2020 - Actualidad',
      current: true
    },
    {
      role: 'Senior .NET Full Stack Developer',
      company: 'Análisis e Investigación',
      period: 'Febrero 2018 - Abril 2020',
      current: false
    },
    {
      role: 'Junior .NET Full Stack Developer',
      company: 'ABIZ',
      period: 'Diciembre 2017 - Enero 2018',
      current: false
    },
    {
      role: 'Junior .NET Full Stack Developer',
      company: 'AUXADI',
      period: 'Agosto 2017 - Diciembre 2017',
      current: false
    }
  ];

  highlights = [
    'Implantación de nuevos cores tecnológicos',
    'Refactorización de sistemas legacy',
    'Gestión completa del ciclo de vida del software',
    'Liderazgo de equipos de desarrollo'
  ];

  cvPath = 'assets/cv.pdf';

  openCV(): void {
    window.open(this.cvPath, '_blank');
  }

  openLinkedIn(): void {
    window.open(this.profile.linkedin, '_blank');
  }
}
