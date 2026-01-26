import { CommonModule } from '@angular/common';
import { LOCALE_ID, NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule, Routes } from '@angular/router';
import { DragDropModule } from '@angular/cdk/drag-drop';
import { MSAL_GUARD_CONFIG, MsalBroadcastService, MsalGuard, MsalGuardConfiguration, MsalModule, MsalService } from '@azure/msal-angular';
import { InteractionType } from '@azure/msal-browser';
import { TableModule } from 'primeng/table';
import { loginRequest } from '../../environments/auth-config';
import { LoggedinComponent } from './loggedin/loggedin.component';
import { LoginComponent } from './login/login.component';
import { MainViewComponent } from './main-view/main-view.component';
import { ProjectCardComponent } from './components/project-card/project-card.component';
import { TaskCardComponent } from './components/task-card/task-card.component';
import { KanbanBoardComponent } from './components/kanban-board/kanban-board.component';

const privateRoutes: Routes = [
  {
    path: '',
    component: MainViewComponent
  },
  {
    path: 'private',
    component: MainViewComponent
  }
];



/**
 * Set your default interaction type for MSALGuard here. If you have any
 * additional scopes you want the user to consent upon login, add them here as well.
 */
export function MSALGuardConfigFactory(): MsalGuardConfiguration {
  return {
    interactionType: InteractionType.Popup,
    authRequest: loginRequest
  };
}


@NgModule({
  declarations: [
    MainViewComponent,
    LoginComponent,
    LoggedinComponent,
    ProjectCardComponent,
    TaskCardComponent,
    KanbanBoardComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    DragDropModule,
    TableModule,
    MsalModule,
    RouterModule.forChild(privateRoutes)
  ],
  providers: [
      
      
      {
        provide: MSAL_GUARD_CONFIG,
        useFactory: MSALGuardConfigFactory
      },
      
    { provide: MsalService, multi:false },
      MsalGuard,
      MsalBroadcastService,
    { provide: LOCALE_ID, useValue: 'es-ES' }],
 // bootstrap: [MsalRedirectComponent]
})
export class PrivateModule { }


