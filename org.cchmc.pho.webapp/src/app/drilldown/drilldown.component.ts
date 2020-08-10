import { Component, OnInit, Inject, HostListener } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatTableDataSource, MatTable, MatTableModule } from '@angular/material/table';
import { RestService } from '../rest.service';
import { NGXLogger } from 'ngx-logger';
import { MetricDrillthruTable, MetricDrillthruRow, MetricDrillthruColumn } from '../models/drillthru';



@Component({
  selector: 'app-drilldown',
  templateUrl: './drilldown.component.html',
  styleUrls: ['./drilldown.component.scss']
})



export class DrilldownComponent implements OnInit {

  selectedMeasureId: string;
  selectedFilterId: string;
  table: MetricDrillthruTable;
  rows: MetricDrillthruRow[];
  primaryRow: MetricDrillthruRow;
  headerColumns: MetricDrillthruColumn[];  
  headerDisplayText: string;
  
  constructor(public rest: RestService,private logger: NGXLogger, @Inject(MAT_DIALOG_DATA) public data: {
    measureId: string,
    filterId: string,
    displayText: string
}, private mdDialogRef: MatDialogRef<DrilldownComponent>)
{
  console.info('incoming parameters: ', data);
  this.selectedMeasureId = data.measureId;
  this.selectedFilterId = data.filterId;
  this.headerDisplayText = data.displayText;
  console.info('selectedFilterId: ', this.selectedFilterId);
  this.getMeasureDetailsTable();
}

  ngOnInit(): void {
    
  }

  getMeasureDetailsTable(){
    this.rest.getMeasureDrilldownTable(this.selectedMeasureId, this.selectedFilterId).subscribe((data) => {
      this.logger.log(data, 'metricDrilldownData');
      console.info('metricDrilldownData: ', data);
      this.table = data;
      this.rows = this.table.rows;
      this.primaryRow = this.table.rows[0];
      console.info('primaryRow: ', this.primaryRow);
      this.headerColumns = this.primaryRow.columns;      
      console.info('headers: ', this.headerColumns);
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
  @HostListener("keydown.esc") 
  public onEsc() {
    this.close(false);
  }

}
