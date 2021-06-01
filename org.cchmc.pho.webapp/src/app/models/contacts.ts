export interface Contact {
    practiceId: number,
    practiceName: string,
    practiceType: string,
    emr: string,
    phone: string,
    fax: string
    websiteURL: string
}

export interface ContactPracticeDetails {
    practiceId: number,
    practiceName: string,
    memberSince: Date,
    practiceManager: string,
    pmEmail: string,
    pic: string
    picEmail: string   
}
