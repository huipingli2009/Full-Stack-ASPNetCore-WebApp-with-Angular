import { formatDate } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import { FormArray, FormBuilder, Validators } from '@angular/forms';
import { NGXLogger } from 'ngx-logger';
import { take } from 'rxjs/operators';
import { ContactPracticeDetails, ContactPracticeLocation, ContactPracticeStaff, ContactPracticeStaffDetails } from 'src/app/models/contacts';
import { RestService } from 'src/app/rest.service';

@Component({
  selector: 'app-practice-detail',
  templateUrl: './practice-detail.component.html',
  styleUrls: ['./practice-detail.component.scss']
})
export class PracticeDetailComponent implements OnInit {

  //location getter
  get locations() {
    return this.ContactDetailsForm.get('locations') as FormArray;
  }
  
  @Input() selectedPracticeId: number;
  
  contactPracticeDetails: ContactPracticeDetails;
  contactPracticeLocations: ContactPracticeLocation[] = [];
  contactPracticeStaffList: ContactPracticeStaff[] = [];
  contactPracticeStaffDetails: ContactPracticeStaffDetails;
  pmEmail: string; 
  picEmail: string; 
  providerEmail: string; 
  selectedStaffId: number;  

  constructor(private logger: NGXLogger, private fb: FormBuilder, private rest: RestService) { }

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
    this.getContactPracticeDetailWithProviders(this.selectedPracticeId);    
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

   //the provider dropdown list
  getContactPracticeStaffList(practiceId: number){
    this.ContactProvidersForm.reset();
    return this.rest.getContactPracticeStaffList(practiceId).pipe(take(1)).subscribe((data: ContactPracticeStaff[])=>{
      this.contactPracticeStaffList = data;
      
      //default selected staff and display his/her info    
      this.selectedStaffId = this.contactPracticeStaffList[0].staffId;
      this.getSelectedContactStaff(this.selectedStaffId);

      //this.getSelectedContactStaff(this.selectedStaffId);
      this.logger.log(this.contactPracticeStaffList,'Practice staff list'); 
    });
  }
   
  //get selected provider/staff's details
  getSelectedContactStaff(staffId: number){
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
}
