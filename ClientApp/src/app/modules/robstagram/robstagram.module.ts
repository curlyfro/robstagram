import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { WebcamModule } from 'ngx-webcam';
import { InfiniteScrollModule } from 'ngx-infinite-scroll';

import { SharedModule } from '../../shared/modules/shared.module';

import { routing } from './robstagram.routing';
import { RootComponent } from './components/root/root.component';
import { HomeComponent } from './components/home/home.component';
import { CameraComponent } from './components/camera/camera.component';

import { AuthGuard } from '../../auth.guard';
import { NavBottomComponent } from './components/nav-bottom/nav-bottom.component';
import { TimeAgoPipe } from 'time-ago-pipe';

import { IconModule } from '../../shared/modules/icon.module';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { PostComponent } from './components/post/post.component';
import { RobstagramService } from '../../api/api.service.generated';
import { GalleryComponent } from './components/gallery/gallery.component';
import { ActivityComponent } from './components/activity/activity.component';
import { PrettierNameArrPipe } from './pipes/prettier-name-arr.pipe';
import { PrettierNameArrDirective } from './directives/prettier-name-arr.directive';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        routing,
        SharedModule,
        WebcamModule,
        IconModule,
        FontAwesomeModule,
        InfiniteScrollModule
    ],
    declarations: [RootComponent, HomeComponent, CameraComponent,
        NavBottomComponent, TimeAgoPipe, PostComponent, GalleryComponent,
        ActivityComponent, PrettierNameArrPipe, PrettierNameArrDirective],
    exports: [],
    providers: [AuthGuard, RobstagramService]
})
export class RobstagramModule { }
