import { Component, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

export interface ProjectDialogData {
  name: string;
  description: string;
}

@Component({
  selector: 'app-edit-project-dialog',
  template: `
    <h1 mat-dialog-title>{{ data.name ? 'Editar Proyecto' : 'Nuevo Proyecto' }}</h1>
    <div mat-dialog-content>
      <mat-form-field appearance="fill" style="width:100%">
        <mat-label>Nombre</mat-label>
        <input matInput [(ngModel)]="data.name" maxlength="200" required>
      </mat-form-field>
      <mat-form-field appearance="fill" style="width:100%">
        <mat-label>Descripción</mat-label>
        <textarea matInput [(ngModel)]="data.description" maxlength="1000"></textarea>
      </mat-form-field>
    </div>
    <div mat-dialog-actions align="end">
      <button mat-button (click)="dialogRef.close()">Cancelar</button>
      <button mat-flat-button color="primary" [disabled]="!data.name.trim()" (click)="dialogRef.close(data)">
        Guardar
      </button>
    </div>
  `
})
export class EditProjectDialogComponent {
  constructor(
    @Inject(MAT_DIALOG_DATA) public data: ProjectDialogData,
    public dialogRef: MatDialogRef<EditProjectDialogComponent>
  ) {}
}
