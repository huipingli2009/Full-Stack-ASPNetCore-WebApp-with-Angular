import { Component, OnInit, ViewChild, Input, HostListener, TemplateRef } from '@angular/core';
import { Patients, PatientDetails, Gender, Conditions } from '../models/patients';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { RestService } from '../rest.service';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NGXLogger } from 'ngx-logger';
import { trigger, state, style, transition, animate } from '@angular/animations';
import { Observable, of } from 'rxjs';
import { tap, startWith, map } from 'rxjs/operators';
import { DataSource } from '@angular/cdk/collections';
import { get } from 'https';
import { PatientsDataSource } from './patients.datasource';
import { MatDialog } from '@angular/material/dialog';
import { DatePipe } from '@angular/common';
import { validateBasis } from '@angular/flex-layout';


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

  @Input()
  checked: Boolean;

  expandedElement: any;
  value = '';

  patients: Patients;
  patientRecords: number;
  patientFormDetails: Observable<PatientDetails>;
  patientDetails: PatientDetails;
  currentPatientId: number;
  filterValues: any = {};
  chronic: string;
  watchFlag: string;
  pcP_StaffID: string;
  gender: string;
  state: string;
  insurance: string;
  pcpId: number;
  pcpFullName: string;
  pmcaScore: string;
  providerPmcaScoreControl: string;
  providerNotesControl: string;
  epicMrn: string;
  filterFormGroup;
  conditions: string;
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
  filteredOptions: Observable<string[]>;
  isActive: boolean;
  form: FormGroup;
  insuranceList: any[] = [];
  genderList: any[] = [];
  pmcaList: any[] = [];
  stateList: any[] = [];

  isLoading = true;

  // Selected Values
  selectedGender;

  displayedColumns: string[] = ['arrow', 'name', 'dob', 'lastEDVisit', 'chronic', 'watchFlag', 'conditions'];
  compareFn: ((f1: any, f2: any) => boolean) | null = this.compareByValue;
  pageEvent: PageEvent;
  dataSource: PatientsDataSource;
  savedPatientData: any;

  isExpansionDetailRow = (i: number, row: object) => row.hasOwnProperty('detailRow');

  constructor(public rest: RestService, private route: ActivatedRoute, private router: Router,
    public fb: FormBuilder, private logger: NGXLogger, public dialog: MatDialog, private datePipe: DatePipe) {
    // this.filterFormGroup = this.fb.group({});
    this.form = this.fb.group({
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      dob: ['', Validators.required],
      email: ['', Validators.required],
      activeStatus: [Boolean, Validators.required],
      gender: ['', Validators.required],
      pcpName: ['', Validators.required],
      insuranceName: ['', Validators.required],
      conditionsControl: [null, Validators.required],
      providerPMCAScore: ['', Validators.required],
      providerNotes: ['', Validators.required],
      phone1: ['', Validators.required],
      addressLine1: ['', Validators.required],
      city: ['', Validators.required],
      state: ['', Validators.required],
      zip: ['', Validators.required]
    });
  }

  ngOnInit() {
    this.patients = this.route.snapshot.data.patients;
    this.dataSource = new PatientsDataSource(this.rest);
    this.loadPatientsWithFilters();
    this.getConditionsList();
    this.getPCPList();
    this.getPopSliceList();
    this.getInsuranceList();
    this.getGenderList();
    this.getPmca();
    this.getStates();
  }

  ngAfterViewInit() {
    // this.sort.sortChange.subscribe(() => this.paginator.pageIndex = 0);
    this.paginator.page
      .pipe(
        tap(() => this.loadPatientsPage())
      )
      .subscribe();
  }

  @HostListener('matSortChange', ['$event'])
  sortChange(e) {
    this.defaultSortedRow = e.active;
    this.defaultSortDirection = e.direction;
    this.loadPatientsWithFilters();
  }

  // compareByValue(f1: any, f2: any) { 
  //   return f1 && f2 && f1.value === f2.value; 
  // }
  compareByValue(o1, o2): boolean {
    return o1.name === o2.name;
  }

  loadPatientsWithFilters() {
    this.dataSource.loadPatients(this.defaultSortedRow, this.defaultSortDirection, 0, 20, this.chronic, this.watchFlag, this.conditions,
      this.providers, this.popSlices, this.patientNameSearch);
    this.rest.findPatients(this.defaultSortedRow, this.defaultSortDirection, 0, 20, this.chronic, this.watchFlag, this.conditions,
      this.providers, this.popSlices, this.patientNameSearch).subscribe((data) => {
        this.patientRecords = data[0].totalRecords;
      });
  }

  loadPatientsPage() {
    this.dataSource.loadPatients(
      this.defaultSortedRow,
      this.defaultSortDirection,
      this.paginator.pageIndex,
      this.paginator.pageSize,
      this.chronic,
      this.watchFlag,
      this.conditions,
      this.providers,
      this.popSlices,
      this.patientNameSearch);
  }


  applySelectedFilter(column: string, filterValue: string) {
    this.filterValues[column] = filterValue;

    // this.dataSource.filter = JSON.stringify(this.filterValues);

    // if (this.dataSource.paginator) {
    //   this.dataSource.paginator.firstPage();
    // }
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

  patientHasPopSlice() {
    this.loadPatientsWithFilters();
  }

  getConditionsList() {
    this.rest.getConditionsList().subscribe((data) => {
      this.conditionsList = data;
    });
  }

  patientHasCondition(e) {
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


  /*Patient Details */
  getPatientDetails(id) {
    this.rest.getPatientDetails(id).subscribe((data) => {
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

      let selectedValues = {
        firstName: data.firstName,
        lastName: data.lastName,
        dob: this.transformDob(data.patientDOB),
        email: data.email,
        activeStatus: data.activeStatus,
        gender: data.gender,
        pcpName: this.pcpFullName,
        insuranceName: data.insuranceName,
        conditionsControl: data.conditions,
        providerPMCAScore: data.providerPMCAScore,
        providerNotes: data.providerNotes,
        phone1: data.phone1,
        addressLine1: data.addressLine1,
        city: data.city,
        state: data.state,
        zip: data.zip
      }
      this.form.setValue(selectedValues);
      console.log(selectedValues);
      console.log(new Date(this.transformDobForPut(this.form.controls['dob'].value)))
      // this.patientFormDetails = this.rest.getPatientDetails(id).pipe(
      //   tap(user => this.form.patchValue(user))
      // );

    });
  }
  transformDob(date) {
    return this.datePipe.transform(date, 'MM/dd/yyyy');
  }
  transformDobForPut(date) {
    return this.datePipe.transform(date, 'yyyy-MM-ddT00:00:00');
  }

  submitForm() {
    this.patientDetails.firstName = this.form.controls['firstName'].value;
    this.patientDetails.lastName = this.form.controls['lastName'].value;
    this.patientDetails.patientDOB = new Date(this.transformDobForPut(this.form.controls['dob'].value));
    this.patientDetails.email = this.form.controls['email'].value;
    this.patientDetails.activeStatus = this.form.controls['activeStatus'].value;
    this.patientDetails.gender = this.form.controls['gender'].value;
    this.patientDetails.pcpFirstName = this.form.controls['pcpName'].value.split(' ')[0];
    this.patientDetails.pcpLastName = this.form.controls['pcpName'].value.split(' ')[1];
    this.patientDetails.insuranceName = this.form.controls['insuranceName'].value;
    this.patientDetails.conditions = this.form.controls['conditionsControl'].value;
    this.patientDetails.providerPMCAScore = this.form.controls['providerPMCAScore'].value;
    this.patientDetails.providerNotes = this.form.controls['providerNotes'].value;
    this.patientDetails.phone1 = this.form.controls['phone1'].value;
    this.patientDetails.addressLine1 = this.form.controls['addressLine1'].value;
    this.patientDetails.city = this.form.controls['city'].value;
    this.patientDetails.state = this.form.controls['state'].value;
    this.patientDetails.zip = this.form.controls['zip'].value;

    console.log('inSubmit', this.patientDetails);
    console.log(this.form.value);
    console.log(this.currentPatientId);
    this.rest.savePatientDetails(this.currentPatientId, this.patientDetails).subscribe(data => {
      this.savedPatientData = data;
    });
    console.log(this.savedPatientData)
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
  getStates() {
    this.rest.getState().subscribe((data) => {
      this.stateList = data;
    });
  }

  openPmcaDialog() {
    const dialogRef = this.dialog.open(this.callPmcaDialog, { disableClose: true });
  }




}
