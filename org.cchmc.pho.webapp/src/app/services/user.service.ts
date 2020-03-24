import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpResponse } from '@angular/common/http';
import { User, CurrentUser } from '../models/user';
import { environment } from 'src/environments/environment';
import { Subscription, Observable, of } from 'rxjs';
import { AuthenticationService } from './authentication.service';
import { map, catchError } from 'rxjs/operators';
import { NGXLogger } from 'ngx-logger';

const httpOptions = {
  headers: new HttpHeaders({
    'Content-Type': 'application/json'
  })
};

@Injectable({ providedIn: 'root' })
export class UserService {
    currentUser: CurrentUser;
    constructor(private http: HttpClient, private authenticationService: AuthenticationService, private logger: NGXLogger) { }

    getCurrentUser(): Observable<any> {
        let currentUserId = this.authenticationService.getCurrentStaffId();
        return this.http.get<any>(`${environment.apiURL}/api/Users/${currentUserId}`).pipe(
            map((data: CurrentUser[]) => {
              this.logger.log('curent user', data);
              return data;
            })
          );
        }
        // getStaffAdminVerbiage(): Observable<any> {
        //   return this.http.get(`${API_URL}/api/Users/verbiage/`, {responseType: 'text'}).pipe(
        //     map((res) => {
        //       return res;
        //     })
        //   );
        // }

        // getUserStaff(id): Observable<any> {
        //   return this.http.get(`${environment.apiURL}/api/Users/${id}`, {responseType: 'text'}).pipe(
        //       map((res) => {
        //           this.logger.log('Get Staff User Data', res);
        //           return res;
        //       })
        //     );
        //   }

          getUserStaff(id): Observable<HttpResponse<string>> {
            return this.http.get<HttpResponse<string>>(`${environment.apiURL}/api/Users/${id}`).pipe(
                map((res) => {
                    this.logger.log('Get Staff User Data', res);
                    return res;
                })
              );
            }
          /* Creates a new user if one does not already exist*/
          createStaffUser(user): Observable<any> {
            return this.http.post<any>(`${environment.apiURL}/api/Users/`, JSON.stringify(user), httpOptions).pipe(
              catchError(this.handleError<any>('UserCreationError'))
            );
          }

          private handleError<T>(operation = 'operation', result?: T) {
            return (error: any): Observable<T> => {
        
              this.logger.error(error); // log to console instead
        
              this.logger.log(`${operation} failed: ${error}`);
        
              return of(result as T);
            };
          }
} 