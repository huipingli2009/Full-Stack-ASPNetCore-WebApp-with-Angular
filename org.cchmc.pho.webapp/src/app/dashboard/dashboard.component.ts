import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { RestService } from '../rest.service';
import { ToastrService, ToastContainerDirective } from 'ngx-toastr';
import { take } from 'rxjs/operators';
import { FormGroup, FormBuilder } from '@angular/forms';
import { Alerts } from '../models/alerts';
import { element } from 'protractor';
import { OverlayContainer } from '@angular/cdk/overlay';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})

export class DashboardComponent implements OnInit {

  @ViewChild(ToastContainerDirective, {static: true}) toastContainer: ToastContainerDirective;
  
  alerts: Alerts[];
  updateAlert: FormGroup;
  alertScheduleId: number;

  constructor(public rest: RestService, private route: ActivatedRoute, private router: Router,
              private toastr: ToastrService, public fb: FormBuilder ) {
    // var id = this.userId.snapshot.paramMap.get('id') TODO: Need User Table;
   }

  ngOnInit() {
    this.getAlerts(3);
  }
 
  getAlerts(id) {
    this.alerts = [];
    this.rest.getAlerts(id).subscribe((data) => {
      console.log('getAlerts', data);
      this.alerts = data;
      this.alertScheduleId = data.Alert_ScheduleId;
      console.log('updateAlertsData', this.alerts[0].Alert_ScheduleId);
      this.toastr.overlayContainer  = this.toastContainer;
      this.showAlert();
    });
  }

  showAlert() {
    if (this.alerts.length > 0) {
      this.alerts.forEach(alert => {
        this.toastr.success('<i class="fas fa-exclamation-triangle alert-icon" title="' + alert.AlertDefinition + '"></i>' +
        alert.AlertMessage + '<a class="alert-link" href="' + alert.URL +
        '">' + alert.URL_Label + ' »</a>', alert.Alert_ScheduleId.toString(), {
          closeButton: true,
          disableTimeOut: true,
          enableHtml: true,
          tapToDismiss: false
        })
        .onTap
        .pipe(take(1))
        .subscribe(() => this.toasterClickedHandler(alert.Alert_ScheduleId));
        console.log("Showing Alert Hit");
      });
    }
}

toasterClickedHandler(sheduleId) {
  console.log('Toastr clicked', sheduleId); // TODO Remove Alert based on id
  // const id = 3;
  
  // this.updateAlert = this.fb.group({
  //   alertActionId: [1]
  // });
  // this.rest.updateAlertActivity(id, sheduleId, this.updateAlert.value).subscribe(res => {});
}
}
