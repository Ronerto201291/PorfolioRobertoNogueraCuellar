import { Component, inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { CommonModule } from '@angular/common';

export interface ProjectDialogData {
  name: string;
  description: string;
}

@Component({
  selector: 'app-edit-project-dialog',
  standalone: true,
  imports: [CommonModule, FormsModule, MatFormFieldModule, MatInputModule, MatButtonModule],
  template: `
    <h1 mat-dialog-title>{{ data.name ? 'Editar Proyecto' : 'Nuevo Proyecto' }}</h1>
    <div mat-dialog-content class="dialog-content">
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
      <button mat-flat-button color="primary" [disabled]="!data.name?.trim()" (click)="dialogRef.close(data)">
        Guardar
      </button>
    </div>
  `
  ,
  styles: [
    `:host { display:block }
     .dialog-content { max-height: 60vh; overflow: auto; padding-right: 4px }
     mat-form-field { display: block; }
    `
  ]
})
export class EditProjectDialogComponent {
  // --- CAMBIO CLAVE AQUÍ ---
  // Usamos inject() en lugar de ponerlo en el constructor
  public data: ProjectDialogData = inject(MAT_DIALOG_DATA);
  public dialogRef = inject(MatDialogRef<EditProjectDialogComponent>);

  constructor() {
    // El constructor queda vacío o puedes borrarlo si no haces nada más
  }
}
