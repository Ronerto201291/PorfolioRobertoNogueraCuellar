/* eslint-disable @typescript-eslint/no-explicit-any */
import { Component, OnInit, OnDestroy, signal } from '@angular/core';
import { RabbitMqActivityComponent } from '../aplicacion/components/rabbitmq-activity/rabbitmq-activity.component';
import { Router, NavigationEnd, Event } from '@angular/router';
import { HttpClient } from '@angular/common/http';
// RXJS: Creadores primero, luego operadores. No los mezcles en la misma línea.
import { interval, Subscription, of, Observable } from 'rxjs';
import { filter, switchMap, catchError } from 'rxjs/operators';

@Component({
  selector: 'app-layout',
  templateUrl: './layout.component.html',
  styleUrls: ['./layout.component.scss']
})
export class LayoutComponent implements OnInit, OnDestroy {
  navItems = [
    { label: 'Proyectos', route: '/projects', icon: '📋' },
    { label: 'Documentación', route: '/docs', icon: '📄' },
    { label: 'Arquitectura', route: '/architecture', icon: '🏗️' },
    { label: 'Sobre mí', route: '/about', icon: '👤' }
  ];

  currentRoute = '/projects';
  rabbitmqStatus = signal<'connected' | 'disconnected'>('disconnected');
  // expose for template
  rabbitmqStatusValue = this.rabbitmqStatus;
  private statusSubscription?: Subscription;

  constructor(private router: Router, private http: HttpClient) {
    this.router.events.pipe(
      filter((event: Event): event is NavigationEnd => event instanceof NavigationEnd)
    ).subscribe((event: NavigationEnd) => {
      this.currentRoute = event.urlAfterRedirects || event.url;
    });
  }

  // Provide the standalone component type for dynamic insertion in template
  rabbitComponent = RabbitMqActivityComponent as any;

  ngOnInit() {
    this.startStatusMonitoring();
  }

  ngOnDestroy() {
    this.statusSubscription?.unsubscribe();
  }

  private startStatusMonitoring(): void {
    this.statusSubscription = interval(10000).pipe(
      switchMap(() => this.checkRabbitMqStatus())
    ).subscribe(status => {
      this.rabbitmqStatus.set(status);
    });

    this.checkRabbitMqStatus().subscribe(status => {
      this.rabbitmqStatus.set(status);
    });
  }

  private checkRabbitMqStatus(): Observable<'connected' | 'disconnected'> {
    return this.http.get<any>('/hc').pipe(
      switchMap((health) => {
        const rabbitMqHealth = health.entries?.['rabbitmq-connection'];
        const status = rabbitMqHealth?.status === 'Healthy' ? 'connected' : 'disconnected';
        return of(status as 'connected' | 'disconnected');
      }),
      catchError(() => of('disconnected' as 'connected' | 'disconnected'))
    );
  }

  isActive(route: string): boolean {
    return this.currentRoute.startsWith(route);
  }

  navigate(route: string): void {
    this.router.navigate([route]);
  }
}
