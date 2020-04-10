export interface FilesList {
    id: number;
    name: string;
    dateCreated: Date;
    lastViewed: Date;
    watchFlag: boolean;
    fileSize: string;
    fileURL: string;
    tags: [
        {
            name: string;
        }
    ];
    description: string;
}