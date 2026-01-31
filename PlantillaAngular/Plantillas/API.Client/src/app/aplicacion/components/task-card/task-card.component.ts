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
          <button *ngIf="task.status !== 'InProgress'" mat-mini-fab color="accent" class="action-fab" aria-label="Mover a En Progreso" (click)="changeStatus('InProgress')" matTooltip="Mover a En Progreso">
            <mat-icon>play_arrow</mat-icon>
          </button>
          <button *ngIf="task.status !== 'Completed'" mat-mini-fab color="primary" class="action-fab" aria-label="Marcar como Completada" (click)="changeStatus('Completed')" matTooltip="Marcar como Completada">
            <mat-icon>check</mat-icon>
          </button>
          <button *ngIf="task.status === 'Completed'" mat-mini-fab color="warn" class="action-fab" aria-label="Reabrir" (click)="changeStatus('Pending')" matTooltip="Reabrir tarea">
            <mat-icon>undo</mat-icon>
          </button>
          <!-- Ionic style buttons (recommended) -->
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

    .task-card.completed {
      background: #f8f9fa;
      border-color: #28a745;
    }

    .task-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 8px;
    }

    .priority {
      font-size: 0.8rem;
      padding: 2px 6px;
      border-radius: 4px;
      font-weight: 500;
    }

    .priority-low { background: #e3f2fd; color: #1976d2; }
    .priority-medium { background: #fff3e0; color: #f57c00; }
    .priority-high { background: #ffebee; color: #d32f2f; }

    .task-id {
      font-size: 0.7rem;
      color: #666;
      font-family: monospace;
    }

    .task-title {
      margin: 0 0 6px 0;
      font-size: 0.9rem;
      font-weight: 600;
      color: #1a1a2e;
    }

    .task-description {
      margin: 0 0 8px 0;
      font-size: 0.8rem;
      color: #666;
      line-height: 1.3;
    }

    .task-footer {
      display: flex;
      justify-content: space-between;
      align-items: center;
    }

    .task-meta {
      flex: 1;
    }

    .due-date {
      font-size: 0.75rem;
      color: #666;
    }

    .due-date.overdue {
      color: #d32f2f;
      font-weight: 500;
    }

    .task-actions {
      display: flex;
      gap: 8px;
      align-items: center !important; /* prevent stretch from parent flex */
      align-self: center;
    }

    .action-btn {
      background: none;
      border: none;
      font-size: 1rem;
      cursor: pointer;
      padding: 2px;
      border-radius: 4px;
      transition: background 0.2s ease;
    }
    .action-fab { box-shadow: none; height:32px; width:32px; min-width:32px; align-self:center }
    button.mat-mini-fab { box-shadow: none; align-self:center }

    .action-fab:hover, button.mat-mini-fab:hover { transform: translateY(-2px); }

    .edit-btn, .delete-btn { width: 36px; height: 36px; align-self:center }
    .edit-btn mat-icon { color: #1976d2; }
    .delete-btn mat-icon { color: #d32f2f; }

    /* Ensure native buttons inside task-actions don't stretch */
    .task-actions button, .task-actions ion-button { align-self: center; }
  `]
  ,
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
