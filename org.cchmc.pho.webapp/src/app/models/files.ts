export interface FilesList {
    id: number;
    name: string;
    dateCreated: Date;
    lastViewed: Date;
    watchFlag: boolean;
    fileURL: string;
    fileType: FileType;
    publishFlag: boolean;
    tags: [
        {
            name: string;
        }
    ];
    description: string;
}

export interface FileDetails {
    id: number;
    name: string;
    resourceType: ResourceType;
    initiative: Initiative;
    author: string;
    fileType: FileType;
    dateCreated: Date;
    lastViewed: Date;
    watchFlag: boolean;
    fileURL: string;
    tags: Tag[];
    description: string;
    publishFlag: boolean;
    practiceOnly: boolean;
    createAlert: boolean;
    webPlacement: ContentPlacement;
}


export interface ResourceType {
    id: number;
    name: string;
}

export interface Tag {
    name: string;
}

export interface Initiative {
    id: number;
    name: string;
}

export interface ContentPlacement {
    id: number;
    name: string;
}

export class FileAction {
    fileResourceId: number;
    fileActionId: number;
}

export interface FileType {
    id: number;
    name: string;
    imageIcon: string;
}