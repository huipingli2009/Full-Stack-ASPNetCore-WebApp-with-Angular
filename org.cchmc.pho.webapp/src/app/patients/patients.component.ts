import { Component, OnInit, ViewChild, Input } from '@angular/core';
import { Patients, PatientDetails } from '../models/patients';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { RestService } from '../rest.service';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NGXLogger } from 'ngx-logger';
import { trigger, state, style, transition, animate } from '@angular/animations';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';


@Component({
  selector: 'app-patients',
  templateUrl: './patients.component.html',
  styleUrls: ['./patients.component.scss'],
  animations: [
    trigger('detailExpand', [
      state('void', style({ height: '0px', minHeight: '0', visibility: 'hidden' })),
      state('*', style({ height: '*', visibility: 'visible' })),
      transition('void <=> *', animate('225ms cubic-bezier(0.4, 0.0, 0.2, 1)')),
    ]),
  ],
})

export class PatientsComponent implements OnInit {

  @ViewChild(MatSort, {static: true}) sort: MatSort;
  @ViewChild(MatPaginator, {static: true}) paginator: MatPaginator;

  @Input()
  checked: Boolean;



  patients: Patients[];
  patientFormDetails: Observable<PatientDetails>;
  patientDetails: PatientDetails[];
  filterValues: any = {};
  chronic: string;
  watchFlag: string;
  pcP_StaffID: string;
  gender: string;
  filterFormGroup;
  conditions: string;
  conditionsList: any[] = [];
  public multiFilterValues = {
    condition: ""
  };
  isActive: boolean;
  form: FormGroup;
  selectedPatient: PatientDetails;

  displayedColumns: string[] = ['arrow', 'name', 'dob', 'lastEDVisit', 'chronic', 'watchFlag', 'conditions'];
  dataSource;

  isExpansionDetailRow = (index, row) => row[index].hasOwnProperty('detailRow');

  constructor(public rest: RestService, private route: ActivatedRoute, private router: Router,
              public fb: FormBuilder, private logger: NGXLogger) { 
                this.filterFormGroup = this.fb.group({});
              }

  ngOnInit() {
    this.getAllPatients();
  }

  getAllPatients() {
    this.rest.getAllPatients().subscribe((data) => {
      this.patients = data;
      this.dataSource = new MatTableDataSource<Patients>(this.patients);
      this.dataSource.sort = this.sort;
      this.dataSource.paginator = this.paginator;
      // this.patients.forEach((patient, index) => {
      //   if (Array.isArray(patient.conditions) && patient.conditions.length > 1) {
      //     console.log(patient.conditions.length);
      //     if (patient.conditions[index].name !== undefined) {
      //       this.conditionsList.push({
      //         condition: patient.conditions[index].name
      //       });
      //     }
      //   }
      //   console.log(this.conditionsList);
      // });
      this.dataSource.filterPredicate = ((data: Patients, filter): boolean => {
        const filterValues = JSON.parse(filter);
  
        return (this.chronic ? data.chronic.toString().trim().toLowerCase().indexOf(filterValues.chronic) !== -1 : true)
        && (this.watchFlag ? data.watchFlag.toString().trim().toLowerCase().indexOf(filterValues.watchFlag) !== -1 : true)
        && (this.pcP_StaffID ? data.pcP_StaffID.toString().trim().toLowerCase().indexOf(filterValues.pcP_StaffID) !== -1 : true)
        && (this.conditions ? data.conditions.toString().trim().toLowerCase().indexOf(filterValues.conditions) !== -1 : true); // Conditions is not working. Need to revisit
      })
    });
  }

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
  
  applySelectedFilter(column: string, filterValue: string) {
    this.filterValues[column] = filterValue;

    this.dataSource.filter = JSON.stringify(this.filterValues);

    if (this.dataSource.paginator) {
      this.dataSource.paginator.firstPage();
    }
  }


}