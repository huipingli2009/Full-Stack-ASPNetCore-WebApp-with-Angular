import { Component, OnInit, ViewChild } from '@angular/core';
import { Patients } from '../models/patients';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { RestService } from '../rest.service';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder } from '@angular/forms';
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

  displayedColumns: string[] = ['arrow', 'name', 'dob', 'lastEDVisit', 'chronic', 'watchFlag', 'conditions', 'pcP_StaffID'];
  dataSource;

  isExpansionDetailRow = (index, row) => row.hasOwnProperty('detailRow');

  constructor(public rest: RestService, private route: ActivatedRoute, private router: Router,
              public fb: FormBuilder, private logger: NGXLogger) { }

  ngOnInit() {
    this.getAllPatients();
  }

  getAllPatients() {
    this.rest.getAllPatients().subscribe((data) => {
      this.patients = data;
      this.dataSource = new MatTableDataSource<Patients>(this.patients);
      this.dataSource.filterPredicate = ((data: Patients, filter: string): boolean => {
        const filterValues = JSON.parse(filter);
  
        return (this.chronic ? data.chronic.toString().trim().toLowerCase().indexOf(filterValues.chronic) !== -1 : true)
        && (this.watchFlag ? data.watchFlag.toString().trim().toLowerCase().indexOf(filterValues.watchFlag) !== -1 : true)
        && (this.pcP_StaffID ? data.pcP_StaffID.toString().trim().toLowerCase().indexOf(filterValues.pcP_StaffID) !== -1 : true); // Working HEre Need to filter on text
      })
      this.dataSource.sort = this.sort;
      this.dataSource.paginator = this.paginator;
      // this.patients.forEach(patient => {
      //   this.patientName = `${patient.lastName}, ${patient.firstName}`;
      // });
    });
  }

  applyChronicFilter(column: string, filterValue: string) {
    this.filterValues[column] = filterValue;

    this.dataSource.filter = JSON.stringify(this.filterValues);

    if (this.dataSource.paginator) {
      this.dataSource.paginator.firstPage();
    }
  }

}