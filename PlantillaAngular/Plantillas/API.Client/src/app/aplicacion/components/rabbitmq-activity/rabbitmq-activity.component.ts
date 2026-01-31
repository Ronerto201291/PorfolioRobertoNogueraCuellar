import { Component, OnInit, OnDestroy, signal, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { interval, Subscription, of } from 'rxjs';
import { switchMap, catchError } from 'rxjs/operators';
import { CommonModule } from '@angular/common';

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
  imports: [CommonModule],
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
        <div *ngIf="recentActivities().length === 0">No recent activity</div>
        <ul>
          <li *ngFor="let a of recentActivities()">{{ formatTime(a.timestamp) }} - {{ a.eventType }} - {{ a.status }}</li>
        </ul>
      </div>
    </div>
  `,
  styles: [``]
})
export class RabbitMqActivityComponent implements OnInit, OnDestroy {
  private http = inject(HttpClient);
  private subscription?: Subscription;

  isVisible = signal(true);
  isExpanded = signal(false);
  summary = signal<ActivitySummary | null>(null);
  recentActivities = signal<EventActivity[]>([]);
  connectionStatus = signal<'connected' | 'disconnected'>('disconnected');

  ngOnInit() {
    this.startMonitoring();
  }

  ngOnDestroy() {
    this.subscription?.unsubscribe();
  }

  private startMonitoring() {
    this.checkConnectionStatus();
    this.subscription = interval(5000).pipe(
      switchMap(() => this.http.get<EventActivity[]>('/api/activity?count=20')),
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

  private updateSummary(activities: EventActivity[]) {
    const sum: ActivitySummary = {
      totalEvents: activities.length,
      publishedCount: activities.filter(a => a.status === 'Published').length,
      consumedCount: activities.filter(a => a.status === 'Consumed').length,
      failedCount: activities.filter(a => a.status === 'Failed').length,
      eventTypeBreakdown: activities.reduce((acc, a) => {
        acc[a.eventType] = (acc[a.eventType] || 0) + 1;
        return acc;
      }, {} as { [key: string]: number }),
      recentActivities: activities
    };
    this.summary.set(sum);
    this.recentActivities.set(activities);
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
