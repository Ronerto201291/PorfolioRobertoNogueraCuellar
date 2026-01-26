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
    
    .task-card:hover {
      box-shadow: 0 4px 12px rgba(0, 0, 0, 0.08);
      transform: translateY(-1px);
    }
    
    .task-card.completed {
      opacity: 0.7;
      background: #fafafa;
    }
    
    .task-card.completed .task-title {
      text-decoration: line-through;
      color: #888;
    }
    
    .task-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 8px;
    }
    
    .priority {
      font-size: 0.75rem;
      padding: 2px 8px;
      border-radius: 4px;
    }
    
    .priority-high { background: #ffe5e5; }
    .priority-medium { background: #fff3e0; }
    .priority-low { background: #e8f5e9; }
    
    .task-id {
      font-size: 0.7rem;
      color: #aaa;
      font-family: monospace;
    }
    
    .task-title {
      margin: 0 0 8px 0;
      font-size: 0.9rem;
      font-weight: 500;
      color: #333;
      line-height: 1.3;
    }
    
    .task-description {
      margin: 0 0 12px 0;
      font-size: 0.8rem;
      color: #666;
      line-height: 1.4;
    }
    
    .task-footer {
      display: flex;
      justify-content: space-between;
      align-items: center;
    }
    
    .task-meta {
      font-size: 0.75rem;
      color: #888;
    }
    
    .due-date.overdue {
      color: #e53935;
    }
    
    .task-actions {
      display: flex;
      gap: 4px;
      opacity: 0;
      transition: opacity 0.2s ease;
    }
    
    .task-card:hover .task-actions {
      opacity: 1;
    }
    
    .action-btn {
      width: 24px;
      height: 24px;
      border: none;
      border-radius: 4px;
      cursor: pointer;
      font-size: 0.7rem;
      display: flex;
      align-items: center;
      justify-content: center;
      transition: all 0.2s ease;
    }
    
    .action-btn.progress {
      background: #e3f2fd;
      color: #1976d2;
    }
    
    .action-btn.complete {
      background: #e8f5e9;
      color: #388e3c;
    }
    
    .action-btn.reopen {
      background: #fff3e0;
      color: #f57c00;
    }
    
    .action-btn:hover {
      transform: scale(1.1);
    }
  `]
})
export class TaskCardComponent {
  @Input() task!: PortfolioTask;
  @Output() statusChange = new EventEmitter<{ task: PortfolioTask; newStatus: string }>();

  getPriorityIcon(): string {
    switch (this.task.priority) {
      case 'High': return '🔴 Alta';
      case 'Medium': return '🟡 Media';
      case 'Low': return '🟢 Baja';
      default: return '⚪ Normal';
    }
  }

  formatDate(dateString: string): string {
    if (!dateString) return '';
    return new Date(dateString).toLocaleDateString('es-ES', { month: 'short', day: 'numeric' });
  }

  isOverdue(): boolean {
    if (!this.task.dueDate) return false;
    return new Date(this.task.dueDate) < new Date() && this.task.status !== 'Completed';
  }

  changeStatus(newStatus: string): void {
    this.statusChange.emit({ task: this.task, newStatus });
  }
}
