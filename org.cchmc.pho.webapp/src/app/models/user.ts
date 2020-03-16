export interface User {
    id: number;
    token?: string;
    refreshToken: string;
    newPassword: string;
    firstName: string;
    lastName: string;
    userName: string;
    email: string;
    isPending: boolean;
    isDeleted: boolean;
    isLockedOut: boolean;
    staffId: number;
    createdDate: Date;
    createdBy: string;
    lastUpdatedDate: Date;
    lastUpdatedBy: string;
    deactivatedDate: Date;
    deactivatedBy: string;
    role: {
        id: 0;
        name: string;
    }
}

export interface UserAuthenticate {
    username: string;
    password: string;
}