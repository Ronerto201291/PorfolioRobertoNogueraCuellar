import { Component, inject, signal } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

export interface TaskDialogData {
  title: string;
  description: string;
  status: string;
  priority: string;
}

@Component({
  selector: 'app-edit-task-dialog',
  standalone: true,
  imports: [CommonModule, FormsModule, MatFormFieldModule, MatInputModule, MatSelectModule, MatButtonModule, MatIconModule],
  template: `
    <h2 class="dialog-title">{{ data.title ? 'Editar Tarea' : 'Nueva Tarea' }}</h2>
    <div class="dialog-body">
      <mat-form-field appearance="outline" class="full">
        <mat-label>Título</mat-label>
        <input matInput [(ngModel)]="data.title" maxlength="200" required />
      </mat-form-field>

      <mat-form-field appearance="outline" class="full">
        <mat-label>Descripción</mat-label>
        <textarea matInput rows="4" [(ngModel)]="data.description" maxlength="1000"></textarea>
      </mat-form-field>

      <div class="row">
        <mat-form-field appearance="outline" class="half">
          <mat-label>Estado</mat-label>
          <mat-select [(ngModel)]="data.status">
            <mat-option value="Pending">Pendiente</mat-option>
            <mat-option value="InProgress">En Progreso</mat-option>
            <mat-option value="Completed">Completada</mat-option>
          </mat-select>
        </mat-form-field>

        <mat-form-field appearance="outline" class="half">
          <mat-label>Prioridad</mat-label>
          <mat-select [(ngModel)]="data.priority">
            <mat-option value="Low">Baja</mat-option>
            <mat-option value="Medium">Media</mat-option>
            <mat-option value="High">Alta</mat-option>
          </mat-select>
        </mat-form-field>
      </div>
    </div>

    <div class="dialog-actions">
      <button mat-stroked-button color="warn" (click)="cancel()">
        <mat-icon>close</mat-icon>
        Cancelar
      </button>
      <button mat-flat-button color="primary" [disabled]="!data.title?.trim() || isSubmitting()" (click)="save()">
        <mat-icon *ngIf="!isSubmitting()">save</mat-icon>
        <mat-icon *ngIf="isSubmitting()" class="rotating">autorenew</mat-icon>
        {{ isSubmitting() ? 'Guardando...' : 'Guardar' }}
      </button>
    </div>
  `,
  styles: [
    `:host { display:block; width:100% }
     .dialog-body { display:flex; flex-direction:column; gap:12px; padding:8px 0 }
     .full { width:100% }
     .row { display:flex; gap:12px }
     .half { flex:1 }
     .dialog-actions { display:flex; justify-content:flex-end; gap:8px; padding-top:12px }
     .rotating { animation:spin 1s linear infinite }
     @keyframes spin { to { transform: rotate(360deg) } }
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
    // Close immediately with data; parent handles persistence
    this.dialogRef.close(this.data);
    // reset submission state in case dialog reopened (safe guard)
    setTimeout(() => this._isSubmitting.set(false), 500);
  }
}
