import { DatePipe } from '@angular/common';
import { Component, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { ActivatedRoute, Router } from '@angular/router';
import * as Chart from 'chart.js';
import { NGXLogger } from 'ngx-logger';
import { environment } from 'src/environments/environment';
import { EdChart, EdChartDetails, Population, Quicklinks, Spotlight } from '../models/dashboard';
import { RestService } from '../rest.service';
import { DrilldownService } from '../drilldown/drilldown.service';
import { AuthenticationService } from '../services/authentication.service';
import { FilterService } from '../services/filter.service';
import * as XLSX from 'xlsx'; 

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})

export class DashboardComponent implements OnInit {

  @ViewChild('callEDDialog') callEDDialog: TemplateRef<any>;

  spotlight: Spotlight[];
  quickLinks: Quicklinks[];
  population: Population[];
  edChart: EdChart[];
  edChartData: any[];
  edChartDetails: EdChartDetails[];
  monthlySpotlightTitle: string;
  monthlySpotlightBody: string;
  monthlySpotlightLink: string;
  monthlySpotlightImageUrl: string;
  edChartTitle: string;
  defaultUrl = environment.apiURL; 
  popData: any[] = [];
  qiData: any[] = [];
  conditionData: any[] = [];
  dataSourceOne: MatTableDataSource<any>;
  displayedColumns: string[] = ['dashboardLabel', 'practiceTotal', 'networkTotal'];
  dataSourceTwo: MatTableDataSource<any>;
  displayedColumnsQi: string[] = ['dashboardLabelQi', 'practiceTotalQi', 'networkTotalQi'];
  dataSourceThree: MatTableDataSource<any>;
  displayedColumnsCondition: string[] = ['dashboardLabelCondition', 'practiceTotalCondition', 'networkTotalCondition'];
  // Chart Options
  canvas: any;
  ctx: any;
  edBarChart: any;
  selectedBar: string;
  isLoggedIn$: boolean;
  patientsMax: number;  

  drilldownOptions = {
    measureId: '42'
  };

  //download to excel
  fileName= 'EDChart_Data.xlsx'; 

  constructor(public rest: RestService, private route: ActivatedRoute, private router: Router,
              public fb: FormBuilder, public dialog: MatDialog, private datePipe: DatePipe, private logger: NGXLogger,
              private authenticationService: AuthenticationService, private filterService: FilterService,
              private drilldownService: DrilldownService) {
    // var id = this.userId.snapshot.paramMap.get('id') TODO: Need User Table;
    this.dataSourceOne = new MatTableDataSource;
    this.dataSourceTwo = new MatTableDataSource;
    this.dataSourceThree = new MatTableDataSource;
  }

  public barChartLabels = [];


  ngOnInit() {
    this.isLoggedIn$ = this.authenticationService.isUserLoggedIn;
    this.getSpotlight();
    this.getQuicklinks();
    this.getPopulation();
    this.getEdChart();
  }
  ngAfterViewInit() {
    const $this = this; // This is the only way to get the bar chart to work using functions out of scope.(Fat arrow does not work)
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
          xAxes: [{
            ticks: {
              callback (value, index, values) {
                return $this.transformDate(value);
              }
            }
          }],
          yAxes: [{
            ticks: {
              max: this.patientsMax
            }
          }]

        },
        tooltips: {
          enabled: true
        },
        onClick (e) {
          let element = this.getElementAtEvent(e);
          if (element.length) {
            // this.selectedBar = element[0]._model.label;
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
      this.monthlySpotlightImageUrl = `${this.defaultUrl}/assets/img/${imageName}`;
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
            networkTotal: item.networkTotal,
            measureId: item.measureId
          });
          this.dataSourceOne.data = this.popData;
        }
        if (item.measureType === 'QI') {
          this.qiData.push({
            dashboardLabel: item.dashboardLabel,
            measureDesc: item.measureDesc,
            practiceTotal: item.practiceTotal,
            networkTotal: item.networkTotal,
            measureId: item.measureId
          });
          this.dataSourceTwo.data = this.qiData;
        }
        if (item.measureType === 'COND') {
          this.conditionData.push({
            dashboardLabel: item.dashboardLabel,
            measureDesc: item.measureDesc,
            practiceTotal: item.practiceTotal,
            networkTotal: item.networkTotal,
            measureId: item.measureId
          });
          this.dataSourceThree.data = this.conditionData;
        }
      });
    });
  }

  // Send click to Filtered Patients
  toFilteredPatients(element) {
    this.filterService.updateIsFilteringPatients(true);
    this.filterService.updateFilterType(element.measureId);
    this.router.navigate(['/patients']);
  }
  // Send click to Filtered Outcomes
  toFilteredOutcomes(element) {
    this.logger.log(element.measureId, "toFilteredOutcomes: measureId");
    this.filterService.updateIsFilteringOutcomes(true);
    this.filterService.updateFilterType(element.measureId);
    this.router.navigate(['/patients']);
  }
  // Send click to Filtered Patients
  toFilteredConditions(element) {
    this.logger.log(element.measureId, "toFilteredConditions: measureId");
    this.filterService.updateIsFilteringConditions(true);
    this.filterService.updateFilterType(element.measureId);
    this.router.navigate(['/patients']);
  }


  /* ED Chart =========================================*/
  getEdChart() {
    this.edChart = [];
    let max = 0;
    this.rest.getEdChartByUser().subscribe((data) => {
      this.edChart = data;
      this.edChartTitle = this.edChart[0].chartTitle;
      this.edChart.forEach(item => {
        this.addData(this.edBarChart,
          this.transformToolTipDate(item.admitDate),
          item.edVisits); // Getting data to the chart, will be easier to update if needed
        if (item.edVisits > max) {
          max = item.edVisits;
        }
      });
      this.patientsMax = max + 1; // This is here to add space above each bar in the chart (Max Number of patients, plus one empty tick on the y-axis)
      this.edBarChart.config.options.scales.yAxes[0].ticks.max = this.patientsMax;
    });
  }

  transformToolTipDate(date) {
    return this.datePipe.transform(date, 'MM/dd/yyyy');
  }

  transformDate(date) {
    return this.datePipe.transform(date, 'EE MM/dd');
  }
  transformAdmitDate(date) {
    return this.datePipe.transform(date, 'yyyyMMdd');
  }

  addData(chart, label, data) {
    chart.data.labels.push(label);
    chart.data.datasets.forEach((dataset) => {
      dataset.data.push(data);
    });
    chart.update();
  }

  /* Open Modal (Dialog) on bar click */
  Showmodal(event, chart, element): void {
    this.selectedBar = this.transformAdmitDate(element[0]._model.label);
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

  OpenReport() {
    window.open(`${this.defaultUrl}/edreport`, '_blank');       
  }

  onSelectedPatient(id: number, name: string){  

    this.rest.selectedPatientId = id;
    this.rest.selectedPatientName = name;
    this.router.navigate(['/patients']);

    this.dialog.closeAll();    
   }  

   public downloadToExcel() {
    /* table id is passed over here */   
    let element = document.getElementById('EDChartData'); 
    const ws: XLSX.WorkSheet =XLSX.utils.table_to_sheet(element);

    /* generate workbook and add the worksheet */
    const wb: XLSX.WorkBook = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(wb, ws, 'Sheet1');

    /* save to file */
    XLSX.writeFile(wb, this.fileName);  
    
  }
}
