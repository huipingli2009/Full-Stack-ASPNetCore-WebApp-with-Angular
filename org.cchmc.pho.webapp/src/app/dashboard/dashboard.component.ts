import { DatePipe } from '@angular/common';
import { Component, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { FormBuilder, FormsModule } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { ActivatedRoute, Router } from '@angular/router';
import * as Chart from 'chart.js';
import { NGXLogger } from 'ngx-logger';
import { environment } from 'src/environments/environment';
import { WebChart, WebChartDataSet, EdChartDetails, Population, Quicklinks, Spotlight, WebChartFilters, WebChartFilterMeasureId, WebChartId, WebChartFilterId, DrillThruMeasureId} from '../models/dashboard';
import { RestService } from '../rest.service';
import { DrilldownService } from '../drilldown/drilldown.service';
import { AuthenticationService } from '../services/authentication.service';
import { FilterService } from '../services/filter.service';
import * as XLSX from 'xlsx'; 
import { UserService } from '../services/user.service';
import { CurrentUser, Role} from '../models/user';
import { take } from 'rxjs/operators';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})

export class DashboardComponent implements OnInit {

  @ViewChild('callEDDialog') callEDDialog: TemplateRef<any>;
  @ViewChild('callNonEDPopulationDialog') callNonEDPopulationDialog: TemplateRef<any>;


  currentUser: CurrentUser;
  currentUserId: number;
  drillThruUser: boolean;
  spotlight: Spotlight[];
  quickLinks: Quicklinks[];
  population: Population[];
  webChart: WebChart;
  graphDatasetsArray = [];
  graphLabelArray = [];
  //webChartData: any[];
  webChartDetails: EdChartDetails[];
  webchartfilterList: any[] = [];
  monthlySpotlightTitle: string;
  monthlySpotlightBody: string;
  monthlySpotlightLink: string;
  monthlySpotlightLinkLabel: string;
  monthlySpotlightImageUrl: string;
  webChartTitle: string;
  webchartfilters: number;
  webChartTopLeftLabel: string; 
  webChartFilterIdEnum = WebChartFilterId;
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
  webChartObj: Chart;
  selectedBar: string;
  isLoggedIn$: boolean;
  patientsMax: number; 
  chartXValue: string[]; 
  reportFilterSelected: boolean = true;

  drilldownOptions = {
    measureId: '42'
  };

  //download to excel
  fileName= 'EDChart_Data.xlsx'; 
  
  //dynamic chart - and setting of initial values
  chartId: number; 
  measureId: number; 
  filterId: number;
  chartData: number[] ;
  chartLabel: string[];
  chartCategorySelected: number;

  constructor(public rest: RestService, private route: ActivatedRoute, private router: Router,
              public fb: FormBuilder, public dialog: MatDialog, private datePipe: DatePipe, private logger: NGXLogger,
              private authenticationService: AuthenticationService, private filterService: FilterService,private userService: UserService,
              private drilldownService: DrilldownService) {
    // var id = this.userId.snapshot.paramMap.get('id') TODO: Need User Table;
    this.dataSourceOne = new MatTableDataSource;
    this.dataSourceTwo = new MatTableDataSource;
    this.dataSourceThree = new MatTableDataSource;
  }

  public barChartLabels = [];


  ngOnInit() {
    this.isLoggedIn$ = this.authenticationService.isUserLoggedIn;
    this.exportChartData();
    this.getSpotlight();
    this.getQuicklinks();
    this.getPopulation();
    this.chartId = WebChartId.PopulationChart; 
    this.measureId = WebChartFilterMeasureId.edChartdMeasureId; 
    this.filterId = WebChartFilterId.dateFilterId;
    this.webchartfilters = this.filterId;
    this.getWebChartFilters(this.chartId, this.measureId); 
    this.logger.log("ngOnInit: filterid= " + this.filterId.toString());    
  }

  ngOnChanges() {
    if (this.webChartObj != undefined){
     
        this.webChartObj.update();  
    }   
  } 
  
  ngAfterViewInit() {  
    //NOTE: May require tweaking in this method after testing.   
    const $this = this;
  }

