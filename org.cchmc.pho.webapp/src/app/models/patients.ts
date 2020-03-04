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
}
