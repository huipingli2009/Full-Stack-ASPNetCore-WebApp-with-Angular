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
    contactPracticeLocations: Array<ContactPracticeLocations>
}

export class ContactPracticeLocations {
    practiceId: number;
    locationId: number;    
    locationName: string;
    officePhone: string;
    fax: string;
    county: string; 
    address: string;
    city: string;  
    zip: string  
}
