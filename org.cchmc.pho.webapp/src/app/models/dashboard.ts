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

export class WebChart {
    practiceId: number;
    headerLabel: string;
    title: string[];
    dataSets: WebChartDataSet[];
    verticalMax: number;
}
export class WebChartDataSet {
    type: string;
    xAxisLabels: string[];
    values: number[];
    legend: string;
    backgroundColor: string;
    backgroundHoverColor: string;
    borderColor: string;
    fill: boolean;
    showLine: boolean;
    borderDash: number[];
    pointStyle: string[];
    pointRadius: number[];
    pointBackgroundColor: string[];
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
    PopulationChart = 2,
    OutcomeChart = 3
}

export enum WebChartFilterMeasureId {
    edChartdMeasureId = 19,
    conditionDefaultMeasureId = 27,
    pcpMeasureId = 27,
    genderMeasureId = 32,
    zipCodeMeasureId = 30,
    payorTypeMeasureId = 27,
    locationMeasureId = 35,
    ageMeasureId = 32,
    potentialActive = 28
}

export enum WebChartFilterId {
    none = -1,
    dateFilterId = 0,
    conditionDefaultFilterId = 1,
    pcpFilterId = 2,
    genderFilterId = 3,
    zipCodeFilterId = 4,
    payorTypeFilterId = 5,
    locationFilterId = 6,
    ageFilterId = 7,
    UFunnel = 10
}

export enum DrillThruMeasureId {
    EDDrillThruMeasureId = 8,
    PatientListDrillThruMeasureId = 6  
}