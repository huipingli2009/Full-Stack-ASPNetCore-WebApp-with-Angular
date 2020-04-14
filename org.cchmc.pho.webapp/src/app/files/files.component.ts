import { Component, OnInit, ViewChild, TemplateRef } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { RestService } from '../rest.service';
import { take } from 'rxjs/operators';
import { NGXLogger } from 'ngx-logger';
import { MatDialog } from '@angular/material/dialog';
import { UserService } from '../services/user.service';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';

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
    , 'fileType', 'actions', 'tags'];
  dataSource: MatTableDataSource<FileList>;

  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild('adminDialog') adminDialog: TemplateRef<any>;
  @ViewChild('adminConfirmDialog') adminConfirmDialog: TemplateRef<any>;
  @ViewChild('adminConfirmDeleteDialog') adminConfirmDeleteDialog: TemplateRef<any>;

  constructor(private rest: RestService, private logger: NGXLogger, private dialog: MatDialog,
    private userService: UserService, private fb: FormBuilder) {

    this.adminFileForm = this.fb.group({
      practiceOnly: ['', Validators.required],
      name: ['', Validators.required],
      fileURL: ['', Validators.required],
      tags: ['', Validators.required],
      author: ['', Validators.required],
      fileType: ['', Validators.required],
      description: ['', Validators.required],
      resourceTypeId: ['', Validators.required],
      initiativeId: ['', Validators.required]
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
    return o1 === o2;
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
  }

  getFileDetials(fileId) {
    this.rest.getFileDetails(fileId).pipe(take(1)).subscribe((data) => {
      this.logger.log('FILE DETAILS', data);
      let joinedTags = [];
      data.tags.forEach(tag => {
        joinedTags.push(tag.name);
      });
      this.logger.log(joinedTags, 'Tags')
      const selectedFileValues = {
        practiceOnly: data.practiceOnly,
        name: data.name,
        fileURL: data.fileURL,
        tags: joinedTags.join(', '),
        author: data.author,
        fileType: data.fileTypeId,
        description: data.description,
        resourceTypeId: data.resourceTypeId,
        initiativeId: data.initiativeId
      };
      this.adminFileForm.setValue(selectedFileValues);
    })
  }

  submitFileAddUpdate() {
    // const updatedUser = {
    //   id: this.selectedStaffUser,
    //   token: this.authService.getToken(),
    //   userName: this.adminUserForm.controls.userName.value,
    //   staffId: this.staffDetails.id,
    //   email: this.staffDetails.email,
    //   role: {
    //     id: this.adminUserForm.controls.roles.value.id,
    //     name: this.adminUserForm.controls.roles.value.name
    //   }
    // };
  }

  updateWatchlistStatus(id, index) {
    // this.rest.updateWatchlistStatus(id).subscribe((data) => {
    // });
    this.logger.log(id, index);
  }

  /* Gets / Sorts for Filtering */
  findFilesFilter() {
    this.rest.findFiles(this.resourceTypeIdFilter, this.initiativeIdFilter, this.tagFilter,
      this.watchFilter, this.nameFilter).pipe(take(1)).subscribe((res) => {
        this.dataSource = new MatTableDataSource(res);
        this.logger.log(res, 'FIND FILTER FILES');
      })
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
    this.watchFilter = event.value;
    if (this.watchFilter === undefined) {
      this.getAllFiles();
    } else {
      this.findFilesFilter();
    }
  }

  /* Dialogs -------------*/
  openAdminDialog(fileId) {
    this.getFileDetials(fileId);
    this.dialog.open(this.adminDialog, { disableClose: true });
  }

  openAdminConfirmDialog() {
    this.dialog.open(this.adminConfirmDialog, { disableClose: true });
  }

  cancelAdminDialog() {
    this.dialog.closeAll();
  }

}
