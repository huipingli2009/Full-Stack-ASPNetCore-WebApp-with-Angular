import { Component, OnInit, ViewChild, SimpleChanges } from '@angular/core';
import { Observable, Subscription } from 'rxjs';
import { AuthenticationService } from '../services/authentication.service';
import { ToastContainerDirective, ToastrService } from 'ngx-toastr';
import { Alerts, AlertAction, AlertActionTaken } from '../models/dashboard';
import { FormGroup, FormBuilder } from '@angular/forms';
import { User } from '../models/user';
import { RestService } from '../rest.service';
import { ActivatedRoute, Router } from '@angular/router';
import { NGXLogger } from 'ngx-logger';
import { take } from 'rxjs/operators';

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
    currentUser: User;
    subscription: Subscription;
    constructor(public rest: RestService, private route: ActivatedRoute, private router: Router,
                private toastr: ToastrService, public fb: FormBuilder, private logger: NGXLogger,
                private authenticationService: AuthenticationService) {
      // var id = this.userId.snapshot.paramMap.get('id') TODO: Need User Table;
      this.logger.log('testing the logging in app.component.ts constructor with NGXLogger');
    //   this.authenticationService.currentUser.subscribe(x => this.currentUser = x);
      

    }

    ngOnInit() {
      this.subscription = this.authenticationService.isLoggedIn.subscribe(res => {
        this.isLoggedIn$ = res;
        if(this.isLoggedIn$ === true) {
          this.getAlerts();
        }
      });
      this.isLoggedIn$ = this.authenticationService.isUserLoggedIn;
      if(this.isLoggedIn$ === true) {
        this.getAlerts();
      }
      //TODO: ALERTS ARE BROKEN UNLESS YOU REFRESH...I need to figure out why this is not responding to subscription
    }

    ngAfterViewInit() {
      this.toastr.overlayContainer = this.toastContainer;
    }

    getAlerts() {
      this.alerts = [];
      this.rest.getAlerts().subscribe((data) => {
        this.alerts = data;
        if (this.alerts.length > 0) {
          this.alerts.forEach(alert => {
            this.logger.log(alert)
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
}
