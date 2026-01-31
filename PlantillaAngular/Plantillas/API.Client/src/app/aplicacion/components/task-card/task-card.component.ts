import { Component, Input, Output, EventEmitter } from '@angular/core';
import { PortfolioTask } from '../../../services/portfolio.service';

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
          <button *ngIf="task.status !== 'InProgress'" 
                  class="action-btn progress" 
                  (click)="changeStatus('InProgress')"
                  title="Mover a En Progreso">
            ▶️
          </button>
          <button *ngIf="task.status !== 'Completed'" 
                  class="action-btn complete" 
                  (click)="changeStatus('Completed')"
                  title="Marcar como Completada">
            ✓
          </button>
          <button *ngIf="task.status === 'Completed'" 
                  class="action-btn reopen" 
                  (click)="changeStatus('Pending')"
                  title="Reabrir">
            ↩️
          </button>
          <button mat-icon-button (click)="editTask.emit(task)" matTooltip="Editar" class="edit-btn">
            <mat-icon>edit</mat-icon>
          </button>
          <button mat-icon-button color="warn" (click)="deleteTask.emit(task)" matTooltip="Eliminar" class="delete-btn">
            <mat-icon>delete</mat-icon>
          </button>
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
      gap: 4px;
      align-items: center;
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

    .action-btn:hover {
      background: #f5f5f5;
    }

    .edit-btn, .delete-btn {
      width: 32px;
      height: 32px;
    }

    .edit-btn {
      color: #1976d2;
    }

    .delete-btn {
      color: #d32f2f;
    }
  `]
})
export class TaskCardComponent {
  @Input() task!: PortfolioTask;
  @Output() statusChanged = new EventEmitter<{ taskId: string; newStatus: string }>();
  @Output() editTask = new EventEmitter<PortfolioTask>();
  @Output() deleteTask = new EventEmitter<PortfolioTask>();

  changeStatus(newStatus: string): void {
    this.statusChanged.emit({ taskId: this.task.taskId, newStatus });
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
