import { DatePipe } from '@angular/common';
import { Component, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { FormBuilder, FormsModule } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { ActivatedRoute, Router } from '@angular/router';
import * as Chart from 'chart.js';
import { NGXLogger } from 'ngx-logger';
import { environment } from 'src/environments/environment';
import { EdChart, EdChartDetails, Population, Quicklinks, Spotlight, WebChartFilters, WebChartFilterMeasureId, WebChartId, WebChartFilterId} from '../models/dashboard';
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
  webChart: EdChart[];
  webChartData: any[];
  webChartDetails: EdChartDetails[];
  webchartfilterselectedFilter: string;
  webchartfilterList: any[] = [];
  monthlySpotlightTitle: string;
  monthlySpotlightBody: string;
  monthlySpotlightLink: string;
  monthlySpotlightLinkLabel: string;
  monthlySpotlightImageUrl: string;
  webChartTitle: string;
  webChartTopLeftLabel: string;
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
  webBarChart: any;
  selectedBar: string;
  isLoggedIn$: boolean;
  patientsMax: number; 
  chartXValue: string[]; 

  drilldownOptions = {
    measureId: '42'
  };

  //download to excel
  fileName= 'EDChart_Data.xlsx'; 
  
  //dynamic chart - and setting of initial values
  chartId: number = WebChartId.PopulationChart; 
  measureId: number = WebChartFilterMeasureId.edChartdMeasureId; 
  filterId: number = WebChartFilterId.dateFilterId; 
  chartData: number[] ;
  chartLabel: string[];

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
    this.getWebChartFilters(this.chartId, this.measureId);
    this.getWebChart(this.chartId, this.measureId, this.filterId);   
  }

  ngOnChanges() {
    if (this.webBarChart != undefined){
     
        this.webBarChart.update();  
    }   
  } 
  
  ngAfterViewInit() {  

    const $this = this; // This is the only way to get the bar chart to work using functions out of scope.(Fat arrow does not work)
    this.canvas = document.getElementById('webChart');     

    this.ctx = this.canvas.getContext('2d'); 

    this.webBarChart = new Chart(this.ctx, {
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
        legend: {
          position: 'bottom'
        },
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
                return value;
              }              
            }
          }],
          yAxes: [{
            ticks: {            
                max:this.patientsMax              
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
      this.logger.log(this.spotlight, "RETURNED SPOTLIGHT FIELDS");
      const imageName = this.spotlight[0].imageHyperlink;
      this.monthlySpotlightTitle = this.spotlight[0].header;
      this.monthlySpotlightBody = this.spotlight[0].body;      
      this.monthlySpotlightImageUrl = this.spotlight[0].imageHyperlink;
      this.monthlySpotlightLink = this.spotlight[0].hyperlink;
      this.monthlySpotlightLinkLabel = this.spotlight[0].hyperLinkLabel;
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
            measureId: item.measureId,
            opDefURL: item.opDefURL,
            opDefURLExists: item.opDefURL === ''? false: true           
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
  getWebChart(chartId: number, measureId: number, filterId: number) {    
  
    this.webChart = [];
    let max = 0;
    let counter = 0;    

   if(this.webBarChart !== undefined) {   

     let n = this.webBarChart.data.labels.length;

     for (counter = 0; counter < n; counter++){
      this.removeData(this.webBarChart);          
    }   
  }   

    this.rest.getWebChartByUser(chartId,measureId,filterId).subscribe((data) => {     

      this.webChart = data; 
   
      this.webChartTitle = this.webChart[0].chartTitle;   
      this.webChartTopLeftLabel = this.webChart[0].chartTopLeftLabel;

      this.webChart.forEach(item => {              

          this.addData(this.webBarChart,
         
          item.chartLabel,
          item.edVisits); // Getting data to the chart, will be easier to update if needed           
     
        if (item.edVisits > max) {
          max = item.edVisits;          
        }    
        
      });      
      
      this.patientsMax = max + 1; // This is here to add space above each bar in the chart (Max Number of patients, plus one empty tick on the y-axis)
      this.webBarChart.config.options.scales.yAxes[0].ticks.max = this.patientsMax;
      this.webBarChart.update();    
      
    });
  }

  //get web chart filters
  getWebChartFilters(chartId: number, measureId: number) {
    this.webchartfilterselectedFilter = null;
    this.rest.getWebChartFilters(chartId, measureId).subscribe((data) => {
      this.webchartfilterList = data;
      this.filterId = this.webchartfilterList[0].filterId;
      
      this.webchartfilterselectedFilter = this.webchartfilterList[0].filterId.toString();
    });
  }

  //chart report condition change
  onWebReportConditionChange(event: any) {
    

    this.filterId = event.value;
    this.chartId = WebChartId.PopulationChart;  
    
    //dynamically pass the parameters to getWebChart function to generate the report
    this.getWebChart(this.chartId, this.measureId, this.filterId);
    this.webBarChart.update();
  }

  updateChartReport(element: any){
    this.logger.log("switching chart object to measureId: " + element.measureId.toString());
    this.measureId = element.measureId;
    this.getWebChartFilters(this.chartId, this.measureId);
    this.getWebChart(this.chartId, element.measureId, this.filterId);
  }

  //click WEB CHART to switch back to ED CHART
  switchBackToEDChart() {
    this.getWebChart(WebChartId.EDChart, WebChartFilterMeasureId.conditionDefaultMeasureId, WebChartFilterId.conditionDefaultFilterId);
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

  removeData(chart) {
    chart.data.labels.pop();
    chart.data.datasets.forEach((dataset) => {
        dataset.data.pop();
    });
    chart.update();
}

  /* Open Modal (Dialog) on bar click */
  Showmodal(event, chart, element): void {
    this.selectedBar = this.transformAdmitDate(element[0]._model.label);
    this.openDialogWithDetails();
  }
  openDialogWithDetails() {
    this.webChartDetails = [];
    this.rest.getWebChartDetails(this.selectedBar).subscribe((data) => {
      this.webChartDetails = data;
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
