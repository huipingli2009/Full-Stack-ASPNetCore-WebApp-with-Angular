import { Component, OnInit, ViewChild, Inject } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { RestService } from '../rest.service';
import { FormGroup, FormBuilder } from '@angular/forms';
import { Alerts, Content, Population, EdChart, EdChartDetails } from '../models/dashboard';
import { MatTableDataSource } from '@angular/material/table';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})

export class DashboardComponent implements OnInit {
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

  constructor(public rest: RestService, private route: ActivatedRoute, private router: Router,
              public fb: FormBuilder, public dialog: MatDialog) {
    // var id = this.userId.snapshot.paramMap.get('id') TODO: Need User Table;
    this.dataSourceOne = new MatTableDataSource;
    this.dataSourceTwo = new MatTableDataSource;
  }
  public barChartOptions = {
    scaleShowVerticalLines: true,
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
onClick: () => {
  console.log(this.edChartData); // Maybe add hidden field
  this.openDialog();
}
  };

  public barChartLabels = [];
  public barChartType = 'bar';
  public barChartLegend = true;
  public barChartData = [
    {
      maxBarThickness: 22,
      backgroundColor: '#FABD9E',
      hoverBackgroundColor: '#F0673C',
      data: this.edChartData,
      label: '# Patients' // 12 Days
    }
  ];

  ngOnInit() {
    this.getAllContent();
    this.getPopulation();
    this.getEdChart();
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
        this.barChartLabels.push(item.chartLabel);
        this.edChartData.push(item.edVisits);
      });
    });
  }

  /* Open Modal (Dialog) on bar click */
  openDialog() {
    const dialogRef = this.dialog.open(ModalEdDetails);

    dialogRef.afterClosed().subscribe(result => {
      console.log(`Dialog result: ${result}`);
    });
  }

  /* Get ED Chart Details */
  getEdChartDetails() {
    this.edChartDetails = [];
    this.rest.getEdChartDetails('02/14/2020').subscribe((data) => {
      this.edChartDetails = data;
      console.log('eddetails', this.edChartDetails);
    });
  }
}

/* Modal for ED Chart Details - This could be seperated into it's own component...Not sure if completely necessary at the moment */
@Component({
    selector: 'modal-ed-details',
    templateUrl: 'modal-ed-details.html',
  })
  export class ModalEdDetails {

    constructor(
      public dialogRef: MatDialogRef<ModalEdDetails>,
      @Inject(MAT_DIALOG_DATA) public data: EdChartDetails) {}
  
    // onNoClick(): void {
    //   this.dialogRef.close();
    // }
  
  }
