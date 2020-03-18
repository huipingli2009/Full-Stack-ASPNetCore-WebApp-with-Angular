import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpErrorResponse, HttpClientModule, HttpParams } from '@angular/common/http';
import { Observable, of, Subject } from 'rxjs';
import { map, catchError, tap } from 'rxjs/operators';
import { Alerts, Population, EdChart, EdChartDetails, Spotlight, Quicklinks } from './models/dashboard';
import { environment } from '../environments/environment';
import { Patients, PatientDetails, Conditions, Providers, PopSlices, Gender, Insurance, Pmca, States } from './models/patients';
import { NGXLogger } from 'ngx-logger';
import { MatSnackBarComponent } from './shared/mat-snack-bar/mat-snack-bar.component';


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

  constructor(private http: HttpClient, private logger: NGXLogger, private snackBar: MatSnackBarComponent) { }
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
  updateAlertActivity(alertScheduleId, alert): Observable<any> {
    return this.http.post<any>(`${API_URL}/api/Alerts/${alertScheduleId}`, JSON.stringify(alert), httpOptions).pipe(
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

  /* Patients Content =======================================================*/

  /*Get All Patients*/
  getAllPatients(): Observable<any> {
    return this.http.get<any>(`${API_URL}/api/Patients/`).pipe(
      map((data: Patients[]) => {
        return data;
      })
    );
  }

  updateWatchlistStatus(patientID): Observable<any> {
    return this.http.put(`${API_URL}/api/Patients/watchlist/${patientID}`, httpOptions).pipe(
      map((data) => {
        return data;
      })
    );
  }
  /*Find Patients by Query*/
  findPatients(
    sortcolumn = 'name', sortdirection = 'Asc',
    pageNumber = 0, rowsPerPage = 20, chronic = '', watchFlag = '', conditionIDs = '',
    staffID = '', popmeasureID = '', namesearch = ''): Observable<Patients[]> {

    return this.http.get(`${API_URL}/api/Patients`, {
      params: new HttpParams()
        .set('sortcolumn', sortcolumn)
        .set('sortdirection', sortdirection)
        .set('pagenumber', pageNumber.toString())
        .set('rowsPerPage', rowsPerPage.toString())
        .set('chronic', chronic.toString())
        .set('watch', watchFlag.toString())
        .set('conditionIDs', conditionIDs)
        .set('staffID', staffID)
        .set('popmeasureID', popmeasureID)
        .set('namesearch', namesearch)
    }).pipe(
      map(res => {        
        var patientsAndCount: Patients[];
        
        patientsAndCount = res['results'];
        return patientsAndCount;
      })
    );
  }

  /*Update Patient Details*/
  savePatientDetails(patientId, patient): Observable<any> {
    console.log('PatientPutInest', JSON.stringify(patient))
    return this.http.put(`${API_URL}/api/Patients/${patientId}`, JSON.stringify(patient), httpOptions).pipe(
      tap(_ => this.snackBar.openSnackBar(`Patient ${patient.firstName} ${patient.lastName} has been updated!`
      , 'Close', 'success-snackbar'))
    );
  }

  /*Get Conditions List */
  getConditionsList(): Observable<any> {
    return this.http.get<any>(`${API_URL}/api/Patients/conditions/`).pipe(
      map((data: Conditions[]) => {
        return data;
      })
    );
  }

  /*Get List of PCPs*/
  getPCPList(): Observable<any> {
    return this.http.get<any>(`${API_URL}/api/Staff/providers/`).pipe(
      map((data: Providers[]) => {
        return data;
      })
    );
  }

  /*Get List of Population Slices*/
  getPopSliceList(): Observable<any> {
    return this.http.get<any>(`${API_URL}/api/Metrics/pop/`).pipe(
      map((data: PopSlices[]) => {
        return data;
      })
    );
  }

  /*Gets base PatientDetails based on Patient Id */
  getPatientDetails(id): Observable<any> {
    const endpoint = `${API_URL}/api/Patients/${id}`;
    return this.http.get<any>(endpoint).pipe(
      map((data: PatientDetails[]) => {
        return data;
      })
    );
  }

  /* Get Insurance */
  getInsurance(): Observable<any> {
    return this.http.get<any>(`${API_URL}/api/Patients/insurance/`).pipe(
      map((data: Insurance[]) => {
        return data;
      })
    );
  }

  /* Get Gender */
  getGender(): Observable<any> {
    return this.http.get<any>(`${API_URL}/api/Patients/gender/`).pipe(
      map((data: Gender[]) => {
        return data;
      })
    );
  }
  /* Get Gender */
  getPmca(): Observable<any> {
    return this.http.get<any>(`${API_URL}/api/Patients/pmca/`).pipe(
      map((data: Pmca[]) => {
        return data;
      })
    );
  }
  /* Get Gender */
  getState(): Observable<any> {
    return this.http.get<any>(`${API_URL}/api/Patients/state/`).pipe(
      map((data: States[]) => {
        return data;
      })
    );
  }



  private handleError<T>(operation = 'operation', result?: T) {
    return (error: any): Observable<T> => {

      // TODO: send the error to remote logging infrastructure
      this.logger.error(error); // log to console instead

      // TODO: better job of transforming error for user consumption
      this.logger.log(`${operation} failed: ${error.message}`);

      // Let the app keep running by returning an empty result.
      return of(result as T);
    };
  }
}
