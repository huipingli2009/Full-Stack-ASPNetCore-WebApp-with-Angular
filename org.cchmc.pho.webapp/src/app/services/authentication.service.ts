import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { NGXLogger } from 'ngx-logger';
import { BehaviorSubject } from 'rxjs';
import { environment } from 'src/environments/environment';
import { UserAuthenticate } from '../models/user';


@Injectable({ providedIn: 'root' })
export class AuthenticationService {

    headers = {
        headers: new HttpHeaders({
            'Content-Type': 'application/json'
        })
    }
    private loggedIn: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
    loginErrorMsg: BehaviorSubject<string> = new BehaviorSubject<string>('');

    constructor(private http: HttpClient, private logger: NGXLogger, public router: Router) { }

    login(user: UserAuthenticate) {
        this.logger.log(JSON.stringify(user), 'IN AUTH SERVICE')
        return this.http.post<any>(`${environment.apiURL}/api/Users/authenticate/`, JSON.stringify(user), this.headers)
            .subscribe((res: any) => {
                sessionStorage.setItem('access_token', res.user.token);
                sessionStorage.setItem('staffId', res.user.staffId);
                this.logger.log('RESPONSE', res);
                if (res.user.token !== null) {
                    this.router.navigate(['/dashboard']);
                    this.loggedIn.next(true);
                }
            }, error => {
                this.loginErrorMsg.next(error);
            })
    }

    getToken() {
        return sessionStorage.getItem('access_token');
    }

    get isUserLoggedIn(): boolean {
        let authToken = sessionStorage.getItem('access_token'); 
        return (authToken !== null) ? true : false;
    }

    getCurrentStaffId() {
        return sessionStorage.getItem('staffId');
    }

    logout() {
        let removeToken = sessionStorage.removeItem('access_token');
        sessionStorage.removeItem('staffId');
        if (removeToken == null) {
            this.router.navigate(['/login']);
        }
    }
}