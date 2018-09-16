import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { JwtHelperService } from '@auth0/angular-jwt';

import { UserRegistration } from '../models/user.registration.interface';
import { ConfigService } from '../utils/config.service';

import { BaseService } from "./base.service";

import { Observable ,  BehaviorSubject } from 'rxjs/Rx';

// Add the RxJS Observable operators we need in this app.
import '../../rxjs-operators';

@Injectable()

export class UserService extends BaseService {

  baseUrl: string = '';

  // Observable navItem source
  private _authNavStatusSource = new BehaviorSubject<boolean>(false);
  // Observable navItem stream
  authNavStatus$ = this._authNavStatusSource.asObservable();

  private loggedIn = false;

  constructor(private http: HttpClient, private configService: ConfigService) {
    super();

    if (!!localStorage.getItem('auth_token'))
    {
      const helper = new JwtHelperService();
      let token = localStorage.getItem('auth_token');
      this.loggedIn = !helper.isTokenExpired(token);
    }
    else 
    {
      this.loggedIn = false;
    }
    
    // NOTE the existence of the token alone does not verify that it is still valid which leads to bugs

    // ?? not sure if this the best way to broadcast the status but seems to resolve issue on page refresh where auth status is lost in
    // header component resulting in authed user nav links disappearing despite the fact user is still logged in
    this._authNavStatusSource.next(this.loggedIn);
    this.baseUrl = configService.getApiURI();
  }

  register(email: string, password: string, firstName: string, lastName: string, location: string): Observable<UserRegistration> {
    let body = JSON.stringify({ email, password, firstName, lastName, location });
    let headers = new HttpHeaders({ 'Content-Type': 'application/json' });

    return this.http.post<UserRegistration>(this.baseUrl + "/accounts", body, { headers: headers })
      // .map(res => true)
      .catch(this.handleError);
  }

  login(userName, password) {
    let headers = new HttpHeaders();
    headers = headers.append('Content-Type', 'application/json');

    return this.http
      .post(
        this.baseUrl + '/auth/login',
        JSON.stringify({ userName, password }), { headers: headers }
      )
      .map(res => JSON.parse(JSON.stringify(res)))
      .map(res => {
        localStorage.setItem('auth_token', res.auth_token);
        this.loggedIn = true;
        this._authNavStatusSource.next(true);
        return true;
      })
      .catch(this.handleError);
  }

  logout() {
    localStorage.removeItem('auth_token');
    this.loggedIn = false;
    this._authNavStatusSource.next(false);
  }

  isLoggedIn() {
    return this.loggedIn;
  }

  facebookLogin(accessToken: string) {
    let headers = new HttpHeaders();
    headers = headers.append('Content-Type', 'application/json');
    let body = JSON.stringify({ accessToken });
    return this.http
      .post(
        this.baseUrl + '/externalauth/facebook', body, { headers: headers })
      .map(res => JSON.parse(JSON.stringify(res)))
      .map(res => {
        localStorage.setItem('auth_token', res.auth_token);
        this.loggedIn = true;
        this._authNavStatusSource.next(true);
        return true;
      })
      .catch(this.handleError);
  }
}
