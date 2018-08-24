import { ModuleWithProviders } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { NavMenuComponent } from './components/nav-menu/nav-menu.component';
import { HomeComponent } from './components/home/home.component';
import { CounterComponent } from './components/counter/counter.component';
import { FetchDataComponent } from './components/fetch-data/fetch-data.component';
import { UploadComponent } from './components/upload/upload.component';

const appRoutes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'counter', component: CounterComponent },
  { path: 'fetch-data', component: FetchDataComponent },
  { path: 'upload', component: UploadComponent },
];

export const routing: ModuleWithProviders = RouterModule.forRoot(appRoutes);

//RouterModule.forRoot([
//  { path: '', component: HomeComponent, pathMatch: 'full' },
//  { path: 'counter', component: CounterComponent },
//  { path: 'fetch-data', component: FetchDataComponent },
//  { path: 'upload', component: UploadComponent },
//  //{ path: 'login', component: LoginComponent },
//])
