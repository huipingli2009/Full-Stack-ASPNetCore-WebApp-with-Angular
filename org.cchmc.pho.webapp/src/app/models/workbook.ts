export class WorkbookReportingMonths {
    formResponseID: number;
    reportingMonth: string;
}

export class WorkbookProvider {
    formResponseID: number;
    staffID: number;
    provider: string;
    phqs: number;
    total: number;
}


export class WorkbookPatient {
    formResponseId: number;
    patientId: number;
    patient: string;
    dob: string;
    phone: string;
    provider: string;
    providerId: number;
    dateOfService: string;
    phQ9_Score: string;
    actionFollowUp: boolean;
    improvement: boolean;
}
