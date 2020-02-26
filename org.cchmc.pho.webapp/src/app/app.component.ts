import { Component, ViewChild } from '@angular/core';
import { Observable } from 'rxjs';
import { ToastContainerDirective, ToastrService } from 'ngx-toastr';
import { Alerts } from './models/dashboard';
import { FormGroup, FormBuilder } from '@angular/forms';
import { RestService } from './rest.service';
import { ActivatedRoute, Router } from '@angular/router';
import { take } from 'rxjs/operators';

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
              private toastr: ToastrService, public fb: FormBuilder) {
    // var id = this.userId.snapshot.paramMap.get('id') TODO: Need User Table;
  }

  ngOnInit() {
    this.getAlerts(3); // TODO: Temp User ID Value
  }

  ngAfterContentChecked(): void {
    this.toastr.overlayContainer = this.toastContainer;
    this.showAlert();

  }

  getAlerts(id) {
    this.alerts = [];
    this.rest.getAlerts(id).subscribe((data) => {
      this.alerts = data;
      // console.log('updateAlertsData', this.alerts[0].Alert_ScheduleId);

    });
  }

  showAlert() {
    if (this.alerts.length > 0) {
      this.alerts.forEach(alert => {
        let str1 = `<i class="fas fa-exclamation-triangle alert-icon" title="${alert.AlertDefinition}"></i>
      ${alert.AlertMessage}<a class="alert-link" href="${alert.URL}">${alert.URL_Label}»</a>`;


        this.toastr.success(str1, alert.Alert_ScheduleId.toString(), {
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
