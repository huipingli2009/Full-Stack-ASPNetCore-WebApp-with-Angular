import { Component, OnInit, ViewChild, Inject, TemplateRef } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { RestService } from '../rest.service';
import { FormGroup, FormBuilder } from '@angular/forms';
import { Alerts, Content, Population, EdChart, EdChartDetails } from '../models/dashboard';
import { MatTableDataSource } from '@angular/material/table';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import * as Chart from 'chart.js';
import { EventEmitter } from 'protractor';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})

export class DashboardComponent implements OnInit {

  @ViewChild('callEDDialog') callEDDialog: TemplateRef<any>;
  
  content: Content[];
  population: Population[] = [];
  edChart: EdChart[];
  edChartData: any[] = [];
  edChartDetails: EdChartDetails[];
  monthlySpotlightTitle: string;
  monthlySpotlightBody: string;
  monthlySpotlightLink: string;
  monthlySpotlightImage: string;
  edChartTitle: string;
  quickLinks: any[] = [];
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
              public fb: FormBuilder, public dialog: MatDialog) {
    // var id = this.userId.snapshot.paramMap.get('id') TODO: Need User Table;
    this.dataSourceOne = new MatTableDataSource;
    this.dataSourceTwo = new MatTableDataSource;
  }

  public barChartLabels = [];


  ngOnInit() {
    this.getAllContent();
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
            left: 47,
            right: 68,
            top: 27,
            bottom: 43
          }
        },
        scales: {
          yAxes: [{
            ticks: {
              beginAtZero: true
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
        this.addData(this.edBarChart, item.admitDate, item.edVisits); // Getting data to the chart, will be easier to update if needed
      });
    });
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
    this.rest.getEdChartDetails(this.selectedBar).subscribe((data) => {
      this.edChartDetails = data;
      const dialogRef = this.dialog.open(this.callEDDialog);
    });
    
    // Leaving this here incase we need to handle some things when a modal closes
    // dialogRef.afterClosed().subscribe(result => {
      
    // });
  }
}