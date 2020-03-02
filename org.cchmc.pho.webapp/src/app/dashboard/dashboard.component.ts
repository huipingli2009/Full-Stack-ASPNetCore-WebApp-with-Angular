import { Component, OnInit, ViewChild, Inject, TemplateRef } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { RestService } from '../rest.service';
import { FormGroup, FormBuilder } from '@angular/forms';
import { Alerts, Population, EdChart, EdChartDetails, Spotlight, Quicklinks } from '../models/dashboard';
import { MatTableDataSource } from '@angular/material/table';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import * as Chart from 'chart.js';
import { EventEmitter } from 'protractor';
import { environment } from 'src/environments/environment';
import { DatePipe } from '@angular/common';
import { NGXLogger, LoggerConfig } from 'ngx-logger';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})

export class DashboardComponent implements OnInit {

  @ViewChild('callEDDialog') callEDDialog: TemplateRef<any>;
  
  spotlight: Spotlight[];
  quickLinks: Quicklinks[];
  population: Population[] = [];
  edChart: EdChart[];
  edChartData: any[] = [];
  edChartDetails: EdChartDetails[];
  monthlySpotlightTitle: string;
  monthlySpotlightBody: string;
  monthlySpotlightLink: string;
  monthlySpotlightImage: string;
  edChartTitle: string;
  defaultUrl = environment.apiURL;
  popData: any[] = [];
  qiData: any[] = [];
  dataSourceOne: MatTableDataSource<any>;
  displayedColumns: string[] = ['dashboardLabel', 'practiceTotal', 'networkTotal'];
  dataSourceTwo: MatTableDataSource<any>;
  displayedColumnsQi: string[] = ['dashboardLabelQi', 'practiceTotalQi', 'networkTotalQi'];
  // Chart Options
  canvas: any;
  ctx: any;
  edBarChart: any;
  selectedBar: string;

  constructor(public rest: RestService, private route: ActivatedRoute, private router: Router,
              public fb: FormBuilder, public dialog: MatDialog, private datePipe: DatePipe, private logger: NGXLogger) {
    // var id = this.userId.snapshot.paramMap.get('id') TODO: Need User Table;
    this.dataSourceOne = new MatTableDataSource;
    this.dataSourceTwo = new MatTableDataSource;
  }

  public barChartLabels = [];


  ngOnInit() {
    this.getSpotlight();
    this.getQuicklinks();
    this.getPopulation();
    this.getEdChart();
  }
  ngAfterViewInit() {
    let $this = this; // This is the only way to get the bar chart to work using functions out of scope.(Fat arrow does not work)
    this.canvas = document.getElementById('edChart');
    this.ctx = this.canvas.getContext('2d');
    this.edBarChart = new Chart(this.ctx, {
      type: 'bar',
      data: {
        labels: [],
        datasets: [{
          label: '# Patients',
          data: [],
          maxBarThickness: 22,
          backgroundColor: '#FABD9E',
          hoverBackgroundColor: '#F0673C'
        }]
      },
      options: {
        responsive: true,
        layout: {
          padding: {
            left: 42,
            right: 53,
            top: 27,
            bottom: 43
          }
        },
        scales: {
          yAxes: [{
            scaleLabel: {
            }
          }]
        },
        onClick: function (e) {
          var element = this.getElementAtEvent(e);
          if (element.length) {
            this.selectedBar = element[0]._model.label;
          }
          $this.Showmodal(e, this, element); // This is the result of a "fake" JQuery this
        }
      }
      });
  }


  // Dahsboard Content
  getSpotlight() {
    this.spotlight = [];
    this.rest.getSpotlight().subscribe((data) => {
      this.spotlight = data;
      const imageName = this.spotlight[0].imageHyperlink;
      this.monthlySpotlightTitle = this.spotlight[0].header;
      this.monthlySpotlightBody = this.spotlight[0].body;
      this.monthlySpotlightImage = `${this.defaultUrl}/assets/img/${imageName}`;
      this.monthlySpotlightLink = this.spotlight[0].hyperlink;
    });
  }

  getQuicklinks() {
    this.quickLinks = [];
    this.rest.getQuicklinks().subscribe((data) => {
      this.quickLinks = data;
    });
  }

  // Metric List
  getPopulation() {
    this.population = [];
    this.rest.getAllKpis().subscribe((data) => {
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
  /* ED Chart =========================================*/
  getEdChart() {
    this.edChart = [];
    this.rest.getEdChartByUser().subscribe((data) => {
      this.edChart = data;
      this.edChartTitle = this.edChart[0].chartTitle;
      this.edChart.forEach(item => {
        this.addData(this.edBarChart,
          this.transformDate(item.admitDate), item.edVisits); // Getting data to the chart, will be easier to update if needed
      });
    });
  }

  transformDate(date) {
    return this.datePipe.transform(date, 'EE MM/dd');
  }
  transformAdmitDate(date) {
    return this.datePipe.transform(date, 'MM/dd/YYYY');
  }

  addData(chart, label, data) {
    chart.data.labels.push(label);
    chart.data.datasets.forEach((dataset) => {
      dataset.data.push(data);
    });
    chart.update();
  }
  
  /* Open Modal (Dialog) on bar click */
  Showmodal(event, chart, element) : void {
    this.openDialogWithDetails();
  }
  openDialogWithDetails() {
    this.edChartDetails = [];
    this.transformAdmitDate(this.selectedBar);
    this.rest.getEdChartDetails(this.selectedBar).subscribe((data) => {
      this.edChartDetails = data;
      const dialogRef = this.dialog.open(this.callEDDialog);
    });
    
    // Leaving this here incase we need to handle some things when a modal closes
    // dialogRef.afterClosed().subscribe(result => {
      
    // });
  }
}