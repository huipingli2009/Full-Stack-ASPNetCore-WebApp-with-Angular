export interface Staff {
    id: number;
    firstName: string;
    lastName: string;
    email: string;
    phone: string;
    isRegistry: boolean;
    responsibilities: string;
    credentials: Credentials;
    position: Position;
    legalDisclaimerSigned: string;
    myPractice: {
        id: number;
        name: string;
    }
    positionType: String;
}

export class StaffDetails {
    id: number;
    firstName: string;
    lastName: string;
    email: string;
    phone: string;
    startDate: string;
    positionId: number;
    credentialId: number;
    npi: number;
    isLeadPhysician: boolean;
    isQITeam: boolean;
    isPracticeManager: boolean;
    isInterventionContact: boolean;
    isQPLLeader: boolean;
    isPHOBoard: boolean;
    isOVPCABoard: boolean;
    isRVPIBoard: boolean;
}

export class Position {
    id: number;
    name: string;
}

export class Credentials {
    id: number;
    name: string;
}

export class Responsibilities {
    id: number;
    name: string;
    type: string;
}

export interface PracticeList {
    currentPracticeId: number;
    practiceList: Array<Practices>;
}

export interface Practices {
    id: number;
    name: string;
}