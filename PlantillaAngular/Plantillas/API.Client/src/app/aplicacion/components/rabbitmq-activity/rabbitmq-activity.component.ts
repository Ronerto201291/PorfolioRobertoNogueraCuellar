import { Component, OnInit, OnDestroy, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { interval, Subscription } from 'rxjs';
import { switchMap, catchError } from 'rxjs/operators';
import { of } from 'rxjs';

export interface EventActivity {
  eventId: string;
  eventType: string;
  timestamp: Date;
  status: string;
  details?: string;
}

export interface ActivitySummary {
  totalEvents: number;
  publishedCount: number;
  consumedCount: number;
  failedCount: number;
  lastActivityAt?: Date;
  eventTypeBreakdown: { [key: string]: number };
  recentActivities?: EventActivity[];
}

@Component({
  selector: 'app-rabbitmq-activity',
  template: `
    <div class="rabbitmq-activity" *ngIf="isVisible()">
      <div class="activity-header">
        <div class="activity-title">
          <span class="rabbit-icon">🐰</span>
          <span>RabbitMQ Activity</span>
          <span class="status-indicator" [class]="getStatusClass()"></span>
        </div>
        <button mat-icon-button (click)="toggleVisibility()" matTooltip="Toggle activity panel">
          <mat-icon>{{ isVisible() ? 'expand_less' : 'expand_more' }}</mat-icon>
        </button>
      </div>

      <div class="activity-content" *ngIf="isExpanded()">
        <!-- Summary -->
        <div class="activity-summary" *ngIf="summary()">
          <div class="summary-item">
            <span class="label">Total Events:</span>
            <span class="value">{{ summary()?.totalEvents }}</span>
          </div>
          <div class="summary-item">
            <span class="label">Published:</span>
            <span class="value published">{{ summary()?.publishedCount }}</span>
          </div>
          <div class="summary-item">
            <span class="label">Consumed:</span>
            <span class="value consumed">{{ summary()?.consumedCount }}</span>
          </div>
          <div class="summary-item" *ngIf="summary()?.failedCount && summary()?.failedCount > 0">
            <span class="label">Failed:</span>
            <span class="value failed">{{ summary()?.failedCount }}</span>
          </div>
        </div>

        <!-- Recent Activity -->
        <div class="recent-activity">
          <h4>Recent Activity</h4>
          <div class="activity-list">
            <div class="activity-item" *ngFor="let activity of recentActivities()">
              <div class="activity-time">
                {{ formatTime(activity.timestamp) }}
              </div>
              <div class="activity-details">
                <span class="event-type">{{ activity.eventType }}</span>
                <span class="event-status" [class]="activity.status.toLowerCase()">
                  {{ activity.status }}
                </span>
              </div>
              <div class="activity-id">
                {{ activity.eventId.slice(0, 8) }}
              </div>
            </div>
            <div class="no-activity" *ngIf="recentActivities().length === 0">
              No recent activity
            </div>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .rabbitmq-activity {
      position: fixed;
      bottom: 20px;
      right: 20px;
      width: 350px;
      background: #fff;
      border: 1px solid #e0e0e0;
      border-radius: 8px;
      box-shadow: 0 4px 12px rgba(0,0,0,0.15);
      z-index: 1000;
      font-family: 'Roboto', sans-serif;
    }

    .activity-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 12px 16px;
      background: #f8f9fa;
      border-bottom: 1px solid #e0e0e0;
      border-radius: 8px 8px 0 0;
    }

    .activity-title {
      display: flex;
      align-items: center;
      gap: 8px;
      font-weight: 500;
      color: #333;
    }

    .rabbit-icon {
      font-size: 1.2em;
    }

    .status-indicator {
      width: 8px;
      height: 8px;
      border-radius: 50%;
      background: #ccc;
    }

    .status-indicator.connected {
      background: #4caf50;
      box-shadow: 0 0 6px rgba(76, 175, 80, 0.5);
    }

    .status-indicator.disconnected {
      background: #f44336;
    }

    .activity-content {
      max-height: 400px;
      overflow-y: auto;
    }

    .activity-summary {
      padding: 16px;
      border-bottom: 1px solid #e0e0e0;
      display: grid;
      grid-template-columns: 1fr 1fr;
      gap: 8px;
    }

    .summary-item {
      display: flex;
      justify-content: space-between;
      align-items: center;
    }

    .summary-item .label {
      font-size: 0.85em;
      color: #666;
    }

    .summary-item .value {
      font-weight: 500;
    }

    .value.published { color: #2196f3; }
    .value.consumed { color: #4caf50; }
    .value.failed { color: #f44336; }

    .recent-activity {
      padding: 16px;
    }

    .recent-activity h4 {
      margin: 0 0 12px 0;
      font-size: 0.9em;
      color: #333;
      font-weight: 500;
    }

    .activity-list {
      max-height: 200px;
      overflow-y: auto;
    }

    .activity-item {
      display: grid;
      grid-template-columns: 60px 1fr 60px;
      gap: 8px;
      padding: 6px 0;
      border-bottom: 1px solid #f0f0f0;
      font-size: 0.8em;
    }

    .activity-item:last-child {
      border-bottom: none;
    }

    .activity-time {
      color: #666;
      font-size: 0.75em;
    }

    .activity-details {
      display: flex;
      flex-direction: column;
      gap: 2px;
    }

    .event-type {
      font-weight: 500;
      color: #333;
    }

    .event-status {
      font-size: 0.7em;
      padding: 2px 6px;
      border-radius: 10px;
      text-transform: uppercase;
      font-weight: 500;
    }

    .event-status.published {
      background: #e3f2fd;
      color: #1976d2;
    }

    .event-status.consumed {
      background: #e8f5e8;
      color: #388e3c;
    }

    .event-status.failed {
      background: #ffebee;
      color: #d32f2f;
    }

    .activity-id {
      font-family: monospace;
      color: #666;
      font-size: 0.7em;
    }

    .no-activity {
      text-align: center;
      color: #999;
      font-style: italic;
      padding: 20px;
    }
  `]
})
export class RabbitMqActivityComponent implements OnInit, OnDestroy {
  private subscription?: Subscription;
  private refreshInterval = 5000; // 5 seconds

