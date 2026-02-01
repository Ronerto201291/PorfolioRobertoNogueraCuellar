import { Component, Input, Output, EventEmitter } from '@angular/core';
import { Task, TaskStatus } from '../../../models/task.models';
import { IonicModule } from '@ionic/angular';

@Component({
  selector: 'app-task-card',
  template: `
    <div class="task-card" [class.done]="task.status === 'Completed'">
      <div class="task-header">
        <span class="status-badge" [class]="getStatusClass()">
          {{ getStatusLabel() }}
        </span>
        <span class="task-id">#{{ task.id.slice(0, 8) }}</span>
      </div>
      
      <h4 class="task-title">{{ task.title }}</h4>
      
      <p *ngIf="task.description" class="task-description">
        {{ task.description | slice:0:100 }}{{ task.description.length > 100 ? '...' : '' }}
      </p>
      
      <div class="task-footer">
        <div class="task-actions">
          <ion-button *ngIf="task.status !== 'InProgress'" fill="clear" class="action-btn" (click)="changeStatus('InProgress')" aria-label="Mover a En Progreso">
            <ion-icon slot="icon-only" name="play-outline"></ion-icon>
          </ion-button>

          <ion-button *ngIf="task.status !== 'Completed'" fill="clear" class="action-btn" (click)="changeStatus('Completed')" aria-label="Marcar como Finalizada">
            <ion-icon slot="icon-only" name="checkmark-outline"></ion-icon>
          </ion-button>

          <ion-button *ngIf="task.status === 'Completed'" fill="clear" class="action-btn" (click)="changeStatus('Pending')" aria-label="Reabrir">
            <ion-icon slot="icon-only" name="return-up-back-outline"></ion-icon>
          </ion-button>

          <ion-button fill="clear" class="action-btn edit-btn" (click)="onEdit($event)" aria-label="Editar tarea">
            <ion-icon slot="icon-only" name="create-outline"></ion-icon>
          </ion-button>
          <ion-button fill="clear" class="action-btn delete-btn" color="danger" (click)="onDelete($event)" aria-label="Eliminar tarea">
            <ion-icon slot="icon-only" name="trash-outline"></ion-icon>
          </ion-button>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .task-card {
      background: #fff;
      border: 1px solid #e8e8e8;
      border-radius: 8px;
      padding: 12px;
      margin-bottom: 8px;
      transition: all 0.2s ease;
      cursor: grab;
    }

    .task-footer { display:flex; justify-content:space-between; align-items:center }

    .task-actions { display:flex; gap:8px; align-items:center }

    ion-button { --padding-start: 6px; --padding-end: 6px; }

    .action-btn {
      height: 36px;
      width: 36px;
      min-width: 36px;
      display: inline-flex;
      align-items: center;
      justify-content: center;
      --padding-start: 6px;
      --padding-end: 6px;
      border-radius: 50%;
      box-shadow: none;
    }

    .status-badge {
      font-size: 0.75rem;
      padding: 4px 8px;
      border-radius: 12px;
      font-weight: 600;
    }

    .status-pending { background:#e3f2fd; color:#1976d2; }
    .status-inprogress { background:#fff3e0; color:#ef6c00; }
    .status-done { background:#e8f5e9; color:#2e7d32; }

    .edit-btn ion-icon { color:#1976d2 }
    .delete-btn ion-icon { color:#d32f2f }
  `],
  standalone: true,
  imports: [IonicModule]
})
export class TaskCardComponent {
  @Input() task!: Task;
  @Output() statusChanged = new EventEmitter<{ taskId: string; newStatus: TaskStatus }>();
  @Output() editTask = new EventEmitter<Task>();
  @Output() deleteTask = new EventEmitter<Task>();

  changeStatus(newStatus: TaskStatus): void {
    this.statusChanged.emit({ taskId: this.task.id, newStatus });
  }

  onEdit(evt: Event): void {
    evt.stopPropagation();
    this.editTask.emit(this.task);
  }

  onDelete(evt: Event): void {
    evt.stopPropagation();
    this.deleteTask.emit(this.task);
  }

  getStatusClass(): string {
    switch (this.task.status) {
      case 'Pending':
        return 'status-pending';
      case 'InProgress':
        return 'status-inprogress';
      case 'Completed':
        return 'status-done';
      default:
        return 'status-pending';
    }
  }

  getStatusLabel(): string {
    switch (this.task.status) {
      case 'Pending':
        return 'Pendiente';
      case 'InProgress':
        return 'En progreso';
      case 'Completed':
        return 'Finalizada';
      default:
        return this.task.status;
    }
  }
}
