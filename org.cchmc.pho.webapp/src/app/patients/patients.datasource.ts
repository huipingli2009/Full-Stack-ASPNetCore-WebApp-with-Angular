import { CollectionViewer, DataSource } from '@angular/cdk/collections';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { catchError, finalize } from 'rxjs/operators';
import { Patients } from '../models/patients';
import { RestService } from '../rest.service';

export class PatientsDataSource implements DataSource<Patients> {

    private lessonsSubject = new BehaviorSubject<Patients[]>([]);
    private loadingSubject = new BehaviorSubject<boolean>(false);

    public loading$ = this.loadingSubject.asObservable();

    constructor(private restService: RestService) { }

    connect(collectionViewer: CollectionViewer): Observable<Patients[]> {
        return this.lessonsSubject.asObservable();
    }

    disconnect(collectionViewer: CollectionViewer): void {
        this.lessonsSubject.complete();
        this.loadingSubject.complete();
    }

    loadPatients(sortcolumn = 'name',
        sortDirection = 'asc', pageIndex = 0, pageSize = 20, chronic = '', watchFlag = '', conditionIDs = '',
        staffID = '', popmeasureID = '', namesearch = '') {

        this.loadingSubject.next(true);

        this.restService.findPatients(sortcolumn, sortDirection,
            pageIndex, pageSize, chronic, watchFlag, conditionIDs, staffID, popmeasureID,
            namesearch).pipe(
                catchError(() => of([])),
                finalize(() => this.loadingSubject.next(false))
            )
            .subscribe(patients => this.lessonsSubject.next(patients));
    }
}