/* eslint-disable @typescript-eslint/no-explicit-any */
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';

export interface Project {
  projectId: string;
  name: string;
  description: string | null;
  createdAt: string;
  tasks: PortfolioTask[];
}

export interface PortfolioTask {
  taskId: string;
  projectId: string;
  title: string;
  description: string | null;
  status: string;
  priority: string;
  dueDate: string | null;
  createdAt: string;
}

export interface GraphQLResponse<T> {
  data: T;
  errors?: Array<{ message: string }>;
}

@Injectable({
  providedIn: 'root'
})
export class PortfolioService {
  private graphqlUrl = '/graphql/portfolio';

  constructor(private http: HttpClient) { }

  // ==========================================
  // QUERIES
  // ==========================================

  getProjects(): Observable<Project[]> {
    const query = `
      query {
        projects {
          projectId
          name
          description
          createdAt
          tasks {
            taskId
            title
            status
            priority
            dueDate
            createdAt
          }
        }
      }
    `;

    return this.http.post<GraphQLResponse<{ projects: Project[] }>>(this.graphqlUrl, { query })
      .pipe(map(response => response.data.projects));
  }

  getProjectById(id: string): Observable<Project | null> {
    const query = `
      query GetProject($id: UUID!) {
        projectById(id: $id) {
          projectId
          name
          description
          createdAt
          tasks {
            taskId
            title
            description
            status
            priority
            dueDate
            createdAt
          }
        }
      }
    `;

    return this.http.post<GraphQLResponse<{ projectById: Project | null }>>(this.graphqlUrl, { 
      query, 
      variables: { id } 
    }).pipe(map(response => response.data.projectById));
  }

  getTasks(): Observable<PortfolioTask[]> {
    const query = `
      query {
        tasks {
          taskId
          projectId
          title
          description
          status
          priority
          dueDate
          createdAt
        }
      }
    `;

    return this.http.post<GraphQLResponse<{ tasks: PortfolioTask[] }>>(this.graphqlUrl, { query })
      .pipe(map(response => response.data.tasks));
  }

  getTasksByStatus(status: string): Observable<PortfolioTask[]> {
    const query = `
      query GetTasksByStatus($status: String!) {
        tasksByStatus(status: $status) {
          taskId
          projectId
          title
          description
          status
          priority
          dueDate
          createdAt
        }
      }
    `;

    return this.http.post<GraphQLResponse<{ tasksByStatus: PortfolioTask[] }>>(this.graphqlUrl, { 
      query, 
      variables: { status } 
    }).pipe(map(response => response.data.tasksByStatus));
  }

  // ==========================================
  // MUTATIONS
  // ==========================================

  createProject(name: string, description: string | null): Observable<Project> {
    const mutation = `
      mutation CreateProject($name: String!, $description: String) {
        createProject(name: $name, description: $description) {
          projectId
          name
          description
          createdAt
        }
      }
    `;

    return this.http.post<GraphQLResponse<{ createProject: Project }>>(this.graphqlUrl, { 
      query: mutation, 
      variables: { name, description } 
    }).pipe(map(response => response.data.createProject));
  }

  deleteProject(id: string): Observable<boolean> {
    const mutation = `
      mutation DeleteProject($id: UUID!) {
        deleteProject(id: $id)
      }
    `;

    return this.http.post<GraphQLResponse<{ deleteProject: boolean }>>(this.graphqlUrl, { 
      query: mutation, 
      variables: { id } 
    }).pipe(map(response => response.data.deleteProject));
  }

  createTask(projectId: string, title: string, status: string, priority: string, description: string | null = null): Observable<PortfolioTask> {
    const mutation = `
      mutation CreateTask($projectId: UUID!, $title: String!, $status: String!, $priority: String!, $description: String) {
        createTask(projectId: $projectId, title: $title, status: $status, priority: $priority, description: $description) {
          taskId
          projectId
          title
          description
          status
          priority
          createdAt
        }
      }
    `;

    return this.http.post<GraphQLResponse<{ createTask: PortfolioTask }>>(this.graphqlUrl, { 
      query: mutation, 
      variables: { projectId, title, status, priority, description } 
    }).pipe(map(response => response.data.createTask));
  }

  updateTaskStatus(id: string, status: string): Observable<PortfolioTask | null> {
    const mutation = `
      mutation UpdateTaskStatus($id: UUID!, $status: String!) {
        updateTaskStatus(id: $id, status: $status) {
          taskId
          status
        }
      }
    `;

    return this.http.post<GraphQLResponse<{ updateTaskStatus: PortfolioTask | null }>>(this.graphqlUrl, { 
      query: mutation, 
      variables: { id, status } 
    }).pipe(map(response => response.data.updateTaskStatus));
  }

  deleteTask(id: string): Observable<boolean> {
    const mutation = `
      mutation DeleteTask($id: UUID!) {
        deleteTask(id: $id)
      }
    `;

    return this.http.post<GraphQLResponse<{ deleteTask: boolean }>>(this.graphqlUrl, { 
      query: mutation, 
      variables: { id } 
    }).pipe(map(response => response.data.deleteTask));
  }
}
