import { Component, OnInit, ViewChild, TemplateRef } from '@angular/core';
import { RestService } from '../rest.service';
import { Staff, Responsibilities, StaffDetails } from '../models/Staff';
import { MatTableDataSource, MatTable } from '@angular/material/table';
import { trigger, state, style, transition, animate } from '@angular/animations';
import { NGXLogger } from 'ngx-logger';
import { FormBuilder, Validators, FormControl, FormArray, FormGroup, AbstractControl, NgForm, FormGroupDirective } from '@angular/forms';
import { DatePipe } from '@angular/common';
import { Sort } from '@angular/material/sort';
import { MatSnackBar } from '@angular/material/snack-bar';
import { CurrentUser, User, UserRoles } from '../models/user';
import { UserService } from '../services/user.service';
import { MatDialog } from '@angular/material/dialog';
import { HttpErrorResponse } from '@angular/common/http';
import { AuthenticationService } from '../services/authentication.service';
import { ErrorInterceptor } from '../helpers/error.interceptor';
import { BehaviorSubject } from 'rxjs';
import { comparePasswords } from '../helpers/password-match.validator';
import { ErrorStateMatcher } from '@angular/material/core';


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
  isUserAdmin: boolean;
  staffUser: User;
  adminVerbiage: string;
  userRoleList: Array<UserRoles>;
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

  constructor(private rest: RestService, private logger: NGXLogger, private fb: FormBuilder, private datePipe: DatePipe,
    private _snackBar: MatSnackBar, private userService: UserService, public dialog: MatDialog,
    private authService: AuthenticationService, private errorIntercept: ErrorInterceptor) {
    this.dataSourceStaff = new MatTableDataSource;
    this.adminUserForm = this.fb.group({
      userName: ['', Validators.required],
      password: ['', [Validators.required, Validators.minLength(8)]],
      confirmPassword: ['', Validators.required],
      roles: new FormArray([])
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
  }



  // look up calls to the web api

  getPositions() {
    this.rest.getPositions().subscribe((data) => {
      this.positions = data;
    });
  }

  getCredentials() {
    this.rest.getCredentials().subscribe((data) => {
      this.credentials = data;
    });
  }

  getResponsibilities() {
    this.rest.getResponsibilities().subscribe((data) => {
      this.responsibilities = data;
    });
  }

  getCurrentUser() {
    this.userService.getCurrentUser().subscribe((data) => {
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
    this.rest.getStaff().subscribe((data) => {
      this.staff = data;
      this.dataSourceStaff = new MatTableDataSource(this.staff);
      this.dataSourceStaff.data = this.staff;

      this.dataSourceStaff.filterPredicate = ((data: Staff, filter): boolean => {
        const filterValues = JSON.parse(filter);

        return (this.positionFilter.value ? data.position.name.toString().trim()
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
    this.rest.getStaffDetails(id).subscribe((data) => {
      data.startDate = this.datePipe.transform(data.startDate, 'MM/dd/yyyy');
      this.staffDetails = data;
      this.StaffDetailsForm.setValue(this.staffDetails);
      this.getStaffUser(data.id);
    });
  }
  getStaffUser(id: number) {
    this.userService.getUserStaff(id).subscribe((data) => {
      this.staffUser = data.body;
      if (this.staffUser) {
        this.selectedStaffUser = data.body.id;
        this.isLockedOut = data.body.isLockedOut;
        this.isDeleted = data.body.isDeleted;
        this.userStatus = data.status;
        this.logger.log(this.staffUser, 'Got Staff user');
        this.adminUserForm.controls.userName.setValue(data.body.userName);
        this.adminUserForm.controls.password.setValue('********');
        this.adminUserForm.controls.confirmPassword.setValue('********');
        const selectedRoles = [];
        selectedRoles.length = 0;
        if (selectedRoles.length > 1) {
          this.staffUser.role.forEach(role => {
            selectedRoles.push(role);
            this.roleOptions.at(role.id - 1).patchValue(selectedRoles);
          });
        } else {
          selectedRoles.push(this.staffUser.role);
          this.roleOptions.at(data.body.role.id - 1).patchValue(selectedRoles);
        }
      }

    },
      error => {
        if (error.status === 404) {
          console.error('Not found');
        }
      });
  }

  getUserRoles() {
    this.userService.getUserRoles().subscribe((data) => {
      this.userRoleList = data;
      this.userRoleList.forEach((r, i) => {
        const control = new FormControl(data.id);
        (this.adminUserForm.controls.roles as FormArray).push(control);
      });
    });
  }

  get roleOptions() {
    return this.adminUserForm ? this.adminUserForm.get('roles') as FormArray : null;
  }

  // update staff record

  updateStaff() {
    this.staffDetails = this.StaffDetailsForm.value;
    this.rest.updateStaff(this.staffDetails).subscribe(res => {
      this.staffDetails = res;
      this.StaffDetailsForm.setValue(this.staffDetails);
      this.openSnackBar(`Details updated for ${this.staffDetails.lastName} ${this.staffDetails.firstName}`, 'Success');
      this.getStaff();
    },
      error => { this.error = error; }
    );
  }

  // Add Staff User
  createStaffUser() {
    const staffUser = {
      id: this.currentUserId,
      token: this.authService.getToken(),
      newPassword: `${this.staffDetails.lastName.substring(0, 3)}${this.staffDetails.firstName.substring(0, 3)}${this.staffDetails.id}!`,
      firstName: this.staffDetails.firstName,
      lastName: this.staffDetails.lastName,
      userName: `${this.staffDetails.firstName.charAt(0)}${this.staffDetails.lastName}`,
      email: this.staffDetails.email,
      staffId: this.staffDetails.id,
      isLockedOut: true,
      role: {
        id: 1
      }
    };
    this.userService.createStaffUser(staffUser).subscribe(res => {
      this.getStaffUser(this.staffDetails.id);
      this.logger.log('post', res);
    });
  }

  //Remove Users Registry Access
  removeUserRegAccess(e) {
    this.logger.log('selected user', e.checked)
    this.userService.removeUserFromRegistry(this.selectedStaffUser, e.checked).subscribe(res => {
      this.logger.log('remived', res);
    });
  }

  //Lockout User
  lockoutUser(e) {
    this.userService.lockoutUser(this.selectedStaffUser).subscribe(res => {
      this.logger.log(res);
    });
  }

  //Make sure Passwords Match
  passwordChanged() {
    this.isPasswordUpdated = true;
  }

  // Get Verbiage for Admin Panel
  getAdminVerbiage() {
    this.rest.getStaffAdminVerbiage().subscribe((res) => {
      this.adminVerbiage = res;
    });
  }

  // for confirmation of successful updation of the staff record
  openSnackBar(message: string, action: string) {
    this._snackBar.open(message, action, {
      duration: 2000,
      panelClass: ['green-snackbar']
    });
  }
  // Update Password
  updatePassword(userId, password) {
    let updatedPass = {
      token: this.authService.getToken(),
      newPassword: password
    };
    this.userService.updateUserPassword(userId, updatedPass).subscribe(res => {
      this.logger.log('Password Updated', res);
    })
  }

  submitForm() {
    const updatedUser = {
      id: this.currentUserId,
      token: this.authService.getToken(),
      userName: this.adminUserForm.controls.userName.value,
      staffId: this.staffDetails.id,
      role: this.adminUserForm.controls.roles.value
    };
    if (this.isPasswordUpdated === true) {
      this.updatePassword(this.selectedStaffUser, this.adminUserForm.controls.password.value);
      this.logger.log('Password needs to be updated seperately')
      this.userService.updateUser(this.selectedStaffUser, updatedUser);
    } else {
      // this.userService.updateUser(this.selectedStaffUser, updatedUser).subscribe(res => {
      //   this.logger.log('update user res in taff', res);
      // })
      this.logger.log('Normal Form Submission');
      // let roles = this.adminUserForm.value.roles;
      // const selectedRoleIds = Object.assign({}, roles , {
      //   role: roles.map((v, i) => (v ? this.userRoleList[i].id : null))
      // .filter(v => v !== null)
      // }
    //   let roles = this.adminUserForm.value.roles;
    //   const selectedRoleIds = Object.assign({}, roles , {
    //     role: roles.map((v, i) => {
    //       return {
    //         id: this.userRoleList[i].id
    //       }
    //     })
    //   .filter(v => v !== null)
    //   }
      
      
    //   );
    // console.log(JSON.stringify(selectedRoleIds));
    }
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
    const control = <FormArray>this.adminUserForm.controls.roles;
    for (let i = control.length - 1; i >= 0; i--) {
      control.removeAt(i);
    }
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


}

export class MyErrorStateMatcher implements ErrorStateMatcher {
  isErrorState(control: FormControl | null, form: FormGroupDirective | NgForm | null): boolean {
    const isSubmitted = form && form.submitted;
    return (control && control.invalid);
  }
}
