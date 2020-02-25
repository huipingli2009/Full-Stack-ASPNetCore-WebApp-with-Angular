import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { RestService } from '../rest.service';
import { ToastrService, ToastContainerDirective } from 'ngx-toastr';
import { take } from 'rxjs/operators';
import { FormGroup, FormBuilder } from '@angular/forms';
import { Alerts, Content, Population } from '../models/dashboard';
import { element } from 'protractor';
import { OverlayContainer } from '@angular/cdk/overlay';
import { TooltipPosition } from '@angular/material/tooltip';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})

export class DashboardComponent implements OnInit {

  @ViewChild(ToastContainerDirective, {static: true}) toastContainer: ToastContainerDirective;

  alerts: Alerts[];
  content: Content[];
  population: Population[] = [];
  updateAlert: FormGroup;
  alertScheduleId: number;
  monthlySpotlightTitle: string;
  monthlySpotlightBody: string;
  monthlySpotlightLink: string;
  monthlySpotlightImage: string;
  quickLinks: any[] = [];
  displayedColumns: string[] = ['dashboardLabel', 'practiceTotal', 'networkTotal'];

  constructor(public rest: RestService, private route: ActivatedRoute, private router: Router,
              private toastr: ToastrService, public fb: FormBuilder ) {
    // var id = this.userId.snapshot.paramMap.get('id') TODO: Need User Table;
   }

  ngOnInit() {
    this.getAlerts(3);
    this.getAllContent();
    this.getPopulation(7);
  }
  ngAfterContentChecked(): void {
    this.toastr.overlayContainer  = this.toastContainer;
    this.showAlert();

  }

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
        console.log('Showing Alert Hit');
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

// Dahsboard Content
getAllContent() {
  this.content = [];
  this.rest.getDashboardContent().subscribe((data) => {
    this.content = data;
    this.content.forEach(content => {
      if (content.header !== 'NULL') {
        this.monthlySpotlightTitle = content.header;
        this.monthlySpotlightBody = content.body;
        this.monthlySpotlightLink = content.hyperlink;
        this.monthlySpotlightImage = content.imageHyperlink;
      }
      if (content.contentPlacement === 'Quick Links') {
        this.quickLinks.push({
          body: content.body,
          link: content.hyperlink
        });
      }
    });
  });
}

// Metric List
getPopulation(id) {
  this.population = [];
  this.rest.getPopulationDetails(id).subscribe((data) => {
    this.population = data;
    console.log('pop data', this.population);
  });
}
}
