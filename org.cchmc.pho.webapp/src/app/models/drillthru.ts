export class MetricDrillthruTable {
    rows: MetricDrillthruRow[];
}

export class MetricDrillthruRow{
    rowNumber: number;
    order: number;
    columns: MetricDrillthruColumn[];
}

export class MetricDrillthruColumn{
    order: number;
    value: string;
    columnName: string;
}

export enum DrillthruMeasurementIdEnum {
    PracticeVisitKPI = 1,
    CCHMCVisitKPI = 2,
    HealthbridgeEncounterKPI = 3,
    DiagnosisDetails = 4,
    CPTCodes = 5,
    FilteredPatientList = 6  
}
