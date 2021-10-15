import { animate, state, style, transition, trigger } from '@angular/animations';
import { Component, ComponentFactoryResolver, OnInit, TemplateRef, ViewChild, ViewContainerRef } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { NGXLogger } from 'ngx-logger';
import { take } from 'rxjs/operators';
import { BoardMembership, Contact, ContactPracticeDetails, ContactPracticeLocation, ContactPracticeStaff, ContactPracticeStaffDetails, PHOMembership, Specialty} from '../models/contacts';
import { RestService } from '../rest.service';
import { ContactsDatasource} from './contacts.datasource';
import { Staff } from '../models/Staff';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { Role } from '../models/user';
import { UserService } from '../services/user.service';
import { DrilldownService } from '../drilldown/drilldown.service';

@Component({
  selector: 'app-contacts',
  templateUrl: './contacts.component.html',
  styleUrls: ['./contacts.component.scss'],
  animations: [
    trigger('detailExpand', [
      state('collapsed', style({ height: '0px', minHeight: '0' })),
      state('expanded', style({ height: '*' })),
      transition('expanded <=> collapsed', animate('225ms cubic-bezier(0.4, 0.0, 0.2, 1)')),
    ]),   
  ]
})
export class ContactsComponent implements OnInit {  
  @ViewChild('contactEmailDialog') contactEmailDialog: TemplateRef<any>;  
  
  //Contact list/details/locations and providers/details
  contactList: Contact[];  
  contactPracticeDetails: ContactPracticeDetails;
  contactPracticeLocations: ContactPracticeLocation[] = [];
  contactPracticeStaffList: ContactPracticeStaff[] = [];
  contactPracticeStaffDetails: ContactPracticeStaffDetails;
  expandedElement: Contact | null;  
  dataSourceContact: ContactsDatasource;
  pmEmail: string; 
  picEmail: string; 
  providerEmail: string; 
  userCanSendEmail: boolean; 
  userCanExportList: boolean;

  //dropdowns for contact header filters
  phoMembershipList: PHOMembership[] = [];
  contactPracticeSpecialties: Specialty[] = [];
  contactPracticeBoardMembership: BoardMembership[] = [];  

  //filters for contact header section
  selectedMembership: string = ""; 
  qplChecked: boolean = false;
  qpl: string;
  specialties: Array<string> = [];   
  filterType: string;
  membership: string;
  board: string;
  contactNameSearch: string;
  textColor : string = 'white';
  selectedStaffId: number;  

  //contact email dialog   
  contactEmailList: Staff[] = []; 
  emailReceiversRole:  string; 
  emailReceivers: string[] = []; 
  dialogRef: MatDialogRef<any>;

  constructor(private rest: RestService, private logger: NGXLogger, 
    private fb: FormBuilder, private userService: UserService, 
    public dialog: MatDialog, private drilldownService: DrilldownService) {}     
  
  //contact practice's data table
  displayedColumns: string[] = ['arrow', 'practiceName', 'practiceType', 'emr', 'phone', 'fax', 'websiteURL'];

  //contact email dialog table
  displayedEmailDialogColumns: string[] = ['emailCheckbox', 'name', 'email', 'practice', 'staffRole'];
  
  ngOnInit(): void {      
    this.dataSourceContact = new ContactsDatasource(this.rest);    
    this.getContactsWithFilters();
    //this.loadPracticeDetail();
    this.getContactPracticeSpecialties();   
    this.getContactPracticePHOMembership(); 
    this.getContactPracticeBoardship();
    this.getCurrentUser();  
  }  

  getCurrentUser() {
    this.userService.getCurrentUser().pipe(take(1)).subscribe((data) => {
      //modify here if additional user roles need to be added for group email functionality
      if (data.role.id === Role.PHOAdmin) {
        this.userCanSendEmail = true;
        this.userCanExportList = true;
      }      
    });
  }
  
  getContactsWithFilters() {      
    this.dataSourceContact.loadContacts(this.qpl, this.specialties.toString(), this.membership, this.board, this.contactNameSearch);       
  } 

