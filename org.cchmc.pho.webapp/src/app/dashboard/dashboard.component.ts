import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { TestService } from '../test.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})

export class DashboardComponent implements OnInit {

  alerts: any = [];

  constructor(public rest: TestService, private route: ActivatedRoute, private router: Router, private toastr: ToastrService) { }

  ngOnInit() {
    this.getAlerts(3);
  }
  getAlerts(id) {
    this.alerts = [];
    this.rest.getAlerts(id).subscribe((data: {}) => {
      console.log(data);
      this.alerts = data;
    });
    console.log(this.alerts);
  }

  showToaster(){
    this.toastr.success(this.alerts.AlertMessage, '', {
      closeButton: true,
      disableTimeOut: true,
      enableHtml: true,
      positionClass: 'toast-inline'

    });
}

}
