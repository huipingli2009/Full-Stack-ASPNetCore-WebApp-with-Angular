import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { NGXLogger } from 'ngx-logger';
import { Observable, of } from 'rxjs';
import { catchError, map, tap } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { CurrentUser, UserRoles } from '../models/user';
import { MatSnackBarComponent } from '../shared/mat-snack-bar/mat-snack-bar.component';
import { AuthenticationService } from './authentication.service';

const httpOptions = {
  headers: new HttpHeaders({
    'Content-Type': 'application/json'
  })
};

@Injectable({ providedIn: 'root' })
export class UserService {
  currentUser: CurrentUser;
  responseStatus: number;
  constructor(private http: HttpClient, private authenticationService: AuthenticationService,
    private logger: NGXLogger, private snackBar: MatSnackBarComponent) { }

  getCurrentUser(): Observable<any> {
    let currentUserId = this.authenticationService.getCurrentStaffId();
    return this.http.get<any>(`${environment.apiURL}/api/Users/${currentUserId}`).pipe(
      map((data: CurrentUser[]) => {
        this.logger.log('curent user', data);
        return data;
      })
    );
  }
  /*Checks if a user exists*/
  getUserStaff(id): Observable<any> {
    return this.http.get<any>(`${environment.apiURL}/api/Users/${id}`,
      { observe: 'response' }
    ).pipe(
      map((res) => {
        this.logger.log('Get Staff User Data', res);
        this.responseStatus = res.status;
        return res;
      })
    );
  }
  /* Creates a new user*/
  createStaffUser(user): Observable<any> {
    this.logger.log('STINGIFIED', JSON.stringify(user));
    return this.http.post<any>(`${environment.apiURL}/api/Users/`, JSON.stringify(user), httpOptions).pipe(
      catchError(this.handleError<any>('UserCreationError'))
    );
  }
  /*Gets list of user roles*/
  getUserRoles(): Observable<any> {
    return this.http.get<any>(`${environment.apiURL}/api/Users/roles/`).pipe(
      map((data: UserRoles[]) => {
        return data;
      })
    );
  }
  /*Delete User */
  removeUserFromRegistry(id, isDeleted) {
    return this.http.patch<any>(`${environment.apiURL}/api/Users/${id}/delete/`, isDeleted, httpOptions).pipe(
      tap(_ => this.snackBar.openSnackBar(`User Removed: ${isDeleted}`
        , 'Close', 'success-snackbar')),
      catchError(this.handleError<any>('Cannot Remove User'))
    );
  }
  /*Lockout User */
  lockoutUser(id) {
    return this.http.patch<any>(`${environment.apiURL}/api/Users/${id}/lockout/`, httpOptions).pipe(
      catchError(this.handleError<any>('Cannot Remove User'))
    );
  }

  /*Update User Password*/
  updateUserPassword(userid, password): Observable<any> {
    return this.http.patch<any>(`${environment.apiURL}/api/Users/${userid}/password/`, JSON.stringify(password), httpOptions).pipe(
      tap(_ => this.snackBar.openSnackBar(`Password Updated! Please login with new password.`
        , 'Close', 'success-snackbar')),
      catchError(this.handleError<any>('Password Update Failed'))
    );
  }
  /* Update User */
  updateUser(userid, user): Observable<any> {
    this.logger.log('stringy updated user', userid, JSON.stringify(user));
    return this.http.put(`${environment.apiURL}/api/Users/${userid}`, JSON.stringify(user), httpOptions).pipe(
      catchError(this.handleError<any>('User Update Failed'))
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