  isVisible = signal<boolean>(true);
  isExpanded = signal<boolean>(false);
  summary = signal<ActivitySummary | null>(null);
  recentActivities = signal<EventActivity[]>([]);
  connectionStatus = signal<'connected' | 'disconnected'>('disconnected');

  constructor(private http: HttpClient) {}

  ngOnInit() {
    this.startMonitoring();
    // Load initial data
    this.loadActivity().subscribe(summary => {
      if (summary) {
        this.summary.set(summary);
        this.recentActivities.set(summary.recentActivities || []);
      }
    });
  }

  ngOnDestroy() {
    this.stopMonitoring();
  }

  toggleVisibility(): void {
    this.isVisible.set(!this.isVisible());
  }

  private startMonitoring(): void {
    // Check connection status
    this.checkConnectionStatus();

    // Monitor activity
    this.subscription = interval(this.refreshInterval).pipe(
      switchMap(() => this.loadActivity())
    ).subscribe(activities => {
      this.recentActivities.set(activities);
      // Calculate summary from activities
      this.updateSummary(activities);
    });
  }

  private stopMonitoring(): void {
    this.subscription?.unsubscribe();
  }

  private checkConnectionStatus(): void {
    this.http.get('/hc').subscribe({
      next: (health: any) => {
        const rabbitMqHealth = health.entries?.['rabbitmq-connection'];
        this.connectionStatus.set(rabbitMqHealth?.status === 'Healthy' ? 'connected' : 'disconnected');
      },
      error: () => this.connectionStatus.set('disconnected')
    });
  }

  private loadActivity() {
    return this.http.get<EventActivity[]>('/api/activity?count=20').pipe(
      catchError(() => of([] as EventActivity[]))
    );
  }

  private updateSummary(activities: EventActivity[]): void {
    const summary: ActivitySummary = {
      totalEvents: activities.length,
      publishedCount: activities.filter(a => a.status === 'Published').length,
      consumedCount: activities.filter(a => a.status === 'Consumed').length,
      failedCount: activities.filter(a => a.status === 'Failed').length,
      lastActivityAt: activities.length > 0 ? activities[0].timestamp : undefined,
      eventTypeBreakdown: activities.reduce((acc, activity) => {
        acc[activity.eventType] = (acc[activity.eventType] || 0) + 1;
        return acc;
      }, {} as { [key: string]: number }),
      recentActivities: activities
    };
    this.summary.set(summary);
  }

  getStatusClass(): string {
    return this.connectionStatus();
  }

  formatTime(date: Date): string {
    const now = new Date();
    const diff = now.getTime() - new Date(date).getTime();
    const seconds = Math.floor(diff / 1000);

    if (seconds < 60) return `${seconds}s`;
    const minutes = Math.floor(seconds / 60);
    if (minutes < 60) return `${minutes}m`;
    const hours = Math.floor(minutes / 60);
    return `${hours}h`;
  }
}
