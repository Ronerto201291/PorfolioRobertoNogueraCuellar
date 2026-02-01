import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { CreateTaskRequest, Task, TaskStatus, UpdateTaskRequest } from '../models/task.models';

@Injectable({
  providedIn: 'root'
})
export class TaskService {
  private readonly baseUrl = '/api';
  private readonly graphqlUrl = '/graphql/portfolio';

  constructor(private http: HttpClient) {}

  getTasks(projectId: string): Observable<Task[]> {
    const query = `
      query GetTasksByProject($projectId: UUID!) {
        tasksByProjectId(projectId: $projectId) {
          taskId
          projectId
          title
          description
          status
          createdAt
        }
      }
    `;

    return this.http
      .post<GraphQLResponse<{ tasksByProjectId: GraphQLTask[] }>>(this.graphqlUrl, {
        query,
        variables: { projectId }
      })
      .pipe(map(response => (response.data?.tasksByProjectId ?? []).map(task => this.mapGraphTask(task))));
  }

  createTask(projectId: string, request: CreateTaskRequest): Observable<Task> {
    const payload: CreateTaskRequest = { ...request, projectId };
    return this.http
      .post<ApiDataResult<CreateTaskResult>>(`${this.baseUrl}/tasks`, payload)
      .pipe(map(result => this.mapApiTask(result.data)));
  }

  updateTask(taskId: string, request: UpdateTaskRequest): Observable<Task> {
    return this.http
      .put<ApiDataResult<UpdateTaskResult>>(`${this.baseUrl}/tasks/${taskId}`, request)
      .pipe(map(result => this.mapApiTask(result.data)));
  }

  deleteTask(taskId: string): Observable<void> {
    return this.http
      .delete<ApiDataResult<DeleteTaskResult>>(`${this.baseUrl}/tasks/${taskId}`)
      .pipe(map(() => void 0));
  }

  private mapApiTask(task: CreateTaskResult | UpdateTaskResult): Task {
    return {
      id: task.taskId,
      projectId: 'projectId' in task ? task.projectId : '',
      title: task.title,
      description: task.description ?? null,
      status: this.normalizeStatus(task.status),
      createdAt: 'createdAt' in task ? task.createdAt : new Date().toISOString()
    };
  }

  private mapGraphTask(task: GraphQLTask): Task {
    return {
      id: task.taskId,
      projectId: task.projectId,
      title: task.title,
      description: task.description ?? null,
      status: this.normalizeStatus(task.status),
      createdAt: task.createdAt
    };
  }

  private normalizeStatus(status: string): TaskStatus {
    if (status === 'Completed' || status === 'InProgress' || status === 'Pending') {
      return status;
    }
    return 'Pending';
  }
}

interface ApiDataResult<T> {
  data: T;
  Data?: T;
  message?: string;
  Message?: string;
  success: boolean;
  Success?: boolean;
}

interface CreateTaskResult {
  taskId: string;
  projectId: string;
  title: string;
  description: string | null;
  status: string;
  createdAt: string;
}

interface UpdateTaskResult {
  taskId: string;
  title: string;
  description: string | null;
  status: string;
}

interface DeleteTaskResult {
  taskId: string;
  message?: string;
}

interface GraphQLTask {
  taskId: string;
  projectId: string;
  title: string;
  description: string | null;
  status: string;
  createdAt: string;
}

interface GraphQLResponse<T> {
  data: T;
  errors?: Array<{ message: string }>;
}
