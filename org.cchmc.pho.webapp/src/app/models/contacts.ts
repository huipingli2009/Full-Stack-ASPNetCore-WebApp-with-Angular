export interface Contact {
    practiceId: number;
    practiceName: string;
    practiceType: string;
    emr: string;
    phone: string;
    fax: string;
    websiteURL: string
}

export interface ContactPracticeDetails {
    practiceId: number;
    practiceName: string;
    memberSince: Date;
    practiceManager: string;
    pmEmail: string;
    pic: string;
    picEmail: string;
    contactPracticeLocations: Array<ContactPracticeLocation>
}

export class ContactPracticeLocation {
    practiceId: number;
    locationId: number;    
    locationName: string;
    officePhone: string;
    fax: string;
    county: string; 
    address: string;
    city: string; 
    state: string; 
    zip: string  
}
