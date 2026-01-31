import { Component, inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';

export interface TaskDialogData {
  title: string;
  description: string;
  status: string;
  priority: string;
}

@Component({
  selector: 'app-edit-task-dialog',
  standalone: true,
  imports: [CommonModule, FormsModule, MatFormFieldModule, MatInputModule, MatSelectModule, MatButtonModule],
  template: `
  standalone: true,
  imports: [CommonModule, FormsModule, MatFormFieldModule, MatInputModule, MatSelectModule, MatButtonModule],
    <h1 mat-dialog-title>{{ data.title ? 'Editar Tarea' : 'Nueva Tarea' }}</h1>
    <div mat-dialog-content>
      <mat-form-field appearance="fill" style="width:100%">
        <mat-label>Título</mat-label>
        <input matInput [(ngModel)]="data.title" maxlength="200" required>
      </mat-form-field>
      <mat-form-field appearance="fill" style="width:100%">
        <mat-label>Descripción</mat-label>
        <textarea matInput [(ngModel)]="data.description" maxlength="1000"></textarea>
      </mat-form-field>
      <mat-form-field appearance="fill" style="width:100%">
        <mat-label>Estado</mat-label>
        <mat-select [(ngModel)]="data.status">
          <mat-option value="Pending">Pendiente</mat-option>
          <mat-option value="InProgress">En Progreso</mat-option>
          <mat-option value="Completed">Completada</mat-option>
        </mat-select>
      </mat-form-field>
      <mat-form-field appearance="fill" style="width:100%">
        <mat-label>Prioridad</mat-label>
        <mat-select [(ngModel)]="data.priority">
          <mat-option value="Low">Baja</mat-option>
          <mat-option value="Medium">Media</mat-option>
          <mat-option value="High">Alta</mat-option>
        </mat-select>
      </mat-form-field>
    </div>
    <div mat-dialog-actions align="end">
      <button mat-button (click)="dialogRef.close()">Cancelar</button>
      <button mat-flat-button color="primary" [disabled]="!data.title.trim()" (click)="dialogRef.close(data)">
        Guardar
      </button>
    </div>
  `
})
export class EditTaskDialogComponent {
  public data: TaskDialogData = inject(MAT_DIALOG_DATA);
  public dialogRef = inject(MatDialogRef) as MatDialogRef<EditTaskDialogComponent>;

  constructor() {}
}
