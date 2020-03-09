import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpErrorResponse, HttpClientModule } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { map, catchError, tap } from 'rxjs/operators';
import { Alerts, Population, EdChart, EdChartDetails, Spotlight, Quicklinks } from './models/dashboard';
import { environment } from '../environments/environment';
import { Patients, PatientDetails } from './models/patients';

// we can now access environment.apiUrl
const API_URL = environment.apiURL;


//const endpoint = 'http://localhost:3000/';
const httpOptions = {
  headers: new HttpHeaders({
    'Content-Type': 'application/json'
  })
};
@Injectable({
  providedIn: 'root'
})
export class RestService {

  constructor(private http: HttpClient) {

  }
  private extractData(res: Response) {
    const body = res;
    return body || {};
  }
  /* Alerts =======================================================*/

  /*Gets All Alerts by ID*/
  getAlerts(): Observable<any> {
    return this.http.get<any>(`${API_URL}/api/Alerts/`).pipe(
      map((data: Alerts[]) => {
        return data;
      })
    );
  }
  /*Updates if Alert is Active*/
  updateAlertActivity(alertSchedId, alert): Observable<any> {
    return this.http.put(`${API_URL}/api/Alerts/${alertSchedId}`, JSON.stringify(alert), httpOptions).pipe(
      tap(_ => console.log(`updated alert id=${alertSchedId}`)),
      catchError(this.handleError<any>('updateAlertActivity'))
    );
  }

  /* Dashboard Content =======================================================*/

  getSpotlight(): Observable<any> {
    return this.http.get<any>(`${API_URL}/api/Contents/spotlights/`).pipe(
      map((data: Spotlight[]) => {
        return data;
      })
    );
  }

  getQuicklinks(): Observable<any> {
    return this.http.get<any>(`${API_URL}/api/Contents/quicklinks/`).pipe(
      map((data: Quicklinks[]) => {
        return data;
      })
    );
  }

  /*Gets All KPIS*/
  getAllKpis(): Observable<any> {
    return this.http.get<any>(`${API_URL}/api/Metrics/kpis/`).pipe(
      map((data: Population[]) => {
        return data;
      })
    );
  }

  /*Gets base ED Chart Information */
  getEdChartByUser(): Observable<any> {
    const endpoint = `${API_URL}/api/Metrics/edcharts/`;
    return this.http.get<any>(endpoint).pipe(
      map((data: EdChart[]) => {
        return data;
      })
   );
  }

   /*Gets base ED Chart Information */
   getEdChartDetails(admitDate): Observable<any> {
    const endpoint = `${API_URL}/api/Metrics/edcharts/${admitDate}`;
    return this.http.get<any>(endpoint).pipe(
      map((data: EdChartDetails[]) => {
        return data;
      })
   );
  }

  /*Get All Patients*/
  getAllPatients(): Observable<any> {
    return this.http.get<any>(`${API_URL}/api/Patients/`).pipe(
      map((data: Patients[]) => {
        return data;
      })
    );
  }

  /*Gets base PatientDetails based on Patient Id */
  getPatientDetails(id): Observable<any> {
    const endpoint = `${API_URL}/api/patientDetails/${id}`;
    return this.http.get<any>(endpoint).pipe(
      map((data: PatientDetails[]) => {
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