  trackContact(index: number, item: Contact): string {
    if (!item) return null;
    return '${item.practiceId}';
  }  
  
  //get PHO membership
  getContactPracticePHOMembership(){
    return this.rest.getContactPracticePHOMembership().pipe(take(1)).subscribe((data: PHOMembership[]) => {
      this.phoMembershipList = data;
      this.logger.log(this.phoMembershipList,'PHO membership list'); 
    })
  }

  //get contact practice specialties
  getContactPracticeSpecialties(){
    return this.rest.getContactPracticeSpecialties().pipe(take(1)).subscribe((data: Specialty[])=>{
      this.contactPracticeSpecialties = data;
      this.logger.log(this.contactPracticeSpecialties, 'Contact practice specialties');
    });
  }

  //get Contact board membership list
  getContactPracticeBoardship(){
    return this.rest.getContactPracticeBoardMembership().pipe(take(1)).subscribe((data: BoardMembership[]) =>{
      this.contactPracticeBoardMembership = data;
      this.logger.log(this.contactPracticeBoardMembership, 'Contact practice board membership');
    });
  } 
  
  //filter for Quality Physician Leader
  hasQPLFilter(event){
    this.qplChecked = !this.qplChecked;
    
    if(this.qplChecked){
      this.textColor = 'yellow';
      this.qpl = 'true';
    }
    else{
      this.textColor = 'white';
      this.qpl = '';
    }    
    
    this.getContactsWithFilters();
  }

  contactHasMembership(){
    this.getContactsWithFilters();
  }

  contactHasSpecialties(event){
    this.specialties = event.value;
    this.getContactsWithFilters();
  }
  
  contactHasBoardMembership(){
    this.getContactsWithFilters();
  }  
 
  //contact search
  searchContacts(event){
    this.contactNameSearch = event.target.value;
    this.getContactsWithFilters();
  }
  
  refreshContactPage(){
    window.location.reload();
  }

  getContactEmailList(managers: boolean, physicians: boolean, all: boolean){
    return this.rest.getContactEmailList(managers, physicians, all).pipe(take(1)).subscribe((data: Staff[]) =>{
      this.contactEmailList = data;  
      this.emailReceiversRole = this.contactEmailList[0].emailFilters;    
      this.logger.log(this.contactEmailList, 'Contact email list');

      //get the original email receiver list based on the email button clicked
      data.forEach(item => this.emailReceivers.push(item.email));      
    });
  }

  openEmailToManagersDialog(){
    this.getContactEmailList(true, false, false);
    this.dialogConfiguration();
  } 

  openEmailToPhysiciansDialog(){
    this.getContactEmailList(false, true, false);
    this.dialogConfiguration();
  }

  openEmailToAllDialog(){
    this.getContactEmailList(false, false, true);
    this.dialogConfiguration();
  }

  dialogConfiguration(){
    this.dialogRef = this.dialog.open(this.contactEmailDialog, {
      width: '800px',
      height: '600px',
      autoFocus: true,
      disableClose: true,     
      data: { 
        name: this.contactEmailList      
      }
    }); 
  }

  openDrilldownDialog() {
    //set default filterId value to -1, to differentiate between a set value and an intentionally null value.
    var filterId = -1;

    this.rest.showViewReportButton = false;  //to hide the View Report button
    var drilldownOptions = {
      drilldownMeasureId: '9',
      filterId: filterId,
      displayText: 'Contact List',
      originMeasureId: ''
    };
    this.drilldownService.open(drilldownOptions);

  }

  updateEmailReceivers(event){ 
    //reset emailReceiver list
    this.emailReceivers = [];     

    //get the updated email receivers from dialog
    this.dialogRef.close(this.contactEmailList.forEach(
      (item) => {
        //exclude those removed and push those only selected to email receiver list
        if (item.email)
        {
          this.emailReceivers.push(item.email);
        }         
      })
    );    
   
     //bcc to the updated email receivers
     window.location.href = "mailto:?bcc=" + this.emailReceivers; 
  }  
}


