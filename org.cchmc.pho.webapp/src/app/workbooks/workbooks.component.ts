import { DatePipe } from '@angular/common';
import { Component, OnDestroy, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { FormArray, FormBuilder, FormControl, Validators } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatSort, Sort } from '@angular/material/sort';
import { MatTable, MatTableDataSource } from '@angular/material/table';
import { NGXLogger } from 'ngx-logger';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, take, takeUntil } from 'rxjs/operators';
import { PatientForWorkbook } from '../models/patients';
import { Followup, WorkbookPatient, WorkbookProvider, WorkbookReportingMonths } from '../models/workbook';
import { RestService } from '../rest.service';
import { DateRequiredValidator } from '../shared/customValidators/customValidator';

@Component({
  selector: 'app-workbooks',
  templateUrl: './workbooks.component.html',
  styleUrls: ['./workbooks.component.scss']
})
export class WorkbooksComponent implements OnInit, OnDestroy {

  constructor(private rest: RestService, private fb: FormBuilder, private datePipe: DatePipe, private logger: NGXLogger, private dialog: MatDialog, private _snackBar: MatSnackBar) { }

  get ProviderWorkbookArray() {
    return this.ProvidersForWorkbookForm.get('ProviderWorkbookArray') as FormArray;
  }

  @ViewChild('FollowUp') followUp: TemplateRef<any>;
  @ViewChild('DeletePatient') DeletePatient: TemplateRef<any>;
  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild('table') table: MatTable<WorkbookPatient>;

  private unsubscribe$ = new Subject();
  displayedColumns: string[] = ['action', 'patient', 'dob', 'phone', 'provider', 'dateOfService', 'phQ9_Score', 'improvement', 'actionFollowUp', 'followUp'];
  workbookReportingMonths: WorkbookReportingMonths[];
  workbookProviders: WorkbookProvider[];
  workbookPatient: WorkbookPatient[];
  sortedData: WorkbookPatient[];
  SearchPatients: PatientForWorkbook[];
  followUpQuestions: Followup;
  newWorkbookPatient: WorkbookPatient;
  removeWorkbookPatient: WorkbookPatient;
  workbookProviderDetail: WorkbookProvider;
  dataSourceWorkbook: MatTableDataSource<WorkbookPatient>;
  formResponseId: number;
  phqsFinal: number;
  totalFinal: number;
  patientTableHeader: number;
  deletingPatientName: string;
  deletingPatientId: number;

  selectedFormResponseID = new FormControl('');
  searchPatient = new FormControl('');
  PatientNameFilter = new FormControl('');

  ProvidersForWorkbookForm = this.fb.group({
    ProviderWorkbookArray: this.fb.array([
      this.fb.group({
        formResponseID: [''],
        staffID: [''],
        provider: [''],
        phqs: [''],
        total: ['']
      })
    ])
  });

  PatientForWorkbookForm = this.fb.group({
    formResponseId: [''],
    patientInfo: [''],
    patientId: ['', Validators.required],
    providerStaffID: ['', Validators.required],
    dateOfService: ['', [DateRequiredValidator]],
    pHQ9Score: ['', Validators.required],
    action: ['', Validators.required],
    dob: [''],
    phone: ['']
  });

  FollowupForm = this.fb.group(
    {
      formResponseId: [''],
      patientId: [''],
      actionPlanGiven: [''],
      managedByExternalProvider: [''],
      dateOfLastCommunicationByExternalProvider: [''],
      followupPhoneCallOneToTwoWeeks: [''],
      dateOfFollowupCall: [''],
      oneMonthFollowupVisit: [''],
      dateOfOneMonthVisit: [''],
      oneMonthFolllowupPHQ9Score: ['']

    }
  );

