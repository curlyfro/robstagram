import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { WebcamModule } from 'ngx-webcam';

import { SharedModule } from '../../shared/modules/shared.module';

import { routing } from './robstagram.routing';
import { RootComponent } from './components/root/root.component';
import { HomeComponent } from './components/home/home.component';
import { CameraComponent } from './components/camera/camera.component';
import { EntryComponent } from './components/entry/entry.component';

import { RobstagramService } from './services/robstagram.service';

import { AuthGuard } from '../../auth.guard';
import { NavBottomComponent } from './components/nav-bottom/nav-bottom.component';
import { TimeAgoPipe } from 'time-ago-pipe';

import { IconModule } from '../../shared/modules/icon.module';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { PostComponent } from './components/post/post.component';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    routing,
    SharedModule,
    WebcamModule,
    IconModule,
    FontAwesomeModule,
  ],
  declarations: [RootComponent, HomeComponent, CameraComponent, EntryComponent, NavBottomComponent, TimeAgoPipe, PostComponent],
  exports: [],
  providers: [AuthGuard, RobstagramService]
})
export class RobstagramModule { }
