import { Component, inject, signal } from '@angular/core';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { TaskStatus } from '../../../models/task.models';

export interface TaskDialogData {
  title: string;
  description: string | null;
  status: TaskStatus;
}

@Component({
  selector: 'app-edit-task-dialog',
  standalone: true,
  imports: [CommonModule, FormsModule, MatDialogModule, MatFormFieldModule, MatInputModule, MatSelectModule, MatButtonModule, MatIconModule],
  template: `
    <h1 mat-dialog-title>{{ data.title ? 'Editar Tarea' : 'Nueva Tarea' }}</h1>
    <div mat-dialog-content class="dialog-content">
      <mat-form-field appearance="fill" style="width:100%">
        <mat-label>Título</mat-label>
        <input matInput name="title" [(ngModel)]="data.title" [ngModelOptions]="{ standalone: true }" maxlength="200" required cdkFocusInitial>
      </mat-form-field>

      <mat-form-field appearance="fill" style="width:100%">
        <mat-label>Descripción</mat-label>
        <textarea matInput name="description" rows="4" [(ngModel)]="data.description" [ngModelOptions]="{ standalone: true }" maxlength="1000"></textarea>
      </mat-form-field>

      <mat-form-field appearance="fill" style="width:100%">
        <mat-label>Estado</mat-label>
        <mat-select name="status" [(ngModel)]="data.status" [ngModelOptions]="{ standalone: true }">
          <mat-option value="Pending">Pendiente</mat-option>
          <mat-option value="InProgress">En Progreso</mat-option>
          <mat-option value="Completed">Finalizada</mat-option>
        </mat-select>
      </mat-form-field>
    </div>

    <div mat-dialog-actions align="end">
      <button mat-button (click)="cancel()">Cancelar</button>
      <button mat-flat-button color="primary" [disabled]="!data.title?.trim() || isSubmitting()" (click)="save()">
        {{ isSubmitting() ? 'Guardando...' : 'Guardar' }}
      </button>
    </div>
  `,
  styles: [
    `:host { display:block }
     .dialog-content { max-height: 60vh; overflow: auto; padding-right: 4px }
     mat-form-field { display: block; }
    `]
})
export class EditTaskDialogComponent {
  public data: TaskDialogData = inject(MAT_DIALOG_DATA);
  public dialogRef = inject(MatDialogRef) as MatDialogRef<EditTaskDialogComponent>;
  private _isSubmitting = signal(false);
  isSubmitting = this._isSubmitting;

  constructor() {}

  cancel(): void {
    this.dialogRef.close();
  }

  save(): void {
    if (!(this.data.title || '').trim()) return;
    this._isSubmitting.set(true);
    this.dialogRef.close(this.data);
    setTimeout(() => this._isSubmitting.set(false), 500);
  }
}
