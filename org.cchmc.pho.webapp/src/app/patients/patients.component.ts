import { Component, OnInit, ViewChild, Input, HostListener } from '@angular/core';
import { Patients, PatientDetails } from '../models/patients';
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

  @ViewChild(MatSort, {static: true}) sort: MatSort;
  @ViewChild(MatPaginator) paginator: MatPaginator;

  @Input()
  checked: Boolean;

  expandedElement: any;
  value = '';

  patients: Patients;
  patientFormDetails: Observable<PatientDetails>;
  patientDetails: PatientDetails;
  filterValues: any = {};
  chronic: string;
  watchFlag: string;
  pcP_StaffID: string;
  gender: string;
  filterFormGroup;
  conditions: string;
  conditionsList: any[] = [];
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

  displayedColumns: string[] = ['arrow', 'name', 'dob', 'lastEDVisit', 'chronic', 'watchFlag', 'conditions'];
  pageEvent: PageEvent;
  dataSource: PatientsDataSource;

  isExpansionDetailRow = (i: number, row: object) => row.hasOwnProperty('detailRow');

  constructor(public rest: RestService, private route: ActivatedRoute, private router: Router,
              public fb: FormBuilder, private logger: NGXLogger) { 
                this.filterFormGroup = this.fb.group({});
              }

  ngOnInit() {
    this.patients = this.route.snapshot.data['patients'];
    // console.log(this.route.snapshot.data["patients"]);
    this.dataSource = new PatientsDataSource(this.rest);
    this.loadPatientsWithFilters();
    this.getConditionsList();
    this.getPCPList();
    this.getPopSliceList();
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

loadPatientsWithFilters() {
  this.dataSource.loadPatients(this.defaultSortedRow, this.defaultSortDirection, 0, 20, this.chronic, this.watchFlag, this.conditions,
    this.providers, this.popSlices, this.patientNameSearch);
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
    })
  }

  patientHasPopSlice() {
    this.loadPatientsWithFilters();
  }

  getConditionsList() {
    this.rest.getConditionsList().subscribe((data) => {
      this.conditionsList = data;
    })
  }

  patientHasCondition(e) {
    this.loadPatientsWithFilters();
  }

  getPCPList() {
    this.rest.getPCPList().subscribe((data) => {
      this.providersList = data;
    })
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
      this.patientFormDetails = data;
      this.patientDetails = data;
      this.isActive = data.activeStatus;
      this.form = this.fb.group({
        activeStatus: [Boolean, Validators.required],
        patientMRNId: ['', Validators.required],
        gender: ['', Validators.required],
        pcpLastName: ['', Validators.required],
        insuranceName: ['', Validators.required],
        conditions: ['', Validators.required],
        pmcaScore: ['', Validators.required],
        providerPMCAScore: ['', Validators.required],
        phone1: ['', Validators.required],
        addressLine1: ['', Validators.required],
        city: ['', Validators.required],
        state: ['', Validators.required],
        zip: ['', Validators.required]
      });
      this.patientFormDetails = this.rest.getPatientDetails(id).pipe(
        tap(user => this.form.patchValue(user))
      );
      console.log(this.patientDetails);
      
    });
  }
  
  
  


}
