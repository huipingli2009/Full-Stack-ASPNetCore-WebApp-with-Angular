import { animate, state, style, transition, trigger } from '@angular/animations';
import { DatePipe } from '@angular/common';
import { Component, ElementRef, HostListener, Input, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { MatDialog, MatDialogConfig } from '@angular/material/dialog';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { ActivatedRoute } from '@angular/router';
import { NGXLogger } from 'ngx-logger';
import { Observable, Subscription } from 'rxjs';
import { tap, take } from 'rxjs/operators';
import { Conditions, Gender, PatientDetails, Patients, NewPatient, patientAdminActionTypeEnum, potentialPtStaus, addPatientProcessEnum } from '../models/patients';
import { RestService } from '../rest.service';
import { PatientsDataSource } from './patients.datasource';
import { FilterService } from '../services/filter.service';
import { MatButtonModule } from '@angular/material/button';
import { MatSnackBarComponent } from '../shared/mat-snack-bar/mat-snack-bar.component';
import { UserService } from '../services/user.service';
import { CurrentUser, User } from '../models/user';
import { DrilldownService } from '../drilldown/drilldown.service';

@Component({
  selector: 'app-patients',
  templateUrl: './patients.component.html',
  styleUrls: ['./patients.component.scss'],
  animations: [
    trigger('detailExpand', [
      state('collapsed', style({ height: '0px', minHeight: '0', visibility: 'hidden' })),
      state('expanded', style({ height: '*', visibility: 'visible' })),
      transition('expanded <=> collapsed', animate('225ms cubic-bezier(0.4, 0.0, 0.2, 1)')),
    ]),
  ],
})

export class PatientsComponent implements OnInit {

  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild('pmcaDialog') callPmcaDialog: TemplateRef<any>;
  @ViewChild('updatePatientDialog') callPatientSaveDialog: TemplateRef<any>;
  @ViewChild('adminDialog') adminDialog: TemplateRef<any>;
  @ViewChild('adminConfirmDialog') adminConfirmDialog: TemplateRef<any>;
  @ViewChild('patientAdminDialog') patientAdminDialog: TemplateRef<any>;
  @ViewChild('patientAdminConfirmDialog') patientAdminConfirmDialog: TemplateRef<any>;

  @Input()
  checked: Boolean;  

  //if patient is selected from ED chart
  get selectedPatientId(): number | null {
    return this.rest.selectedPatientId;
  }

  get selectedPatientName(): string | null {
    return this.rest.selectedPatientName;
  }

  expandedElement: any;
  value = '';

  patients: Patients;
  patientRecords: number;
  patientFormDetails: Observable<PatientDetails>;
  patientDetails: PatientDetails;
  currentPatientId: number;
  currentUser: CurrentUser;
  currentUserId: number;
  filterValues: any = {};
  chronic: string;
  watchFlag: string;
  pcP_StaffID: string;
  gender: string;
  genderMap: any = { M: 'Male', F: 'Female', U: 'Unknown' };
  state: string;
  insurance: string;
  pcpId: number;
  pcpFullName: string;
  pmcaScore: string;
  providerPmcaScoreControl: string;
  providerNotesControl: string;
  epicMrn: string;
  filterFormGroup;
  conditions: Array<string>;
  conditionsList: Array<Conditions>;
  providers: string;
  providersList: any[] = [];
  popSlices: string;
  popSliceList: any[] = [];
  options: string[];
  defaultSortedRow = 'name';
  defaultSortDirection = 'asc';
  patientNameControl = new FormControl();
  patientNameSearch: string;
  patientNameSearchValue: string;
  filteredOptions: Observable<string[]>;
  isActive: boolean;
  potentialPatient: boolean = false;
  form: FormGroup;
  insuranceList: any[] = [];
  genderList: Gender;
  pmcaList: any[] = [];
  stateList: any[] = [];
  LocationList: Location[];
  newPatientValues: NewPatient;
  addPatientForm: FormGroup;
  patientAdminForm: FormGroup;
  patientAdminForm_DuplicatePatients: FormGroup;
  isLoading = true;
  isAddingPatientAndContinue: boolean;
  isAddingPatientAndExit: boolean;
  isUserAdmin: boolean;
  acceptPatient: boolean;
  declinePatient: boolean;
  mergeWithNewPatient: boolean;
  mergeWithOldPatient: boolean;
  possibleDuplicatePatient: boolean;
  patientAdminActionEnum = patientAdminActionTypeEnum;
  addNewPatientProcessEnum = addPatientProcessEnum;

  //Outcome Pop list
  outcomes: string;
  outcomePopList: any[] = []; 

  // Selected Values
  selectedGender;

  displayedColumns: string[] = ['arrow', 'name', 'dob', 'lastEDVisit', 'chronic', 'watchFlag', 'conditions', 'button'];
  compareFn: ((f1: any, f2: any) => boolean) | null = this.compareByValue;
  compareShortFn: ((f1: any, f2: any) => boolean) | null = this.compareByShortValue;
  pageEvent: PageEvent;
  dataSource: PatientsDataSource;
  savedPatientData: any;
  isDisabled: boolean;
  subscription: Subscription;
  patientSubscription: Subscription;
  isFilteringPatients: boolean;
  isFilteringOutcomes: boolean;
  isFilteringConditions: boolean;
  filterType: string;

  isExpansionDetailRow = (i: number, row: object) => row.hasOwnProperty('detailRow');

  constructor(public rest: RestService, private route: ActivatedRoute, public fb: FormBuilder, private userService: UserService,
    private logger: NGXLogger, public dialog: MatDialog, private datePipe: DatePipe, private snackBar: MatSnackBarComponent,
    private filterService: FilterService, private drilldownService: DrilldownService) {
    this.form = this.fb.group({
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      dob: ['', Validators.required],
      email: [''],
      activeStatus: [Boolean],
      gender: [''],
      pcpName: [''],
      insuranceName: [''],
      conditionsControl: [null],
      providerPMCAScore: [''],
      providerNotes: [''],
      phone1: [''],
      addressLine1: [''],
      city: [''],
      state: [''],
      zip: [''],
      locations: ['']
    });

    this.addPatientForm = this.fb.group({
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      dob: ['', Validators.required],
      gender: ['', Validators.required],
      pcpName: ['', Validators.required]
    });

    this.patientAdminForm = this.fb.group({
      firstName: '',
      lastName: '',
      patientDOB: '',
      gender: '',
      genderId: '',
      pcpId: '',
      pcpName: ''
    });

    this.patientAdminForm_DuplicatePatients = this.fb.group({
      potentialDuplicateFirstName: '',
      potentialDuplicateLastName: '',
      potentialDuplicateDOB: '',
      potentialDuplicatePCPFirstName: '',
      potentialDuplicatePCPLastName: '',
      potentialDuplicateGender: '',
      potentialDuplicatePatientMRNId: '',
      potentialDuplicatePCPName: ''
    });

    this.subscription = this.filterService.getIsFilteringPatients().subscribe(res => {
      this.isFilteringPatients = res;
    });
    //TH - check for outcome filtering    
    this.subscription = this.filterService.getIsFilteringOutcomes().subscribe(res => {
      this.isFilteringOutcomes = res;
      this.logger.log(res, "Patients: IsFilteringOutcomes ", res);
    });
    //TH - check for condition filtering    
    this.subscription = this.filterService.getIsFilteringConditions().subscribe(res => {
      this.isFilteringConditions = res;
      this.logger.log(res, "Patients: IsFilteringConditions ", res);
    });
    this.subscription = this.filterService.getFilterType().subscribe(res => { 
      this.filterType = res;
      this.logger.log(res, "Patients: getFilterType ", res);
    });

  }

  ngOnInit() {
    this.patients = this.route.snapshot.data.patients;
    this.dataSource = new PatientsDataSource(this.rest);
    this.getCurrentUser();
    this.getConditionsList();
    this.loadPatientsWithFilters();
    this.getPCPList();
    this.getPopSliceList();
    this.getInsuranceList();
    this.getGenderList();
    this.getPmca();
    this.getStates();
    this.getPrimaryLocations(); 
    this.getOutcomeMetricList();    
  }

  ngAfterViewInit() {
    this.paginator.page
      .pipe(
        tap(() => this.loadPatientsPage())
      )
      .subscribe();
  }

  ngOnDestroy() {
    this.subscription.unsubscribe();
    if (this.patientSubscription && !this.patientSubscription.closed) {
      this.patientSubscription.unsubscribe();
    }
    this.filterService.updateIsFilteringPatients(false);
    this.filterService.updateIsFilteringOutcomes(false);
    this.filterService.updateIsFilteringConditions(false);
    this.filterService.updateFilterType('');
  }

  @HostListener('matSortChange', ['$event'])
  sortChange(e) {
    this.defaultSortedRow = e.active;
    this.defaultSortDirection = e.direction;
    this.loadPatientsWithFilters();
  }

  compareByValue(o1, o2): boolean {
    return o1.id === o2.id;
  }

  compareByShortValue(o1, o2): boolean {
    return o1.shortName === o2.shortName;
  } 

  getCurrentUser() {
    this.userService.getCurrentUser().pipe(take(1)).subscribe((data) => {
      this.currentUser = data;
      this.currentUserId = data.id;
      if (data.role.id === 3) {
        this.isUserAdmin = true;
      } else { this.isUserAdmin = false; }
    });
  }

  loadPatientsWithFilters() {
    if (this.isFilteringPatients === true) { // This is to handle if the user is coming from Dashboard or not.
      this.popSlices = this.filterType;
    }
    if (this.isFilteringOutcomes === true){
      this.outcomes = this.filterType;
    }
    if (this.isFilteringConditions === true){
      this.conditions = [this.filterType];
    }

    //if patient is coming from ED chart
    if (this.selectedPatientId) {
      this.patientNameSearch = this.selectedPatientId.toString();
      this.patientNameSearchValue = this.selectedPatientName;
      this.getPatientDetails(this.selectedPatientId);
      this.patientSubscription = this.dataSource.PatientData$.subscribe((patients) => {
        this.logger.log("Patients", patients);
        this.logger.log("selected Patient ID", this.selectedPatientId);
        //assign the expanded element only when there is one record in the table
        if (patients.length == 1) {
          this.expandedElement = patients[0];
        }
        this.logger.log("selected Patient", this.expandedElement);
      });
    }

    this.potentialPatient = +(this.popSlices) == potentialPtStaus.PopSlice ? true : false;

    this.dataSource.loadPatients(this.defaultSortedRow, this.defaultSortDirection, 0, 20, this.chronic, this.watchFlag, this.conditions.toString(),
      this.providers, this.popSlices,  this.outcomes, this.patientNameSearch);
    this.rest.findPatients(this.defaultSortedRow, this.defaultSortDirection, 0, 20, this.chronic, this.watchFlag, this.conditions.toString(),
      this.providers, this.popSlices,  this.outcomes, this.patientNameSearch).subscribe((data) => {
        this.patientRecords = data[0].totalRecords;
      });


    //reset selectedPatientId
    this.rest.selectedPatientId = null;
    this.rest.selectedPatientName = null;
    //reset outcome filter
    this.filterService.updateIsFilteringOutcomes(false);
    this.filterService.updateIsFilteringConditions(false);
  }

  loadPatientsPage() {
    this.dataSource.loadPatients(
      this.defaultSortedRow,
      this.defaultSortDirection,
      this.paginator.pageIndex,
      this.paginator.pageSize,
      this.chronic,
      this.watchFlag,
      this.conditions.toString(),
      this.providers,
      this.popSlices,
      this.outcomes,
      this.patientNameSearch);
  }

  applySelectedFilter(column: string, filterValue: string) {
    this.filterValues[column] = filterValue;
  }

  isChronicFilter(e) {
    if (e.checked === true) {
      this.chronic = 'true';
      this.loadPatientsWithFilters();
    } else {
      this.chronic = '';
      this.loadPatientsWithFilters();
    }
  }

  isOnWatchlist(e) {
    if (e.checked === true) {
      this.watchFlag = 'true';
      this.loadPatientsWithFilters();
    } else {
      this.watchFlag = '';
      this.loadPatientsWithFilters();
    }
  }

  getPopSliceList() {
    this.rest.getPopSliceList().subscribe((data) => {
      this.popSliceList = data;
    });
  }

  getOutcomeMetricList(){
    this.rest.GetPopulationOutcomeMetrics().subscribe(data =>{
      this.outcomePopList = data;
    })
  }

  patientHasPopSlice() {
    this.loadPatientsWithFilters();
  }

  getConditionsList() {
    this.rest.getConditionsList().subscribe((data) => {
      this.conditionsList = data;
    });
  }

  patientHasCondition(e) {
    this.logger.log(e, "patientHasConditon argument: ");
    this.logger.log(this.conditions, "patientHasCondition conditions: ")
    this.loadPatientsWithFilters();
  }

  outcomesFilterSelected(){
    this.loadPatientsWithFilters();
  }

  getPCPList() {
    this.rest.getPCPList().subscribe((data) => {
      data.forEach(element => {
        element.name = element.name.split(',', 3)[0];
      });
      this.providersList = data;
    });
  }
  patientHasPCP() {
    this.loadPatientsWithFilters();
  }

  searchPatientsByName(e) {
    this.patientNameSearch = e.target.value;
    this.loadPatientsWithFilters();
  }

  updateWatchlistStatus(id, index) {
    this.rest.updateWatchlistStatus(id).subscribe((data) => {
    });
  }

  //ADD PATIENT
  openAdminAddDialog() {

    this.addPatientForm.reset();
    this.dialog.open(this.adminDialog, { disableClose: true });
    this.isActive = true;
  }

  // Type = submission type. 1 = Save And Continue / 2 = Save And Exit
  openAdminConfirmDialog(type) {
    if (type === addPatientProcessEnum.SaveAndContinue) {
      this.isAddingPatientAndContinue = true;
      this.isAddingPatientAndExit = false;
    }
    if (type === addPatientProcessEnum.SaveAndExit) {
      this.isAddingPatientAndContinue = false;
      this.isAddingPatientAndExit = true;
    }
    this.dialog.open(this.adminConfirmDialog, { disableClose: true });
  }


  submitPatientAddUpdate() {
    this.newPatientValues = this.addPatientForm.value;
    this.newPatientValues.firstName = this.addPatientForm.controls.firstName.value;
    this.newPatientValues.lastName = this.addPatientForm.controls.lastName.value;
    this.newPatientValues.dob = new Date(this.transformDobForPut(this.addPatientForm.controls.dob.value));
    this.newPatientValues.genderId = this.addPatientForm.controls.gender.value.id;
    this.newPatientValues.pcP_StaffID = this.addPatientForm.controls.pcpName.value.id;


    this.logger.log('inSubmitPatientAddUpdate', this.newPatientValues);
    this.logger.log(this.addPatientForm.value);
    this.rest.addPatient(this.newPatientValues).subscribe(data => {
      if (data) {
        let id = <number>data;
        this.logger.log(id, 'New Patient');
        if (this.isAddingPatientAndContinue) {
          this.patientNameSearch = id.toString();
          this.loadPatientsWithFilters();
        }
        if (this.isAddingPatientAndExit) {
          this.loadPatientsPage();
        }
        this.snackBar.openSnackBar(`Patient ${this.newPatientValues.firstName + ' ' + this.newPatientValues.lastName} added to the registry`, 'Close', 'success-snackbar');
      }
      else {
        this.logger.log('patient exists');
      }
      this.cancelAdminDialog();


    },
      error => {
        if (error) {
          console.info('AddPatient Error: ', error);
          if (error === 'patient already exists') {
            this.logger.log('patient exists: ' + this.newPatientValues.firstName + ' ' + this.newPatientValues.lastName);
            this.snackBar.openSnackBar(`Patient ${this.newPatientValues.firstName + ' ' + this.newPatientValues.lastName} already exists in registry`, 'Close', 'warn-snackbar');
          }
          else {
            this.logger.log('unexpected error caught: ' + error)
            this.snackBar.openSnackBar(`Oops! Something has gone wrong. Please contact your PHO Administrator`, 'Close', 'warn-snackbar');
          }

        }
        else {
          this.snackBar.openSnackBar(`Oops! Something has gone wrong. Please contact your PHO Administrator`, 'Close', 'warn-snackbar');
        }

      });

  }


  cancelAdminDialog() {
    this.isAddingPatientAndContinue = false;
    this.isAddingPatientAndExit = false;
    this.loadPatientsPage();
    this.dialog.closeAll();
  }

  cancelPatientAdminDialog() {
    this.dialog.closeAll();

  }


  /*Patient Details */
  getPatientDetails(id) {
    this.rest.getPatientDetails(id, this.potentialPatient).subscribe((data) => {
      this.currentPatientId = data.id;
      this.isLoading = false;
      this.patientDetails = data;
      this.isActive = data.activeStatus;
      this.pmcaScore = data.pmcaScore;
      this.epicMrn = data.patientMRNId;
      this.pcpFullName = `${data.pcpFirstName} ${data.pcpLastName}`;
      this.providerPmcaScoreControl = data.providerPMCAScore;
      this.providerNotesControl = data.providerNotes;
      this.selectedGender = data.gender;

      const selectedValues = {
        firstName: data.firstName,
        lastName: data.lastName,
        dob: this.transformDob(data.patientDOB),
        email: data.email,
        activeStatus: data.activeStatus,
        gender: {
          id: data.genderId,
          shortName: data.gender
        },
        pcpName: {
          id: data.pcpId,
          name: this.pcpFullName
        },
        insuranceName: {
          id: data.insuranceId,
          name: data.insuranceName
        },
        conditionsControl: data.conditions,
        providerPMCAScore: data.providerPMCAScore,
        providerNotes: data.providerNotes,
        phone1: data.phone1,
        addressLine1: data.addressLine1,
        city: data.city,
        state: {
          id: data.stateId,
          shortName: data.state
        },
        zip: data.zip,
        locations:{
          id: data.primaryLocationId,
          name: data.primaryLocation
        }
      };
      this.form.setValue(selectedValues);
    });
  }
  transformDob(date) {
    return this.datePipe.transform(date, 'MM/dd/yyyy');
  }
  transformDobForPut(date) {
    return this.datePipe.transform(date, 'yyyy-MM-ddT00:00:00');
  }

  updatePmcaScore() {
    const { value, valid } = this.form;
    if (valid) {
      this.providerPmcaScoreControl = this.form.controls.providerPMCAScore.value;
      this.providerNotesControl = this.form.controls.providerNotes.value;
      this.form.controls.providerNotes.setValidators([]);
      this.dialog.closeAll();
    }

  }

  pmcaProviderScoreChanged() {
    this.form.controls.providerNotes.setValidators([Validators.required]);
    this.form.controls.providerNotes.updateValueAndValidity();
  }

  cancelPmcaUpdate() {
    this.form.controls.providerNotes.setValidators([]);
    this.form.controls.providerNotes.updateValueAndValidity();
    this.form.controls.providerPMCAScore.setValue(this.providerPmcaScoreControl);
    this.form.controls.providerNotes.setValue(this.providerNotesControl);
    this.dialog.closeAll();
  }

  openPatientSaveDialog() {
    const { value, valid } = this.form;
    if (valid) {
      this.dialog.open(this.callPatientSaveDialog, { disableClose: true });
    }
  }

  submitForm() {
    this.patientDetails.firstName = this.form.controls.firstName.value;
    this.patientDetails.lastName = this.form.controls.lastName.value;
    this.patientDetails.patientDOB = new Date(this.transformDobForPut(this.form.controls.dob.value));
    this.patientDetails.email = this.form.controls.email.value;
    this.patientDetails.activeStatus = this.form.controls.activeStatus.value;
    this.patientDetails.genderId = this.form.controls.gender.value.id;
    this.patientDetails.gender = this.form.controls.gender.value.shortName;
    this.patientDetails.pcpId = this.form.controls.pcpName.value.id;
    this.patientDetails.pcpFirstName = this.form.controls.pcpName.value.name.split(' ')[0];
    this.patientDetails.pcpLastName = this.form.controls.pcpName.value.name.split(' ')[1];
    this.patientDetails.insuranceId = this.form.controls.insuranceName.value.id;
    this.patientDetails.insuranceName = this.form.controls.insuranceName.value.name;
    this.patientDetails.conditions = this.form.controls.conditionsControl.value;
    this.patientDetails.providerPMCAScore = Number(this.form.controls.providerPMCAScore.value);
    this.patientDetails.providerNotes = this.form.controls.providerNotes.value;
    this.patientDetails.phone1 = this.form.controls.phone1.value;
    this.patientDetails.addressLine1 = this.form.controls.addressLine1.value;
    this.patientDetails.city = this.form.controls.city.value;
    this.patientDetails.stateId = this.form.controls.state.value.id;
    this.patientDetails.state = this.form.controls.state.value.shortName;
    this.patientDetails.zip = this.form.controls.zip.value;
    this.patientDetails.primarylocationId = this.form.controls.locations.value.id;
    this.patientDetails.primarylocation = this.form.controls.locations.value.name;

    this.logger.log('inSubmit', this.patientDetails);
    this.logger.log(this.form.value);
    this.logger.log(this.currentPatientId);
    this.rest.savePatientDetails(this.currentPatientId, this.patientDetails).subscribe(data => {
      this.savedPatientData = data;
      this.patientNameSearch = '';
      this.loadPatientsWithFilters();
    });
  }

  /* Patient Details - Form Elements*/
  getInsuranceList() {
    this.rest.getInsurance().subscribe((data) => {
      this.insuranceList = data;
    });
  }
  getGenderList() {
    this.rest.getGender().subscribe((data) => {
      this.genderList = data;
    });
  }
  getPmca() {
    this.rest.getPmca().subscribe((data) => {
      this.pmcaList = data;
    });
  }

  getPrimaryLocations() {
    this.rest.getLocations().pipe(take(1)).subscribe((data) => {
      this.LocationList = data;
    });
  }
  getStates() {
    this.rest.getState().subscribe((data) => {
      this.stateList = data;
    });
  }

  openPmcaDialog() {
    const dialogRef = this.dialog.open(this.callPmcaDialog, { disableClose: true });
  }

  openPatientAdminDialog(id: number) {

    this.patientAdminForm.patchValue({
      firstName: this.patientDetails.firstName,
      lastName: this.patientDetails.lastName,
      patientDOB: this.transformDob(this.patientDetails.patientDOB),
      gender: this.patientDetails.gender,
      genderId: this.patientDetails.genderId,
      pcpId: this.patientDetails.pcpId,
      pcpName: this.patientDetails.pcpFirstName + " " + this.patientDetails.pcpLastName
    });

    this.patientAdminForm_DuplicatePatients.patchValue({
      potentialDuplicateFirstName: this.patientDetails.potentialDuplicateFirstName,
      potentialDuplicateLastName: this.patientDetails.potentialDuplicateLastName,
      potentialDuplicateDOB: this.transformDob(this.patientDetails.potentialDuplicateDOB),
      potentialDuplicatePCPFirstName: this.patientDetails.potentialDuplicatePCPFirstName,
      potentialDuplicatePCPLastName: this.patientDetails.potentialDuplicatePCPLastName,
      potentialDuplicatePCPName: this.patientDetails.potentialDuplicatePCPFirstName + ' ' + this.patientDetails.potentialDuplicatePCPLastName,
      potentialDuplicateGender: this.patientDetails.potentialDuplicateGender,
      potentialDuplicatePatientMRNId: this.patientDetails.potentialDup_PAT_MRN_ID
    });

    this.possibleDuplicatePatient = +((this.patientDetails.potentialDuplicateFirstName) != '' && (this.patientDetails.potentialDuplicateLastName) != '') ? true : false;

    this.dialog.open(this.patientAdminDialog, { disableClose: true });
  }

  openPatientAdminConfirmDialog(type) {
    if (type === patientAdminActionTypeEnum.Accept) {
      this.acceptPatient = true;
      this.declinePatient = false;
      this.mergeWithNewPatient = false;
      this.mergeWithOldPatient = false;
    }
    if (type === patientAdminActionTypeEnum.Decline) {
      this.acceptPatient = false;
      this.declinePatient = true;
      this.mergeWithNewPatient = false;
      this.mergeWithOldPatient = false;
    }

    if (type === patientAdminActionTypeEnum.Update) {
      this.acceptPatient = false;
      this.declinePatient = false;
      this.mergeWithNewPatient = true;
      this.mergeWithOldPatient = false;
    }

    this.dialog.open(this.patientAdminConfirmDialog, { disableClose: true });
  }

  openPatientAdminAcceptDialog() {
    this.dialog.closeAll();
  }

  submitPotentialPatientForm(choice: number) {

    const potentialPatientId: number = this.patientDetails.id;
    let message = '';

    if (choice == patientAdminActionTypeEnum.Accept) {
      this.logger.log('Potential Patient Added');
      message = 'Accepted';
    }

    if (choice == patientAdminActionTypeEnum.Decline) {
      this.logger.log('Potential Patient Declined');
      message = 'Declined';
    }

    if (choice == patientAdminActionTypeEnum.Update) {
      this.logger.log('Updated existing patient with new patient');
      message = 'Updated';
    }

    this.rest.addPotentialPatient(potentialPatientId, choice).subscribe(data => {
      this.snackBar.openSnackBar(`Patient: ${this.patientDetails.firstName + ' ' + this.patientDetails.lastName} was ${message}`, 'Close', 'success-snackbar');

      this.loadPatientsWithFilters();
    });

    this.dialog.closeAll();
  }

  openDrilldownDialog(measure, display) {
    var drilldownOptions = {
      measureId: measure, //'1',
      filterId: this.currentPatientId, //'381886'
      displayText: display
    };

    this.drilldownService.open(drilldownOptions);
  }

  onClicked(value: number) {
    if (value > 0) {
      this.patientNameSearch = value.toString();
      this.loadPatientsWithFilters();
    }
    else {
      console.log('An error occurs');
    }
  }
}

