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
import { FileAction } from '../models/files';

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
  fileTypeList = [
    {
      id: 1,
      name: 'PDF'
    },
    {
      id: 2,
      name: 'DOCX'
    },
    {
      id: 3,
      name: 'EXCEL'
    },
    {
      id: 4,
      name: 'PPT'
    },
    {
      id: 5,
      name: 'CSV'
    },
    {
      id: 6,
      name: 'JPEG'
    },
    {
      id: 7,
      name: 'PNG'
    },
    {
      id: 8,
      name: 'ZIP'
    }
  ];

  // Filters
  resourceTypeIdFilter: number;
  initiativeIdFilter: number;
  tagFilter: string;
  watchFilter: boolean;
  nameFilter: string;

  displayedColumns: string[] = ['icon', 'name', 'dateCreated', 'lastViewed', 'watchFlag'
    , 'fileType', 'actions', 'tags', 'button'];
  dataSource: MatTableDataSource<FileList>;

  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild('adminDialog') adminDialog: TemplateRef<any>;
  @ViewChild('adminConfirmDialog') adminConfirmDialog: TemplateRef<any>;
  @ViewChild('adminConfirmDeleteDialog') adminConfirmDeleteDialog: TemplateRef<any>;

  constructor(private rest: RestService, private logger: NGXLogger, private dialog: MatDialog,
    private userService: UserService, private fb: FormBuilder, private changeDetectorRefs: ChangeDetectorRef) {

    this.adminFileForm = this.fb.group({
      practiceOnly: ['', Validators.required],
      name: ['', Validators.required],
      fileURL: ['', Validators.required],
      tags: [''],
      author: [''],
      fileType: ['', Validators.required],
      description: ['', Validators.required],
      resourceTypeId: ['', Validators.required],
      initiativeId: ['']
    });

  }

  ngOnInit(): void {
    this.getCurrentUser();
    this.getAllFiles();
    this.getResourceTypes();
    this.getInitiatives();
    this.getTagsList();
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

      const selectedFileValues = {
        practiceOnly: data.practiceOnly,
        name: data.name,
        fileURL: data.fileURL,
        tags: joinedTags.join(', '),
        author: data.author,
        fileType: {
          id: data.fileTypeId,
          name: data.fileType
        },
        description: data.description,
        resourceTypeId: data.resourceTypeId,
        initiativeId: data.initiativeId
      };
      this.logger.log(selectedFileValues, 'SELECTED FILE')
      this.adminFileForm.setValue(selectedFileValues);
    })
  }

  updateFileAction(fileResourceId) {
    this.logger.log(fileResourceId);
    this.fileAction = new FileAction();
    this.fileAction.fileResourceId = fileResourceId;
    this.fileAction.fileActionId = 1;
    this.rest.updateFileAction(this.fileAction).pipe(take(1)).subscribe(res => {
    },
      error => { this.error = error; });
  }

  submitFileAddUpdate() {
    let tagsSplit = this.adminFileForm.controls.tags.value.replace(/\s*,\s*/g, ",");
    tagsSplit = tagsSplit.split(',');
    tagsSplit = tagsSplit.map(function (e) {
      return { name: e };
    });
    let publishFile;
    if (this.isPublishingFile === true || this.isPublishingWithAlert === true) {
      publishFile = true;
    } else { publishFile = false; }
    const fileFormValues = {
      id: this.currentFileId,
      name: this.adminFileForm.controls.name.value,
      resourceTypeId: this.adminFileForm.controls.resourceTypeId.value,
      initiativeId: this.adminFileForm.controls.initiativeId.value,
      author: this.adminFileForm.controls.author.value,
      fileTypeId: this.adminFileForm.controls.fileType.value.id,
      fileType: this.adminFileForm.controls.fileType.value.name,
      dateCreated: this.currentDateCreated,
      lastViewed: this.currentLastViewed,
      watchFlag: this.currentWatchFlag,
      tags: tagsSplit,
      fileURL: this.adminFileForm.controls.fileURL.value,
      description: this.adminFileForm.controls.description.value,
      practiceOnly: this.adminFileForm.controls.practiceOnly.value,
      publishFlag: publishFile,
      createAlert: this.isPublishingWithAlert
    };
    this.logger.log(fileFormValues, 'FORM SUBMISSION');
    if (this.isAddingNewFile === true) {
      const fileFormValues = {
        name: this.adminFileForm.controls.name.value,
        resourceTypeId: this.adminFileForm.controls.resourceTypeId.value,
        initiativeId: this.adminFileForm.controls.initiativeId.value,
        author: this.adminFileForm.controls.author.value,
        fileTypeId: this.adminFileForm.controls.fileType.value.id,
        fileType: this.adminFileForm.controls.fileType.value.name,
        tags: tagsSplit,
        fileURL: this.adminFileForm.controls.fileURL.value,
        description: this.adminFileForm.controls.description.value,
        practiceOnly: this.adminFileForm.controls.practiceOnly.value,
        publishFlag: publishFile,
        createAlert: this.isPublishingWithAlert
      };
      this.rest.addNewFile(fileFormValues).pipe(take(1)).subscribe((data) => {
        this.logger.log(data, 'New File');
        this.getTagsList();
      });
    } else {
      this.rest.updateFileDetails(fileFormValues).pipe(take(1)).subscribe((data) => {
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

}
