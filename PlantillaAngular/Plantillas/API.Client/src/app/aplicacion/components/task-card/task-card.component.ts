import { Component, Input, Output, EventEmitter } from '@angular/core';
import { PortfolioTask } from '../../../services/portfolio.service';
import { IonicModule } from '@ionic/angular';

@Component({
  selector: 'app-task-card',
  template: `
    <div class="task-card" [class.completed]="task.status === 'Completed'">
      <div class="task-header">
        <span class="priority" [class]="'priority-' + task.priority.toLowerCase()">
          {{ getPriorityIcon() }}
        </span>
        <span class="task-id">#{{ task.taskId.slice(0, 8) }}</span>
      </div>
      
      <h4 class="task-title">{{ task.title }}</h4>
      
      <p *ngIf="task.description" class="task-description">
        {{ task.description | slice:0:100 }}{{ task.description.length > 100 ? '...' : '' }}
      </p>
      
      <div class="task-footer">
        <div class="task-meta">
          <span *ngIf="task.dueDate" class="due-date" [class.overdue]="isOverdue()">
            📅 {{ formatDate(task.dueDate) }}
          </span>
        </div>
        
        <div class="task-actions">
          <!-- Restore original small rounded icon buttons using ion-button with fill="clear" -->
          <ion-button *ngIf="task.status !== 'InProgress'" fill="clear" size="small" (click)="changeStatus('InProgress')" aria-label="Mover a En Progreso">
            <ion-icon name="play-outline"></ion-icon>
          </ion-button>

          <ion-button *ngIf="task.status !== 'Completed'" fill="clear" size="small" (click)="changeStatus('Completed')" aria-label="Marcar como Completada">
            <ion-icon name="checkmark-outline"></ion-icon>
          </ion-button>

          <ion-button *ngIf="task.status === 'Completed'" fill="clear" size="small" (click)="changeStatus('Pending')" aria-label="Reabrir">
            <ion-icon name="return-up-back-outline"></ion-icon>
          </ion-button>

          <ion-button fill="clear" size="small" (click)="onEdit($event)" aria-label="Editar tarea" class="edit-btn">
            <ion-icon name="create-outline"></ion-icon>
          </ion-button>
          <ion-button fill="clear" size="small" color="danger" (click)="onDelete($event)" aria-label="Eliminar tarea" class="delete-btn">
            <ion-icon name="trash-outline"></ion-icon>
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

    .edit-btn ion-icon { color:#1976d2 }
    .delete-btn ion-icon { color:#d32f2f }
  `],
  standalone: true,
  imports: [IonicModule]
})
export class TaskCardComponent {
  @Input() task!: PortfolioTask;
  @Output() statusChanged = new EventEmitter<{ taskId: string; newStatus: string }>();
  @Output() editTask = new EventEmitter<PortfolioTask>();
  @Output() deleteTask = new EventEmitter<PortfolioTask>();

  changeStatus(newStatus: string): void {
    this.statusChanged.emit({ taskId: this.task.taskId, newStatus });
  }

  onEdit(evt: Event): void {
    evt.stopPropagation();
    this.editTask.emit(this.task);
  }

  onDelete(evt: Event): void {
    evt.stopPropagation();
    this.deleteTask.emit(this.task);
  }

  getPriorityIcon(): string {
    switch (this.task.priority) {
      case 'High': return '🔴';
      case 'Medium': return '🟡';
      case 'Low': return '🟢';
      default: return '⚪';
    }
  }

  isOverdue(): boolean {
    if (!this.task.dueDate) return false;
    return new Date(this.task.dueDate) < new Date();
  }

  formatDate(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleDateString('es-ES', { 
      day: '2-digit', 
      month: '2-digit',
      year: 'numeric'
    });
  }
}
