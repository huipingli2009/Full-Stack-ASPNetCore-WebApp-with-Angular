import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { User, CurrentUser } from '../models/user';
import { environment } from 'src/environments/environment';
import { Subscription, Observable } from 'rxjs';
import { AuthenticationService } from './authentication.service';
import { map } from 'rxjs/operators';
import { NGXLogger } from 'ngx-logger';

@Injectable({ providedIn: 'root' })
export class UserService {
    currentUser: CurrentUser;
    constructor(private http: HttpClient, private authenticationService: AuthenticationService, private logger: NGXLogger) { }

    getCurrentUser(): Observable<any> {
        let currentUserId = this.authenticationService.getCurrentUserId();
        return this.http.get<any>(`${environment.apiURL}/api/Users/${currentUserId}`).pipe(
            map((data: CurrentUser[]) => {
                this.logger.log('Get Current User Data', data);
              return data;
            })
          );
        }
} 