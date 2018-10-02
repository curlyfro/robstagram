import { ModuleWithProviders } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { HomeComponent } from './components/home/home.component';
import { UploadComponent } from './components/upload/upload.component';

const appRoutes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'upload', component: UploadComponent },
];

export const routing: ModuleWithProviders = RouterModule.forRoot(appRoutes, {onSameUrlNavigation: 'reload'});
