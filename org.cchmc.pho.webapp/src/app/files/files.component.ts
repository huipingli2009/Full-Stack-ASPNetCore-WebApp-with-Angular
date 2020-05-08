import { Component, OnInit, ViewChild, TemplateRef, ChangeDetectorRef } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { RestService } from '../rest.service';
import { take } from 'rxjs/operators';
import { NGXLogger } from 'ngx-logger';
import { MatDialog } from '@angular/material/dialog';
import { UserService } from '../services/user.service';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { FileAction, FileDetails, FileType } from '../models/files';
import {MatButtonModule} from '@angular/material/button';

@Component({
  selector: 'app-files',
  templateUrl: './files.component.html',
  styleUrls: ['./files.component.scss']
})
export class FilesComponent implements OnInit {

  fileList: FileList[];
  watchFlag: string;
  resources: string;
  initiatives: string;
  tags: string;
  resourcesList: any[] = [];
  initiativesList: any[] = [];
  fileTypeList: FileType[] = [];
  tagList: any[] = [];
  isUserAdmin: boolean;
  adminFileForm: FormGroup;
  compareFn: ((f1: any, f2: any) => boolean) | null = this.compareByValue;
  isSavingDraft: boolean;
  isPublishingFile: boolean;
  isPublishingWithAlert: boolean;
  currentFileId: number;
  currentDateCreated: Date;
  currentLastViewed: Date;
  currentWatchFlag: boolean;
  isAddingNewFile: boolean;
  error: any;
  deletingFileId: number;
  fileAction: FileAction;
  selectedFileValues: FileDetails;
  

  // Filters
  resourceTypeIdFilter: number;
  initiativeIdFilter: number;
  tagFilter: string;
  watchFilter: boolean;
  nameFilter: string;

  displayedColumns: string[] = ['icon', 'name', 'dateCreated', 'lastViewed', 'watchFlag'
    , 'fileType', 'actions', 'tags', 'button'];
  dataSource: MatTableDataSource<FileList>;
  recentlyAddedFileList: MatTableDataSource<FileList>;
  recentlyViewedFileList: MatTableDataSource<FileList>;
  mostPopularFileList: MatTableDataSource<FileList>;

  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild('adminDialog') adminDialog: TemplateRef<any>;
  @ViewChild('adminConfirmDialog') adminConfirmDialog: TemplateRef<any>;
  @ViewChild('adminConfirmDeleteDialog') adminConfirmDeleteDialog: TemplateRef<any>;

  //Files Analytics
  RecentlyAddedFiles: FileList[];
  RecentlyViewedFiles: FileList[];
  MostPopularFiles: FileList[];

  toggle5_RecentlyAdded: boolean = true;
  toggle5_RecentlyViewed: boolean = true;
  toggle5_MostPopular: boolean = true;

  recentlyAddedFilesdisplayedColumns: string[] = ['icon','name', 'dateCreated'];
  recentlyViewedFilesdisplayedColumns: string[] = ['icon','name', 'lastViewed'];
  mostPopularFilesdisplayedColumns: string[] = ['icon','name', 'viewCount'];


  constructor(private rest: RestService, private logger: NGXLogger, private dialog: MatDialog,
    private userService: UserService, private fb: FormBuilder, private changeDetectorRefs: ChangeDetectorRef) {

    this.adminFileForm = this.fb.group({

      practiceOnly: false,
      name: ['', Validators.required],
      fileURL: ['', Validators.required],
      tags: [''],
      author: [''],
      fileType: ['', Validators.required],
      description: ['', Validators.required],
      resourceType: ['', Validators.required],
      initiative: ['']
    });
  }

  ngOnInit(): void {
    this.getCurrentUser();
    this.getAllFiles();
    this.getResourceTypes();
    this.getInitiatives();
    this.getTagsList();
    this.getRecentlyAddedFiles();
    this.getRecentlyViewedFiles();
    this.getMostPopularFiles();
    this.getFileTypes();
  }

  compareByValue(o1, o2): boolean {
    return o1.id === o2.id;
  }

  getCurrentUser() {
    this.userService.getCurrentUser().subscribe((data) => {
      if (data.role.id === 3) {
        this.isUserAdmin = true;
      } else { this.isUserAdmin = false; }
    });
  }

  getAllFiles() {
    this.rest.getAllFiles().pipe(take(1)).subscribe((data) => {
      this.fileList = data;
      this.dataSource = new MatTableDataSource(this.fileList);
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
      this.logger.log(this.fileList, 'FILES');
    })
    this.changeDetectorRefs.detectChanges();
  }

  getRecentlyAddedFiles() {
    this.rest.getRecentlyAddedFiles(this.toggle5_RecentlyAdded).pipe(take(1)).subscribe((data) => {
      this.RecentlyAddedFiles = data;
      this.recentlyAddedFileList = new MatTableDataSource(this.RecentlyAddedFiles); //source data for table       
      this.logger.log(this.RecentlyAddedFiles, 'RECENTLY ADDED FILES');
    })
  }

