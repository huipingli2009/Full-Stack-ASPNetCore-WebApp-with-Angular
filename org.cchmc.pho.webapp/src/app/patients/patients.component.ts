import { Component, OnInit, ViewChild } from '@angular/core';
import { Patients } from '../models/patients';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { RestService } from '../rest.service';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormControl } from '@angular/forms';
import { NGXLogger } from 'ngx-logger';
import { trigger, state, style, transition, animate } from '@angular/animations';


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


  patients: Patients[];
  filterValues: any = {};
  chronic: string;
  watchFlag: string;
  pcP_StaffID: string;
  filterFormGroup;
  conditions: string;
  conditionsList: any[] = [];
  public multiFilterValues = {
    condition: ""
  };

  displayedColumns: string[] = ['arrow', 'name', 'dob', 'lastEDVisit', 'chronic', 'watchFlag', 'conditions'];
  dataSource;

  isExpansionDetailRow = (index, row) => row.hasOwnProperty('detailRow');

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

  applySelectedFilter(column: string, filterValue: string) {
    this.filterValues[column] = filterValue;

    this.dataSource.filter = JSON.stringify(this.filterValues);

    if (this.dataSource.paginator) {
      this.dataSource.paginator.firstPage();
    }
  }


}