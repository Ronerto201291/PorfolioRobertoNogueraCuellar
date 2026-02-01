/* eslint-disable @typescript-eslint/no-explicit-any */
import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { IonicModule } from '@ionic/angular';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { MatChipsModule } from '@angular/material/chips';
import { PortfolioService, Project } from '../../services/portfolio.service';
import { ProjectCardComponent } from '../components/project-card/project-card.component';
import { KanbanBoardComponent } from '../components/kanban-board/kanban-board.component';
import { TaskCardComponent } from '../components/task-card/task-card.component';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { EditProjectDialogComponent, ProjectDialogData } from '../components/edit-project-dialog/edit-project-dialog.component';
import { EditTaskDialogComponent, TaskDialogData } from '../components/edit-task-dialog/edit-task-dialog.component';
import { TaskService } from '../../services/task.service';
import { CreateTaskRequest, Task, TaskStatus, UpdateTaskRequest } from '../../models/task.models';

@Component({
  selector: 'app-main-view',
  templateUrl: './main-view.component.html',
  styleUrls: ['./main-view.component.scss'],
  standalone: true,
  imports: [CommonModule, FormsModule, IonicModule, MatButtonModule, MatIconModule, MatListModule, MatChipsModule, ProjectCardComponent, KanbanBoardComponent, TaskCardComponent]
})
export class MainViewComponent implements OnInit {

  projects = signal<Project[]>([]);
  selectedProject = signal<Project | null>(null);
  projectTasks = signal<Task[]>([]);
  isLoading = signal<boolean>(false);
  isTasksLoading = signal<boolean>(false);
  isUpdating = signal<boolean>(false);
  errorMessage = signal<string | null>(null);
  successMessage = signal<string | null>(null);
  
  // Modal para crear proyecto (legacy - será reemplazado por Angular Material)
  showCreateModal = signal<boolean>(false);
  newProjectName = '';
  newProjectDescription = '';
  isCreating = signal<boolean>(false);

  constructor(
    private portfolioService: PortfolioService,
    private taskService: TaskService,
    private dialog: MatDialog,
    private snackBar: MatSnackBar
  ) { }

  ngOnInit() {
    this.loadProjects();
  }

  loadProjects(): void {
    this.isLoading.set(true);
    this.errorMessage.set(null);
    
    this.portfolioService.getProjects().subscribe({
      next: (projects) => {
        const projectList = projects ?? [];
        this.projects.set(projectList);
        const current = this.selectedProject();
        const selected = current && projectList.some(p => p.projectId === current.projectId)
          ? current
          : (projectList[0] ?? null);
        this.selectedProject.set(selected);
        if (selected) {
          this.loadTasks(selected.projectId);
        } else {
          this.projectTasks.set([]);
        }
        this.isLoading.set(false);
      },
      error: (error) => {
        console.error('Error al cargar proyectos:', error);
        this.errorMessage.set('No se pudieron cargar los proyectos. Asegúrate de que la API está ejecutándose.');
        this.isLoading.set(false);
      }
    });
  }

  selectProject(project: Project): void {
    this.selectedProject.set(project);
    this.successMessage.set(null);
    this.loadTasks(project.projectId);
  }

  // ========== PROYECTOS ==========

  openCreateProjectDialog(): void {
    const dialogRef = this.dialog.open(EditProjectDialogComponent, {
      width: '400px',
      data: { name: '', description: '' } as ProjectDialogData
    });
    dialogRef.afterClosed().subscribe({
      next: (result) => {
        if (result) {
          this.portfolioService.createProject(result.name, result.description).subscribe({
            next: () => {
              this.snackBar.open('Proyecto creado', 'Cerrar', { duration: 2000 });
              this.loadProjects();
            },
            error: () => this.snackBar.open('Error al crear proyecto', 'Cerrar', { duration: 3000 })
          });
        }
      }
    });
  }