  getRecentlyViewedFiles() {
    this.rest.getRecentlyViewedFiles(this.toggle5_RecentlyViewed).pipe(take(1)).subscribe((data) => {
      this.RecentlyViewedFiles = data;
      this.recentlyViewedFileList = new MatTableDataSource(this.RecentlyViewedFiles);
      this.logger.log(this.RecentlyViewedFiles, 'RECENTLY VIEWED FILES');
    })
  }

  getMostPopularFiles() {
    this.rest.getMostPopularFiles(this.toggle5_MostPopular).pipe(take(1)).subscribe((data) => {
      this.MostPopularFiles = data;
      this.mostPopularFileList = new MatTableDataSource(this.MostPopularFiles);
      this.logger.log(this.MostPopularFiles, 'MOST POPULAR FILES');
    })
  }

  getFileDetials(fileId) {
    this.currentFileId = fileId;
    this.rest.getFileDetails(fileId).pipe(take(1)).subscribe((data) => {
      this.logger.log('FILE DETAILS', data);
      this.currentDateCreated = data.dateCreated;
      this.currentLastViewed = data.lastViewed;
      this.currentWatchFlag = data.watchFlag;
      let joinedTags = [];
      data.tags.forEach(tag => {
        joinedTags.push(tag.name);
      });
   
      this.selectedFileValues = data;
               
      this.logger.log(this.selectedFileValues, 'SELECTED FILE')
      this.adminFileForm.get('name').setValue(this.selectedFileValues.name);
      this.adminFileForm.get('fileURL').setValue(this.selectedFileValues.fileURL);
      this.adminFileForm.get('name').setValue(this.selectedFileValues.name);
      this.adminFileForm.get('tags').setValue(joinedTags.join(', '));
      this.adminFileForm.get('author').setValue(this.selectedFileValues.author);
      this.adminFileForm.get('fileType').setValue(this.selectedFileValues.fileType);
      this.adminFileForm.get('description').setValue(this.selectedFileValues.description);
      this.adminFileForm.get('resourceType').setValue(this.selectedFileValues.resourceType);
      this.adminFileForm.get('initiative').setValue(this.selectedFileValues.initiative);
    })
  }

  updateFileAction(fileResourceId) {
    this.logger.log(fileResourceId);
    this.fileAction = new FileAction();
    this.fileAction.fileResourceId = fileResourceId;
    this.fileAction.fileActionId = 1;
    this.rest.updateFileAction(this.fileAction).pipe(take(1)).subscribe(res => {
      this.getAllFiles();
    },
      error => { this.error = error; });
  }

  submitFileAddUpdate() {
    let tagControl = this.adminFileForm.get('tags');
    let tagsSplit;
    if ((tagControl.value != '') && (tagControl.value != null)) {
      tagsSplit = tagControl.value.replace(/\s*,\s*/g, ",");
      tagsSplit = tagsSplit.split(',');
      tagsSplit = tagsSplit.map(function (e) {
        return { name: e };
      });
    }
    let publishFile;
    if (this.isPublishingFile === true || this.isPublishingWithAlert === true) {
      publishFile = true;
    } else { publishFile = false; }
    this.selectedFileValues = this.adminFileForm.value;
    this.selectedFileValues.tags = tagsSplit;
    this.selectedFileValues.id = this.currentFileId;
    this.selectedFileValues.dateCreated = this.currentDateCreated;
    this.selectedFileValues.lastViewed = this.currentLastViewed;
    this.selectedFileValues.watchFlag = this.currentWatchFlag;
    this.selectedFileValues.publishFlag = publishFile;
    this.selectedFileValues.createAlert = this.isPublishingWithAlert;
    this.logger.log(this.selectedFileValues, 'FORM SUBMISSION');
    if (this.isAddingNewFile === true) {
      this.selectedFileValues = this.adminFileForm.value;
      this.selectedFileValues.tags = tagsSplit;
      this.selectedFileValues.publishFlag = publishFile;
      this.selectedFileValues.createAlert = this.isPublishingWithAlert;
      this.rest.addNewFile(this.selectedFileValues).pipe(take(1)).subscribe((data) => {
        this.logger.log(data, 'New File');
        this.getTagsList();
      });
    } else {
      this.rest.updateFileDetails(this.selectedFileValues).pipe(take(1)).subscribe((data) => {
        this.logger.log(data, 'PUT FILES');
        this.getTagsList();
      });
    }
    this.cancelAdminDialog();

  }

  updateWatchlistStatus(id, index) {
    this.rest.updateFileWatchlistStatus(id).subscribe((data) => {
    });
  }

  /* Delete File (Admin ONly)*/
  deleteFile() {
    this.rest.deleteFile(this.deletingFileId).subscribe((data) => {
      this.logger.log('File Delete', data);
    });
    this.getAllFiles();
  }

