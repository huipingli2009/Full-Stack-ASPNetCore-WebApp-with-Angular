export class Staff {
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