  openEditProjectDialog(project: Project): void {
    const dialogRef = this.dialog.open(EditProjectDialogComponent, {
      width: '400px',
      data: { name: project.name, description: project.description } as ProjectDialogData
    });
    dialogRef.afterClosed().subscribe({
      next: (result) => {
        if (result) {
          const updated = { ...project, name: result.name, description: result.description };
          this.portfolioService.updateProject(updated).subscribe({
            next: () => {
              this.snackBar.open('Proyecto actualizado', 'Cerrar', { duration: 2000 });
              this.loadProjects();
            },
            error: () => this.snackBar.open('Error al actualizar proyecto', 'Cerrar', { duration: 3000 })
          });
        }
      }
    });
  }

  confirmDeleteProject(project: Project): void {
    const dialogRef = this.dialog.open(ConfirmDialog, {
      width: '320px',
      data: { message: `¿Eliminar el proyecto "${project.name}"?` }
    });
    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.portfolioService.deleteProject(project.projectId).subscribe({
          next: () => {
            this.snackBar.open('Proyecto eliminado', 'Cerrar', { duration: 2000 });
            this.loadProjects();
          },
          error: () => this.snackBar.open('Error al eliminar proyecto', 'Cerrar', { duration: 3000 })
        });
      }
    });
  }

  // ========== TAREAS ==========

  openCreateTaskDialog(project: Project): void {
    const dialogRef = this.dialog.open(EditTaskDialogComponent, {
      width: '400px',
      data: {
        title: '',
        description: '',
        status: 'Pending'
      } as TaskDialogData
    });
    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        const request: CreateTaskRequest = {
          projectId: project.projectId,
          title: result.title,
          description: result.description || null,
          status: result.status as TaskStatus
        };
        this.taskService.createTask(project.projectId, request).subscribe({
          next: () => {
            this.snackBar.open('Tarea creada', 'Cerrar', { duration: 2000 });
            this.loadTasks(project.projectId);
          },
          error: () => this.snackBar.open('Error al crear tarea', 'Cerrar', { duration: 3000 })
        });
      }
    });
  }

  openEditTaskDialog(task: Task): void {
    const dialogRef = this.dialog.open(EditTaskDialogComponent, {
      width: '400px',
      data: {
        title: task.title,
        description: task.description,
        status: task.status
      } as TaskDialogData
    });
    dialogRef.afterClosed().subscribe({
      next: (result) => {
        if (result) {
          const request: UpdateTaskRequest = {
            title: result.title,
            description: result.description || null,
            status: result.status as TaskStatus
          };
          this.taskService.updateTask(task.id, request).subscribe({
            next: () => {
              this.snackBar.open('Tarea actualizada', 'Cerrar', { duration: 2000 });
              const projectId = this.selectedProject()?.projectId;
              if (projectId) this.loadTasks(projectId);
            },
            error: () => this.snackBar.open('Error al actualizar tarea', 'Cerrar', { duration: 3000 })
          });
        }
      }
    });
  }

  confirmDeleteTask(task: Task): void {
    const dialogRef = this.dialog.open(ConfirmDialog, {
      width: '320px',
      data: { message: `¿Eliminar la tarea "${task.title}"?` }
    });
    dialogRef.afterClosed().subscribe({
      next: (result) => {
        if (result) {
          this.taskService.deleteTask(task.id).subscribe({
            next: () => {
              this.snackBar.open('Tarea eliminada', 'Cerrar', { duration: 2000 });
              const projectId = this.selectedProject()?.projectId;
              if (projectId) this.loadTasks(projectId);
            },
            error: () => this.snackBar.open('Error al eliminar tarea', 'Cerrar', { duration: 3000 })
          });
        }
      }
    });
  }

  // ========== LEGACY (para compatibilidad) ==========

  openCreateModal(): void {
    this.newProjectName = '';
    this.newProjectDescription = '';
    this.showCreateModal.set(true);
  }

  closeCreateModal(): void {
    this.showCreateModal.set(false);
  }

  createProject(): void {
    if (!this.newProjectName.trim()) {
      this.errorMessage.set('El nombre del proyecto es obligatorio');
      return;
    }

    this.isCreating.set(true);
    this.portfolioService.createProject(this.newProjectName.trim(), this.newProjectDescription.trim() || null)
      .subscribe({
        next: (result: any) => {
          if (result?.projectId) {
            this.showSuccess('Proyecto creado exitosamente');
            this.closeCreateModal();
            this.loadProjects();
          } else {
            this.errorMessage.set('Error al crear el proyecto');
          }
          this.isCreating.set(false);
        },
        error: (error) => {
          console.error('Error al crear proyecto:', error);
          this.errorMessage.set('Error al crear el proyecto. Inténtalo de nuevo.');
          this.isCreating.set(false);
        }
      });
  }

  onTaskStatusChanged(event: { taskId: string; newStatus: TaskStatus }): void {
    this.isUpdating.set(true);
    this.successMessage.set(null);
    const task = this.projectTasks().find(t => t.id === event.taskId);
    if (!task) {
      this.isUpdating.set(false);
      return;
    }
    const request: UpdateTaskRequest = {
      title: task.title,
      description: task.description,
      status: event.newStatus
    };
    this.taskService.updateTask(task.id, request).subscribe({
      next: () => {
        this.showSuccess(`Tarea movida a ${this.getStatusLabel(event.newStatus)}`);
        const projectId = this.selectedProject()?.projectId;
        if (projectId) this.loadTasks(projectId);
        this.isUpdating.set(false);
      },
      error: (error) => {
        console.error('Error al actualizar tarea:', error);
        this.errorMessage.set('Error al actualizar la tarea. Inténtalo de nuevo.');
        this.isUpdating.set(false);
        const projectId = this.selectedProject()?.projectId;
        if (projectId) this.loadTasks(projectId);
      }
    });
  }

  // ========== HELPERS ==========

  getTotalTasks(): number {
    return this.projectTasks().length;
  }

  getCompletedTasks(): number {
    return this.projectTasks().filter(t => t.status === 'Completed').length;
  }

  getInProgressTasks(): number {
    return this.projectTasks().filter(t => t.status === 'InProgress').length;
  }

  getProjectProgress(project: Project): number {
    if (!project.tasks || project.tasks.length === 0) return 0;
    const completed = project.tasks.filter(t => t.status === 'Done' || t.status === 'Completed').length;
    return Math.round((completed / project.tasks.length) * 100);
  }

  private showSuccess(message: string): void {
    this.successMessage.set(message);
    setTimeout(() => this.successMessage.set(null), 3000);
  }

  private getStatusLabel(status: TaskStatus): string {
    switch (status) {
      case 'Pending': return 'Pendiente';
      case 'InProgress': return 'En Progreso';
      case 'Completed': return 'Finalizada';
      default: return status;
    }
  }

  private loadTasks(projectId: string): void {
    this.isTasksLoading.set(true);
    this.taskService.getTasks(projectId).subscribe({
      next: (tasks) => {
        this.projectTasks.set(tasks ?? []);
        this.isTasksLoading.set(false);
      },
      error: (error) => {
        console.error('Error al cargar tareas:', error);
        this.errorMessage.set('No se pudieron cargar las tareas del proyecto.');
        this.projectTasks.set([]);
        this.isTasksLoading.set(false);
      }
    });
  }
}

// Diálogo de confirmación simple
import { Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { Component as NgComponent } from '@angular/core';

@NgComponent({
  selector: 'confirm-dialog',
  template: `
    <h2 mat-dialog-title>Confirmar</h2>
    <mat-dialog-content>
      <p>{{ data.message }}</p>
    </mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-button (click)="dialogRef.close(false)">Cancelar</button>
      <button mat-flat-button color="warn" (click)="dialogRef.close(true)">Eliminar</button>
    </mat-dialog-actions>
  `
})
export class ConfirmDialog {
  constructor(
    public dialogRef: MatDialogRef<ConfirmDialog>,
    @Inject(MAT_DIALOG_DATA) public data: { message: string }
  ) {}
}