  /* Gets / Sorts for Filtering */
  findFilesFilter() {
    this.rest.findFiles(this.resourceTypeIdFilter, this.initiativeIdFilter, this.tagFilter,
      this.watchFilter, this.nameFilter).pipe(take(1)).subscribe((res) => {
        this.dataSource = new MatTableDataSource(res);
        this.logger.log(res, 'FIND FILTER FILES');
      })
  }
  searchByFileName(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();
  }
  getResourceTypes() {
    this.rest.getFileResources().pipe(take(1)).subscribe((data) => {
      this.resourcesList = data;
    })
  }

  resourceTypeSelection(event) {
    this.resourceTypeIdFilter = event.value;
    if (this.resourceTypeIdFilter === undefined) {
      this.getAllFiles();
    } else {
      this.findFilesFilter();
    }
  }
  getInitiatives() {
    this.rest.getFileInitiatives().pipe(take(1)).subscribe((data) => {
      this.initiativesList = data;
    })
  }

  getFileTypes() {
    this.rest.getFileTypes().pipe(take(1)).subscribe((data) => {
      this.fileTypeList = data;
    })
  }

  initiativesSelection(event) {
    this.initiativeIdFilter = event.value;
    if (this.initiativeIdFilter === undefined) {
      this.getAllFiles();
    } else {
      this.findFilesFilter();
    }
  }
  getTagsList() {
    this.rest.getFileTags().pipe(take(1)).subscribe((data) => {
      this.tagList = data;
    })
  }

  tagsSelection(event) {
    this.tagFilter = event.value;
    if (this.tagFilter === undefined) {
      this.getAllFiles();
    } else {
      this.findFilesFilter();
    }
  }
  isOnWatchlist(event) {
    this.watchFilter = event.checked;
    if (this.watchFilter) {
      this.findFilesFilter();
    } else {
      this.getAllFiles();
    }
  }

  /* Dialogs -------------*/
  openAdminDialog(fileId) {
    this.getFileDetials(fileId);
    this.dialog.open(this.adminDialog, { disableClose: true });
  }
  openAdminAddDialog() {

    this.adminFileForm.reset();
    this.dialog.open(this.adminDialog, { disableClose: true });
    this.isAddingNewFile = true;
  }

  // Type = submission type. 1 = Save Draft / 2 = Publish File / 3 = Publish with Alert
  openAdminConfirmDialog(type) {
    if (type === 1) {
      this.isSavingDraft = true;
      this.isPublishingWithAlert = false;
      this.isPublishingFile = false;
    }
    if (type === 2) {
      this.isPublishingFile = true;
      this.isPublishingWithAlert = false;
      this.isSavingDraft = false;
    }
    if (type === 3) {
      this.isPublishingWithAlert = true;
      this.isPublishingFile = true;
      this.isSavingDraft = false;
    }
    this.dialog.open(this.adminConfirmDialog, { disableClose: true });
  }

  openFileDeleteDialog(fileId) {
    this.deletingFileId = fileId;
    this.dialog.open(this.adminConfirmDeleteDialog, { disableClose: true })
  }

  cancelAdminDialog() {
    this.isSavingDraft = false;
    this.isPublishingFile = false;
    this.isPublishingWithAlert = false;
    this.isAddingNewFile = false;
    this.getAllFiles();
    this.dialog.closeAll();
  }

  cancelConfirmDialog() {
    this.isSavingDraft = false;
    this.isPublishingFile = false;
    this.isPublishingWithAlert = false;
    this.isAddingNewFile = false;
    this.deletingFileId = undefined;
  }
 
  toggleRecentlyAddedTop5(): void {
    this.toggle5_RecentlyAdded = !this.toggle5_RecentlyAdded;

    this.rest.getRecentlyAddedFiles(this.toggle5_RecentlyAdded).pipe(take(1)).subscribe((data) => {
      this.RecentlyAddedFiles = data;
      this.recentlyAddedFileList = new MatTableDataSource(this.RecentlyAddedFiles); //source data for table       
      this.logger.log(this.RecentlyAddedFiles, 'RECENTLY ADDED FILES');
    })
   
  }

  updateFile():void {   
    this.updateFileAction(34); //using file id = 34 for testing
  }
 
  toggleRecentlyViewedTop5(): void {
    this.toggle5_RecentlyViewed = !this.toggle5_RecentlyViewed;

    this.rest.getRecentlyViewedFiles(this.toggle5_RecentlyViewed).pipe(take(1)).subscribe((data) => {
      this.RecentlyViewedFiles = data;
      this.recentlyViewedFileList = new MatTableDataSource(this.RecentlyViewedFiles);
      this.logger.log(this.RecentlyViewedFiles, 'RECENTLY VIEWED FILES');
    })
  }

  toggleMostPopular5(): void {
    this.toggle5_MostPopular = !this.toggle5_MostPopular;

    this.rest.getMostPopularFiles(this.toggle5_MostPopular).pipe(take(1)).subscribe((data) => {
      this.MostPopularFiles = data;
      this.mostPopularFileList = new MatTableDataSource(this.MostPopularFiles);
      this.logger.log(this.MostPopularFiles, 'MOST POPULAR FILES');
    })
  } 

}
