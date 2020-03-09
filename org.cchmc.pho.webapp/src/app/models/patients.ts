export class Patients {
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
    conditions: [
        {
            id: number;
            name: string;
        }
    ]
    close?: boolean;
}
export class PatientDetails {
     id: number;
     patientMRNId: string;
     clarityPatientId: string  ;
     practiceId: number ;
     firstName: string  ;
     middleName: string  ;
     lastName: string  ;
     patientDOB: Date  ;
     pcpId: number ;
     pcpFirstName: string  ;
     pcpLastName: string  ;
     insuranceId: number ;
     insuranceName: string  ;
     addressLine1: string  ;
     addressLine2: string  ;
     city: string  ;
     state: string  ;
     zip: string  ;
     conditions: [
      {
         id: number ;
         name: string;
      }
    ] ;
     pmcaScore: number ;
     providerPMCAScore: number ;
     providerNotes: string  ;
     activeStatus: boolean ;
     pendingStatusConfirmation: boolean ;
     genderId: number ;
     gender: string  ;
     email: string  ;
     phone1: string  ;
     phone2: string  ;
     practiceVisits: number ;
     cchmcEncounters: number ;
     healthBridgeEncounters: number ;
     uniqueDXs: number ;
     uniqueCPTCodes: number ;
     lastPracticeVisit: Date  ;
     lastCCHMCAdmit: Date  ;
     lastHealthBridgeAdmit: Date  ;
     lastDiagnosis: string  ;
     cchmcAppointment: Date;
}
