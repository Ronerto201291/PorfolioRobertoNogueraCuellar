import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { interval, Subscription, of } from 'rxjs';
import { switchMap, catchError } from 'rxjs/operators';
import { EventActivity } from '../aplicacion/components/rabbitmq-activity/rabbitmq-activity.component';

@Injectable({
  providedIn: 'root'
})
export class RabbitMqLoggerService {
  private subscription?: Subscription;
  private lastEventId?: string;
  private isEnabled = true;

  constructor(private http: HttpClient) {
    this.startLogging();
  }

  private startLogging(): void {
    if (!this.isEnabled) return;

    this.subscription = interval(2000).pipe( // Check every 2 seconds
      switchMap(() => this.http.get<EventActivity[]>('/api/activity?count=5')),
      catchError(() => of([] as EventActivity[]))
    ).subscribe(activities => {
      if (activities && activities.length > 0) {
        const latestActivity = activities[0];
        if (latestActivity.eventId !== this.lastEventId) {
          this.logActivity(latestActivity);
          this.lastEventId = latestActivity.eventId;
        }
      }
    });
  }

  private logActivity(activity: EventActivity): void {
    const timestamp = new Date(activity.timestamp).toLocaleTimeString();
    const eventType = activity.eventType;
    const status = activity.status;
    const eventId = activity.eventId.slice(0, 8);

    const styles = this.getLogStyles(status);

    console.log(
      `%c[RabbitMQ] ${status} ${eventType} (EventId=${eventId})`,
      styles,
      { timestamp, details: (activity as unknown as { details?: unknown }).details }
    );
  }

  private getLogStyles(status: string): string {
    switch (status.toLowerCase()) {
      case 'published':
        return 'color: #1976d2; font-weight: bold; background: #e3f2fd; padding: 2px 4px; border-radius: 3px;';
      case 'consumed':
        return 'color: #388e3c; font-weight: bold; background: #e8f5e8; padding: 2px 4px; border-radius: 3px;';
      case 'failed':
        return 'color: #d32f2f; font-weight: bold; background: #ffebee; padding: 2px 4px; border-radius: 3px;';
      default:
        return 'color: #666; font-weight: bold;';
    }
  }

  enable(): void {
    this.isEnabled = true;
    this.startLogging();
  }

  disable(): void {
    this.isEnabled = false;
    this.subscription?.unsubscribe();
  }

  ngOnDestroy(): void {
    this.subscription?.unsubscribe();
  }
}
