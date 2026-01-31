import { Component, OnInit, OnDestroy, signal, inject } from '@angular/core';
import { addIcons } from 'ionicons';
import { addOutline, refreshOutline, createOutline, trashOutline } from 'ionicons/icons';
import { HttpClient } from '@angular/common/http';
import { interval, Subscription, of } from 'rxjs';
import { switchMap, catchError, finalize } from 'rxjs/operators';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { IonicModule } from '@ionic/angular';

// Interfaces estricta para evitar el error de 'any'
interface HealthResponse {
  status: string;
  entries?: {
    [key: string]: { status: string };
  };
}

export interface EventActivity {
  eventId: string;
  eventType: string;
  timestamp: Date;
  status: string;
}

export interface ActivitySummary {
  totalEvents: number;
  publishedCount: number;
  consumedCount: number;
  failedCount: number;
  eventTypeBreakdown: { [key: string]: number };
  recentActivities?: EventActivity[];
}

@Component({
  selector: 'app-rabbitmq-activity',
  standalone: true,
  imports: [CommonModule, FormsModule, MatFormFieldModule, MatInputModule, MatButtonModule, MatIconModule, MatSnackBarModule, IonicModule],
  // Inline template to avoid missing external template file
  template: `
    <div class="rabbitmq-activity" *ngIf="isVisible()">
      <div class="activity-header">
        <div class="activity-title">
          <span class="rabbit-icon">🐰</span>
          <span>RabbitMQ Activity</span>
          <span class="status-indicator" [class]="getStatusClass()"></span>
        </div>
        <button (click)="isExpanded.set(!isExpanded())">{{ isExpanded() ? '▲' : '▼' }}</button>
      </div>
      <div *ngIf="isExpanded()" class="activity-content">
        <div *ngIf="summary()">
          <div>Total: {{ summary()?.totalEvents }}</div>
          <div>Published: {{ summary()?.publishedCount }}</div>
          <div>Consumed: {{ summary()?.consumedCount }}</div>
          <div *ngIf="summary()?.failedCount">Failed: {{ summary()?.failedCount }}</div>
        </div>
      <div class="subscribe-email" style="margin:12px 0; display:flex; gap:8px; align-items:center;">
        <input placeholder="Email para notificaciones" [(ngModel)]="emailToSubscribe" style="flex:1" />
        <ion-button fill="solid" color="primary" size="small" (click)="subscribeEmail()">Añadir</ion-button>
      </div>
        <div *ngIf="recentActivities().length === 0">No recent activity</div>
        <ul>
          <li *ngFor="let a of recentActivities()">{{ formatTime(a.timestamp) }} - {{ a.eventType }} - {{ a.status }}</li>
        </ul>
      </div>
    </div>
  `,
  styles: [
    `
    .rabbitmq-activity { width: 320px; }
    .activity-header { display:flex; justify-content:space-between; align-items:center; padding:8px; }
    .activity-content { padding:8px; }
    .subscribe-email input { padding:8px; border:1px solid #ddd; border-radius:4px; }
    `
  ]
})
export class RabbitMqActivityComponent implements OnInit, OnDestroy {
  constructor() {
    // Register the specific icons to avoid extra network requests
    try {
      addIcons({ addOutline, refreshOutline, createOutline, trashOutline });
    } catch {
      // ignore in non-browser contexts
    }
  }
  private http = inject(HttpClient);
  private subscription?: Subscription;
  private snack = inject(MatSnackBar);

  isVisible = signal(true);
  isExpanded = signal(false);
  summary = signal<ActivitySummary | null>(null);
  recentActivities = signal<EventActivity[]>([]);
  connectionStatus = signal<'connected' | 'disconnected'>('disconnected');
  emailToSubscribe = '';
  isSubmitting = signal(false);

  private emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

  isEmailValid(): boolean {
    return this.emailRegex.test((this.emailToSubscribe || '').trim());
  }

  ngOnInit() {
    this.startMonitoring();
  }

  subscribeEmail(): void {
    const email = (this.emailToSubscribe || '').trim();
    if (!this.isEmailValid()) {
      this.snack.open('Introduce un email válido', 'Cerrar', { duration: 2000 });
      return;
    }

    this.isSubmitting.set(true);
    // Simple POST a backend para registrar notificaciones (endpoint debe implementarse en backend)
    this.http.post('/api/notifications/subscribe', { email }).pipe(
      catchError(() => of(null)),
      finalize(() => this.isSubmitting.set(false))
    ).subscribe(result => {
      if (result) {
        this.snack.open('Email añadido', 'Cerrar', { duration: 2500 });
        this.emailToSubscribe = '';
      } else {
        this.snack.open('No se pudo añadir el email', 'Cerrar', { duration: 2500 });
      }
    });
  }

  ngOnDestroy() {
    this.subscription?.unsubscribe();
  }

  private startMonitoring() {
    this.checkConnectionStatus();
    this.subscription = interval(5000).pipe(
      switchMap(() => this.http.get<any>('/api/activity?count=20')),
      catchError(() => of([] as EventActivity[]))
    ).subscribe(activities => this.updateSummary(activities));
  }

  private checkConnectionStatus(): void {
    this.http.get('/hc').pipe(
      catchError(() => of(null as any))
    ).subscribe({
      next: (health: any) => {
        const rabbitMqHealth = health?.entries ? health.entries['rabbitmq-connection'] : null;
        this.connectionStatus.set(rabbitMqHealth?.status === 'Healthy' ? 'connected' : 'disconnected');
      },
      error: () => this.connectionStatus.set('disconnected')
    });
  }

  // Helper removed duplicate; single implementation below handles formatting

  private updateSummary(activities: any) {
    // Normalize incoming data to an array
    const list: EventActivity[] = Array.isArray(activities)
      ? activities
      : (activities && (activities.recentActivities || activities.items || activities.data)) || [];

    const sum: ActivitySummary = {
      totalEvents: list.length,
      publishedCount: list.filter(a => a.status === 'Published').length,
      consumedCount: list.filter(a => a.status === 'Consumed').length,
      failedCount: list.filter(a => a.status === 'Failed').length,
      eventTypeBreakdown: list.reduce((acc, a) => {
        acc[a.eventType] = (acc[a.eventType] || 0) + 1;
        return acc;
      }, {} as { [key: string]: number }),
      recentActivities: list
    };
    this.summary.set(sum);
    this.recentActivities.set(list);
  }

  getStatusClass(): string {
    return this.connectionStatus() === 'connected' ? 'connected' : 'disconnected';
  }

  formatTime(date: Date | string | undefined): string {
    if (!date) return 'now';
    const d = typeof date === 'string' ? new Date(date) : date;
    const diff = Math.floor((new Date().getTime() - d.getTime()) / 1000);
    if (diff < 60) return `${diff}s`;
    if (diff < 3600) return `${Math.floor(diff / 60)}m`;
    return `${Math.floor(diff / 3600)}h`;
  }
}
