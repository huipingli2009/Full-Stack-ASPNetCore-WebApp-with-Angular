import { Component, ViewChild } from '@angular/core';
import { Observable } from 'rxjs';
import { ToastContainerDirective, ToastrService } from 'ngx-toastr';
import { Alerts, AlertAction, AlertActionTaken } from './models/dashboard';
import { FormGroup, FormBuilder } from '@angular/forms';
import { RestService } from './rest.service';
import { ActivatedRoute, Router } from '@angular/router';
import { take } from 'rxjs/operators';
import { NGXLogger } from 'ngx-logger';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  @ViewChild(ToastContainerDirective, { static: true }) toastContainer: ToastContainerDirective;

  title = 'phoweb';
  alerts: Alerts[];
  alertAction: AlertAction;
  alertScheduleId: number;
  updateAlert: FormGroup;
  constructor(public rest: RestService, private route: ActivatedRoute, private router: Router,
    private toastr: ToastrService, public fb: FormBuilder, private logger: NGXLogger) {
    // var id = this.userId.snapshot.paramMap.get('id') TODO: Need User Table;
    this.logger.log("testing the logging in app.component.ts constructor with NGXLogger");

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
          let toasterMessage = `<i class="fas fa-exclamation-triangle alert-icon" title="${alert.definition}"></i>
        ${alert.message}<a class="alert-link" href="${alert.url}">${alert.linkText}»</a>`;


          var activeToaster = this.toastr.success(toasterMessage, alert.alertScheduleId.toString(), {
            closeButton: true,
            disableTimeOut: true,
            enableHtml: true,
            tapToDismiss: false
          });
          activeToaster.onHidden
            .pipe(take(1))
            .subscribe((data) => this.toasterCloseHandler(alert.alertScheduleId));
          activeToaster.onTap
            .pipe(take(1))
            .subscribe((data) => this.toasterClickHandler(alert.alertScheduleId));

        });
      }

    });
  }


  toasterCloseHandler(alertScheduleId) {
    this.alertAction = { alertActionId: AlertActionTaken.close };
    this.rest.updateAlertActivity(alertScheduleId, this.alertAction).subscribe(res => { });
  }

  toasterClickHandler(alertScheduleId) {
    this.alertAction = { alertActionId: AlertActionTaken.click };
    this.rest.updateAlertActivity(alertScheduleId, this.alertAction).subscribe(res => { });
  }

}
