import { ModuleWithProviders } from '@angular/core';
import { RouterModule } from '@angular/router';

import { RootComponent } from './components/root/root.component';
import { HomeComponent } from './components/home/home.component';
import { CameraComponent } from './components/camera/camera.component';

import { AuthGuard } from '../../auth.guard';
import { PostComponent } from './components/post/post.component';
import { GalleryComponent } from './components/gallery/gallery.component';
import { ActivityComponent } from './components/activity/activity.component';

export const routing: ModuleWithProviders = RouterModule.forChild([
  {
    path: 'robstagram',
    component: RootComponent, canActivate: [AuthGuard],

    children: [
      { path: '', component: HomeComponent },
      { path: 'home', component: HomeComponent },
      { path: 'camera', component: CameraComponent },
      { path: 'post', component: PostComponent },
      { path: 'gallery', component: GalleryComponent },
      { path: 'activity', component: ActivityComponent },
    ],

    runGuardsAndResolvers: 'always',
  }
]);
