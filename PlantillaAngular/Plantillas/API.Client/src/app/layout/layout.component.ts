/* eslint-disable @typescript-eslint/no-explicit-any */
import { Component } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';
import { filter } from 'rxjs/operators';

@Component({
  selector: 'app-layout',
  templateUrl: './layout.component.html',
  styleUrls: ['./layout.component.scss']
})
export class LayoutComponent {
  
  navItems = [
    { label: 'Proyectos', route: '/projects', icon: '📋' },
    { label: 'Documentación', route: '/docs', icon: '📄' },
    { label: 'Arquitectura', route: '/architecture', icon: '🏗️' },
    { label: 'Sobre mí', route: '/about', icon: '👤' }
  ];

  currentRoute = '/projects';

  constructor(private router: Router) {
    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd)
    ).subscribe((event: any) => {
      this.currentRoute = event.urlAfterRedirects || event.url;
    });
  }

  isActive(route: string): boolean {
    return this.currentRoute.startsWith(route);
  }

  navigate(route: string): void {
    this.router.navigate([route]);
  }
}
