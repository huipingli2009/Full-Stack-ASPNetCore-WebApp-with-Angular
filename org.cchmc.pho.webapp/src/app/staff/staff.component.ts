import { animate, state, style, transition, trigger } from '@angular/animations';
import { DatePipe } from '@angular/common';
import { Component, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, FormGroupDirective, NgForm, Validators } from '@angular/forms';
import { ErrorStateMatcher } from '@angular/material/core';
import { MatDialog } from '@angular/material/dialog';
import { Sort } from '@angular/material/sort';
import { MatTable, MatTableDataSource } from '@angular/material/table';
import { NGXLogger } from 'ngx-logger';
import { take } from 'rxjs/operators';
import { ErrorInterceptor } from '../helpers/error.interceptor';
import { comparePasswords } from '../helpers/password-match.validator';
import { Responsibilities, Staff, StaffDetails } from '../models/Staff';
import { CurrentUser, User } from '../models/user';
import { RestService } from '../rest.service';
import { AuthenticationService } from '../services/authentication.service';
import { UserService } from '../services/user.service';
import { DateRequiredValidator } from '../shared/customValidators/customValidator';
import { MatSnackBarComponent } from '../shared/mat-snack-bar/mat-snack-bar.component';


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
  matcher = new MyErrorStateMatcher();
  displayedColumns: string[] = ['arrow', 'name', 'email', 'phone', 'position', 'credentials', 'isRegistry', 'responsibilities'];
  positions: Position[];
  currentUser: CurrentUser;
  currentUserId: number;
  currentPracticeId: number;
  isUserAdmin: boolean;
  staffUser: User;
  adminVerbiage: string;
  userRoleList: {};
  compareFn: ((f1: any, f2: any) => boolean) | null = this.compareByValue;
  adminUserForm: FormGroup;
  isLockedOut: boolean;
  isDeleted: boolean;
  userStatus: number;
  selectedStaffUser: number;
  isPasswordUpdated: boolean;

  @ViewChild('table') table: MatTable<Staff>;
  @ViewChild('adminDialog') adminDialog: TemplateRef<any>;
  @ViewChild('updateUserDialog') updateUserDialog: TemplateRef<any>;

  credentials: Credential[];
  filterValues: any = {};
  sortedData: Staff[];
  responsibilities: Responsibilities[];
  staffDetails: StaffDetails;
  error: any;
  staff: Staff[];
  expandedElement: Staff | null;
  dataSourceStaff: MatTableDataSource<any>;

  responsibilityFilter = new FormControl('');
  staffNameFilter = new FormControl('');
  positionFilter = new FormControl('');


  StaffDetailsForm = this.fb.group({
    id: [''],
    firstName: [''],
    lastName: [''],
    email: ['', [Validators.required, Validators.email]],
    phone: ['', Validators.required],
    startDate: ['', [DateRequiredValidator]],
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

  constructor(private rest: RestService, private logger: NGXLogger, private fb: FormBuilder, private datePipe: DatePipe,
    private snackBar: MatSnackBarComponent, private userService: UserService, public dialog: MatDialog,
    private authService: AuthenticationService, private errorIntercept: ErrorInterceptor) {
    this.dataSourceStaff = new MatTableDataSource;
    this.adminUserForm = this.fb.group({
      userName: ['', Validators.required],
      password: ['', [Validators.required, Validators.minLength(8)]],
      confirmPassword: ['', Validators.required],
      roles: ['']
    }, {
      validator: comparePasswords('password', 'confirmPassword')
    });
    this.getUserRoles();
  }


  ngOnInit(): void {
    this.getCurrentUser();
    this.getStaff();
    this.getPositions();
    this.getCredentials();
    this.getResponsibilities();
    this.getAdminVerbiage();
    this.getCurrentPractice();
  }



  // look up calls to the web api

  getPositions() {
    this.rest.getPositions().pipe(take(1)).subscribe((data) => {
      this.positions = data;
    });
  }

  getCredentials() {
    this.rest.getCredentials().pipe(take(1)).subscribe((data) => {
      this.credentials = data;
    });
  }

  getResponsibilities() {
    this.rest.getResponsibilities().pipe(take(1)).subscribe((data) => {
      this.responsibilities = data;
    });
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


  // get staff information

  getStaff() {
    this.staff = [];
    this.rest.getStaff().pipe(take(1)).subscribe((data) => {
      this.staff = data;
      this.dataSourceStaff = new MatTableDataSource(this.staff);
      this.dataSourceStaff.data = this.staff;

      this.dataSourceStaff.filterPredicate = ((data: Staff, filter): boolean => {
        const filterValues = JSON.parse(filter);

        return (this.positionFilter.value ? data.position.positionType.toString().trim()
          .toLowerCase().indexOf(filterValues.position.toLowerCase()) !== -1 : true)
          && (this.responsibilityFilter.value ? data.responsibilities
            .toString().trim().toLowerCase().indexOf(filterValues.responsibilities.toLowerCase()) !== -1 : true)
          && ((this.staffNameFilter.value ? data.firstName.toString().trim()
            .toLowerCase().indexOf(filterValues.StaffName.toLowerCase()) !== -1 : true)
            || (this.staffNameFilter.value ? data.lastName.toString()
              .trim().toLowerCase().indexOf(filterValues.StaffName.toLowerCase()) !== -1 : true));

      });
    });
  }

  getStaffDetails(id: number) {
    this.rest.getStaffDetails(id).pipe(take(1)).subscribe((data) => {
      this.staffDetails = data;
      this.StaffDetailsForm.setValue(this.staffDetails);
      this.getStaffUser(data.id);
    });
  }
  getStaffUser(id: number) {
    this.userService.getUserStaff(id).pipe(take(1)).subscribe((data) => {
      this.staffUser = data.body;
      if (this.staffUser) {
        this.selectedStaffUser = data.body.id;
        this.isLockedOut = data.body.isLockedOut;
        this.isDeleted = data.body.isDeleted;
        this.userStatus = data.status;
        this.logger.log(this.staffUser, 'Got Staff user');
        const selectedValues = {
          userName: data.body.userName,
          password: '********',
          confirmPassword: '********',
          roles: this.staffUser.role
        }
        this.adminUserForm.setValue(selectedValues);
      }

    },
      error => {
        if (error.status === 404) {
          console.error('Not found');
        }
      });
  }

  getUserRoles() {
    this.userService.getUserRoles().pipe(take(1)).subscribe((data) => {
      this.userRoleList = data;
    });
  }
  // Get current practice
  getCurrentPractice() {
    this.rest.getPracticeList().pipe(take(1)).subscribe((data) => {
      this.currentPracticeId = data.currentPracticeId;
    })
  }

  // update staff record

  updateStaff() {
    this.staffDetails = this.StaffDetailsForm.value;
    this.staffDetails.npi = Number(this.StaffDetailsForm.controls.npi.value);
    this.rest.updateStaff(this.staffDetails).pipe(take(1)).subscribe(res => {
      this.staffDetails = res;
      this.StaffDetailsForm.setValue(this.staffDetails);
      this.snackBar.openSnackBar(`Details updated for ${this.staffDetails.lastName} ${this.staffDetails.firstName}`, 'Close', 'success-snackbar')

      this.getStaff();
    },
      error => { this.error = error; }
    );
  }

  // Add Staff User
  createStaffUser() {
    let practiceIdForUsername = '';
    if (this.currentPracticeId < 10) {
      practiceIdForUsername = `0${this.currentPracticeId}`;
    } else { practiceIdForUsername = this.currentPracticeId.toString() }
    const staffUser = {
      id: this.currentUserId,
      token: this.authService.getToken(),
      newPassword: `${this.staffDetails.lastName.substring(0, 3)}${this.staffDetails.firstName.substring(0, 3)}${this.staffDetails.id}!`,
      firstName: this.staffDetails.firstName,
      lastName: this.staffDetails.lastName,
      userName: `${this.staffDetails.firstName.charAt(0)}${this.staffDetails.lastName}${this.staffDetails.id}${practiceIdForUsername}`,
      email: this.staffDetails.email,
      staffId: this.staffDetails.id,
      isLockedOut: true,
      role: {
        id: 1
      }
    };
    this.userService.createStaffUser(staffUser).pipe(take(1)).subscribe(res => {
      this.getStaffUser(this.staffDetails.id);
      this.logger.log('post', res);
    });
  }

  //Remove Users Registry Access
  removeUserRegAccess(e) {
    this.logger.log('selected user', e.checked)
    this.userService.removeUserFromRegistry(this.selectedStaffUser, e.checked).pipe(take(1)).subscribe(res => {
      this.logger.log('remived', res);
    });
  }

  //Lockout User
  lockoutUser(e) {
    this.userService.lockoutUser(this.selectedStaffUser).pipe(take(1)).subscribe(res => {
      this.logger.log(res);
    });
  }

  //Make sure Passwords Match
  passwordChanged() {
    this.isPasswordUpdated = true;
  }

  // Get Verbiage for Admin Panel
  getAdminVerbiage() {
    this.rest.getStaffAdminVerbiage().pipe(take(1)).subscribe((res) => {
      this.adminVerbiage = res;
    });
  }

  // Update Password
  updatePassword(userId, password) {
    let updatedPass = {
      token: this.authService.getToken(),
      newPassword: password
    };
    this.userService.updateUserPassword(userId, updatedPass).pipe(take(1)).subscribe(res => {
      this.logger.log('Password Updated', res);
    })
  }
  /*Update User */
  updateStaffUser(id, user) {
    this.userService.updateUser(id, user).pipe(take(1)).subscribe(res => {
      this.logger.log('update user res in taff', res);
    })
  }

  submitForm() {
    const updatedUser = {
      id: this.selectedStaffUser,
      token: this.authService.getToken(),
      userName: this.adminUserForm.controls.userName.value,
      staffId: this.staffDetails.id,
      email: this.staffDetails.email,
      role: {
        id: this.adminUserForm.controls.roles.value.id,
        name: this.adminUserForm.controls.roles.value.name
      }
    };
    if (this.isPasswordUpdated === true) {
      this.updatePassword(this.selectedStaffUser, this.adminUserForm.controls.password.value);
      this.updateStaffUser(this.selectedStaffUser, updatedUser);
      this.dialog.closeAll();
    } else {
      this.logger.log(JSON.stringify(updatedUser), 'updated user')
      this.updateStaffUser(this.selectedStaffUser, updatedUser);
      this.dialog.closeAll();
    }
  }

  compareByValue(o1, o2): boolean {
    return o1.name === o2.name;
  }


  // for sorting the  table columns
  onSortData(sort: Sort) {

    this.positionFilter.setValue('');
    this.staffNameFilter.setValue('');
    this.responsibilityFilter.setValue('');

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


  // for filtering the table columns
  applySelectedFilter(column: string, filterValue: string) {
    if (column == 'position') {
      this.positionFilter.setValue(filterValue);
      this.responsibilityFilter.setValue('');
      this.staffNameFilter.setValue('');
    }
    if (column == 'responsibilities') {
      filterValue = '';
      this.positionFilter.setValue('');
      this.staffNameFilter.setValue('');
      filterValue = this.responsibilityFilter.value;
    }

    if (column == 'StaffName') {
      filterValue = '';
      this.responsibilityFilter.setValue('');
      this.positionFilter.setValue('');
      filterValue = this.staffNameFilter.value;
    }

    this.filterValues[column] = filterValue;
    this.dataSourceStaff.filter = JSON.stringify(this.filterValues);
    this.table.dataSource = this.dataSourceStaff.filteredData;
    if (this.dataSourceStaff.paginator) {
      this.dataSourceStaff.paginator.firstPage();
    }
  }

  // Dialogs -------------
  openAdminDialog() {
    this.logger.log(this.userService.responseStatus);
    if (this.userService.responseStatus === 404) {
      this.createStaffUser();
    } else { this.getStaffUser(this.staffDetails.id) }
    const dialogRef = this.dialog.open(this.adminDialog, { disableClose: true });
  }

  cancelAdminDialog() {
    this.getUserRoles();
    this.isPasswordUpdated = false;
    this.dialog.closeAll();
  }


  confirmStaffUserUpdate() {
    const { value, valid } = this.adminUserForm;
    if (valid) {
      this.logger.log('Form Valid')
      this.dialog.open(this.updateUserDialog, { disableClose: true });
    }
  }

  trackStaff(index: number, item: Staff): string {
    return '${item.id}';
  }
}



export class MyErrorStateMatcher implements ErrorStateMatcher {
  isErrorState(control: FormControl | null, form: FormGroupDirective | NgForm | null): boolean {
    const isSubmitted = form && form.submitted;
    return (control && control.invalid);
  }
}
