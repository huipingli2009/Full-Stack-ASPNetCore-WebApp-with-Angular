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
export interface DuplicatePatient extends Patients {
    gender: string;
    genderId: number;
    pcpId: number;
    patientMRNId: string;
    headerText: string;
    detailHeaderText: string;
    matchType: number;
    allowContinue: boolean;
    allowReactivate: boolean;
    allowKeepAndSave: boolean;
    allowMerge: boolean;
}

export interface MergePatientConfirmation {
    matchType: number;
    mergeAction: number;
    topPatientId: number;
    bottomPatientId: number;
    pcP_StaffID: number;
    topPatientFirstName: string;
    topPatientLastName: string;
    topPatientDob: string;
    topPatientGenderId: number;
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
    primarylocation: string;
    primarylocationId: number;
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
    lastCCHMCAppointment: Date;
    nextCCHMCAppointment: Date;
    potentialPatient: boolean;
    potentialDuplicateFirstName: string;
    potentialDuplicateLastName: string;
    potentialDuplicateDOB: Date;
    potentialDuplicatePCPFirstName: string;
    potentialDuplicatePCPLastName: string;
    potentialDuplicatePCPName: string;
    potentialDuplicateGender: string;
    potentialDup_PAT_MRN_ID: string;
    
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

export enum patientAdminActionTypeEnum {
    Accept = 1,
    Decline = 2,
    Update = 3  
}


export enum patientDuplicateMatchTypeEnum {
    CompleteMatch = 1,
    PartialMatch = 2 
}

export enum patientDuplicateSaveTypeEnum {
    Update = 1,
    New = 2 
}

export enum patientDuplicateActionEnum {
    Continue = 1,
    Reactivate = 2,
    KeepAndSave = 3,
    Merge = 4
}

export enum addPatientProcessEnum {
    SaveAndContinue = 1,
    SaveAndExit = 2     
}
export class Location {
    id: number;
    name: string;
}

export class Outcomes {
    measureId: number;
    dashboardLabel: string;
}