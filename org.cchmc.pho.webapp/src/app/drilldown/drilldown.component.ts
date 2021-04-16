import { Component, OnInit, Inject, HostListener } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { RestService } from '../rest.service';
import { NGXLogger } from 'ngx-logger';
import { MetricDrillthruTable, MetricDrillthruRow, MetricDrillthruColumn } from '../models/drillthru';
import * as XLSX from 'xlsx'; 
import { environment } from 'src/environments/environment';
import { FilterService } from '../services/filter.service';
import { WebChartFilterMeasureId } from '../models/dashboard';

@Component({
  selector: 'app-drilldown',
  templateUrl: './drilldown.component.html',
  styleUrls: ['./drilldown.component.scss']
})

export class DrilldownComponent implements OnInit {

  get showViewReportButton(): boolean | null {
    return this.rest.showViewReportButton;
  }

  drilldownMeasureId: string;
  originMeasureId: string;
  selectedFilterId: string;
  table: MetricDrillthruTable;
  rows: MetricDrillthruRow[];
  primaryRow: MetricDrillthruRow;
  headerColumns: MetricDrillthruColumn[];  
  headerDisplayText: string;
  fileName= 'Data_Export.xlsx'; 
  isLoading: boolean;
  defaultUrl = environment.apiURL; 
  displayViewReport: boolean = false;

  
  constructor(public rest: RestService,private router: Router,private logger: NGXLogger, private filterService: FilterService, @Inject(MAT_DIALOG_DATA) public data: {
    drilldownMeasureId: string,
    filterId: string,
    displayText: string,
    originMeasureId: string
}, private mdDialogRef: MatDialogRef<DrilldownComponent>)
{
  this.isLoading = true;
  this.drilldownMeasureId = data.drilldownMeasureId;
  this.originMeasureId = data.originMeasureId;
  this.selectedFilterId = data.filterId;
  this.headerDisplayText = data.displayText;

  this.getMeasureDetailsTable();
}

  ngOnInit(): void {} 

  getMeasureDetailsTable(){
    this.logger.log(this.drilldownMeasureId, 'getMeasureDetailsTable');
    this.displayViewReport = this.showViewReportButton;

    this.rest.getMeasureDrilldownTable(this.drilldownMeasureId, this.selectedFilterId).subscribe((data) => {
      this.logger.log(data, 'metricDrilldownData');
      this.table = data;
      this.rows = this.table.rows;
      this.primaryRow = this.table.rows[0];
      this.headerColumns = this.primaryRow.columns; 
      this.isLoading = false;   
    });    
  }

  public cancel() {
    this.close(false);
  }
  public close(value) {
    this.mdDialogRef.close(value);
  }
  public confirm() {
    this.close(true);
  }
  
  public exportExcel() {
       /* table id is passed over here */   
       let element = document.getElementById('drilldownTable'); 
       const ws: XLSX.WorkSheet =XLSX.utils.table_to_sheet(element);

       /* generate workbook and add the worksheet */
       const wb: XLSX.WorkBook = XLSX.utils.book_new();
       XLSX.utils.book_append_sheet(wb, ws, 'Sheet1');

       /* save to file */
       XLSX.writeFile(wb, this.fileName);
  }

  onSelectedPatient(id: number){  
    this.rest.selectedPatientId = id;

    if (this.originMeasureId == WebChartFilterMeasureId.potentialActive.toString()){
      this.filterService.updateIsFilteringPatients(true);
      this.filterService.updateFilterType(this.originMeasureId);
    }
    //this.rest.selectedPatientName = name;
    this.router.navigate(['/patients']);
    this.close(false);
   } 

  public OpenReport() {
    window.open(`${this.defaultUrl}/edreport`, '_blank');       
  } 
}