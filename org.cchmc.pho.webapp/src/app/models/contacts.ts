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

export interface ContactPracticeStaff {
    staffId: number;
    staffName: string
}

export interface ContactPracticeStaffDetails {
    id: number;
    staffName: string;      
    email: string;
    phone: string;      
    position: string;      
    npi: number;
    locations: string;
    specialty: string;
    commSpecialist: boolean;
    ovpcaPhysician: boolean;
    ovpcaMidLevel: boolean;
    responsibilities: string;
    boardMembership: string;     
    notesAboutProvider: string      
}
export interface Boardship {
    id: number;
    boardName: string;
    description: string;
    hyperlink: string
}

export interface Specialty {
    id: number;
    specialtyArea: string;
}

export interface PHOMembership {
    id: number;
    membership: string;
}
