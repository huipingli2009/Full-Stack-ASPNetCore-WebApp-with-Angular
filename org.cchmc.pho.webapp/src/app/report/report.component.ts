import { Component, OnInit } from '@angular/core';
import { take } from 'rxjs/operators';
import { EdChartDetails } from '../models/dashboard';
import { RestService } from '../rest.service';
const parameter: string = '19010101';
@Component({
  selector: 'app-report',
  templateUrl: './report.component.html',
  styleUrls: ['./report.component.scss']
})

export class ReportComponent implements OnInit {
  edChartDetails: EdChartDetails[];
  constructor(private rest: RestService) {
  }

  ngOnInit() {
    this.GetEdChartData();
  }
  GetEdChartData() {
    this.rest.getEdChartDetails(parameter).pipe(take(1)).subscribe((data) => {
      this.edChartDetails = data;

    });
  }



}
