import { ModuleWithProviders } from '@angular/core';
import { RouterModule } from '@angular/router';

import { RegistrationFormComponent } from './components/registration-form/registration-form.component';
import { LoginFormComponent } from './components/login-form/login-form.component';
//import { FacebookLoginComponent } from './facebook-login/facebook-login.component';

export const routing: ModuleWithProviders = RouterModule.forChild([
  { path: 'register', component: RegistrationFormComponent },
  { path: 'login', component: LoginFormComponent },
  //{ path: 'facebook-login', component: FacebookLoginComponent }
]);
