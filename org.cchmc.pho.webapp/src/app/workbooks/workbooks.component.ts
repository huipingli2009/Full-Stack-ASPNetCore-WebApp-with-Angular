import { DatePipe } from '@angular/common';
import { Component, OnDestroy, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { MatSort, Sort } from '@angular/material/sort';
import { MatTable, MatTableDataSource } from '@angular/material/table';
import { NGXLogger } from 'ngx-logger';
import { Subject } from 'rxjs'; 
import { debounceTime, distinctUntilChanged, pairwise, take, takeUntil } from 'rxjs/operators';
import { PatientForWorkbook, Providers } from '../models/patients';
import { Followup, WorkbookDepressionPatient, WorkbookAsthmaPatient, WorkbookProvider, WorkbookReportingPeriod, WorkbookPractice, WorkbookForm, WorkbookFormValueEnum, Treatment, WorkbookConfirmation, QIWorkbookPractice, QIWorkbookQuestions as QIWorkbookParent, Section, Question} from '../models/workbook';
import { RestService } from '../rest.service';
import { DateRequiredValidator } from '../shared/customValidators/customValidator';
import { MatSnackBarComponent } from '../shared/mat-snack-bar/mat-snack-bar.component';

@Component({
  selector: 'app-workbooks',
  templateUrl: './workbooks.component.html',
  styleUrls: ['./workbooks.component.scss']
})
export class WorkbooksComponent implements OnInit, OnDestroy {

  constructor(private rest: RestService, private fb: FormBuilder, private datePipe: DatePipe, private logger: NGXLogger, private dialog: MatDialog, private snackBar: MatSnackBarComponent) 
  {}

  get ProviderWorkbookArray() {
    return this.ProvidersForWorkbookForm.get('ProviderWorkbookArray') as FormArray;
  }

  // get QIWorkbookQuestionsArray() {
  //   //var data = this.QIWorkbookSectionTrackingForm.get('QIWorkbookQuestionsArray');
  //   return this.QIWorkbookSectionTrackingForm.get('QIWorkbookQuestionsArray') as FormArray;
  // }

  get QIWorkbookSectionsArray() {   
    return this.QIWorkbookSectionTrackingForm.get('QIWorkbookSectionsArray') as FormGroup;   
  }

  // get QIWorkbookQuestionArray(): FormArray {
  //   return this.QIWorkbookSectionTrackingForm.get('QIWorkbookQuestionArray') as FormArray;
  // }

  @ViewChild('FollowUp') followUp: TemplateRef<any>;
  @ViewChild('DeletePatient') DeletePatient: TemplateRef<any>;
  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild('table') table: MatTable<WorkbookDepressionPatient>;
  @ViewChild('EditProvidersDialog') editProvidersDialog: TemplateRef<any>;
  @ViewChild('PracticeQIWorkbook') practiceQIWorkbook: TemplateRef<any>;

  //declarations
  //generic
  private unsubscribe$ = new Subject();
  displayedDepressionColumns: string[] = ['action', 'patient', 'dob', 'phone', 'provider', 'dateOfService', 'phQ9_Score', 'improvement', 'actionFollowUp', 'followUp'];
  displayedAsthmaColumns: string[] = ['action', 'patient', 'dob', 'phone', 'provider', 'dateOfService', 'assessmentcompleted', 'treatment', 'asthma_Score', 'followUp'];
  workbookFormValueEnum = WorkbookFormValueEnum;
  workbookReportingPeriods: WorkbookReportingPeriod[];
  workbooksFormList: WorkbookForm[];
  treatmentList: Treatment[];
/*   workbooks: string;
  reportingPeriods: string; */
  selectedFormResponseID: number;
  selectedWorkbookFormId: number;
  selectedReportingPeriod: WorkbookReportingPeriod;
  workbookSelector= new FormControl('');
  reportingPeriodSelector = new FormControl('');


  //depression  
  workbookPractice: WorkbookPractice;
  sortedData: WorkbookDepressionPatient[];
  SearchPatients: PatientForWorkbook[]; 
  followUpQuestions: Followup;
  workbookProviderDetail: WorkbookProvider;
  newDepressionWorkbookPatient: WorkbookDepressionPatient;
  removeDepressionWorkbookPatient: WorkbookDepressionPatient;
  workbookDepressionConfirmations: WorkbookConfirmation;
  workbookProviders: WorkbookProvider[];
  dataSourceDepressionWorkbook: MatTableDataSource<WorkbookDepressionPatient>;
  formResponseId: number;
  phqsFinal = 0;
  totalFinal: number;
  patientTableHeader: number;
  deletingPatientName: string;
  deletingPatientId: number;
  addingPatientName: string;
  hasSelectedPatient: boolean;
  searchPatient = new FormControl('');
  PatientNameFilter = new FormControl('');
  selectedPCPEdit = new FormControl('');
  allProviders: Providers[] = [];
  selectedEditProviderDisplay: string;
  selectedEditProviderId: number;
  displayDepressionWorkbook: boolean;

  //asthma
  dataSourceAsthmaWorkbook: MatTableDataSource<WorkbookAsthmaPatient>;
  asthmaFormResponseId: number;
  workbookDepressionPatient: WorkbookDepressionPatient[];
  workbookAsthmaPatient: WorkbookAsthmaPatient[]; 
  newAsthmaWorkbookPatient: WorkbookAsthmaPatient; 
  removeAsthmaWorkbookPatient: WorkbookAsthmaPatient;   
  totalAsthmaPatients: number;
  sortedAsthmaData: WorkbookAsthmaPatient[];
  //workbooksInitiative: string;

  //for QI workbook 
  qiworkbookprctice: QIWorkbookPractice;
  qiWorkbookParent: QIWorkbookParent;
  qiworkbookquestions: Question[];
  qiworkbooksections: Section[];
  btnToggleVisible: boolean = false;
  qiSectionHeader: string;
  qiQuestionNUM: string;
  qiQuestionDEN: string;
  qiNumeratorLabel: string;
  updateQuestion: Question;

//workbook fomrs
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

  DepressionPatientForWorkbookForm = this.fb.group({
    formResponseId: [''],
    patientInfo: [''],
    patientId: ['', Validators.required],
    providerStaffID: ['', Validators.required],
    dateOfService: ['', [DateRequiredValidator]],
    pHQ9Score: ['', Validators.required],
    action: ['false'],
    dob: [''],
    phone: ['']
  });
  
  AsthmaPatientForWorkbookForm = this.fb.group({
    formResponseId: [''],
    patientInfo: [''],
    patientId: ['', Validators.required],
    providerStaffID: ['', Validators.required],
    dateOfService: ['', [DateRequiredValidator]],
    action: ['false'],
    dob: [''],
    phone: [''],
    asthma_Score: [0],
    assessmentcompleted: ['false'],
    treatment: [10]
  });

  QIWorkbookSectionTrackingForm = this.fb.group({
          formResponseId: '',
          sectionId: '',
          sectionHeader: '',
          dataEntered: '',
          questionId: '',                
          questionDEN: '',
          questionNUM: '',
          numeratorLabel: '',
          numerator: '',
          denominator: ''  
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
      oneMonthFolllowupPHQ9Score: [''],
      phQ9FollowUpNotes: ['']
    }
  );

  editProvidersForm = this.fb.group({
    pcpName: ['', Validators.required]
  });

  ProviderConfirmationForm = this.fb.group({
    allProvidersConfirm: ['']
  });
  PatientConfirmationForm = this.fb.group({
    noPatientsConfirm: ['']
  });

  //event handlers - generic (all workbooks)

  ngOnInit(): void {
    this.selectedWorkbookFormId = this.workbookFormValueEnum.depression;
    
    this.logger.log('setting selected workbookFormId in ngOnInit', this.selectedWorkbookFormId.toString());
    this.getWorkbooksForms();
    this.getWorkbookReportingPeriods();
    this.onDepressionProviderValueChanges();
    this.onPatientSearchValueChanges();
    this.onWorkbooksForPatientSearchValueChanges();
    this.getTreatments();
    this.PHQFollowUpQuestionValidators();    

    //test for getting current value and prev value from formArray
    // this.onValueChanges();
  }

  ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }  
  
  getWorkbooksForms() {
    this.workbooksFormList = [];
    this.rest.getWorkbooksForms().subscribe((data) => {
      data.forEach((item) => {
        this.workbooksFormList.push(item);
      });
      this.logger.log('getWorkbooksForms data: ', this.workbooksFormList);       
      //set initial value - depression
      const toSelect = this.workbooksFormList.find(c => c.id == this.workbookFormValueEnum.depression);
      this.logger.log(toSelect);
      this.workbookSelector.setValue(toSelect.id);
      this.selectedWorkbookFormId = toSelect.id;
      this.logger.log('setting selected workbookFormId in getWorkbooksForms: ', this.selectedWorkbookFormId); 
    });  

  } 
  
  //on change of the workbook selection
  onWorkbookSelectionChange(event) : void {
    this.logger.log('onWorkbookSelectionChange event: ', event.value);
    //set flags
    this.selectedWorkbookFormId = event.value;
    this.logger.log('setting selected workbookFormId in getWorkbooksForms: ', this.selectedWorkbookFormId); 
    //repopulate dates
    this.getWorkbookReportingPeriods();
  }
  
  //for getting the reporting months for a workbook
  getWorkbookReportingPeriods() { 
    this.logger.log('getWorkbookReportingPeriods selectedWorkbookFormId: ', this.selectedWorkbookFormId); 
    this.workbookReportingPeriods = [];
    this.rest.getWorkbookReportingPeriods(this.selectedWorkbookFormId.toString(), "").pipe(take(1)).subscribe((data) => {
      this.workbookReportingPeriods = data;      
      this.logger.log('getWorkbookReportingMonths: ', data);
      this.logger.log('getWorkbookReportingMonths: ', this.workbookReportingPeriods);
      //set initial default      
      this.setSelectedReportingPeriod(this.workbookReportingPeriods[0]);
      //call toggler
      this.toggleWorkbookDisplay();
    });

  } 

  setSelectedReportingPeriod(selectedReportingPeriod: WorkbookReportingPeriod){
    this.reportingPeriodSelector.setValue(selectedReportingPeriod.formResponseId);
    this.selectedFormResponseID = selectedReportingPeriod.formResponseId;
    this.selectedReportingPeriod = selectedReportingPeriod;
  }
 
  //on change of the reporting data for workbook
  onReportingDateSelectionChange(event) : void {
    this.selectedFormResponseID = event.value;
    this.logger.log('onReportingDateSelectionChange event: ', event.value);
    this.logger.log('onReportingDateSelectionChange event: ', this.selectedFormResponseID);
    this.selectedReportingPeriod = this.workbookReportingPeriods.find(p => p.formResponseId == this.selectedFormResponseID);
    this.toggleWorkbookDisplay();
  }
  //on change of the reporting data for workbook
  onReportingDateSelectionChangeForPatient(formResponseId: number) : void {
    this.selectedFormResponseID = formResponseId;
    this.selectedReportingPeriod = this.workbookReportingPeriods.find(p => p.formResponseId == this.selectedFormResponseID);
    this.toggleWorkbookDisplay();
  }

  //switches out display modes for workbooks, which one will populate
  toggleWorkbookDisplay(): void{
    this.logger.log("toggleWorkbookDisplay formResponseId: ", this.selectedFormResponseID);
    if (this.selectedWorkbookFormId == WorkbookFormValueEnum.depression){
      this.getDepressionWorkbookProviders(this.selectedFormResponseID);
      this.getDepressionWorkbookPatients(this.selectedFormResponseID);
      this.getDepressionConfirmations(this.selectedFormResponseID);
      this.getWorkbookPractice(this.selectedFormResponseID);
    }
    if (this.selectedWorkbookFormId == WorkbookFormValueEnum.asthma){
      this.getAsthmaWorkbookPatients(this.selectedFormResponseID);
      this.getAsthmaWorkbookPractice(this.selectedFormResponseID);      
    }
    if (this.selectedWorkbookFormId == WorkbookFormValueEnum.qualityimprovement){
      
      this.getQIWorkbookPractice(this.selectedFormResponseID);  
      this.getQIWorkbookQuestions(this.selectedFormResponseID);
    }
  }

  toggleDepressionPatientFormEnabled(): void{
    if(this.workbookDepressionConfirmations.noPatientsConfirmed === true){
      this.DepressionPatientForWorkbookForm.disable();
      this.logger.log("disable patient entry");
      }
      else { 
        this.DepressionPatientForWorkbookForm.enable(); 
        this.logger.log("enable patient entry");
      }
  }

  toggleDepressionConfirmationEnabled(enable: boolean): void{
    if (enable === true){
      this.PatientConfirmationForm.enable();
    }else{
      this.workbookDepressionConfirmations.noPatientsConfirmed = false;
      this.PatientConfirmationForm.get('noPatientsConfirm').setValue(false);
      this.PatientConfirmationForm.disable();
    }
  }
  
  //find specific patient to add to the workbook
  onPatientSearchValueChanges(): void {
    this.searchPatient.valueChanges.pipe(debounceTime(1000), distinctUntilChanged(), takeUntil(this.unsubscribe$)).subscribe(values => {
      if (values != "") {
        this.findPatientsForAddingtoWorkbook(values);
      }
    });
  }

//event handlers - depression specific
  
  //for updating the depression provider values
  onDepressionProviderValueChanges(): void {
    this.logger.log("onDepressionProviderValueChanges");
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

  //for updating the depression confirmations
  onConfirmationChange(): void {
    this.logger.log("onConfirmationChange");
    let updateData = false;
    // this.logger.log(this.ProviderConfirmationForm.get('allProvidersConfirm').value, "allProvidersConfirm");
    // this.logger.log(this.PatientConfirmationForm.get('noPatientsConfirm').value, "noPatientsConfirm");
    if (this.ProviderConfirmationForm.get('allProvidersConfirm').value != null)
    {
      updateData = true;
      this.workbookDepressionConfirmations.allProvidersConfirmed = this.ProviderConfirmationForm.get('allProvidersConfirm').value;
    }
    if (this.PatientConfirmationForm.get('noPatientsConfirm').value != null)
    {
      updateData = true;
      this.workbookDepressionConfirmations.noPatientsConfirmed = this.PatientConfirmationForm.get('noPatientsConfirm').value;
    }
    //enable disable controls
    this.toggleDepressionPatientFormEnabled();
    if (updateData == true){
      this.updateDepressionWorkbookConfirmations(this.workbookDepressionConfirmations);
    }    
  }

  onProviderDepressionWorkbookChange(index: number) { 
    this.logger.log("onProviderDepressionWorkbookChange");
    let provider = this.ProviderWorkbookArray.at(index);
    this.workbookProviderDetail = provider.value;
    this.workbookProviderDetail.phqs = Number(this.workbookProviderDetail.phqs);
    this.workbookProviderDetail.total = Number(this.workbookProviderDetail.total);
    this.updateDepressionWorkbookProviders(this.workbookProviderDetail);

    if (this.workbookProviders.filter(p => p.phqs == 0 || p.total==0).length == 0){
      this.logger.log("no empty provider totals", this.ProviderWorkbookArray);
    }
    else{
      this.logger.log("still some empty providers", this.workbookProviders);
    }
  }
  
  onSelectedPatient(event: any): void {
    this.hasSelectedPatient = true;
    if (this.selectedWorkbookFormId == WorkbookFormValueEnum.depression){
      this.DepressionPatientForWorkbookForm.get('dob').setValue(this.datePipe.transform(event.value.dob, 'MM/dd/yyyy'));
      this.DepressionPatientForWorkbookForm.get('phone').setValue(event.value.phone);
      this.DepressionPatientForWorkbookForm.get('patientId').setValue(event.value.patientId);
    } 
    if (this.selectedWorkbookFormId == WorkbookFormValueEnum.asthma){      
      this.AsthmaPatientForWorkbookForm.get('dob').setValue(this.datePipe.transform(event.value.dob, 'MM/dd/yyyy'));
      this.AsthmaPatientForWorkbookForm.get('phone').setValue(event.value.phone);
      this.AsthmaPatientForWorkbookForm.get('patientId').setValue(event.value.patientId);
      this.AsthmaPatientForWorkbookForm.get('treatment').setValue(event.value.treatment);
      this.AsthmaPatientForWorkbookForm.get('assessmentcompleted').setValue(event.value.assessmentcompleted);
      //this.AsthmaPatientForWorkbookForm.get('actionplangiven').setValue(event.value.actionplangiven);
      this.AsthmaPatientForWorkbookForm.get('asthma_Score').setValue(event.value.asthma_Score);
    }
    this.addingPatientName = `${event.value.firstName} ${event.value.lastName}`;
  }  
  
  //for getting reporting months based on patient name
  onWorkbooksForPatientSearchValueChanges(): void {
    this.PatientNameFilter.valueChanges.pipe(debounceTime(1000), distinctUntilChanged(), takeUntil(this.unsubscribe$)).subscribe(values => {
      if (values != "") {
        this.getWorkbookReportingMonthsForPatient(values);
      }
    });
  }  

//helper methods - generic (all workbooks)

  //get workbook practice
  getWorkbookPractice(formResponseid: number) {
    this.rest.getWorkbookPractice(formResponseid).pipe(take(1)).subscribe((data) => {
      this.workbookPractice = data;
    })
  } 

  //for getting workbook providers
  getAllProviders() {
    this.rest.getPCPList().pipe(take(1)).subscribe((data) => {
      this.allProviders = data;
    })
  }

  //helper methods - generic (all workbooks)

   //get practice QI workbook 
   getQIWorkbookPractice(formResponseid: number) {
    this.rest.getQIWorkbookPractice(formResponseid).pipe(take(1)).subscribe((data) => {
      this.workbookPractice = data;
    })
  }  

  //lookup patient to add. Depression and possibly asthma.
  findPatientsForAddingtoWorkbook(searchterm: string) {
    this.rest.findPatientsForAddingtoWorkbook(searchterm).pipe(take(1)).subscribe((data) => {
      this.SearchPatients = data;
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
    const data = this.workbookDepressionPatient.slice();
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

//helper methods - depression specific

  //for getting workbook providers
  getDepressionWorkbookProviders(formResponseid: number) {
    this.rest.getWorkbookProviders(formResponseid).pipe(take(1)).subscribe((data) => {
      this.workbookProviders = data;
      this.AssignDepressionProviderWorkbookArray();
    })
  }  

  AssignDepressionProviderWorkbookArray() {
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

  //Reset Add Patient Form
  resetAddDepressionPatient() {
    this.DepressionPatientForWorkbookForm.reset();
    this.hasSelectedPatient = false;
    this.DepressionPatientForWorkbookForm.get('action').setValue('false');
  }

  updateDepressionWorkbookProviders(workbookProviderDetails: WorkbookProvider) {
    this.rest.updateWorkbookForProvider(workbookProviderDetails).subscribe(res => {
      //after we get this data, doublecheck for changed confirmation values
      this.getDepressionConfirmations(this.selectedFormResponseID);
      this.toggleDepressionPatientFormEnabled();
    })
  }

  updateDepressionWorkbookConfirmations(workbookConfirmations: WorkbookConfirmation) {
    this.rest.updateWorkbookConfirmations(workbookConfirmations).subscribe(res => {
    })
  }
  
  //adding patient to the workbook
  AddDepressionPatientForWorkbook() {
    this.hasSelectedPatient = false;
    this.newDepressionWorkbookPatient = new WorkbookDepressionPatient();
    this.newDepressionWorkbookPatient.formResponseId = this.selectedFormResponseID;
    this.newDepressionWorkbookPatient.patientId = this.DepressionPatientForWorkbookForm.get('patientId').value;
    this.newDepressionWorkbookPatient.phone = this.DepressionPatientForWorkbookForm.get('phone').value;
    this.newDepressionWorkbookPatient.providerId = this.DepressionPatientForWorkbookForm.get('providerStaffID').value;
    this.newDepressionWorkbookPatient.dateOfService = this.DepressionPatientForWorkbookForm.get('dateOfService').value;
    this.newDepressionWorkbookPatient.phQ9_Score = this.DepressionPatientForWorkbookForm.get('pHQ9Score').value;    
    this.newDepressionWorkbookPatient.actionFollowUp = JSON.parse(this.DepressionPatientForWorkbookForm.controls.action.value);
    this.AddDepressionPatientToWorkbook(this.newDepressionWorkbookPatient, this.addingPatientName);
  }

  AddDepressionPatientToWorkbook(newWorkbookPatient: WorkbookDepressionPatient, patientName: string) {
    this.rest.AddPatientToDepressionWorkbook(this.newDepressionWorkbookPatient).pipe(take(1)).subscribe(res => {
      this.DepressionPatientForWorkbookForm.reset();
      this.getDepressionWorkbookPatients(this.selectedFormResponseID);
      this.searchPatient.reset();
      (res) ? this.snackBar.openSnackBar(`Patient ${patientName} added to the workbook`, 'Close', 'success-snackbar') : this.snackBar.openSnackBar(`Patient ${patientName} already exists on the current workbook`, 'Close', 'warn-snackbar')
    })
  }

  //For removing patient from the workbook
  onDepressionPatientDelete(element: any) {
    this.deletingPatientName = element.patient;
    this.deletingPatientId = element.patientId;
    this.dialog.open(this.DeletePatient);
  } 

  OnRemovePatientClick() {
    if (this.selectedWorkbookFormId == WorkbookFormValueEnum.depression){
      this.removeDepressionWorkbookPatient = new WorkbookDepressionPatient();
      this.removeDepressionWorkbookPatient.formResponseId = this.selectedFormResponseID;
      this.removeDepressionWorkbookPatient.patientId = this.deletingPatientId;
      this.RemoveDepressionPatientFromWorkbook(this.removeDepressionWorkbookPatient);
    }
    if (this.selectedWorkbookFormId == WorkbookFormValueEnum.asthma){
      this.removeAsthmaWorkbookPatient = new WorkbookAsthmaPatient();
    this.removeAsthmaWorkbookPatient.formResponseId = this.selectedFormResponseID;
    this.removeAsthmaWorkbookPatient.patientId = this.deletingPatientId;
    this.RemoveAsthmaPatientFromWorkbook(this.removeAsthmaWorkbookPatient);
    }

  }

  RemoveDepressionPatientFromWorkbook(removeWorkbookPatient: WorkbookDepressionPatient) {
    this.rest.RemovePatientFromWorkbook(this.removeDepressionWorkbookPatient).pipe(take(1)).subscribe(res => {
      this.DepressionPatientForWorkbookForm.reset();
      this.getDepressionWorkbookPatients(this.selectedFormResponseID);
      this.snackBar.openSnackBar(`Patient ${this.deletingPatientName} removed from the workbook`, 'Close', 'success-snackbar')
    })
  }
  RemoveAsthmaPatientFromWorkbook(removeWorkbookPatient: WorkbookAsthmaPatient) {
    this.rest.RemoveAsthmaPatientFromWorkbook(this.removeAsthmaWorkbookPatient).pipe(take(1)).subscribe(res => {
      this.AsthmaPatientForWorkbookForm.reset();
      this.getAsthmaWorkbookPatients(this.selectedFormResponseID);
      this.getAsthmaWorkbookPractice(this.selectedFormResponseID);
      this.snackBar.openSnackBar(`Patient ${this.deletingPatientName} removed from the workbook`, 'Close', 'success-snackbar');
    })
  }

  //for getting patients for specific reporting period of workbook
  getDepressionWorkbookPatients(formResponseid: number) {
    this.rest.getWorkbookDepressionPatients(formResponseid).pipe(take(1)).subscribe((data) => {
      this.workbookDepressionPatient = data;
      this.logger.log(this.workbookDepressionPatient, 'Workbook Patient')
      this.patientTableHeader = this.workbookDepressionPatient.length;
      this.dataSourceDepressionWorkbook = new MatTableDataSource(this.workbookDepressionPatient);
      this.dataSourceDepressionWorkbook.data = this.workbookDepressionPatient;
      this.DepressionPatientForWorkbookForm.get('action').setValue('false');
      this.toggleDepressionConfirmationEnabled(this.workbookDepressionPatient.length === 0);
    })
  }

  //for getting confirmation flags for depression worsheet
  getDepressionConfirmations(formResponseid: number) {
    this.rest.getWorkbookDepressionConfirmations(formResponseid).pipe(take(1)).subscribe((data) => {
      this.workbookDepressionConfirmations = data;
      this.ProviderConfirmationForm.get('allProvidersConfirm').setValue(this.workbookDepressionConfirmations.allProvidersConfirmed);
      this.PatientConfirmationForm.get('noPatientsConfirm').setValue(this.workbookDepressionConfirmations.noPatientsConfirmed);
      this.toggleDepressionPatientFormEnabled();
    })
  }

  trackDepressionPatients(index: number, item: WorkbookDepressionPatient): string {
    return '${item.patientId}';
  }
  
  trackByForPatientSearch(index, patient): string {
    return '${patient.patientId}';
  }

  getWorkbookReportingMonthsForPatient(patientName: string) {
    this.rest.getWorkbookReportingMonthsForPatient(patientName, this.selectedWorkbookFormId.toString()).pipe(take(1)).subscribe((data) => {
      this.workbookReportingPeriods = data;
        this.workbookReportingPeriods.forEach((element, index, reportData) => {
        this.logger.log(this.workbookReportingPeriods[0].formResponseId, "getWorkbookReportingMonthsForPatient");
        this.selectedFormResponseID = this.workbookReportingPeriods[0].formResponseId;
        this.selectedWorkbookFormId = this.workbookReportingPeriods[0].formId;
        this.setSelectedReportingPeriod(this.workbookReportingPeriods[0]);
        this.onReportingDateSelectionChangeForPatient(this.selectedFormResponseID);
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
        this.FollowupForm.get('phQ9FollowUpNotes').setValue(this.followUpQuestions.phQ9FollowUpNotes);
       
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
      this.followUpQuestions.oneMonthFolllowupPHQ9Score = this.FollowupForm.get('oneMonthFolllowupPHQ9Score').value;
      this.followUpQuestions.phQ9FollowUpNotes = this.FollowupForm.get('phQ9FollowUpNotes').value;
  
      this.UpdateFollowUpQuestionResponses(this.followUpQuestions);
    }
    UpdateFollowUpQuestionResponses(followUp: Followup) {
      this.rest.UpdateFollowUpQuestionResponses(followUp).pipe(take(1)).subscribe((data) => {
        this.getDepressionWorkbookPatients(this.selectedFormResponseID);
        this.CloseDialog();
  
      })
    }

    
  openEditProvidersDialog() {

    this.editProvidersForm.reset();
    this.getAllProviders();
    this.dialog.open(this.editProvidersDialog, { disableClose: true });
  }

  cancelEditProvidersDialog() {
    this.dialog.closeAll();
  }

  submitEditProviders(type) {
    let staffId = this.editProvidersForm.controls.pcpName.value.id;

    this.logger.log(staffId, 'StaffId');
    this.logger.log(this.selectedFormResponseID, 'FormResponseId');
    if (type === 1) {
      
    this.rest.AddProviderToWorkbook(staffId, this.selectedFormResponseID).subscribe(data => {    
      if (data){
        this.logger.log(staffId, 'New Workbook Provider');
        this.cancelEditProvidersDialog();
        this.editProvidersForm.reset();
        this.getDepressionWorkbookProviders(this.selectedFormResponseID);
        this.snackBar.openSnackBar(`Provider added to the workbook`, 'Close', 'success-snackbar');
      } 
      else 
      {
        this.logger.log('AddRemove Provider Error: API return data is null');
        this.snackBar.openSnackBar(`Oops! Something has gone wrong. Please contact your PHO Administrator`, 'Close', 'warn-snackbar');
      }
      },
      error => {
        if (error){
          this.logger.log('unexpected error caught: ' + error)
          this.snackBar.openSnackBar(`Oops! Something has gone wrong. Please contact your PHO Administrator`, 'Close', 'warn-snackbar');
        }
      });  
    }
    if (type === 2) {
      this.rest.RemoveProviderFromWorkbook(staffId, this.selectedFormResponseID).subscribe(data => {    
        if (data){
          this.logger.log(staffId, 'Removed Workbook Provider');
          this.cancelEditProvidersDialog();
          this.editProvidersForm.reset();
          this.getDepressionWorkbookProviders(this.selectedFormResponseID);
          this.snackBar.openSnackBar(`Provider removed from the workbook`, 'Close', 'success-snackbar');
        } 
        else 
        {
          this.logger.log('patient exists');
        }
        },
        error => {
          if (error){
            console.info('AddRemove Provider Error: ', error);
            this.logger.log('unexpected error caught: ' + error)
            this.snackBar.openSnackBar(`Oops! Something has gone wrong. Please contact your PHO Administrator`, 'Close', 'warn-snackbar');
          }
          else
          {
          this.snackBar.openSnackBar(`Oops! Something has gone wrong. Please contact your PHO Administrator`, 'Close', 'warn-snackbar');
          }
        });  
    }
  }
  
  onEditProviderSelectionChange(event: any): void {
    this.logger.log(event.value.name, 'edit provider selection changed');
    this.selectedEditProviderDisplay = event.value.name;
    this.selectedEditProviderId = event.value.id;
  }

  checkForExistingWorkbookProvider(id){
    return ((this.workbookProviders.filter(x => x.staffID === id).length > 0) ? 1 : 0);
  }  

//helper methods - asthma specific

  //for getting patients for specific reporting period of workbook
  getAsthmaWorkbookPatients(formResponseid: number) {
    this.rest.getWorkbookAsthmaPatients(formResponseid).pipe(take(1)).subscribe((data) => {
      this.workbookAsthmaPatient = data;
      this.logger.log(this.workbookAsthmaPatient, 'Workbook Patient');
      this.patientTableHeader = this.workbookAsthmaPatient.length;
      this.dataSourceAsthmaWorkbook = new MatTableDataSource(this.workbookAsthmaPatient);
      this.dataSourceAsthmaWorkbook.data = this.workbookAsthmaPatient;
      this.logger.log(this.workbookAsthmaPatient, 'Workbook Patient');
      this.AsthmaPatientForWorkbookForm.get('action').setValue('false');
      this.totalAsthmaPatients = this.workbookAsthmaPatient.length;
    })
  }

    //for getting patients for specific reporting period of workbook
    getTreatments() {
      this.treatmentList = [];
      this.rest.getTreatments().pipe(take(1)).subscribe((data) => {
        this.treatmentList = data;
        this.logger.log(this.treatmentList, 'Treatments');
      })
    }

  
  //adding patient to the workbook 
  AddAsthmaPatientForWorkbook() {
    this.hasSelectedPatient = false;
    this.newAsthmaWorkbookPatient = new WorkbookAsthmaPatient();
    this.newAsthmaWorkbookPatient = <WorkbookAsthmaPatient> this.AsthmaPatientForWorkbookForm.value;
    this.newAsthmaWorkbookPatient.treatment = this.treatmentList.find(t => t.treatmentId == this.AsthmaPatientForWorkbookForm.get('treatment').value);
    this.newAsthmaWorkbookPatient.formResponseId = this.selectedFormResponseID;
    this.newAsthmaWorkbookPatient.dob = null;
    this.newAsthmaWorkbookPatient.providerId = (!this.AsthmaPatientForWorkbookForm.get('providerStaffID').value ? 0 : JSON.parse(this.AsthmaPatientForWorkbookForm.get('providerStaffID').value));   
    this.newAsthmaWorkbookPatient.assessmentcompleted = (!this.AsthmaPatientForWorkbookForm.get('assessmentcompleted').value ? false : JSON.parse(this.AsthmaPatientForWorkbookForm.get('assessmentcompleted').value)); 
    this.newAsthmaWorkbookPatient.asthma_Score = (!this.AsthmaPatientForWorkbookForm.get('asthma_Score').value ? 0 : JSON.parse(this.AsthmaPatientForWorkbookForm.get('asthma_Score').value));
    this.logger.log(this.newAsthmaWorkbookPatient, "newworkbookasthmapatient"); 

    this.AddAsthmaPatientToWorkbook(this.newAsthmaWorkbookPatient, this.addingPatientName);
  }

  FormatAsthmaField(input: any){
    if (input != null && input.value != null){
      if(!isNaN(Number(input.value))){
        return Number(input.value);
      } else{
          return 0;
      }
    } else{
      return 0;
    }
  }

  AddAsthmaPatientToWorkbook(newWorkbookPatient: WorkbookAsthmaPatient, patientName: string) {
    this.rest.AddPatientToAsthmaWorkbook(newWorkbookPatient).pipe(take(1)).subscribe(res => {
      this.AsthmaPatientForWorkbookForm.reset();
      this.getAsthmaWorkbookPatients(this.selectedFormResponseID);
      this.getAsthmaWorkbookPractice(this.selectedFormResponseID);
      this.searchPatient.reset();
      (res) ? this.snackBar.openSnackBar(`Patient ${patientName} added to the workbook`, 'Close', 'success-snackbar') : this.snackBar.openSnackBar(`Patient ${patientName} already exists on the current workbook`, 'Close', 'warn-snackbar')
    })
  }

  trackAsthmaPatients(index: number, item: WorkbookAsthmaPatient): string {
    return '${item.patientId}';
  }
  
  //For removing patient from the workbook
  onAsthmaPatientDelete(element: any) {
    this.deletingPatientName = element.patient;
    this.deletingPatientId = element.patientId;
    this.dialog.open(this.DeletePatient);
  } 
  
  //Reset Add Patient Form
  resetAddAsthmaPatient() {
    this.AsthmaPatientForWorkbookForm.reset();
    this.hasSelectedPatient = false;
    this.AsthmaPatientForWorkbookForm.get('action').setValue('false');
  }

  
  //get workbook practice
  getAsthmaWorkbookPractice(formResponseid: number) {
    this.rest.getAsthmaWorkbookPractice(formResponseid).pipe(take(1)).subscribe((data) => {
      this.workbookPractice = data;
      this.logger.log(data, "getAsthmaWorkbookPractice");
    })
  }

    // for sorting the workbook patient table columns 
    onSortAsthmaData(sort: Sort) {
      const data = this.workbookAsthmaPatient.slice();
      if (!sort.active || sort.direction === '') {
        this.sortedAsthmaData = data;
        return;
      }
  
      this.sortedAsthmaData = data.sort((a, b) => {
        const isAsc = sort.direction === 'asc';
        switch (sort.active) {
          case 'patient': return this.compare(a.patient.toString(), b.patient.toString(), isAsc);
          case 'dob': return this.compare(a.dob, b.dob, isAsc);
          case 'provider': return this.compare(a.provider, b.provider, isAsc);
          case 'dateOfService': return this.compare(a.dateOfService, b.dateOfService, isAsc);
  
          default: return 0;
        }
      });
      this.table.dataSource = this.sortedData; 
    }

    PHQFollowUpQuestionValidators() {    

      const dateOfFollowupCall = this.FollowupForm.get('dateOfFollowupCall');
      const dateOfOneMonthVisit = this.FollowupForm.get('dateOfOneMonthVisit');    

      this.FollowupForm.get('followupPhoneCallOneToTwoWeeks').valueChanges
        .subscribe(followupPhoneCallOneToTwoWeeks =>{
          if (followupPhoneCallOneToTwoWeeks ==='Yes'){
            dateOfFollowupCall.setValidators([Validators.required]);             
          }
          else{            
            this.FollowupForm.get('dateOfFollowupCall').clearValidators();
            this.FollowupForm.get('dateOfFollowupCall').reset();
          }
          dateOfFollowupCall.updateValueAndValidity();
        })

        this.FollowupForm.get('oneMonthFollowupVisit').valueChanges
        .subscribe(oneMonthFollowupVisit =>{
          if (oneMonthFollowupVisit ==='Yes'){
            dateOfOneMonthVisit.setValidators([Validators.required]);              
          } 
          else
          {            
            this.FollowupForm.get('dateOfOneMonthVisit').clearValidators();
            this.FollowupForm.get('dateOfOneMonthVisit').reset();
          }     
          dateOfOneMonthVisit.updateValueAndValidity();
        })     
    }    
    
    //get QI workbook questions
    getQIWorkbookQuestions(formResponseid: number) {
      //const qiWorkbookQuestionsArray = this.QIWorkbookQuestionArray;  
      this.rest.getQIWorkbookQuestions(formResponseid).pipe(take(1)).subscribe((data) => {
        this.qiworkbooksections = data.qiSection;
        this.qiWorkbookParent = data;       
        this.logger.log(this.qiWorkbookParent);
        this.QIWorkbookSectionTrackingForm.patchValue({
          formResponseId: this.qiWorkbookParent.formResponseId,
          sectionId: this.qiWorkbookParent.qiSection[0].sectionId,
          sectionHeader: this.qiWorkbookParent.qiSection[0].sectionHeader,
          dataEntered: this.qiWorkbookParent.qiSection[0].dataEntered,
          questionId: this.qiWorkbookParent.qiSection[0].qiQuestion[0].questionId,                
          questionDEN: this.qiWorkbookParent.qiSection[0].qiQuestion[0].questionDEN,
          questionNUM: this.qiWorkbookParent.qiSection[0].qiQuestion[0].questionNUM,
          numeratorLabel: this.qiWorkbookParent.qiSection[0].qiQuestion[0].numeratorLabel,
          numerator: this.qiWorkbookParent.qiSection[0].qiQuestion[0].numerator,
          denominator: this.qiWorkbookParent.qiSection[0].qiQuestion[0].denominator  
        }); 
        this.qiSectionHeader =   this.qiWorkbookParent.qiSection[0].sectionHeader;
        this.qiQuestionNUM = this.qiWorkbookParent.qiSection[0].qiQuestion[0].questionNUM;
        this.qiQuestionDEN = this.qiWorkbookParent.qiSection[0].qiQuestion[0].questionDEN;
        this.qiNumeratorLabel = this.qiWorkbookParent.qiSection[0].qiQuestion[0].numeratorLabel;
        this.updateQuestion = this.qiWorkbookParent.qiSection[0].qiQuestion[0];
      }); 
    }

    onQIWorkbookChange() { 
      this.updateQuestion.numerator = parseInt(this.QIWorkbookSectionTrackingForm.get('numerator').value);
      this.updateQuestion.denominator = parseInt(this.QIWorkbookSectionTrackingForm.get('denominator').value);
      var dataEntered = this.QIWorkbookSectionTrackingForm.get('dataEntered').value;
      this.rest.updateQIWorkbookConfirmations(this.selectedFormResponseID, dataEntered, this.updateQuestion).subscribe(res => {
        //after we get this data, doublecheck for changed confirmation values
        //do extra stuff
        this.logger.log('made it');
        this.getQIWorkbookQuestions(this.selectedFormResponseID);
      })
    }


  //   AssignQIWorkbookQuestionsArray() {
      
  //     const qiWorkbookQuestionsArray = this.QIWorkbookQuestionArray;     
  //     let clearArray = this.QIWorkbookSectionTrackingForm.controls['QIWorkbookQuestionArray'] as FormArray;
     
  //     clearArray.clear();  
  //     // clearArray1.clear();      
      
  //     if (qiWorkbookQuestionsArray.length > 0) {
  //       qiWorkbookQuestionsArray.removeAt(0);
  //     }

  //     //var data = this.qiworkbooksections;
  //     this.qiworkbookquestions.forEach(results => {
  //       qiWorkbookQuestionsArray.push(this.fb.group(results)) ;            
  //     }); 
  //     console.log('test') ;    
  // } 

    
    // onValueChanges(): void {
    //   this.QIWorkbookQuestionsArray.controls['sectionHeader']
    //   .valueChanges
    //   .pipe(pairwise())
    //   .subscribe(([prev, next]: [any, any]) => {
    //     console.log('PREV1', prev);
    //     console.log('NEXT1', next);
    //   });     
    // }
   
  }

