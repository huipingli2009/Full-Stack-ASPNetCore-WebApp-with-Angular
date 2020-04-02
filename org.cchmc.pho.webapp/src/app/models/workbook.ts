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
    improvement: string;
}

export class Followup {
    formResponseId: number;
    patientId: number;
    actionPlanGiven: boolean;
    managedByExternalProvider: boolean;
    dateOfLastCommunicationByExternalProvider: string;
    followupPhoneCallOneToTwoWeeks: boolean;
    dateOfFollowupCall: string;
    oneMonthFollowupVisit: boolean;
    dateOfOneMonthVisit: string;
    oneMonthFolllowupPHQ9Score: number;
}

export class WorkbookPractice {
    formResponseId: number;
    header: string;
    line1: string;
    line2: string;
    jobAidURL: string;
    line3: string;
}
