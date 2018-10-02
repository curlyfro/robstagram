import { Injectable } from '@angular/core';
import { JwtHelperService } from '@auth0/angular-jwt';
import { BaseService } from './base.service';
import { Observable, BehaviorSubject } from 'rxjs';
import { AuthService, RegistrationViewModel, AccountsService,
   CredentialsViewModel,
   ProfileData} from '../../api/api.service.generated';

@Injectable()
export class UserService extends BaseService {
  // Observable navItem source
  private _authNavStatusSource = new BehaviorSubject<boolean>(false);
  // Observable navItem stream
  authNavStatus$ = this._authNavStatusSource.asObservable();

  private loggedIn = false;

  constructor(private authService: AuthService, private accountService: AccountsService) {
    super();

    if (!!localStorage.getItem('auth_token')) {
      const helper = new JwtHelperService();
      const token = localStorage.getItem('auth_token');
      if (token !== 'undefined') {
        this.loggedIn = !helper.isTokenExpired(token);
      }
    } else {
      this.loggedIn = false;
    }

    // NOTE the existence of the token alone does not verify that it is still valid which leads to bugs

    // ?? not sure if this the best way to broadcast the status but seems to resolve issue on page refresh where auth status is lost in
    // header component resulting in authed user nav links disappearing despite the fact user is still logged in
    this._authNavStatusSource.next(this.loggedIn);
  }

  register(credentials: RegistrationViewModel): Observable<string> {
    return this.accountService.register(credentials);
  }

  login(credentials: CredentialsViewModel) {
    return this.authService.login(credentials)
      .map(res => JSON.parse(res))
      .map(res => {
        localStorage.setItem('auth_token', res.auth_token);
        this.loggedIn = true;
        this._authNavStatusSource.next(true);
        return true;
      });
  }

  logout() {
    localStorage.removeItem('auth_token');
    this.loggedIn = false;
    this._authNavStatusSource.next(false);
  }

  isLoggedIn() {
    return this.loggedIn;
  }

  getProfile(): Observable<ProfileData> {
    return this.accountService.getProfile();
  }

  // facebookLogin(accessToken: string) {
  //   let headers = new HttpHeaders();
  //   headers = headers.append('Content-Type', 'application/json');
  //   let body = JSON.stringify({ accessToken });
  //   return this.http
  //     .post(
  //       this.baseUrl + '/externalauth/facebook', body, { headers: headers })
  //     .map(res => JSON.parse(JSON.stringify(res)))
  //     .map(res => {
  //       localStorage.setItem('auth_token', res.auth_token);
  //       this.loggedIn = true;
  //       this._authNavStatusSource.next(true);
  //       return true;
  //     })
  //     .catch(this.handleError);
  // }
}
