import { CollectionViewer, DataSource } from "@angular/cdk/collections";
import { BehaviorSubject, Observable, of } from "rxjs";
import { catchError, finalize } from "rxjs/operators";
import { Contact } from "../models/contacts";
import { RestService } from "../rest.service";

export class ContactsDatasource implements DataSource<Contact>{
    private contactSubject = new BehaviorSubject<Contact[]>([]);
    private loadingSubject = new BehaviorSubject<boolean>(false);

    public loading$ = this.loadingSubject.asObservable();
    public ContactData$ = this.contactSubject.asObservable();
    
    constructor(private restService: RestService) {}
    
    connect(collectionViewer: CollectionViewer): Observable<Contact[]>{
        return this.contactSubject.asObservable();
    }

    disconnect(collectionViewer: CollectionViewer): void {
        this.contactSubject.complete();
        this.loadingSubject.complete();
    }
    
    loadContacts(qpl = '', specialties = '', membership = '', board = '', namesearch = ''){
        this.loadingSubject.next(true);
        this.restService.findContacts(qpl, specialties, membership, board, namesearch).pipe(
                catchError(() => of([])),
                finalize(() => this.loadingSubject.next(false))
            )
            .subscribe(contact => this.contactSubject.next(contact));
    }
}
