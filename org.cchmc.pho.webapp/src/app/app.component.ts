import { Component, ViewChild } from '@angular/core';
import { Observable } from 'rxjs';
import { ToastContainerDirective, ToastrService } from 'ngx-toastr';
import { Alerts } from './models/dashboard';
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
  alertScheduleId: number;
  updateAlert: FormGroup;

  constructor(public rest: RestService, private route: ActivatedRoute, private router: Router,
    private toastr: ToastrService, public fb: FormBuilder, private logger: NGXLogger) {
    // var id = this.userId.snapshot.paramMap.get('id') TODO: Need User Table;
    this.logger.log("testing app component.ts constructor with NGXLogger");
  }

  ngOnInit() {
    this.getAlerts(); // TODO: Temp User ID Value
  }

  ngAfterContentChecked(): void {
    this.toastr.overlayContainer = this.toastContainer;
    this.showAlert();

  }

  getAlerts() {
    this.alerts = [];
    this.rest.getAlerts().subscribe((data) => {
      this.alerts = data;
      // console.log('updateAlertsData', this.alerts[0].Alert_ScheduleId);

    });
  }

  showAlert() {
    if (this.alerts.length > 0) {
      this.alerts.forEach(alert => {
        let str1 = `<i class="fas fa-exclamation-triangle alert-icon" title="${alert.definition}"></i>
      ${alert.message}<a class="alert-link" href="${alert.url}">${alert.linkText}»</a>`;


        this.toastr.success(str1, alert.alertScheduleId.toString(), {
          closeButton: true,
          disableTimeOut: true,
          enableHtml: true,
          tapToDismiss: false
        })
          .onTap
          .pipe(take(1));
        // .subscribe(() => this.toasterClickedHandler(alert.Alert_ScheduleId));
      });
    }
  }

  toasterClickedHandler(sheduleId) {
    // console.log('Toastr clicked', sheduleId); // TODO Remove Alert based on id
    // const id = 3;

    // this.updateAlert = this.fb.group({
    //   alertActionId: [1]
    // });
    // this.rest.updateAlertActivity(id, sheduleId, this.updateAlert.value).subscribe(res => {});
  }

}
