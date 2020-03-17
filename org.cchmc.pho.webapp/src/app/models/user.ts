export interface User {
    status: string;
    user: {
        id: number;
        token: string;
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
    };
    refreshToken: string;
}

export interface UserAuthenticate {
    username: string;
    password: string;
}

export interface FakeUser {
    id: number;
    username: string;
    password: string;
    firstName: string;
    lastName: string;
    token?: string;
}