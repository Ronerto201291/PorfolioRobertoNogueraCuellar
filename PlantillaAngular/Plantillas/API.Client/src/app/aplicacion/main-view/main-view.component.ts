/* eslint-disable @typescript-eslint/no-explicit-any */
import { Component, OnInit, signal } from '@angular/core';
import { IonicModule } from '@ionic/angular';
import { PortfolioService, Project, PortfolioTask } from '../../services/portfolio.service';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { EditProjectDialogComponent, ProjectDialogData } from '../components/edit-project-dialog/edit-project-dialog.component';
import { EditTaskDialogComponent, TaskDialogData } from '../components/edit-task-dialog/edit-task-dialog.component';

@Component({
  selector: 'app-main-view',
  templateUrl: './main-view.component.html',
  styleUrls: ['./main-view.component.scss'],
  standalone: true,
  imports: [IonicModule]
})
export class MainViewComponent implements OnInit {

  projects = signal<Project[]>([]);
  selectedProject = signal<Project | null>(null);
  isLoading = signal<boolean>(false);
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
        if (projectList.length > 0 && !this.selectedProject()) {
          this.selectedProject.set(projectList[0]);
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
  }

  // ========== PROYECTOS ==========

  openCreateProjectDialog(): void {
    const dialogRef = this.dialog.open(EditProjectDialogComponent, {
      width: '400px',
      data: { name: '', description: '' } as ProjectDialogData
    });
    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.portfolioService.createProject(result.name, result.description).subscribe({
          next: (project) => {
            this.snackBar.open('Proyecto creado', 'Cerrar', { duration: 2000 });
            this.loadProjects();
          },
          error: () => this.snackBar.open('Error al crear proyecto', 'Cerrar', { duration: 3000 })
        });
      }
    });
  }

  openEditProjectDialog(project: Project): void {
    const dialogRef = this.dialog.open(EditProjectDialogComponent, {
      width: '400px',
      data: { name: project.name, description: project.description } as ProjectDialogData
    });
    dialogRef.afterClosed().subscribe(result => {
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
        status: 'Pending',
        priority: 'Medium'
      } as TaskDialogData
    });
    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.portfolioService.createTask(
          project.projectId,
          result.title,
          result.status,
          result.priority,
          result.description
        ).subscribe({
          next: () => {
            this.snackBar.open('Tarea creada', 'Cerrar', { duration: 2000 });
            this.loadProjects();
          },
          error: () => this.snackBar.open('Error al crear tarea', 'Cerrar', { duration: 3000 })
        });
      }
    });
  }

  openEditTaskDialog(task: PortfolioTask): void {
    const dialogRef = this.dialog.open(EditTaskDialogComponent, {
      width: '400px',
      data: {
        title: task.title,
        description: task.description,
        status: task.status,
        priority: task.priority
      } as TaskDialogData
    });
    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        const updated = { ...task, ...result };
        this.portfolioService.updateTask(updated).subscribe({
          next: () => {
            this.snackBar.open('Tarea actualizada', 'Cerrar', { duration: 2000 });
            this.loadProjects();
          },
          error: () => this.snackBar.open('Error al actualizar tarea', 'Cerrar', { duration: 3000 })
        });
      }
    });
  }

  confirmDeleteTask(task: PortfolioTask): void {
    const dialogRef = this.dialog.open(ConfirmDialog, {
      width: '320px',
      data: { message: `¿Eliminar la tarea "${task.title}"?` }
    });
    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.portfolioService.deleteTask(task.taskId).subscribe({
          next: () => {
            this.snackBar.open('Tarea eliminada', 'Cerrar', { duration: 2000 });
            this.loadProjects();
          },
          error: () => this.snackBar.open('Error al eliminar tarea', 'Cerrar', { duration: 3000 })
        });
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

  onTaskStatusChanged(event: { taskId: string; newStatus: string }): void {
    this.isUpdating.set(true);
    this.successMessage.set(null);
    
    this.portfolioService.updateTaskStatus(event.taskId, event.newStatus).subscribe({
      next: (updatedTask) => {
        if (updatedTask) {
          const project = this.selectedProject();
          if (project) {
            const taskIndex = project.tasks.findIndex(t => t.taskId === event.taskId);
            if (taskIndex !== -1) {
              project.tasks[taskIndex].status = event.newStatus;
              this.selectedProject.set({ ...project });
            }
          }
          this.showSuccess(`Tarea movida a ${this.getStatusLabel(event.newStatus)}`);
        }
        this.isUpdating.set(false);
      },
      error: (error) => {
        console.error('Error al actualizar tarea:', error);
        this.errorMessage.set('Error al actualizar la tarea. Inténtalo de nuevo.');
        this.isUpdating.set(false);
        this.loadProjects();
      }
    });
  }

  // ========== HELPERS ==========

  getTotalTasks(): number {
    return this.projects().reduce((total, project) => total + (project.tasks?.length || 0), 0);
  }

  getCompletedTasks(): number {
    return this.projects().reduce((total, project) => 
      total + (project.tasks?.filter(t => t.status === 'Completed').length || 0), 0);
  }

  getInProgressTasks(): number {
    return this.projects().reduce((total, project) => 
      total + (project.tasks?.filter(t => t.status === 'InProgress').length || 0), 0);
  }

  getProjectProgress(project: Project): number {
    if (!project.tasks || project.tasks.length === 0) return 0;
    const completed = project.tasks.filter(t => t.status === 'Completed').length;
    return Math.round((completed / project.tasks.length) * 100);
  }

  private showSuccess(message: string): void {
    this.successMessage.set(message);
    setTimeout(() => this.successMessage.set(null), 3000);
  }

  private getStatusLabel(status: string): string {
    switch (status) {
      case 'Pending': return 'Pendiente';
      case 'InProgress': return 'En Progreso';
      case 'Completed': return 'Completada';
      default: return status;
    }
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


