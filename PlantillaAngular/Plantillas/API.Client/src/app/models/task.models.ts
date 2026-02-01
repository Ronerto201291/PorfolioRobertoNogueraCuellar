export type TaskStatus = 'Pending' | 'InProgress' | 'Completed';

export interface Task {
  id: string;
  projectId: string;
  title: string;
  description: string | null;
  status: TaskStatus;
  createdAt: string;
}

export interface CreateTaskRequest {
  projectId: string;
  title: string;
  description?: string | null;
  status: TaskStatus;
}

export interface UpdateTaskRequest {
  title: string;
  description?: string | null;
  status: TaskStatus;
}
