import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest, HttpResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { NGXLogger } from 'ngx-logger';
import { Observable, of, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AuthenticationService } from '../services/authentication.service';


@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
    constructor(private authenticationService: AuthenticationService, private router: Router, private logger: NGXLogger) { }

    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        return next.handle(request).pipe(catchError(err => {
            if (err.status === 401) {
                // auto logout if 401 response returned from api
                this.authenticationService.logout();
                this.router.navigate(['/login']);
            }
            if (err.status === 404) {
                const res = new HttpResponse({
                    body: null,
                    headers: err.headers,
                    status: err.status,
                    statusText: err.statusText,
                    url: err.url
                });
                this.logger.log('404 User Not Found');
                return of(res);
            } else {
                const error = err.error.message || err.error.status;
                return throwError(error);
            }
        }));
    }
}
