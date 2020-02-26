export class Alerts {
    id: number;
    AlertId: number;
    alertScheduleId: number;
    message: string;
    url: string;
    linkText: string;
    definition: string;
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