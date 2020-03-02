export class Patients {
    patientId: number;
    firstName: string;
    lastName: string;
    practiceID: number;
    pcP_StaffID: number;
    dob: string;
    status: [
        {
            id: number,
            name: string;
        }
    ]
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
