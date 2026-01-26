/* eslint-disable @typescript-eslint/no-explicit-any */
import { Component, OnInit, signal } from '@angular/core';
import { PortfolioService, Project } from '../../services/portfolio.service';

@Component({
  selector: 'app-main-view',
  templateUrl: './main-view.component.html',
  styleUrls: ['./main-view.component.scss']
})
export class MainViewComponent implements OnInit {

  projects = signal<Project[]>([]);
  selectedProject = signal<Project | null>(null);
  isLoading = signal<boolean>(false);
  isUpdating = signal<boolean>(false);
  errorMessage = signal<string | null>(null);
  successMessage = signal<string | null>(null);
  
  // Modal para crear proyecto
  showCreateModal = signal<boolean>(false);
  newProjectName = '';
  newProjectDescription = '';
  isCreating = signal<boolean>(false);

  constructor(private portfolioService: PortfolioService) { }

  ngOnInit() {
    this.loadProjects();
  }

  loadProjects(): void {
    this.isLoading.set(true);
    this.errorMessage.set(null);
    
    this.portfolioService.getProjects().subscribe({
      next: (projects) => {
        this.projects.set(projects);
        if (projects.length > 0 && !this.selectedProject()) {
          this.selectedProject.set(projects[0]);
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

  // ========== CREAR PROYECTO ==========
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
            this.loadProjects(); // Recargar lista
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

  getStatusLabel(status: string): string {
    switch (status) {
      case 'Pending': return 'Pendiente';
      case 'InProgress': return 'En Progreso';
      case 'Completed': return 'Completada';
      default: return status;
    }
  }

  showSuccess(message: string): void {
    this.successMessage.set(message);
    setTimeout(() => this.successMessage.set(null), 3000);
  }

  getProjectProgress(project: Project): number {
    if (!project.tasks || project.tasks.length === 0) return 0;
    const completed = project.tasks.filter(t => t.status === 'Completed').length;
    return Math.round((completed / project.tasks.length) * 100);
  }

  getTotalTasks(): number {
    return this.projects().reduce((sum, p) => sum + (p.tasks?.length || 0), 0);
  }

  getCompletedTasks(): number {
    return this.projects().reduce((sum, p) => 
      sum + (p.tasks?.filter(t => t.status === 'Completed').length || 0), 0);
  }

  getInProgressTasks(): number {
    return this.projects().reduce((sum, p) => 
      sum + (p.tasks?.filter(t => t.status === 'InProgress').length || 0), 0);
  }
}


