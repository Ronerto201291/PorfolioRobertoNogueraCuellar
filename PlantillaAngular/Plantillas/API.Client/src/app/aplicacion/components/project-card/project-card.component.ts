import { Component, Input, Output, EventEmitter } from '@angular/core';
import { IonicModule } from '@ionic/angular';
import { CommonModule } from '@angular/common';
import { Project } from '../../../services/portfolio.service';

@Component({
  selector: 'app-project-card',
  template: `
    <ion-card (click)="onSelect()" [class.selected]="selected">
      <ion-item lines="none">
        <ion-icon slot="start" name="folder-outline" aria-hidden="true"></ion-icon>
        <ion-label>
          <h2>{{ project.name }}</h2>
          <p class="description">{{ project.description ? (project.description | slice:0:80) : '' }}{{ (project.description?.length || 0) > 80 ? '...' : '' }}</p>
          <div class="project-stats">
            <span class="stat"><span class="stat-icon">📝</span>{{ project.tasks.length || 0 }} tareas</span>
            <span class="stat"><span class="stat-icon">✅</span>{{ getCompletedCount() }} hechas</span>
          </div>
        </ion-label>

        <ion-buttons slot="end">
          <ion-button fill="clear" color="medium" (click)="onEdit($event)" aria-label="Editar proyecto">
            <ion-icon slot="icon-only" name="create-outline"></ion-icon>
          </ion-button>
          <ion-button fill="clear" color="danger" (click)="onDelete($event)" aria-label="Eliminar proyecto">
            <ion-icon slot="icon-only" name="trash-outline"></ion-icon>
          </ion-button>
        </ion-buttons>
      </ion-item>
    </ion-card>
  `,
  styles: [`
    ion-card {
      --background: #fff;
      border-radius: 12px;
      margin: 8px 0;
      box-shadow: 0 1px 3px rgba(0,0,0,0.06);
    }

    ion-item {
      align-items: center;
    }

    ion-icon[slot="start"] {
      font-size: 1.6rem;
      color: var(--ion-color-primary);
      margin-right: 8px;
    }

    ion-label h2 {
      margin: 0;
      font-size: 1rem;
      font-weight: 600;
    }

    ion-label p.description {
      margin: 4px 0 0 0;
      font-size: 0.85rem;
      color: #666;
    }

    .project-stats { display:flex; gap:12px; margin-top:8px; font-size:0.85rem; color:#777 }

    /* Ensure action buttons do not stretch vertically */
    ion-buttons[slot="end"] {
      display: flex;
      align-items: center;
      gap: 6px;
    }

    ion-button[fill="clear"] {
      height: 36px;
      width: 36px;
      min-width: 36px;
      display: inline-flex;
      align-items: center;
      justify-content: center;
      --padding-start: 6px;
      --padding-end: 6px;
    }

    ion-button[fill="clear"] ion-icon {
      font-size: 18px;
    }
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
    return this.project.tasks?.filter(t => t.status === 'Done' || t.status === 'Completed').length || 0;
  }
}
