export class Alerts {
    id: number;
    alertId: number;
    alertScheduleId: number;
    message: string;
    url: string;
    linkText: string;
    definition: string;
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
    measureDesc: string;
    measureType: string;
    practiceTotal: number;
    networkTotal: number;
}

export class EdChart {
    practiceId: number;
    admitDate: Date;
    chartLabel: string;
    chartTitle: string;
    edVisits: number;
}
export class EdChartDetails {
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
}