import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpErrorResponse, HttpClientModule } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { map, catchError, tap } from 'rxjs/operators';
import { Alerts, Content, Population } from './models/dashboard';
import { environment } from '../environments/environment';

// we can now access environment.apiUrl
const API_URL = environment.apiURL;


//const endpoint = 'http://localhost:3000/';
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
    return this.http.get<any>(API_URL+'/api/Alerts/' + '?id=' + id).pipe(
      map((data: Alerts[]) => {
        return data;
      })
   );
  }
  /*Updates if Alert is Active*/
  updateAlertActivity(id, Alert_ScheduleId, alert): Observable<any> {
    return this.http.put(API_URL+'/api/AlertActivity/' + Alert_ScheduleId + '/' + id, JSON.stringify(alert), httpOptions).pipe(
      tap(_ => console.log(`updated alert id=${id}`)),
      catchError(this.handleError<any>('updateAlertActivity'))
    );
  }

  /* Dashboard Content =======================================================*/

  getDashboardContent(): Observable<any> {
    return this.http.get<any>(API_URL+'/api/Contents/').pipe(
      map((data: Content[]) => {
        return data;
      })
   );
  }

  /*Gets All Alerts by ID*/
  getPopulationDetails(id): Observable<any> {
    return this.http.get<any>(API_URL+'/api/Metrics/' + '?practiceId=' + id).pipe(
      map((data: Population[]) => {
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
