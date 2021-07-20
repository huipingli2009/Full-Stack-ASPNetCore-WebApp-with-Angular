import { animate, state, style, transition, trigger } from '@angular/animations';
import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, Validators } from '@angular/forms';
import { NGXLogger } from 'ngx-logger';
import { take } from 'rxjs/operators';
import { BoardMembership, Contact, ContactPracticeDetails, ContactPracticeLocation, ContactPracticeStaff, ContactPracticeStaffDetails, PHOMembership, Specialty} from '../models/contacts';
import { RestService } from '../rest.service';
import { formatDate } from '@angular/common' ;
import { ContactsDatasource} from './contacts.datasource';
import { FilterService } from '../services/filter.service';

@Component({ 
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

  //location getter
  get locations() {
    return this.ContactDetailsForm.get('locations') as FormArray;
  }

  contactList: Contact[];
  contactPracticeDetails: ContactPracticeDetails;
  contactPracticeLocations: ContactPracticeLocation[] = [];
  contactPracticeStaffList: ContactPracticeStaff[] = [];
  selectedContactStaff: ContactPracticeStaff;
  contactPracticeStaffDetails: ContactPracticeStaffDetails;
  expandedElement: Contact | null;  
  dataSourceContact: ContactsDatasource;
  pmEmail: string; 
  picEmail: string; 
  providerEmail: string; 

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

  constructor(private rest: RestService, private logger: NGXLogger, private fb: FormBuilder, private filterService: FilterService) {} 
  
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

  ContactProvidersForm = this.fb.group({
    id: [''],
    staffName: [''],      
    email: [''],
    phone: [''],      
    position: [''],      
    npi: [''],
    locations: [''],
    specialty: [''],
    commSpecialist: [''],
    ovpcaPhysician: [''],
    ovpcaMidLevel: [''],
    responsibilities: [''],
    boardMembership: [''],     
    notesAboutProvider: ['']     
  });

  ngOnInit(): void { 
    this.dataSourceContact = new ContactsDatasource(this.rest);    
    this.getContactsWithFilters();   
    this.getContactPracticeSpecialties();   
    this.getContactPracticePHOMembership(); 
    this.getContactPracticeBoardship();
  }

  getContactsWithFilters() {    
    this.dataSourceContact.loadContacts(this.qpl, this.specialties.toString(), this.membership, this.board, this.contactNameSearch);
    this.rest.findContacts(this.qpl, this.specialties.toString(), this.membership, this.board, this.contactNameSearch).subscribe((data) => {
    this.contactList = data;
    });        
  }

  getContactPracticeDetailWithProviders(practiceId: number){
    this.getContactPracticeDetails(practiceId);
    this.getContactPracticeStaffList(practiceId);
  }
  
  getContactPracticeDetails(id: number){
    this.rest.getContactPracticeDetails(id).pipe(take(1)).subscribe((data) =>
    {
      this.contactPracticeDetails = data;

      this.ContactDetailsForm.get('practiceId').setValue(this.contactPracticeDetails.practiceId);
      this.ContactDetailsForm.get('practiceName').setValue(this.contactPracticeDetails.practiceName);
      
      //use formatDate function to change the format
      this.ContactDetailsForm.get('memberSince').setValue(formatDate(this.contactPracticeDetails.memberSince,'MM/dd/yyyy','en'));
      
      this.ContactDetailsForm.get('practiceManager').setValue(this.contactPracticeDetails.practiceManager);
      this.ContactDetailsForm.get('pmEmail').setValue(this.contactPracticeDetails.pmEmail);
      this.ContactDetailsForm.get('pic').setValue(this.contactPracticeDetails.pic);
      this.ContactDetailsForm.get('picEmail').setValue(this.contactPracticeDetails.picEmail);

      this.pmEmail = this.contactPracticeDetails.pmEmail;  
      this.picEmail = this.contactPracticeDetails.picEmail;
      //Practice locations
      this.ContactDetailsForm.setControl('locations', this.populateExistingLocations(this.contactPracticeDetails.contactPracticeLocations));     
    });     
  }
  
  populateExistingLocations(locations: Array<ContactPracticeLocation>): FormArray {
    let counter = 0;
    const locationFormArray = new FormArray([]);
    locations.forEach(element => {
      counter += 1;
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
      this.contactPracticeLocations = data;
      this.logger.log(this.contactPracticeLocations,'Practice locations');   
    });   
  } 

  //the provider dropdown list
  getContactPracticeStaffList(practiceId: number){
    this.ContactProvidersForm.reset();
    return this.rest.getContactPracticeStaffList(practiceId).pipe(take(1)).subscribe((data: ContactPracticeStaff[])=>{
      this.contactPracticeStaffList = data;
      this.logger.log(this.contactPracticeStaffList,'Practice staff list'); 
    });
  } 
   
  //get selected provider/staff's details
  contactStaffSelected(event){
    const staffId = event.value;
    return this.rest.getContactStaffDetails(staffId).pipe(take(1)).subscribe((data: ContactPracticeStaffDetails)=>{
      this.contactPracticeStaffDetails = data;

      this.ContactProvidersForm.get('id').setValue(this.contactPracticeStaffDetails.id);
      this.ContactProvidersForm.get('staffName').setValue(this.contactPracticeStaffDetails.staffName);
      this.ContactProvidersForm.get('email').setValue(this.contactPracticeStaffDetails.email);
      this.ContactProvidersForm.get('phone').setValue(this.contactPracticeStaffDetails.phone);
      this.ContactProvidersForm.get('position').setValue(this.contactPracticeStaffDetails.position);
      this.ContactProvidersForm.get('npi').setValue(this.contactPracticeStaffDetails.npi);
      this.ContactProvidersForm.get('locations').setValue(this.contactPracticeStaffDetails.locations);
      this.ContactProvidersForm.get('specialty').setValue(this.contactPracticeStaffDetails.specialty);
      this.ContactProvidersForm.get('commSpecialist').setValue(this.contactPracticeStaffDetails.commSpecialist);
      this.ContactProvidersForm.get('ovpcaPhysician').setValue(this.contactPracticeStaffDetails.ovpcaPhysician);
      this.ContactProvidersForm.get('ovpcaMidLevel').setValue(this.contactPracticeStaffDetails.ovpcaMidLevel);
      this.ContactProvidersForm.get('responsibilities').setValue(this.contactPracticeStaffDetails.responsibilities);
      this.ContactProvidersForm.get('boardMembership').setValue(this.contactPracticeStaffDetails.boardMembership);
      this.ContactProvidersForm.get('notesAboutProvider').setValue(this.contactPracticeStaffDetails.notesAboutProvider);

      this.providerEmail = this.contactPracticeStaffDetails.email;
      this.logger.log(this.contactPracticeStaffDetails,'Contact practice staff details'); 
    });
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
}

