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
    get clickable(): boolean { // read-only property with getter function (this is not the same thing as a “function-property”)
        if (this.columnName.toUpperCase() === "PATIENTID"){
            return true;
        }else{
            return false;
        }
    }
}

export enum DrillthruMeasurementIdEnum {
    PracticeVisitKPI = 1,
    CCHMCVisitKPI = 2,
    HealthbridgeEncounterKPI = 3,
    DiagnosisDetails = 4,
    CPTCodes = 5,
    FilteredPatientList = 6  
}
