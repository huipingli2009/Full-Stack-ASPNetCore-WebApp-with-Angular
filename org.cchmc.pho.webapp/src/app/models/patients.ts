export interface Patients {
    resultCount: number;
    patientId: number;
    firstName: string;
    lastName: string;
    practiceID: number;
    pcP_StaffID: number;
    dob: string;
    activeStatus: boolean;
    potentiallyActiveStatus: boolean;
    lastEDVisit: string;
    chronic: boolean;
    watchFlag: boolean;
    conditions: Array<Conditions>;
    close?: boolean;
    totalRecords: number;
}
export interface PatientClass {
    resultCount: number;
    results: Patients[];

}
export interface PatientDetails {
    id: number;
    patientMRNId: string;
    clarityPatientId: string;
    practiceId: number;
    firstName: string;
    middleName: string;
    lastName: string;
    patientDOB: Date;
    pcpId: number;
    pcpFirstName: string;
    pcpLastName: string;
    insuranceId: number;
    insuranceName: string;
    addressLine1: string;
    addressLine2: string;
    city: string;
    stateId: number;
    state: string;
    zip: string;
    conditions: Array<Conditions>;
    pmcaScore: number;
    providerPMCAScore: number;
    providerNotes: string;
    activeStatus: boolean;
    pendingStatusConfirmation: boolean;
    genderId: number;
    gender: string;
    email: string;
    phone1: string;
    phone2: string;
    practiceVisits: number;
    cchmcEncounters: number;
    healthBridgeEncounters: number;
    uniqueDXs: number;
    uniqueCPTCodes: number;
    lastPracticeVisit: Date;
    lastCCHMCAdmit: Date;
    lastHealthBridgeAdmit: Date;
    lastDiagnosis: string;
    cchmcAppointment: Date;
    potentialPatient: boolean;
    potentialDuplicateFirstName: string;
    potentialDuplicateLastName: string;
    potentialDuplicateDOB: Date;
    potentialDuplicatePCPFirstName: string;
    potentialDuplicatePCPLastName: string;
    potentialDuplicatePCPName: string;
    potentialDuplicateGender: string;
    potentialDuplicatePatientMRNId: string;
    
}

export interface PotentialDuplicatePatient extends PatientDetails{
    potentialDuplicateFirstName: string;
    potentialDuplicateLastName: string;
    potentialDuplicateDOB: Date;
    potentialDuplicatePCPFirstName: string;
    potentialDuplicatePCPLastName: string;
    potentialDuplicateGender: string;
    potentialDuplicatePatientMRNId: string;
}

export interface NewPatient {
    firstName: string;
    lastName: string;
    practiceID: number;
    pcP_StaffID: number;
    dob: Date;
    genderId: number;
    activeStatus: boolean;
    pendingStatusConfirmation: boolean;
    lastEDVisit: string;
    chronic: boolean;
    watchFlag: boolean;
    conditions: Array<Conditions>;
    totalRecords: number;    
}

export class Conditions {
    id: number;
    name: string;
    description: string;
}

export class Providers {
    id: number;
    name: string;
}

export class PopSlices {
    id: number;
    label: string;
}

export enum potentialPtStaus {
    PopSlice = 28 //potentialPatient = true   
}

export class Insurance {
    id: number;
    name: string;
}

export interface Gender {
    id: number;
    shortName: string;
    name: string;
}

export class Pmca {
    id: number;
    score: string;
    description: string;
}

export class States {
    id: number;
    shortName: string;
    name: string;
}

export class PatientForWorkbook {
    patientId: number;
    firstName: string;
    lastName: string;
    dob: string;
    phone: string;
}