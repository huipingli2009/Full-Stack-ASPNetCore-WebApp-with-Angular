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
    hyperLinkLabel: string;

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
    conditionId: number;
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
    chartTopLeftLabel: string
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

export class WebChartFilters {
    filterId: number;
    filterLabel: string;
}

export enum WebChartId {
    EDChart = 1,    
    PopulationChart = 2
}

export enum WebChartFilterMeasureId {
    edChartdMeasureId = 19,
    conditionDefaultMeasureId = 27,
    pcpMeasureId = 27,
    genderMeasureId = 32,
    zipCodeMeasureId = 30,
    payorTypeMeasureId = 27,
    locationMeasureId = 35,
    ageMeasureId = 32
}

export enum WebChartFilterId {
    dateFilterId = 0,
    conditionDefaultFilterId = 1,
    pcpFilterId = 2,
    genderFilterId = 3,
    zipCodeFilterId = 4,
    payorTypeFilterId = 5,
    locationFilterId = 6,
    ageFilterId = 7
}

export enum DrillThruMeasureId {
    EDDrillThruMeasureId = 8,
    NonEDDrillThruMeasureId = 6  
}