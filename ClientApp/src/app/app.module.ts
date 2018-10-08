import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpModule, XHRBackend } from '@angular/http';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { AuthenticateXHRBackend } from './authenticate-xhr.backend';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { routing } from './app.routing';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './components/nav-menu/nav-menu.component';
import { HomeComponent } from './components/home/home.component';
import { UploadComponent } from './components/upload/upload.component';

import { AccountModule } from './modules/account/account.module';
import { RobstagramModule } from './modules/robstagram/robstagram.module';
import { TokenInterceptor } from './interceptors/token.interceptor';
import { UploadService } from './shared/services/upload.service';
import { ProfileComponent } from './modules/account/components/profile/profile.component';

import { ToastrModule } from 'ngx-toastr';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    UploadComponent,
    ProfileComponent
  ],
  imports: [
    AccountModule,
    RobstagramModule,
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpModule,
    HttpClientModule,
    FormsModule,
    BrowserAnimationsModule,
    ToastrModule.forRoot({
      timeOut: 3000,
      positionClass: 'toast-bottom-center',
      preventDuplicates: true
    }),
    routing,
  ],
  providers: [
    UploadService,
    { provide: XHRBackend, useClass: AuthenticateXHRBackend },
    { provide: HTTP_INTERCEPTORS, useClass: TokenInterceptor, multi: true }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
