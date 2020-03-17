import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpErrorResponse } from '@angular/common/http';
import { BehaviorSubject, Observable, throwError } from 'rxjs';
import { map, tap } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { User, UserAuthenticate } from '../models/user';
import { NGXLogger } from 'ngx-logger';
import { Router } from '@angular/router';


@Injectable({ providedIn: 'root' })
export class AuthenticationService {

    headers = {
        headers: new HttpHeaders({
            'Content-Type': 'application/json'
        })
    }
    currentUser = {};
    private loggedIn: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);

    constructor(private http: HttpClient, private logger: NGXLogger, public router: Router) { }

    login(user: UserAuthenticate) {
        this.logger.log(JSON.stringify(user), 'IN AUTH SERVICE')
        return this.http.post<any>(`${environment.apiURL}/api/Users/authenticate/`, JSON.stringify(user), this.headers)
            .subscribe((res: any) => {
                localStorage.setItem('access_token', res.user.token);
                this.logger.log('Token', res.user.token);
                this.logger.log('RESPONSE', res);
                this.loggedIn.next(!this.loggedIn.value);
                if (res.user.token !== null) {
                    this.router.navigate(['/dashboard']);
                }
            })
    }

    getToken() {
        return localStorage.getItem('access_token');
    }

    get isLoggedIn() {
        this.logger.log('islogged in', this.loggedIn)
        return this.loggedIn.asObservable();
    }

    get isUserLoggedIn(): boolean {
        let authToken = localStorage.getItem('access_token'); 
        return (authToken !== null) ? true : false;
    }

    logout() {
        let removeToken = localStorage.removeItem('access_token');
        if (removeToken == null) {
            this.router.navigate(['/login']);
        }
    }

    handleError(error: HttpErrorResponse) {
        let msg = '';
        if (error.error instanceof ErrorEvent) {
            // client-side error
            msg = error.error.message;
        } else {
            // server-side error
            msg = `Error Code: ${error.status}\nMessage: ${error.message}`;
        }
        return throwError(msg);
    }
}