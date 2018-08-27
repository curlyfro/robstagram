import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { WebcamModule } from 'ngx-webcam';

import { SharedModule } from '../../shared/modules/shared.module';

import { routing } from './robstagram.routing';
import { RootComponent } from './components/root/root.component';
import { HomeComponent } from './components/home/home.component';
import { CameraComponent } from './components/camera/camera.component';

import { RobstagramService } from './services/robstagram.service';

import { AuthGuard } from '../../auth.guard';


@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    routing,
    SharedModule,
    WebcamModule
  ],
  declarations: [RootComponent, HomeComponent, CameraComponent],
  exports: [],
  providers: [AuthGuard, RobstagramService]
})
export class RobstagramModule { }
