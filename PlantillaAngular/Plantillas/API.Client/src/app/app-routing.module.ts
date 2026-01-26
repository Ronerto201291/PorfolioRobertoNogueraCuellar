import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LayoutComponent } from './layout/layout.component';

const routes: Routes = [
  {
    path: '',
    component: LayoutComponent,
    children: [
      {
        path: '',
        redirectTo: 'projects',
        pathMatch: 'full'
      },
      {
        path: 'projects',
        loadChildren: () => import('./aplicacion/private.module').then(m => m.PrivateModule),
      },
      {
        path: 'docs',
        loadChildren: () => import('./pages/docs/docs.module').then(m => m.DocsModule),
      },
      {
        path: 'architecture',
        loadChildren: () => import('./pages/architecture/architecture.module').then(m => m.ArchitectureModule),
      },
      {
        path: 'about',
        loadChildren: () => import('./pages/about/about.module').then(m => m.AboutModule),
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
