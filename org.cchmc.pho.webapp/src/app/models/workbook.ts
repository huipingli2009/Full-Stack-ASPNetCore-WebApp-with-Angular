
export class WorkbookForm {
    id: number;
    label: string;
}

export class WorkbookReportingPeriod {
    formResponseId: number;    
    formId: number;
    practiceId: number;
    QuestionId: number;
    reportingPeriod: string;
}

export class WorkbookProvider {
    formResponseID: number;
    staffID: number;
    provider: string;
    phqs: number;
    total: number;
}

export abstract class WorkbookPatient{
    formResponseId: number;
    patientId: number;
    patient: string;
    dob: string;
    phone: string;
    provider: string;
    providerId: number;
    dateOfService: string;
} 
 
export class WorkbookAsthmaPatient extends WorkbookPatient {    
    asthma_Score: number;
    assessmentcompleted: boolean;
    treatment: Treatment;
    actionplangiven: boolean;
}

export class WorkbookDepressionPatient extends WorkbookPatient {    
    phQ9_Score: string;
    actionFollowUp: boolean;
    improvement: string;
    FollowUpResponse: boolean;
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
    oneMonthFolllowupPHQ9Score: string;
    phQ9FollowUpNotes: string;
}

export abstract class WorkbookPracticeBase {
    formResponseId: number;
    header: string;
    line1: string;
    line2: string;
    jobAidURL: string;
    line3: string;
}

export class WorkbookPractice extends WorkbookPracticeBase {}

export class QIWorkbookPractice extends WorkbookPracticeBase {}

export enum WorkbookFormValueEnum {
    asthma = 1,    
    depression = 3,
    qualityimprovement = 5
}

export interface Treatment {
    treatmentId: string;
    treatmentLabel: string;
}

export class WorkbookConfirmation {
    formResponseID: number;
    allProvidersConfirmed: boolean;
    noPatientsConfirmed: boolean;
}


export class QIWorkbookQuestions {
    formResponseId: number;
    qiSection: Array<Section>
}

export class Section {    
    sectionId: number; 
    sectionHeader: string;
    qiQuestion: Array<Question>;  
    dataEntered: boolean;
}

export class Question {    
    questionId: number;   
    questionDEN: string;
    questionNUM: string;
    numeratorLabel: string;
    numerator: number;
    denominator: number;   
}

