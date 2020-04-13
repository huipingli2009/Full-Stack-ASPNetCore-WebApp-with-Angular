export interface FilesList {
    id: number;
    name: string;
    dateCreated: Date;
    lastViewed: Date;
    watchFlag: boolean;
    fileURL: string;
    fileTypeId: number;
    fileType: string;
    tags: [
        {
            name: string;
        }
    ];
    description: string;
}

export interface FileTags {
    name: string;
}

export interface FileResources {
    id: number;
    name: string;
}

export interface FileInitiatives {
    id: number;
    name: string;
}