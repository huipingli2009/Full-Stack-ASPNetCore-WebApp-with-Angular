export interface Contact {
    PracticeId: number,
    PracticeName: string,
    PracticeType: string,
    EMR: string,
    Phone: string,
    Fax: string
    WebsiteURL: string
}

export interface ContactPracticeDetails {
    PracticeId: number,
    PracticeName: string,
    MemberSince: Date,
    PracticeManager: string,
    PMEmail: string,
    PIC: string
    PICEmail: string   
}
