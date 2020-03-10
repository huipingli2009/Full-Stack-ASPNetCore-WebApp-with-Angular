import { Component, OnInit } from '@angular/core';
import { RestService } from '../rest.service';
import { Staff } from '../models/Staff'
import { MatTableDataSource } from '@angular/material/table';


@Component({
  selector: 'app-staff',
  templateUrl: './staff.component.html',
  styleUrls: ['./staff.component.scss']
})
export class StaffComponent implements OnInit {
  displayedColumns: string[] = ['id', 'firstName', 'lastName', 'email', 'phone1', 'positionId', 'credentialId', 'isRegistry', 'responsibilities'];
  staff: Staff[];
  dataSourceStaff: MatTableDataSource<any>;
  constructor(private rest: RestService) {
    this.dataSourceStaff = new MatTableDataSource;
  }

  ngOnInit(): void {
    this.getStaff();
  }

  getStaff() {
    this.staff = [];
    this.rest.getStaff().subscribe((data) => {
      this.staff = data;
      this.dataSourceStaff.data = this.staff;
    });
  }

}