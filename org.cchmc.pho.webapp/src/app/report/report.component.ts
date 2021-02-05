import { Component, OnInit } from '@angular/core';
import { take } from 'rxjs/operators';
import { EdChartDetails } from '../models/dashboard';
import { RestService } from '../rest.service';
import * as XLSX from 'xlsx'; 

const parameter: string = '19010101';
@Component({
  selector: 'app-report',
  templateUrl: './report.component.html',
  styleUrls: ['./report.component.scss']
})

export class ReportComponent implements OnInit {
  edChartDetails: EdChartDetails[];

  //download to excel
  fileName= 'EDChart_Report.xlsx'; 

  constructor(private rest: RestService) {
  }

  ngOnInit() {
    this.GetEdChartData();
  }
  GetEdChartData() {
    this.rest.getWebChartDetails(parameter).pipe(take(1)).subscribe((data) => {
      this.edChartDetails = data;

    });
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
