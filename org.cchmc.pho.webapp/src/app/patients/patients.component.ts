import { Component, OnInit, ViewChild } from '@angular/core';
import { Patients } from '../models/patients';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';

@Component({
  selector: 'app-patients',
  templateUrl: './patients.component.html',
  styleUrls: ['./patients.component.scss']
})
export class PatientsComponent implements OnInit {

  ELEMENT_DATA: Patients[] = [
    {patientId: 12345, practiceID: 7, status: [{id: 1, name: 'active'}], pcP_StaffID: 112354654,
      firstName: 'John', lastName: 'Smith', dob: '02/12/1988',
    lastEDVisit: '04/16/2012', chronic: true, watchFlag: true, conditions: [{id: 1, name: 'Asthma'}]},
  ]

  displayedColumns: string[] = ['name', 'dob', 'lastEDVisit', 'chronic', 'watchFlag', 'conditions'];
  dataSource = new MatTableDataSource<Patients>(this.ELEMENT_DATA);

  @ViewChild(MatPaginator, {static: true}) paginator: MatPaginator;
  @ViewChild(MatSort, {static: true}) sort: MatSort;


  constructor() { }

  ngOnInit(): void {
    this.dataSource.sort = this.sort;
    this.dataSource.paginator = this.paginator;
  }
  ngAfterViewInit(){}

}
