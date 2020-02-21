import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { TestService } from '../test.service';
import { ToastrService } from 'ngx-toastr';
import { take } from 'rxjs/operators';
import { FormGroup, FormBuilder } from '@angular/forms';
import { Alerts } from '../models/alerts';
import { element } from 'protractor';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})

export class DashboardComponent implements OnInit {

  alerts: Alerts[];
  updateAlert: FormGroup;
  alertScheduleId: number;

  constructor(public rest: TestService, private route: ActivatedRoute, private router: Router,
    private toastr: ToastrService, public fb: FormBuilder) {
    // var id = this.userId.snapshot.paramMap.get('id') TODO: Need User Table;
   }

  ngOnInit() {
    this.getAlerts(3);
  }
  ngAfterContentChecked(): void {
    this.showAlert();
  }
  // getAlerts(id) {
  //   // this.alerts = [];
  //   this.rest.getAlerts(id).subscribe((data) => {
  //     console.log('getAlerts', data);
  //     this.alerts = data.data;
  //     this.alertScheduleId = data.alertScheduleId;
  //     console.log('updateAlertsData', this.alertScheduleId);
  //   });
  // }
  getAlerts(id) {
    this.alerts = [];
    this.rest.getAlerts(id).subscribe((data) => {
      console.log('getAlerts', data);
      this.alerts = data;
      this.alertScheduleId = data.Alert_ScheduleId;
      console.log('updateAlertsData', this.alerts[0].Alert_ScheduleId);
    });
  }

  showAlert() {
    if (this.alerts.length > 0) {
      this.alerts.forEach(alert => {
        this.toastr.success('<i class="fas fa-exclamation-triangle alert-icon"></i>' +
        alert.AlertMessage + '<a class="alert-link" href="' + alert.URL +
        '">' + alert.URL_Label + '</a>', alert.Alert_ScheduleId.toString(), {
          closeButton: true,
          disableTimeOut: true,
          enableHtml: true,
          positionClass: 'toast-inline',
          tapToDismiss: false
        })
        .onTap
        .pipe(take(1))
        .subscribe(() => this.toasterClickedHandler(alert.Alert_ScheduleId));
      });
    }
}

toasterClickedHandler(sheduleId) {
  console.log('Toastr clicked', sheduleId);
  const id = 3;
  
  this.updateAlert = this.fb.group({
    alertActionId: [1]
  });
  this.rest.updateAlertActivity(id, sheduleId, this.updateAlert.value).subscribe(res => {});
}
}
