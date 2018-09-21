import { Injectable } from '@angular/core';
import {
    HttpRequest,
    HttpHandler,
    HttpEvent,
    HttpInterceptor
} from '@angular/common/http';

// import { OidcSecurityService } from 'angular-auth-oidc-client';
import { Observable } from 'rxjs/Observable';

@Injectable()
export class TokenInterceptor {
    /**
     * Creates an instance of TokenInterceptor.
     * @param {OidcSecurityService} auth
     * @memberof TokenInterceptor
     */
    constructor() { }
    // constructor(public auth: OidcSecurityService) { }

    /**
     * Intercept all HTTP request to add JWT token to Headers
     * @param {HttpRequest<any>} request
     * @param {HttpHandler} next
     * @returns {Observable<HttpEvent<any>>}
     * @memberof TokenInterceptor
     */
    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {

        // request = request.clone({
        //     setHeaders: {
        //         Authorization: `Bearer ${this.auth.getToken()}`
        //     }
        // });

        request = request.clone({
            setHeaders: {
                Authorization: `Bearer ${localStorage.getItem('auth_token')}`
            }
        });

        return next.handle(request);
    }
}
