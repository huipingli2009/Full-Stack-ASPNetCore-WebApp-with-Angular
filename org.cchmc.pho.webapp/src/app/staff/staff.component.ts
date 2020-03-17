import { Component, OnInit, ViewChild } from '@angular/core';
import { RestService } from '../rest.service';
import { Staff, Responsibilities, StaffDetails } from '../models/Staff'
import { MatTableDataSource, MatTable } from '@angular/material/table';
import { trigger, state, style, transition, animate } from '@angular/animations';
import { NGXLogger } from 'ngx-logger';
import { FormBuilder, Validators } from '@angular/forms';
import { DatePipe } from '@angular/common';
import { Sort } from '@angular/material/sort';
import { MatSnackBar } from '@angular/material/snack-bar';


@Component({
  selector: 'app-staff',
  templateUrl: './staff.component.html',
  styleUrls: ['./staff.component.scss'],
  animations: [
    trigger('detailExpand', [
      state('collapsed', style({ height: '0px', minHeight: '0' })),
      state('expanded', style({ height: '*' })),
      transition('expanded <=> collapsed', animate('225ms cubic-bezier(0.4, 0.0, 0.2, 1)')),
    ]),
  ],
})
export class StaffComponent implements OnInit {
  displayedColumns: string[] = ['arrow', 'name', 'email', 'phone', 'position', 'credentials', 'isRegistry', 'responsibilities'];
  positions: Position[];

  @ViewChild('table') table: MatTable<Staff>;

  credentials: Credential[];
  filterValues: any = {};
  sortedData: Staff[];
  positionFilter: string;
  responsibilityFilter: string;
  staffNameFilter: string;
  responsibilities: Responsibilities[];
  staffDetails: StaffDetails;
  error: any;
  staff: Staff[];
  expandedElement: Staff | null;
  dataSourceStaff: MatTableDataSource<any>;

  StaffDetailsForm = this.fb.group({
    id: [''],
    firstName: [''],
    lastName: [''],
    email: ['', [Validators.required, Validators.email]],
    phone: ['', Validators.required],
    startDate: ['', Validators.required],
    positionId: ['', Validators.required],
    credentialId: ['', Validators.required],
    npi: ['', Validators.required],
    isLeadPhysician: [''],
    isQITeam: [''],
    isPracticeManager: [''],
    isInterventionContact: [''],
    isQPLLeader: [''],
    isPHOBoard: [''],
    isOVPCABoard: [''],
    isRVPIBoard: ['']
  });

  constructor(private rest: RestService, private logger: NGXLogger, private fb: FormBuilder, private datePipe: DatePipe, private _snackBar: MatSnackBar) {
    this.dataSourceStaff = new MatTableDataSource;
  }


  ngOnInit(): void {
    this.getStaff();
    this.getPositions();
    this.getCredentials();
    this.getResponsibilities();
    this.setFilter();
  }



  //look up calls to the web api 

  getPositions() {
    this.rest.getPositions().subscribe((data) => {
      this.positions = data;
    })
  }

  getCredentials() {
    this.rest.getCredentials().subscribe((data) => {
      this.credentials = data;
    })
  }

  getResponsibilities() {
    this.rest.getResponsibilities().subscribe((data) => {
      this.responsibilities = data;
    })
  }


  // get staff information

  getStaff() {
    this.staff = [];
    this.rest.getStaff().subscribe((data) => {
      this.staff = data;
      this.dataSourceStaff = new MatTableDataSource(this.staff);
      this.dataSourceStaff.data = this.staff;
    });
  }

  getStaffDetails(id: number) {
    this.rest.getStaffDetails(id).subscribe((data) => {
      data.startDate = this.datePipe.transform(data.startDate, 'MM/dd/yyyy');
      this.staffDetails = data;
      this.StaffDetailsForm.setValue(this.staffDetails);
      this.logger.log(this.StaffDetailsForm.value);
    });
  }

  //update staff record 

  updateStaff() {
    this.staffDetails = this.StaffDetailsForm.value;
    this.rest.updateStaff(this.staffDetails).subscribe(res => {
      this.staffDetails = res;
      this.StaffDetailsForm.setValue(this.staffDetails);
      this.openSnackBar(`Details updated for ${this.staffDetails.lastName} ${this.staffDetails.firstName}`, "Success")
      this.getStaff();
    },
      error => { this.error = error }
    )
  }

  //for confirmation of successful updation of the staff record 
  openSnackBar(message: string, action: string) {
    this._snackBar.open(message, action, {
      duration: 2000,
      panelClass: ['green-snackbar']
    });
  }

  //setting filter predicate for filtering data on different columns of the table
  setFilter() {
    this.dataSourceStaff.filterPredicate = ((data: Staff, filter): boolean => {
      const filterValues = JSON.parse(filter);

      return (this.positionFilter ? data.position.name.toString().trim().toLowerCase().indexOf(filterValues.position.toLowerCase()) !== -1 : true)
        && (this.responsibilityFilter ? data.responsibilities.toString().trim().toLowerCase().indexOf(filterValues.responsibilities.toLowerCase()) !== -1 : true)
        && ((this.staffNameFilter ? data.firstName.toString().trim().toLowerCase().indexOf(filterValues.StaffName.toLowerCase()) !== -1 : true)
          || (this.staffNameFilter ? data.lastName.toString().trim().toLowerCase().indexOf(filterValues.StaffName.toLowerCase()) !== -1 : true))

    })
  }

  // for sorting the  table columns 
  onSortData(sort: Sort) {

    this.positionFilter = "";
    this.staffNameFilter = "";
    this.responsibilityFilter = "";

    const data = this.staff.slice();
    if (!sort.active || sort.direction === '') {
      this.sortedData = data;
      return;
    }

    this.sortedData = data.sort((a, b) => {
      const isAsc = sort.direction === 'asc';
      switch (sort.active) {
        case 'IsRegistry': return this.compare(a.isRegistry.toString(), b.isRegistry.toString(), isAsc);
        case 'CredentialName': return this.compare(a.credentials.name, b.credentials.name, isAsc);
        case 'PositionName': return this.compare(a.position.name, b.position.name, isAsc);
        case 'StaffName': return this.compare(a.lastName + a.firstName, b.lastName + b.firstName, isAsc);

        default: return 0;
      }
    });
    this.table.dataSource = this.sortedData;


  }
  compare(a: number | string, b: number | string, isAsc: boolean) {
    return (a < b ? -1 : 1) * (isAsc ? 1 : -1);
  }


  //for filtering the table columns 
  applySelectedFilter(column: string, filterValue: string) {
    if (column == 'position') {
      this.positionFilter = filterValue;
      this.responsibilityFilter = "";
      this.staffNameFilter = "";
    }
    if (column == 'responsibilities') {
      filterValue = "";
      this.positionFilter = "";
      this.staffNameFilter = "";
      filterValue = this.responsibilityFilter;
    }

    if (column == 'StaffName') {
      filterValue = "";
      this.responsibilityFilter = "";
      this.positionFilter = "";
      filterValue = this.staffNameFilter;
    }

    this.filterValues[column] = filterValue;
    this.dataSourceStaff.filter = JSON.stringify(this.filterValues);
    this.table.dataSource = this.dataSourceStaff.filteredData;
    if (this.dataSourceStaff.paginator) {
      this.dataSourceStaff.paginator.firstPage();
    }
  }


}