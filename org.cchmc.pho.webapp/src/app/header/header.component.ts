import { Component, OnInit, ViewChild, SimpleChanges } from '@angular/core';
import { Observable, Subscription } from 'rxjs';
import { AuthenticationService } from '../services/authentication.service';
import { ToastContainerDirective, ToastrService } from 'ngx-toastr';
import { Alerts, AlertAction, AlertActionTaken } from '../models/dashboard';
import { FormGroup, FormBuilder } from '@angular/forms';
import { User, CurrentUser } from '../models/user';
import { RestService } from '../rest.service';
import { ActivatedRoute, Router } from '@angular/router';
import { NGXLogger } from 'ngx-logger';
import { take } from 'rxjs/operators';
import { PracticeList, Practices } from '../models/Staff';
import { UserService } from '../services/user.service';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html'
})
export class HeaderComponent {

  isLoggedIn$: boolean;

  @ViewChild(ToastContainerDirective, { static: false }) toastContainer: ToastContainerDirective;
  error: any;
  title = 'phoweb';
  alerts: Alerts[];
  alertAction: AlertAction;
  alertScheduleId: number;
  updateAlert: FormGroup;
  currentUser: CurrentUser;
  firstName: string;
  lastName: string;
    subscription: Subscription;
    practiceForm: FormGroup;
    practiceList: Array<Practices>;
    currentPracticeId: number;
    currentPractice: string;
  constructor(public rest: RestService, private route: ActivatedRoute, private router: Router,
    private toastr: ToastrService, public fb: FormBuilder, private logger: NGXLogger,
    private authenticationService: AuthenticationService, private userService: UserService) {
    this.logger.log('testing the logging in app.component.ts constructor with NGXLogger');
      this.practiceForm = this.fb.group({
          practiceControl: ['']
      });

  }

  ngOnInit() {
    this.getAlerts();
      this.getCurrentUser();
      this.getPracticeList();
  }

  ngAfterViewInit() {
    this.toastr.overlayContainer = this.toastContainer;
  }

  getCurrentUser() {
    this.userService.getCurrentUser().subscribe((data) => {
      this.currentUser = data;
      this.firstName = data.firstName;
      this.lastName = data.lastName;
    });
  }

  getAlerts() {
    this.alerts = [];
    this.rest.getAlerts().subscribe((data) => {
      this.alerts = data;
      if (this.alerts.length > 0) {
        this.alerts.forEach(alert => {
          const toastrMessage = `<i class="fas fa-exclamation-triangle alert-icon" title="${alert.definition}"></i>
          ${alert.message}<a class="alert-link" href="${alert.url}" target="_blank">${alert.linkText}»</a>`;


          let activeToastr = this.toastr.success(toastrMessage, alert.alertScheduleId.toString(), {
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
      this.practiceForm.controls['practiceControl'].setValue(data.currentPracticeId);
      this.logger.log('Practice List', this.practiceList, 'ID', this.currentPracticeId); // Working here
    })
  }

  switchPractice(practiceId) {
    let staffId = 62; //TODO: Need to remove after gettin other branch, also make sure practice select is fidden if not admin
    let newPractice = {
      id: staffId,
      myPractice: {
        id: practiceId
      }
    };
    this.rest.switchPractice(newPractice).subscribe(res => {
      this.logger.log('SWITCHED', res);
    });
  }
}