  ngOnInit(): void {
    this.getWorkbookReportingMonths();
    this.onProviderValueChanges();
    this.onPatientSearchValueChanges();
    this.onWorkbooksForPatientSearchValueChanges();
  }

  ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }


  //for getting the reporting months for a workbook
  getWorkbookReportingMonths() {
    this.rest.getWorkbookReportingMonths().pipe(take(1)).subscribe((data) => {
      this.workbookReportingMonths = data;
      this.workbookReportingMonths.forEach((element, index, reportData) => {
        this.workbookReportingMonths[index].reportingMonth = this.datePipe.transform(this.workbookReportingMonths[index].reportingMonth, 'MM/dd/yyyy');
        this.selectedFormResponseID.setValue(this.workbookReportingMonths[0].formResponseID);
        this.onReportingDateSelectionChange();
      });
    })
  }


  //on change of the reporting data for workbook
  onReportingDateSelectionChange() {
    this.formResponseId = this.selectedFormResponseID.value;
    this.getWorkbookProviders(this.formResponseId);
    this.getWorkbookPatients(this.formResponseId);
  }

  //for getting workbook providers
  getWorkbookProviders(formResponseid: number) {
    this.rest.getWorkbookProviders(formResponseid).pipe(take(1)).subscribe((data) => {
      this.workbookProviders = data;
      this.logger.log('provider workboos', this.workbookProviders)
      this.AssignProviderWorkbookArray();
    })
  }

  AssignProviderWorkbookArray() {
    const providerArray = this.ProviderWorkbookArray;
    let clearArray = this.ProvidersForWorkbookForm.controls['ProviderWorkbookArray'] as FormArray;
    clearArray.clear();
    if (providerArray.length > 0) {
      providerArray.removeAt(0);
    }
    this.workbookProviders.forEach(provider => {
      providerArray.push(this.fb.group(provider));
    });
  }

  //for updating the provider values
  onProviderValueChanges(): void {
    this.ProvidersForWorkbookForm.get('ProviderWorkbookArray').valueChanges.pipe(takeUntil(this.unsubscribe$)).subscribe(values => {
      this.phqsFinal = 0;
      this.totalFinal = 0;
      const ctrl = <FormArray>this.ProvidersForWorkbookForm.controls['ProviderWorkbookArray'];
      ctrl.controls.forEach(x => {
        let parsedphqs = parseInt(x.get('phqs').value)
        let parsedtotal = parseInt(x.get('total').value)
        this.phqsFinal += parsedphqs
        this.totalFinal += parsedtotal

      });

    })
  }

  onProviderWorkbookChange(index: number) {
    let provider = this.ProviderWorkbookArray.at(index);
    this.workbookProviderDetail = provider.value;
    this.workbookProviderDetail.phqs = Number(this.workbookProviderDetail.phqs);
    this.workbookProviderDetail.total = Number(this.workbookProviderDetail.total);
    this.updateWorkbookProviders(this.workbookProviderDetail);
  }

  updateWorkbookProviders(workbookProviderDetails: WorkbookProvider) {
    this.rest.updateWorkbookForProvider(workbookProviderDetails).subscribe(res => {
    })
  }

  //for getting patients for specific reporting period of workbook
  getWorkbookPatients(formResponseid: number) {
    this.rest.getWorkbookPatients(formResponseid).pipe(take(1)).subscribe((data) => {
      this.workbookPatient = data;
      this.patientTableHeader = this.workbookPatient.length;
      this.dataSourceWorkbook = new MatTableDataSource(this.workbookPatient);
      this.dataSourceWorkbook.data = this.workbookPatient;
    })
  }

  trackPatients(index: number, item: WorkbookPatient): string {
    return '${item.patientId}';
  }

  //find specific patient to add to the workbook
  onPatientSearchValueChanges(): void {
    this.searchPatient.valueChanges.pipe(debounceTime(1000), distinctUntilChanged(), takeUntil(this.unsubscribe$)).subscribe(values => {
      if (values != "") {
        this.findPatientsForAddingtoWorkbook(values);
      }

    });
  }
  findPatientsForAddingtoWorkbook(searchterm: string) {
    this.rest.findPatientsForAddingtoWorkbook(searchterm).pipe(take(1)).subscribe((data) => {
      this.SearchPatients = data;
    })
  }

  trackByForPatientSearch(index, patient): string {
    return '${patient.patientId}';
  }

  //adding patient to the workbook

  onSelectedPatient(event: any): void {
    this.PatientForWorkbookForm.get('dob').setValue(this.datePipe.transform(event.value.dob, 'MM/dd/yyyy'));
    this.PatientForWorkbookForm.get('phone').setValue(event.value.phone);
    this.PatientForWorkbookForm.get('patientId').setValue(event.value.patientId);
  }

  AddPatientForWorkbook() {
    this.newWorkbookPatient = new WorkbookPatient();
    this.newWorkbookPatient.formResponseId = this.selectedFormResponseID.value;
    this.newWorkbookPatient.patientId = this.PatientForWorkbookForm.get('patientId').value;
    this.newWorkbookPatient.phone = this.PatientForWorkbookForm.get('phone').value;
    this.newWorkbookPatient.providerId = this.PatientForWorkbookForm.get('providerStaffID').value;
    this.newWorkbookPatient.dateOfService = this.PatientForWorkbookForm.get('dateOfService').value;
    this.newWorkbookPatient.phQ9_Score = this.PatientForWorkbookForm.get('pHQ9Score').value;
    this.newWorkbookPatient.actionFollowUp = (Boolean)(this.PatientForWorkbookForm.get('action').value);
    this.AddPatientToWorkbook(this.newWorkbookPatient);
  }

  AddPatientToWorkbook(newWorkbookPatient: WorkbookPatient) {
    this.rest.AddPatientToWorkbook(this.newWorkbookPatient).pipe(take(1)).subscribe(res => {
      this.PatientForWorkbookForm.reset();
      this.getWorkbookPatients(this.selectedFormResponseID.value);
      this.searchPatient.reset();
      this.openSnackBar(`Patient added to the workbook`, "Success")
    })
  }

  //For removing patient from the workbook
  onPatientDelete(element: any) {
    this.deletingPatientName = element.patient;
    this.deletingPatientId = element.patientId;
    this.dialog.open(this.DeletePatient);
  }

  OnRemovePatientClick() {
    this.removeWorkbookPatient = new WorkbookPatient();
    this.removeWorkbookPatient.formResponseId = this.selectedFormResponseID.value;
    this.removeWorkbookPatient.patientId = this.deletingPatientId;
    this.RemovePatientFromWorkbook(this.removeWorkbookPatient);
  }

  RemovePatientFromWorkbook(removeWorkbookPatient: WorkbookPatient) {
    this.rest.RemovePatientFromWorkbook(this.removeWorkbookPatient).pipe(take(1)).subscribe(res => {
      this.PatientForWorkbookForm.reset();
      this.getWorkbookPatients(this.selectedFormResponseID.value);
      this.openSnackBar(`Patient removed from the workbook`, "Success")
    })
  }

  //for getting reporting months based on patient name

  onWorkbooksForPatientSearchValueChanges(): void {
    this.PatientNameFilter.valueChanges.pipe(debounceTime(1000), distinctUntilChanged(), takeUntil(this.unsubscribe$)).subscribe(values => {
      if (values != "") {
        this.getWorkbookReportingMonthsForPatient(values);
      }
    });
  }

  getWorkbookReportingMonthsForPatient(patientName: string) {
    this.rest.getWorkbookReportingMonthsForPatient(patientName).pipe(take(1)).subscribe((data) => {
      this.workbookReportingMonths = data;
      this.workbookReportingMonths.forEach((element, index, reportData) => {
        this.workbookReportingMonths[index].reportingMonth = this.datePipe.transform(this.workbookReportingMonths[index].reportingMonth, 'MM/dd/yyyy');
        this.selectedFormResponseID.setValue(this.workbookReportingMonths[0].formResponseID);
        this.onReportingDateSelectionChange();
      });
    })
  }


  //for getting the follow-up questions

  FollowUpForPatient(element: any) {
    this.getFollowUpQuestions(element.formResponseId, element.patientId, element.patient);
  }

  getFollowUpQuestions(formResponseid: number, patientID: number, patient: string) {
    this.rest.getFollowUpQuestions(formResponseid, patientID).pipe(take(1)).subscribe((data) => {
      this.followUpQuestions = data;
      this.FollowupForm.setValue(this.followUpQuestions);
      this.FollowupForm.get('actionPlanGiven').setValue(this.followUpQuestions.actionPlanGiven ? 'Yes' : 'No');
      this.FollowupForm.get('managedByExternalProvider').setValue(this.followUpQuestions.managedByExternalProvider ? 'Yes' : 'No');
      this.FollowupForm.get('followupPhoneCallOneToTwoWeeks').setValue(this.followUpQuestions.followupPhoneCallOneToTwoWeeks ? 'Yes' : 'No');
      this.FollowupForm.get('oneMonthFollowupVisit').setValue(this.followUpQuestions.oneMonthFollowupVisit ? 'Yes' : 'No');
      this.openDialog();
    })
  }


  //updating the follow-up questions  

  updateFollowUpQuestion() {
    this.followUpQuestions = new Followup();
    this.followUpQuestions.actionPlanGiven = (this.FollowupForm.get('actionPlanGiven').value === 'Yes') ? true : false;
    this.followUpQuestions.managedByExternalProvider = (this.FollowupForm.get('managedByExternalProvider').value === 'Yes') ? true : false;
    this.followUpQuestions.followupPhoneCallOneToTwoWeeks = (this.FollowupForm.get('followupPhoneCallOneToTwoWeeks').value === 'Yes') ? true : false;
    this.followUpQuestions.oneMonthFollowupVisit = (this.FollowupForm.get('oneMonthFollowupVisit').value === 'Yes') ? true : false;
    this.followUpQuestions.formResponseId = this.FollowupForm.get('formResponseId').value;
    this.followUpQuestions.patientId = this.FollowupForm.get('patientId').value;
    this.followUpQuestions.dateOfLastCommunicationByExternalProvider = this.FollowupForm.get('dateOfLastCommunicationByExternalProvider').value;
    this.followUpQuestions.dateOfFollowupCall = this.FollowupForm.get('dateOfFollowupCall').value;
    this.followUpQuestions.dateOfOneMonthVisit = this.FollowupForm.get('dateOfOneMonthVisit').value;
    this.followUpQuestions.oneMonthFolllowupPHQ9Score = (Number)(this.FollowupForm.get('oneMonthFolllowupPHQ9Score').value);
    this.UpdateFollowUpQuestionResponses(this.followUpQuestions);
  }
  UpdateFollowUpQuestionResponses(followUp: Followup) {
    this.rest.UpdateFollowUpQuestionResponses(followUp).pipe(take(1)).subscribe((data) => {
      this.CloseDialog();
      this.getWorkbookPatients(this.formResponseId);
    })
  }

  openDialog() {
    const dialogRef = this.dialog.open(this.followUp);

  }
  CloseDialog() {
    const dialogRef = this.dialog.closeAll();
  }


  // for sorting the workbook patient table columns 
  onSortData(sort: Sort) {
    const data = this.workbookPatient.slice();
    if (!sort.active || sort.direction === '') {
      this.sortedData = data;
      return;
    }

    this.sortedData = data.sort((a, b) => {
      const isAsc = sort.direction === 'asc';
      switch (sort.active) {
        case 'patient': return this.compare(a.patient.toString(), b.patient.toString(), isAsc);
        case 'dob': return this.compare(a.dob, b.dob, isAsc);
        case 'provider': return this.compare(a.provider, b.provider, isAsc);
        case 'dateOfService': return this.compare(a.dateOfService, b.dateOfService, isAsc);
        case 'phQ9_Score': return this.compare(a.phQ9_Score, b.phQ9_Score, isAsc);

        default: return 0;
      }
    });
    this.table.dataSource = this.sortedData;


  }
  compare(a: number | string, b: number | string, isAsc: boolean) {
    return (a < b ? -1 : 1) * (isAsc ? 1 : -1);
  }

  openSnackBar(message: string, action: string) {
    this._snackBar.open(message, action, {
      duration: 2000,
      panelClass: ['green-snackbar']
    });
  }

}
