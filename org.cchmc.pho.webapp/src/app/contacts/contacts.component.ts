import { animate, state, style, transition, trigger } from '@angular/animations';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { MatTableDataSource } from '@angular/material/table';
import { NGXLogger } from 'ngx-logger';
import { take } from 'rxjs/operators';
import { Contact, ContactPracticeDetails, ContactPracticeLocations } from '../models/contacts';
import { RestService } from '../rest.service';
import { formatDate } from '@angular/common' ;

@Component({
  //selector: 'app-contacts',
  templateUrl: './contacts.component.html',
  styleUrls: ['./contacts.component.scss'],
  animations: [
    trigger('detailExpand', [
      state('collapsed', style({ height: '0px', minHeight: '0' })),
      state('expanded', style({ height: '*' })),
      transition('expanded <=> collapsed', animate('225ms cubic-bezier(0.4, 0.0, 0.2, 1)')),
    ]),
  ],
})
export class ContactsComponent implements OnInit {

  contactList: Contact[];
  contactPracticeDetails: ContactPracticeDetails;
  contactPracticeLocations: ContactPracticeLocations;
  expandedElement: Contact | null;
  dataSourceContact: MatTableDataSource<any>;
  
  displayedColumns: string[] = ['arrow', 'practiceName', 'practiceType', 'emr', 'phone', 'fax', 'websiteURL'];

  ContactDetailsForm = this.fb.group({
    practiceId: [''],
    practiceName: [''],
    memberSince: [''],
    practiceManager: [''],
    pmEmail: ['', [Validators.required, Validators.email]],
    pic: [''],
    picEmail: ['', [Validators.required, Validators.email]]  
  });


  constructor(private rest: RestService, private logger: NGXLogger, private fb: FormBuilder) {
    this.dataSourceContact = new MatTableDataSource;
   }

  ngOnInit(): void {
    this.getAllContacts();

    //for testing
    this.getContactPracticeDetails(7);
    //this.getContactPracticeLocations(7);
  }

  getAllContacts() {
    return this.rest.getAllContacts().pipe(take(1)).subscribe((data) =>
    {
      this.contactList = data;
      this.dataSourceContact = new MatTableDataSource(this.contactList);
      this.dataSourceContact.data = this.contactList;
      this.logger.log(this.contactList, 'Contacts');
    });
  }

  getContactPracticeDetails(id: number){
    this.rest.getContactPracticeDetails(id).pipe(take(1)).subscribe((data) =>
    {
      this.contactPracticeDetails = data;

      this.ContactDetailsForm.get('practiceId').setValue(this.contactPracticeDetails.practiceId);
      this.ContactDetailsForm.get('practiceName').setValue(this.contactPracticeDetails.practiceName);
      
      //use formatDate function to change the format
      this.ContactDetailsForm.get('memberSince').setValue(formatDate(this.contactPracticeDetails.memberSince,'yyyy-MM-dd','en'));
      
      this.ContactDetailsForm.get('practiceManager').setValue(this.contactPracticeDetails.practiceManager);
      this.ContactDetailsForm.get('pmEmail').setValue(this.contactPracticeDetails.pmEmail);
      this.ContactDetailsForm.get('pic').setValue(this.contactPracticeDetails.pic);
      this.ContactDetailsForm.get('picEmail').setValue(this.contactPracticeDetails.picEmail);
      //this.logger.log(this.contactPracticeDetails, 'ContactPracticeDetails');
    });   
  } 

  getContactPracticeLocations(id: number){
    return this.rest.getContactPracticeLocations(id).pipe(take(1)).subscribe((data)=>
      this.contactPracticeLocations = data
    );
    console.log(this.contactPracticeLocations);
  }
}

