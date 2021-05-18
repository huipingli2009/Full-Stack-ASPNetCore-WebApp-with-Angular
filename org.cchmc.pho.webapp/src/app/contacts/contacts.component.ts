import { Component, OnInit } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { NGXLogger } from 'ngx-logger';
import { take } from 'rxjs/operators';
import { ContactList } from '../models/contacts';
import { RestService } from '../rest.service';

@Component({
  selector: 'app-contacts',
  templateUrl: './contacts.component.html',
  styleUrls: ['./contacts.component.scss']
})
export class ContactsComponent implements OnInit {

  contactList: ContactList[];
  dataSourceContact: MatTableDataSource<any>;
  
  displayedColumns: string[] = ['arrow', 'practiceName', 'practiceType', 'emr', 'phone', 'fax', 'websiteURL'];

  constructor(private rest: RestService, private logger: NGXLogger) {
    this.dataSourceContact = new MatTableDataSource;
   }

  ngOnInit(): void {
    this.getAllContacts();
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
}

