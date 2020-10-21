export class Alerts {
    id: number;
    alertId: number;
    alertScheduleId: number;
    message: string;
    url: string;
    linkText: string;
    definition: string;
    target: string;
    filterType: string;
    filterName: string;
    filterValue: number;
}

export interface AlertAction {
    alertActionId: number;
}

export enum AlertActionTaken {
    click = 2,
    close = 3
}

export class Spotlight {
    header: string;
    placementOrder: number;
    body: string;
    hyperlink: string;
    imageHyperlink: string;
    locationId: number;
}
export class Quicklinks {
    placementOrder: number;
    body: string;
    hyperlink: string;
    locationId: number;
}
export class Population {
    practiceId: number;
    dashboardLabel: string;
    measureId: number;
    measureDesc: string;
    measureType: string;
    practiceTotal: number;
    networkTotal: number;
    opDefURL: string;
    opDefLinkExists: boolean;
}

export class EdChart {
    practiceId: number;
    admitDate: Date;
    chartLabel: string;
    chartTitle: string;
    edVisits: number;
}
export class EdChartDetails {
    patientId: number;
    patientMRN: string;
    patientEncounterID: string;
    facility: string;
    patientName: string;
    patientDOB: Date;
    pcp: string;
    hospitalAdmission: Date;
    hospitalDischarge: Date;
    visitType: string;
    primaryDX: string;
    primaryDX_10Code: string;
    dX2: string;
    dX2_10Code: string;
    inpatientVisit: string;
    edVisitCount: string;
    ucVisitCount: string;
    admitCount: string;
}