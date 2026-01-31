import { Component, Input, Output, EventEmitter } from '@angular/core';
import { IonicModule } from '@ionic/angular';
import { CommonModule } from '@angular/common';
import { Project } from '../../../services/portfolio.service';

@Component({
  selector: 'app-project-card',
  template: `
    <div class="project-card" 
         [class.selected]="selected"
         (click)="onSelect()">
      <div class="project-icon">📁</div>
        <div class="project-info">
        <h3>{{ project.name }}</h3>
        <p class="description">{{ project.description ? (project.description | slice:0:80) : '' }}{{ (project.description?.length || 0) > 80 ? '...' : '' }}</p>
        <div class="project-stats">
          <span class="stat">
            <span class="stat-icon">📝</span>
            {{ project.tasks.length || 0 }} tareas
          </span>
          <span class="stat">
            <span class="stat-icon">✅</span>
            {{ getCompletedCount() }} hechas
          </span>
        </div>
      </div>
      <div class="project-actions">
        <ion-button fill="clear" size="small" (click)="onEdit($event)" aria-label="Editar proyecto">
          <ion-icon name="create-outline"></ion-icon>
        </ion-button>
        <ion-button fill="clear" size="small" color="danger" (click)="onDelete($event)" aria-label="Eliminar proyecto">
          <ion-icon name="trash-outline"></ion-icon>
        </ion-button>
      </div>
    </div>
  `,
  styles: [`
    .project-card {
      display: flex;
      gap: 16px;
      padding: 16px;
      background: #fff;
      border: 2px solid #e8e8e8;
      border-radius: 12px;
      cursor: pointer;
      transition: all 0.2s ease;
      position: relative;
    }
    
    .project-card:hover {
      border-color: #667eea;
      transform: translateY(-2px);
      box-shadow: 0 4px 12px rgba(102, 126, 234, 0.15);
    }
    
    .project-card.selected {
      border-color: #667eea;
      background: linear-gradient(135deg, #f5f7ff 0%, #fff 100%);
    }
    
    .project-icon {
      font-size: 2rem;
      width: 48px;
      height: 48px;
      display: flex;
      align-items: center;
      justify-content: center;
      background: #f0f4ff;
      border-radius: 12px;
    }
    
    .project-info {
      flex: 1;
      min-width: 0;
    }
    
    .project-info h3 {
      margin: 0 0 6px 0;
      font-size: 1rem;
      font-weight: 600;
      color: #1a1a2e;
    }
    
    .description {
      margin: 0 0 12px 0;
      font-size: 0.85rem;
      color: #666;
      line-height: 1.4;
    }
    
    .project-stats {
      display: flex;
      gap: 16px;
    }
    
    .stat {
      display: flex;
      align-items: center;
      gap: 4px;
      font-size: 0.8rem;
      color: #888;
    }
    
    .stat-icon {
      font-size: 0.9rem;
    }

    .project-actions {
      display: flex;
      gap: 8px;
      opacity: 0;
      transition: opacity 0.2s ease, transform 0.2s ease;
      transform: translateY(-4px);
      align-items: center; /* prevent children stretch */
    }

    .project-card:hover .project-actions {
      opacity: 1;
      transform: translateY(0);
    }

    button.mat-icon-button { padding: 6px; border-radius: 8px; }
    button.mat-icon-button:hover { background: rgba(0,0,0,0.04); }
  `]
  ,
  standalone: true,
  imports: [CommonModule, IonicModule]
})
export class ProjectCardComponent {
  @Input() project!: Project;
  @Input() selected = false;
  @Output() selectProject = new EventEmitter<Project>();
  @Output() editProject = new EventEmitter<Project>();
  @Output() deleteProject = new EventEmitter<Project>();

  onSelect(): void {
    this.selectProject.emit(this.project);
  }

  onEdit(evt: Event): void {
    evt.stopPropagation();
    this.editProject.emit(this.project);
  }

  onDelete(evt: Event): void {
    evt.stopPropagation();
    this.deleteProject.emit(this.project);
  }

  getCompletedCount(): number {
    return this.project.tasks?.filter(t => t.status === 'Completed').length || 0;
  }
}
