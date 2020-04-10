import { Component, OnInit, ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { RestService } from '../rest.service';
import { take } from 'rxjs/operators';
import { NGXLogger } from 'ngx-logger';
import { MatDialog } from '@angular/material/dialog';

@Component({
  selector: 'app-files',
  templateUrl: './files.component.html',
  styleUrls: ['./files.component.scss']
})
export class FilesComponent implements OnInit {

  fileList: FileList[];
  watchFlag: string;
  resources: string;
  resourcesList: any[] = [];
  initiativesList: any[] = [];
  tagList: any[] = [];

  displayedColumns: string[] = ['icon', 'name', 'dateCreated', 'lastViewed', 'watchFlag'
  , 'fileType', 'actions', 'tags'];
  dataSource: MatTableDataSource<FileList>;

  @ViewChild(MatPaginator, {static: true}) paginator: MatPaginator;
  @ViewChild(MatSort, {static: true}) sort: MatSort;

  constructor(private rest: RestService, private logger: NGXLogger, private dialog: MatDialog) {
    
   }

  ngOnInit(): void {
    this.getAllFiles();
    this.getResourceTypes();
  }

  getAllFiles() {
    this.rest.getAllFiles().pipe(take(1)).subscribe((data) => {
      this.fileList = data;
      this.dataSource = new MatTableDataSource(this.fileList);
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
      this.logger.log(this.fileList, 'FILES');
    })
  }

  updateWatchlistStatus(id, index) {
    // this.rest.updateWatchlistStatus(id).subscribe((data) => {
    // });
    this.logger.log(id, index);
  }

  /* Gets / Sorts for Filtering */
  getResourceTypes() {
    this.rest.getFileResources().pipe(take(1)).subscribe((data) => {
      this.resourcesList = data;
    })
  }

  resourceTypeSelection() {
    this.logger.log('Resource Selected');
  }

}
