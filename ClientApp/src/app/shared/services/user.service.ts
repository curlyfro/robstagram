import { Injectable } from '@angular/core';
import { JwtHelperService } from '@auth0/angular-jwt';
import { BaseService } from './base.service';
import { Observable, BehaviorSubject } from 'rxjs';
import { AuthService, RegistrationViewModel, AccountsService,
   CredentialsViewModel,
   ProfileData} from '../../api/api.service.generated';

@Injectable()
export class UserService extends BaseService {
  private _authStatusSource = new BehaviorSubject<boolean>(false);
  public authStatus$ = this._authStatusSource.asObservable();
  private _userProfileSource = new BehaviorSubject<ProfileData>(undefined);
  public userProfile$ = this._userProfileSource.asObservable();
  private _loggedIn = false;

  constructor(private authService: AuthService, private accountService: AccountsService) {
    super();
    this.validateAuthState();
    if (this._loggedIn) {
      this.updateUserProfile();
    }
  }

  private validateAuthState() {
    // check if token already exits in local storage
    if (!!localStorage.getItem('auth_token')) {
      // if token exists validate its expiration date
      const helper = new JwtHelperService();
      const token = localStorage.getItem('auth_token');
      if (token !== 'undefined') {
        this._loggedIn = !helper.isTokenExpired(token);
      } else {
        // undefined token needs to be removed
        localStorage.removeItem('auth_token');
        this._loggedIn = false;
      }
    } else {
      // no token no authorization
      this._loggedIn = false;
    }
    this._authStatusSource.next(this._loggedIn);
  }

  private updateUserProfile() {
    this.accountService.getProfile().subscribe(
      (profile: ProfileData) => {
        this._userProfileSource.next(profile);
      },
      error => {
        console.log(error);
        this._userProfileSource.next(undefined);
      });  
  }

  public register(credentials: RegistrationViewModel): Observable<string> {
    return this.accountService.register(credentials);
  }

  public login(credentials: CredentialsViewModel) {
    return this.authService.login(credentials)
      .map(res => JSON.parse(res))
      .map(res => {
        localStorage.setItem('auth_token', res.auth_token);
        this._loggedIn = true;
        this._authStatusSource.next(this._loggedIn);
        this.updateUserProfile();
        return true;
      });
  }

  public logout() {
    localStorage.removeItem('auth_token');
    this._loggedIn = false;
    this._authStatusSource.next(this._loggedIn);
    this._userProfileSource.next(undefined);
  }

  public isLoggedIn(): boolean {
    this.validateAuthState();
    return this._loggedIn;
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
