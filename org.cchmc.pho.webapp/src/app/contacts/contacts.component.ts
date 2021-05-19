import { animate, state, style, transition, trigger } from '@angular/animations';
import { Component, OnInit } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { ok } from 'assert';
import { NGXLogger } from 'ngx-logger';
import { take } from 'rxjs/operators';
import { Contact, ContactPracticeDetails } from '../models/contacts';
import { RestService } from '../rest.service';

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
  ],
})
export class ContactsComponent implements OnInit {

  contactList: Contact[];
  contactPracticeDetails: ContactPracticeDetails;
  expandedElement: Contact | null;
  dataSourceContact: MatTableDataSource<any>;
  
  displayedColumns: string[] = ['arrow', 'practiceName', 'practiceType', 'emr', 'phone', 'fax', 'websiteURL'];

  constructor(private rest: RestService, private logger: NGXLogger) {
    this.dataSourceContact = new MatTableDataSource;
   }

  ngOnInit(): void {
    this.getAllContacts();

    //for testing
    this.getContactPracticeDetails(7);
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
    return this.rest.getContactPracticeDetails(id).pipe(take(1)).subscribe((data) =>
    {
      this.contactPracticeDetails = data;
      this.logger.log(this.contactPracticeDetails, 'ContactPracticeDetails');
    });
  }
}

