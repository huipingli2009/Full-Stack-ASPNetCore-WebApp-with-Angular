import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor, HttpResponse } from '@angular/common/http';
import { Observable, throwError, of, BehaviorSubject } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AuthenticationService } from '../services/authentication.service';
import { Router } from '@angular/router';


@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
    constructor(private authenticationService: AuthenticationService, private router: Router) { }

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
                console.log('404 User Not Found');
                return of(res);
            } else {
                const error = err.error.message || err.statusText;
                return throwError(error);
            }
        }));
    }
}
