import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpErrorResponse, HttpClientModule } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { map, catchError, tap } from 'rxjs/operators';
import { Alerts, Content } from './models/dashboard';

const endpoint = 'http://localhost:3000/';
const httpOptions = {
  headers: new HttpHeaders({
    'Content-Type':  'application/json'
  })
};
@Injectable({
  providedIn: 'root'
})
export class RestService {

  constructor( private http: HttpClient) {

  }
  private extractData(res: Response) {
    const body = res;
    return body || { };
  }
  /* Alerts =======================================================*/

  /*Gets All Alerts by ID*/
  getAlerts(id): Observable<any> {
    return this.http.get<any>(endpoint + 'Alerts/' + '?id=' + id).pipe(
      map((data: Alerts[]) => {
        return data;
      })
   );
  }
  /*Updates if Alert is Active*/
  updateAlertActivity(id, Alert_ScheduleId, alert): Observable<any> {
    return this.http.put(endpoint + 'AlertActivity/' + Alert_ScheduleId + '/' + id, JSON.stringify(alert), httpOptions).pipe(
      tap(_ => console.log(`updated alert id=${id}`)),
      catchError(this.handleError<any>('updateAlertActivity'))
    );
  }

  /* Dashboard Content =======================================================*/

  getDashboardContent(): Observable<any> {
    return this.http.get<any>(endpoint + 'Contents/').pipe(
      map((data: Content[]) => {
        return data;
      })
   );
  }
  


  private handleError<T>(operation = 'operation', result?: T) {
    return (error: any): Observable<T> => {

      // TODO: send the error to remote logging infrastructure
      console.error(error); // log to console instead

      // TODO: better job of transforming error for user consumption
      console.log(`${operation} failed: ${error.message}`);

      // Let the app keep running by returning an empty result.
      return of(result as T);
    };
  }
}
