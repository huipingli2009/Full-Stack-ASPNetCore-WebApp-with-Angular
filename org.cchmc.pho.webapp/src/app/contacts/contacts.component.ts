import { animate, state, style, transition, trigger } from '@angular/animations';
import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, Validators } from '@angular/forms';
import { MatTableDataSource } from '@angular/material/table';
import { NGXLogger } from 'ngx-logger';
import { take } from 'rxjs/operators';
import { Contact, ContactPracticeDetails, ContactPracticeLocation} from '../models/contacts';
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

  constructor(private rest: RestService, private logger: NGXLogger, private fb: FormBuilder) {
    this.dataSourceContact = new MatTableDataSource;
  }

  get locations() {
    return this.ContactDetailsForm.get('locations') as FormArray;
  }

  contactList: Contact[];
  contactPracticeDetails: ContactPracticeDetails;
  contactPracticeLocations: ContactPracticeLocation[] = [];
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
    picEmail: ['', [Validators.required, Validators.email]],
    locations: this.fb.array([
      this.fb.group({
        practiceId: [''],
        locationId: [''],    
        locationName: [''],
        officePhone: [''],
        fax: [''],
        county: [''], 
        address: [''],
        city: [''],
        state: [''],  
        zip: ['']  
      })
    ]) 
  });

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

      //Practice locations
      this.ContactDetailsForm.setControl('locations', this.populateExistingLocations(this.contactPracticeDetails.contactPracticeLocations));     
    });   
  }
  
  populateExistingLocations(locations: Array<ContactPracticeLocation>): FormArray {
    const locationFormArray = new FormArray([]);
    locations.forEach(element => {
      locationFormArray.push(this.fb.group({
        practiceId: element.practiceId,
        locationId: element.locationId,
        locationName: element.locationName,
        officePhone: element.officePhone,
        fax: element.fax,
        county: element.county, 
        address: element.address,
        city: element.city, 
        state: element.state,
        zip: element.zip 
      }));      
    });
    return locationFormArray;
  }

  getContactPracticeLocations(id: number){
    return this.rest.getContactPracticeLocations(id).pipe(take(1)).subscribe((data)=>{
     //loop thru contact practice locations and push to contactPracticeLocations 
     for (let i = 0; i < data.length; i++){
        this.contactPracticeLocations.push(data[i]);
     }
      this.logger.log(this.contactPracticeLocations,'Practice locations');
    });   
  }
}

