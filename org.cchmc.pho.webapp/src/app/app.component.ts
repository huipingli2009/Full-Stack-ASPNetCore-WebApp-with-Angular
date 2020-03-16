import { Component, ViewChild } from '@angular/core';
import { Observable } from 'rxjs';
import { ToastContainerDirective, ToastrService } from 'ngx-toastr';
import { Alerts, AlertAction, AlertActionTaken } from './models/dashboard';
import { FormGroup, FormBuilder } from '@angular/forms';
import { RestService } from './rest.service';
import { ActivatedRoute, Router } from '@angular/router';
import { take } from 'rxjs/operators';
import { NGXLogger } from 'ngx-logger';
import { User } from './models/user';
import { AuthenticationService } from './services/authentication.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  @ViewChild(ToastContainerDirective, { static: true }) toastContainer: ToastContainerDirective;
  error: any;
  title = 'phoweb';
  alerts: Alerts[];
  alertAction: AlertAction;
  alertScheduleId: number;
  updateAlert: FormGroup;
  currentUser: User;
  constructor(public rest: RestService, private route: ActivatedRoute, private router: Router,
    private toastr: ToastrService, public fb: FormBuilder, private logger: NGXLogger,
     private authenticationService: AuthenticationService) {
    // var id = this.userId.snapshot.paramMap.get('id') TODO: Need User Table;
    this.logger.log("testing the logging in app.component.ts constructor with NGXLogger");
    this.authenticationService.currentUser.subscribe(x => this.currentUser = x);

  }

  ngOnInit() {
    this.toastr.overlayContainer = this.toastContainer;
    this.getAlerts(); // TODO: Temp User ID Value
  }




  getAlerts() {
    this.alerts = [];
    this.rest.getAlerts().subscribe((data) => {
      this.alerts = data;

      if (this.alerts.length > 0) {
        this.alerts.forEach(alert => {
          let toastrMessage = `<i class="fas fa-exclamation-triangle alert-icon" title="${alert.definition}"></i>
        ${alert.message}<a class="alert-link" href="${alert.url}" target="_blank">${alert.linkText}»</a>`;


          var activeToastr = this.toastr.success(toastrMessage, alert.alertScheduleId.toString(), {
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
    window.open(url, "_blank");
    this.rest.updateAlertActivity(alertScheduleId, this.alertAction).subscribe(res => { },
      error => {
        this.error = error;
      })


  }

  logout() {
    this.authenticationService.logout();
    this.router.navigate(['/login']);
}

}
