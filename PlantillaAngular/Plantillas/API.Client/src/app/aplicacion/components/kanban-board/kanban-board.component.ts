import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CdkDragDrop, moveItemInArray, transferArrayItem } from '@angular/cdk/drag-drop';
import { PortfolioTask } from '../../../services/portfolio.service';

export interface KanbanColumn {
  id: string;
  title: string;
  icon: string;
  color: string;
  tasks: PortfolioTask[];
}

@Component({
  selector: 'app-kanban-board',
  template: `
    <div class="kanban-board">
      <div class="kanban-column" *ngFor="let column of columns">
        <div class="column-header" [style.background]="column.color">
          <span class="column-icon">{{ column.icon }}</span>
          <span class="column-title">{{ column.title }}</span>
          <span class="column-count">{{ column.tasks.length }}</span>
        </div>
        
        <div class="column-content"
             cdkDropList
             [cdkDropListData]="column.tasks"
             [id]="column.id"
             [cdkDropListConnectedTo]="getConnectedLists(column.id)"
             (cdkDropListDropped)="onDrop($event, column.id)">
          
          <app-task-card 
            *ngFor="let task of column.tasks"
            [task]="task"
            (statusChanged)="onStatusChange($event)"
            (editTask)="onEditTask($event)"
            (deleteTask)="onDeleteTask($event)"
            cdkDrag
            [cdkDragData]="task">
            
            <div class="drag-placeholder" *cdkDragPlaceholder></div>
          </app-task-card>
          
          <div *ngIf="column.tasks.length === 0" class="empty-column">
            <span class="empty-icon">📭</span>
            <span class="empty-text">Sin tareas</span>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .kanban-board {
      display: grid;
      grid-template-columns: repeat(3, 1fr);
      gap: 20px;
      height: 100%;
    }
    
    @media (max-width: 1024px) {
      .kanban-board {
        grid-template-columns: 1fr;
      }
    }
    
    .kanban-column {
      background: #f8f9fa;
      border-radius: 12px;
      display: flex;
      flex-direction: column;
      min-height: 400px;
    }
    
    .column-header {
      display: flex;
      align-items: center;
      gap: 8px;
      padding: 14px 16px;
      border-radius: 12px 12px 0 0;
      color: #fff;
      font-weight: 600;
    }
    
    .column-icon {
      font-size: 1.1rem;
    }
    
    .column-title {
      flex: 1;
    }
    
    .column-count {
      background: rgba(255,255,255,0.25);
      padding: 2px 10px;
      border-radius: 12px;
      font-size: 0.85rem;
    }
    
    .column-content {
      flex: 1;
      padding: 12px;
      overflow-y: auto;
      min-height: 200px;
    }
    
    .empty-column {
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      height: 150px;
      color: #aaa;
    }
    
    .empty-icon {
      font-size: 2rem;
      margin-bottom: 8px;
    }
    
    .empty-text {
      font-size: 0.85rem;
    }
    
    .drag-placeholder {
      background: #e3f2fd;
      border: 2px dashed #90caf9;
      border-radius: 8px;
      min-height: 80px;
      margin-bottom: 8px;
    }
    
    .cdk-drag-preview {
      box-shadow: 0 8px 24px rgba(0,0,0,0.15);
      border-radius: 8px;
    }
    
    .cdk-drag-animating {
      transition: transform 250ms cubic-bezier(0, 0, 0.2, 1);
    }
    
    .cdk-drop-list-dragging .task-card:not(.cdk-drag-placeholder) {
      transition: transform 250ms cubic-bezier(0, 0, 0.2, 1);
    }
  `]
})
export class KanbanBoardComponent {
  @Input() tasks: PortfolioTask[] = [];
  @Output() taskStatusChanged = new EventEmitter<{ taskId: string; newStatus: string }>();
  @Output() editTask = new EventEmitter<PortfolioTask>();
  @Output() deleteTask = new EventEmitter<PortfolioTask>();

  get columns(): KanbanColumn[] {
    return [
      {
        id: 'Pending',
        title: 'Pendiente',
        icon: '📋',
        color: '#667eea',
        tasks: this.tasks.filter(t => t.status === 'Pending')
      },
      {
        id: 'InProgress',
        title: 'En Progreso',
        icon: '🔄',
        color: '#f5a623',
        tasks: this.tasks.filter(t => t.status === 'InProgress')
      },
      {
        id: 'Completed',
        title: 'Completada',
        icon: '✅',
        color: '#4caf50',
        tasks: this.tasks.filter(t => t.status === 'Completed')
      }
    ];
  }

  getConnectedLists(currentId: string): string[] {
    return ['Pending', 'InProgress', 'Completed'].filter(id => id !== currentId);
  }

  onDrop(event: CdkDragDrop<PortfolioTask[]>, columnId: string): void {
    if (event.previousContainer === event.container) {
      moveItemInArray(event.container.data, event.previousIndex, event.currentIndex);
    } else {
      const task = event.item.data as PortfolioTask;
      transferArrayItem(
        event.previousContainer.data,
        event.container.data,
        event.previousIndex,
        event.currentIndex
      );
      this.taskStatusChanged.emit({ taskId: task.taskId, newStatus: columnId });
    }
  }

  onStatusChange(event: { taskId: string; newStatus: string }): void {
    this.taskStatusChanged.emit({ taskId: event.taskId, newStatus: event.newStatus });
  }

  onEditTask(task: PortfolioTask): void {
    this.editTask.emit(task);
  }

  onDeleteTask(task: PortfolioTask): void {
    this.deleteTask.emit(task);
  }
}
