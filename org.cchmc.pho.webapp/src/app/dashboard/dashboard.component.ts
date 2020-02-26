import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { RestService } from '../rest.service';
import { FormGroup, FormBuilder } from '@angular/forms';
import { Alerts, Content, Population } from '../models/dashboard';
import { MatTableDataSource } from '@angular/material/table';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})

export class DashboardComponent implements OnInit {

  content: Content[];
  population: Population[] = [];
  monthlySpotlightTitle: string;
  monthlySpotlightBody: string;
  monthlySpotlightLink: string;
  monthlySpotlightImage: string;
  quickLinks: any[] = [];
  popData: any[] = [];
  qiData: any[] = [];
  dataSourceOne: MatTableDataSource<any>;
  displayedColumns: string[] = ['dashboardLabel', 'practiceTotal', 'networkTotal'];
  dataSourceTwo: MatTableDataSource<any>;
  displayedColumnsQi: string[] = ['dashboardLabelQi', 'practiceTotalQi', 'networkTotalQi'];

  constructor(public rest: RestService, private route: ActivatedRoute, private router: Router,
              public fb: FormBuilder) {
    // var id = this.userId.snapshot.paramMap.get('id') TODO: Need User Table;
    this.dataSourceOne = new MatTableDataSource;
    this.dataSourceTwo = new MatTableDataSource;
  }

  ngOnInit() {
    this.getAllContent();
    this.getPopulation(7); // TODO: Temp Practice ID Value
  }

  // Dahsboard Content
  getAllContent() {
    this.content = [];
    this.rest.getDashboardContent().subscribe((data) => {
      this.content = data;
      this.content.forEach(content => {
        if (content.header !== null) {
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
      this.population.forEach(item => {
        if (item.measureType === 'POP') {
          this.popData.push({
            dashboardLabel: item.dashboardLabel,
            measureDesc: item.measureDesc,
            practiceTotal: item.practiceTotal,
            networkTotal: item.networkTotal
          });
          this.dataSourceOne.data = this.popData;
        }
        if (item.measureType === 'QI') {
          this.qiData.push({
            dashboardLabel: item.dashboardLabel,
            measureDesc: item.measureDesc,
            practiceTotal: item.practiceTotal,
            networkTotal: item.networkTotal
          });
          this.dataSourceTwo.data = this.qiData;
        }
      });
    });
  }
}
