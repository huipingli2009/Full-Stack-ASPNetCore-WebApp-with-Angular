import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { NGXLogger } from 'ngx-logger';
import { Observable, of } from 'rxjs';
import { catchError, map, tap } from 'rxjs/operators';
import { environment } from '../environments/environment';
import { Alerts, EdChart, EdChartDetails, Population, Quicklinks, Spotlight } from './models/dashboard';
import { Conditions, Gender, Insurance, PatientDetails, PatientForWorkbook, Patients, Pmca, PopSlices, Providers, States } from './models/patients';
import { PracticeList, Responsibilities, Staff, StaffDetails } from './models/Staff';
import { Followup, WorkbookPatient, WorkbookProvider, WorkbookReportingMonths, WorkbookPractice } from './models/workbook';
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

  /* Staff Component =======================================================*/

  /*Get all the staff */
  getStaff(): Observable<any> {
    const endpoint = `${API_URL}/api/staff`;
    return this.http.get<Staff[]>(endpoint).pipe(
      map((data: Staff[]) => {
        return data;
      })
    );
  }

  /*Get specific staff details */
  getStaffDetails(id: number): Observable<any> {
    const endpoint = `${API_URL}/api/Staff/${id}`;
    return this.http.get<StaffDetails>(endpoint).pipe(
      map((data: StaffDetails) => {
        return data;
      })
    );
  }

  /*Updates Staff*/
  updateStaff(StaffDetails): Observable<any> {
    return this.http.put<StaffDetails>(`${API_URL}/api/Staff/${StaffDetails.id}`, JSON.stringify(StaffDetails), httpOptions).pipe(
      catchError(this.handleError<any>('updateStaff'))
    );
  }

  /*Get all Credentials */
  getCredentials(): Observable<any> {
    const endpoint = `${API_URL}/api/staff/credentials`;
    return this.http.get<Credential[]>(endpoint).pipe(
      map((data: Credential[]) => {
        return data;
      })
    );
  }

  /*Get all Positions */
  getPositions(): Observable<any> {
    const endpoint = `${API_URL}/api/staff/positions`;
    return this.http.get<Position[]>(endpoint).pipe(
      map((data: Position[]) => {
        return data;
      })
    );
  }

  /*Get all Responsibilities */
  getResponsibilities(): Observable<any> {
    const endpoint = `${API_URL}/api/staff/responsibilities`;
    return this.http.get<Responsibilities[]>(endpoint).pipe(
      map((data: Responsibilities[]) => {
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

  getStaffAdminVerbiage(): Observable<any> {
    return this.http.get(`${API_URL}/api/Users/verbiage/`, { responseType: 'text' }).pipe(
      map((res) => {
        return res;
      })
    );
  }

  /* Practice Switch =======================================================*/
  getPracticeList(): Observable<any> {
    return this.http.get<any>(`${API_URL}/api/Staff/practicelist/`).pipe(
      map((data: PracticeList[]) => {
        return data;
      })
    );
  }

  /* Workbook Component =======================================================*/

  /* for getting the reporting month and form response ID */
  getWorkbookReportingMonths(): Observable<any> {
    return this.http.get<WorkbookReportingMonths[]>(`${API_URL}/api/Workbooks/lookups`).pipe(
      map((data: WorkbookReportingMonths[]) => {
        return data;
      })
    );
  }

  /* for getting providers for Depression workbook for a spefic reporting date*/

  getWorkbookProviders(formResponseid: number): Observable<any> {
    let paramsValue = new HttpParams();
    paramsValue = paramsValue.append("formResponseId", formResponseid.toString());
    return this.http.get<WorkbookProvider[]>(`${API_URL}/api/Workbooks/providers`, { params: paramsValue }).pipe(
      map((data: WorkbookProvider[]) => {
        return data;
      })
    );
  }

  /*Update workbook for Staff*/
  updateWorkbookForProvider(WorkbookProvider: WorkbookProvider): Observable<any> {
    return this.http.put(`${API_URL}/api/Workbooks/provider/${WorkbookProvider.staffID}`, JSON.stringify(WorkbookProvider), httpOptions).pipe(
      catchError(this.handleError<any>('update staff workbook'))
    );
  }

  /* for getting the patients for a form response ID */
  getWorkbookPatients(formResponseid: number): Observable<any> {
    let paramsValue = new HttpParams();
    paramsValue = paramsValue.append("formResponseId", formResponseid.toString());
    return this.http.get<WorkbookProvider[]>(`${API_URL}/api/Workbooks/patients`, { params: paramsValue }).pipe(
      map((data: WorkbookProvider[]) => {
        return data;
      })
    );
  }

  /* for searching on patient name to add to workbook */
  findPatientsForAddingtoWorkbook(searchTerm: string): Observable<any> {
    return this.http.get<PatientForWorkbook[]>(`${API_URL}/api/Patients/simple/${searchTerm}`).pipe(
      map((data: PatientForWorkbook[]) => {
        return data;
      })
    );
  }


  /*addition of patient to the work book*/
  AddPatientToWorkbook(workbookPatient: WorkbookPatient): Observable<any> {
    this.logger.log(JSON.stringify(workbookPatient));
    return this.http.post<boolean>(`${API_URL}/api/Workbooks/Patients/${workbookPatient.patientId}`, JSON.stringify(workbookPatient), httpOptions).pipe(
      catchError(this.handleError<any>('adding patient to the workbook'))
    );
  }
  switchPractice(staffWithPractice): Observable<any> {
    return this.http.put(`${API_URL}/api/Staff/switchpractice`, JSON.stringify(staffWithPractice), httpOptions).pipe(
      tap(_ => this.snackBar.openSnackBar(`Current Practice Switched!`
        , 'Close', 'success-snackbar'))
    );
  }

  /*Removing patient from the work book*/
  RemovePatientFromWorkbook(workbookPatient: WorkbookPatient): Observable<any> {
    this.logger.log(JSON.stringify(workbookPatient));
    return this.http.delete<boolean>(`${API_URL}/api/Workbooks/Patients/${workbookPatient.formResponseId}/${workbookPatient.patientId}`, httpOptions).pipe(
      catchError(this.handleError<any>('removing patient from the workbook'))
    );
  }


  /*getting workbook Practice details*/
  getWorkbookPractice(formResponseid: number): Observable<any> {
    let paramsValue = new HttpParams();
    paramsValue = paramsValue.append("formResponseId", formResponseid.toString());
    return this.http.get<WorkbookPractice>(`${API_URL}/api/Workbooks/practice`, { params: paramsValue }).pipe(
      map((data: WorkbookPractice) => {
        return data;
      })
    );
  }

  /* for getting follow-up question for a patient */
  getFollowUpQuestions(formResponseid: number, patientID: number): Observable<any> {
    let paramsValue = new HttpParams();
    paramsValue = paramsValue.append("formResponseId", formResponseid.toString());
    paramsValue = paramsValue.append("patientID", patientID.toString());

    return this.http.get<Followup[]>(`${API_URL}/api/Workbooks/patientfollowup`, { params: paramsValue }).pipe(
      map((data: Followup[]) => {
        return data;
      })
    );
  }

  /* for update follow-up question responses for a patient */
  UpdateFollowUpQuestionResponses(followup: Followup): Observable<any> {
    return this.http.put(`${API_URL}/api/Workbooks/patientfollowup/${followup.patientId}`, JSON.stringify(followup), httpOptions).pipe(
      map((data: Followup[]) => {
        return data;
      })
    );
  }

  /* for getting the reporting month and form response ID for a Patient */
  getWorkbookReportingMonthsForPatient(patientName: string): Observable<any> {
    let paramsValue = new HttpParams();
    paramsValue = paramsValue.append("nameSearch", patientName);
    return this.http.get<WorkbookReportingMonths[]>(`${API_URL}/api/Workbooks/lookups`, { params: paramsValue }).pipe(
      map((data: WorkbookReportingMonths[]) => {
        return data;
      })
    );
  }
  //update user legal disclaimer
  updateUserLegalDisclaimer(): Observable<any> {
    return this.http.put<boolean>(`${API_URL}/api/Staff/legalstatus`, httpOptions).pipe(
      map((data: boolean) => {
        this.logger.log(data);
        return data;
      }
      )
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



