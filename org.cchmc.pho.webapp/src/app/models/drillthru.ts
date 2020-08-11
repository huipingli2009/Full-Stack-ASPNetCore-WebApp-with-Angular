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
