import { Component, ViewChild, TemplateRef } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { NGXLogger } from 'ngx-logger';
import { ToastContainerDirective, ToastrService } from 'ngx-toastr';
import { Subscription } from 'rxjs';
import { take } from 'rxjs/operators';
import { AlertAction, AlertActionTaken, Alerts } from '../models/dashboard';
import { Practices } from '../models/Staff';
import { CurrentUser } from '../models/user';
import { RestService } from '../rest.service';
import { AuthenticationService } from '../services/authentication.service';
import { UserService } from '../services/user.service';
import { MatDialog } from '@angular/material/dialog';
import { MyErrorStateMatcher } from '../staff/staff.component';
import { comparePasswords } from '../helpers/password-match.validator';
import { NoSpaceValidator } from '../helpers/no-spaces.validator';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html'
})
export class HeaderComponent {

  isLoggedIn$: boolean;

  @ViewChild(ToastContainerDirective, { static: false }) toastContainer: ToastContainerDirective;
  @ViewChild('passwordDialog') passwordDialog: TemplateRef<any>;
  @ViewChild('updatePassConfirmDialog') updatePassConfirmDialog: TemplateRef<any>;
  error: any;
  title = 'phoweb';
  alerts: Alerts[];
  alertAction: AlertAction;
  alertScheduleId: number;
  updateAlert: FormGroup;
  currentUser: CurrentUser;
  isUserAdmin: boolean;
  firstName: string;
  lastName: string;
  subscription: Subscription;
  practiceForm: FormGroup;
  practiceList: Array<Practices>;
  currentPracticeId: number;
  currentPractice: string;
  currentUsername: string;
  currentUserId: string;
  userPracticeName: string;
  matcher = new MyErrorStateMatcher();
  updateUserForm: FormGroup;
  isPasswordUpdated: boolean;
  passwordVerbiage: string;
  constructor(public rest: RestService, private route: ActivatedRoute, private router: Router,
              private toastr: ToastrService, public fb: FormBuilder, private logger: NGXLogger,
              private authenticationService: AuthenticationService, private userService: UserService,
              public dialog: MatDialog) {
    this.logger.log('testing the logging in app.component.ts constructor with NGXLogger');
    this.practiceForm = this.fb.group({
      practiceControl: ['']
    });
    this.updateUserForm = this.fb.group({
      password: ['', [Validators.required, Validators.minLength(8), NoSpaceValidator.cannotContainSpace,
        Validators.pattern('((?=.*\\d)(?=.*[a-z])(?=.*[A-Z]).{8,30})')]],
      confirmPassword: ['', Validators.required]
    }, {
      validator: comparePasswords('password', 'confirmPassword')
    });

  }

  ngOnInit() {
    this.getAlerts();
    this.getCurrentUser();
    this.getPracticeList();
    this.getPasswordVerbiage();
  }

  ngAfterViewInit() {
    this.toastr.overlayContainer = this.toastContainer;
  }

  getCurrentUser() {
    this.userService.getCurrentUser().subscribe((data) => {
      this.currentUser = data;
      this.currentUserId = data.id;
      this.currentUsername = data.userName;
      this.firstName = data.firstName;
      this.lastName = data.lastName;
      if (data.role.id === 3) {
        this.isUserAdmin = true;
      } else { this.isUserAdmin = false; }
    });
  }

  getAlerts() {
    this.alerts = [];
    this.rest.getAlerts().subscribe((data) => {
      this.alerts = data;
      this.logger.log(this.alerts, 'ALERTS');
      if (this.alerts.length > 0) {
        this.alerts.forEach(alert => {
          let toastrMessage = `<i class="fas fa-exclamation-triangle alert-icon" title="${alert.definition}"></i>
          ${alert.message}<a class="alert-link" href="${alert.url}" target="${alert.target}">${alert.linkText}»</a>`;
          if (alert.target === '') {
            toastrMessage = `<i class="fas fa-exclamation-triangle alert-icon" title="${alert.definition}"></i>
          ${alert.message}<a class="alert-link" href="${alert.url}">${alert.linkText}»</a>`;
          }
          if (alert.url === '') {
            toastrMessage = `<i class="fas fa-exclamation-triangle alert-icon" title="${alert.definition}"></i>
          ${alert.message}`;
          }


          const activeToastr = this.toastr.success(toastrMessage, alert.alertScheduleId.toString(), {
            closeButton: true,
            disableTimeOut: true,
            enableHtml: true,
            tapToDismiss: false
          });
          activeToastr.onHidden
            .pipe(take(1))
            .subscribe((data) => this.toastrCloseHandler(alert.alertScheduleId));
          activeToastr.onTap
            .pipe(take(1))
            .subscribe((data) => this.toastrClickHandler(alert.alertScheduleId, alert.url));

        });
      }

    });
  }


  toastrCloseHandler(alertScheduleId: number) {
    this.alertAction = { alertActionId: AlertActionTaken.close };
    this.rest.updateAlertActivity(alertScheduleId, this.alertAction).subscribe(res => { },
      error => {
        this.error = error;
      });
  }

  toastrClickHandler(alertScheduleId: number, url: string) {
    this.alertAction = { alertActionId: AlertActionTaken.click };
    window.open(url, '_blank');
    this.rest.updateAlertActivity(alertScheduleId, this.alertAction).subscribe(res => { },
      error => {
        this.error = error;
      });


  }

  logout() {
    this.authenticationService.logout();
    this.isLoggedIn$ = false;
    this.router.navigate(['/login']);
  }

  // Practice Switch -------------------------------
  getPracticeList() {
    this.rest.getPracticeList().subscribe(data => {
      this.practiceList = data.practiceList;
      this.currentPracticeId = data.currentPracticeId;
      this.practiceForm.controls.practiceControl.setValue(data.currentPracticeId);
    });
  }

  switchPractice(practiceId) {
    const staffId = this.authenticationService.getCurrentStaffId();
    const newPractice = {
      id: Number(staffId),
      myPractice: {
        id: practiceId
      }
    };
    this.rest.switchPractice(newPractice).subscribe(res => {
      this.logger.log('SWITCHED', res);
      location.reload();
    });
  }

  /* Password Change */
  getPasswordVerbiage() {
    this.rest.getStaffAdminVerbiage().pipe(take(1)).subscribe((res) => {
      this.passwordVerbiage = res;
    });
  }

 // Pass Validation
 checkError(controlName: string, errorName: string) {
  return this.updateUserForm.controls[controlName].hasError(errorName);
}

  passwordChanged() {
    this.isPasswordUpdated = true;
  }

  openPasswordDialog() {
    const selectedValues = {
      password: '********',
      confirmPassword: '********'
    }
    this.updateUserForm.setValue(selectedValues);
    this.dialog.open(this.passwordDialog, { disableClose: true });
  }

  confirmPasswordUpdate() {
    const { value, valid } = this.updateUserForm;
    if (valid) {
      this.dialog.open(this.updatePassConfirmDialog, { disableClose: true });
    }
  }

  updatePassword() {
    let userId = this.currentUserId;
    let updatedPass = {
      token: this.authenticationService.getToken(),
      newPassword: this.updateUserForm.controls.password.value
    };
    this.userService.updateUserPassword(userId, updatedPass).pipe(take(1)).subscribe(res => {
      if (res === true) {
        this.dialog.closeAll();
        setTimeout(() => {  this.logout(); }, 2000);
      }
    });
  }

  cancelPasswordUpdateDialog() {
    this.isPasswordUpdated = false;
    this.dialog.closeAll();
  }
}
