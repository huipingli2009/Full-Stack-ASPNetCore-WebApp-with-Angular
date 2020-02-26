export class Alerts {
    id: number;
    AlertId: number;
    Alert_ScheduleId: number;
    AlertMessage: string;
    URL: string;
    URL_Label: string;
    AlertDefinition: string;
}
export class Content {
    header: string;
    placementOrder: number;
    body: string;
    hyperlink: string;
    imageHyperlink: string;
    contentPlacement: string;
}
export class Population {
    practiceId: number;
    dashboardLabel: string;
    measureDesc: string;
    measureType: string;
    practiceTotal: number;
    networkTotal: number;
}