  exportChartData() {
    this.userService.getCurrentUser().pipe(take(1)).subscribe((data) => {
      this.currentUser = data;
      this.currentUserId = data.id;

      //PHO Member and PHO Leader are excluded from exporting/viewing chart data details
      if (data.role.id === Role.PHOMember || data.role.id === Role.PHOLeader) {
        this.drillThruUser = false;
      } 
      else { 
        this.drillThruUser = true; 
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
            measureId: item.measureId,
            opDefURL: item.opDefURL
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
            conditionId: item.conditionId,
            opDefURL: item.opDefURL   
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
    this.filterService.updateFilterType(element.conditionId);
    this.router.navigate(['/patients']);
  }


  /* ED Chart =========================================*/
  getWebChart(chartId: number, measureId: number, filterId: number) {    
    
    this.webChart = null;
    let max = 0;
    let counter = 0;    

   if(this.webChartObj !== undefined) {   

     let n = this.webChartObj.data.labels.length;

     for (counter = 0; counter < n; counter++){
      this.removeData(this.webChartObj);          
    }   
  }   

    this.rest.showViewReportButton = measureId === WebChartFilterMeasureId.edChartdMeasureId;

    this.rest.getWebChartByUser(chartId,measureId,filterId).subscribe((data) => {     
      this.webChart = data;
      this.webChartTitle = this.webChart.title;   
      this.webChartTopLeftLabel = this.webChart.headerLabel;
  
      this.graphDatasetsArray = [];
      this.graphLabelArray = [];
      var chartType = '';
      var i;

      // assign x axis labels
      this.graphLabelArray = this.webChart.dataSets[0].xAxisLabels;

      for (i=0; i < this.webChart.dataSets.length; i++)
      { 
        var highestValue = Math.max.apply(null, this.webChart.dataSets[i].values);
        if (highestValue > this.patientsMax) { this.patientsMax = (highestValue+1); }
        chartType = this.webChart.dataSets[i].type;

        //Create a chartJS dataset for each dataset we've received from API
        this.graphDatasetsArray[i] = 
                          {
                          label: this.webChart.dataSets[i].legend,
                          data: this.webChart.dataSets[i].values, 
                          maxBarThickness: 22,
                          lineTension: 0,
                          backgroundColor: this.webChart.dataSets[i].backgroundColor,
                          hoverBackgroundColor: this.webChart.dataSets[i].backgroundHoverColor,
                          borderColor: this.webChart.dataSets[i].borderColor,
                          fill: this.webChart.dataSets[i].fill
                          }
      }
      
      //SET THE GRAPH CONFIGURATION VALUES
      var chartConfig = {
        type: chartType,
        data: {
        labels: this.graphLabelArray,
        datasets: this.graphDatasetsArray
        },
        options: this.getChartOptions(this.patientsMax)   
      };
      

      this.logger.log(chartConfig, "chartConfig");        
      this.canvas = document.getElementById('webChart');     
      this.ctx = this.canvas.getContext('2d'); 
      this.webChartObj = new Chart(this.ctx, chartConfig);
      

      //this.patientsMax = max + 1; // This is here to add space above each bar in the chart (Max Number of patients, plus one empty tick on the y-axis)

    });
  }

  getChartOptions(yAxisTickMax: number){
    const $this = this;
    if (this.chartId === WebChartId.OutcomeChart){
      return {
        responsive: true,
        legend: {
          labels: {
            }
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
              beginAtZero: true,   //force the y-axis to start at 0      
                max: yAxisTickMax,
                stepSize: 1
            }
          }]        
        },
        tooltips: {
          enabled: true
        }   
      };
    }
    else{
      return {
        responsive: true,
        legend: {
          labels: {
            }
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
              beginAtZero: true,   //force the y-axis to start at 0      
                max: yAxisTickMax
            }
          }]        
        },
        tooltips: {
          enabled: true
        },
        onClick (e) {
          let element = this.getElementAtEvent(e);                  
          $this.Showmodal(e, this, element);
        }    
      };
    }
  }

  //get web chart filters
  getWebChartFilters(chartId: number, measureId: number) {
    this.rest.getWebChartFilters(chartId, measureId).subscribe((data) => {
      this.logger.log("filterlist: " + data[0] + " data: " + data);
      this.webchartfilterList = null;
      if (data.length > 0){        
        this.filterId = data[0].filterId;
        this.webchartfilterList = data;
        this.webchartfilters = data[0].filterId;
      }
      else
      { 
        this.filterId = WebChartFilterId.none; 
        this.webchartfilters = WebChartFilterId.none;
      }
      //New filters NECESSARILY means new chart generation. Trigger it here.
      this.getWebChart(this.chartId, this.measureId, this.filterId);
    });
  }

  //chart report condition change
  onWebReportConditionChange(event: any) {
    
    this.reportFilterSelected = false;
    this.filterId = event.value;
    this.reportFilterSelected = true;
    //dynamically pass the parameters to getWebChart function to generate the report
    this.getWebChart(this.chartId, this.measureId, this.filterId);
    //this.webChartObj.update();
  }

  updateChartReport(element: any){
    this.logger.log("switching chart object to measureId: " + element.measureId.toString() + " filterId: " + this.filterId.toString());
    this.measureId = element.measureId;
    if([26,14,15,16,25,20,21,22].includes(this.measureId)){
      this.chartId = WebChartId.OutcomeChart;
    }
    else
    {
      this.chartId = WebChartId.PopulationChart;
    }
    this.getWebChartFilters(this.chartId, this.measureId);
    //this.generateChart();
    this.logger.log("switching chart object 2 to measureId: " + element.measureId.toString() + " filterId: " + this.filterId.toString());
  }

  //click WEB CHART to switch back to ED CHART
  switchBackToEDChart() {
    this.measureId = WebChartFilterMeasureId.edChartdMeasureId;
    this.chartId = WebChartId.PopulationChart;
    this.getWebChartFilters(this.chartId, this.measureId);
    //this.generateChart();
  }

  transformToolTipDate(date) {
    return this.datePipe.transform(date, 'MM/dd/yyyy');
  }

  transformDate(date) {
    return this.datePipe.transform(date, 'EE MM/dd');
  }
  transformAdmitDate(date) {
    let localYear = new Date().getFullYear();
    this.logger.log("formatteddate " + this.datePipe.transform(date + ' , ' + localYear.toString(), 'yyyyMMdd').toString());
    return this.datePipe.transform(date + ' , ' + localYear, 'yyyyMMdd').toString();
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
    
    this.logger.log("starting ED modal");

    let drillThruMeasureId;
    let tempFilterId;   
    //Only drilldown for non outcome charts 
    if (this.chartId !== WebChartId.OutcomeChart){

      if (this.measureId === WebChartFilterMeasureId.edChartdMeasureId){
        this.logger.log("measure is edchart, loading dialog");
        this.logger.log("selected bar: " + element[0]._model.label);      
        this.selectedBar = element[0]._model.label;     
        
        drillThruMeasureId = DrillThruMeasureId.EDDrillThruMeasureId;
        tempFilterId = element[0]._index + 1;     
      }
      else {   //all the non-ED chart reports 
        this.logger.log("measure is non edchart, loading dialog");
        this.logger.log("selected bar: " + element[0]._index);     
      
        drillThruMeasureId = DrillThruMeasureId.PatientListDrillThruMeasureId;
        tempFilterId = element[0]._index + 1;       
      }
      //only if security allows
      if (this.drillThruUser){
        this.openDrilldownDialog(drillThruMeasureId,tempFilterId);
      }
    }
    
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

  openDrilldownDialog(measure,filterId) { 

    let drillThruText;   

    //Only ED Chart with Date Selected as Filter displays 'ED Details', the rest displays 'Patient Details'
    if (measure == DrillThruMeasureId.EDDrillThruMeasureId && this.filterId == WebChartFilterId.dateFilterId) {
      drillThruText = 'ED Details';       
    }
    else {
      drillThruText = 'Patient Details';      
    }
    
    var drilldownOptions = {
      measureId: measure, 
      filterId: filterId, 
      displayText: drillThruText     
    };
    this.drilldownService.open(drilldownOptions);
  }
}
