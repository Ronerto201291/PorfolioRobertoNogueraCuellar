import { Component, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

export interface TaskDialogData {
  title: string;
  description: string;
  status: string;
  priority: string;
}

@Component({
  selector: 'app-edit-task-dialog',
  template: `
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
  constructor(
    @Inject(MAT_DIALOG_DATA) public data: TaskDialogData,
    public dialogRef: MatDialogRef<EditTaskDialogComponent>
  ) {}
}
