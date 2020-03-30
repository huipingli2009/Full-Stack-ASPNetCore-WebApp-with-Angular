import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { BehaviorSubject } from 'rxjs';
import { environment } from 'src/environments/environment';
import { UserAuthenticate } from '../models/user';
import { NGXLogger } from 'ngx-logger';
import { Router } from '@angular/router';


@Injectable({ providedIn: 'root' })
export class AuthenticationService {

    headers = {
        headers: new HttpHeaders({
            'Content-Type': 'application/json'
        })
    }
    private loggedIn: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
    loginErrorMsg: BehaviorSubject<string> =  new BehaviorSubject<string>('');

    constructor(private http: HttpClient, private logger: NGXLogger, public router: Router) { }

    login(user: UserAuthenticate) {
        this.logger.log(JSON.stringify(user), 'IN AUTH SERVICE')
        return this.http.post<any>(`${environment.apiURL}/api/Users/authenticate/`, JSON.stringify(user), this.headers)
            .subscribe((res: any) => {
                localStorage.setItem('access_token', res.user.token);
                localStorage.setItem('staffId', res.user.staffId);
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
        return localStorage.getItem('access_token');
    }

    get isUserLoggedIn(): boolean {
        let authToken = localStorage.getItem('access_token'); 
        return (authToken !== null) ? true : false;
    }

    getCurrentStaffId() {
        return localStorage.getItem('staffId');
    }

    logout() {
        let removeToken = localStorage.removeItem('access_token');
        if (removeToken == null) {
            this.router.navigate(['/login']);
        }
    }
}