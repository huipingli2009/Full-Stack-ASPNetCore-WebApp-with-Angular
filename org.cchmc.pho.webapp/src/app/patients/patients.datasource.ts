import { CollectionViewer, DataSource } from '@angular/cdk/collections';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { catchError, finalize } from 'rxjs/operators';
import { Patients } from '../models/patients';
import { RestService } from '../rest.service';

export class PatientsDataSource implements DataSource<Patients> {

    private patientSubject = new BehaviorSubject<Patients[]>([]);
    private loadingSubject = new BehaviorSubject<boolean>(false);

    public loading$ = this.loadingSubject.asObservable();
    public PatientData$ = this.patientSubject.asObservable();
    private pageNumber: number;

    constructor(private restService: RestService) { }

    connect(collectionViewer: CollectionViewer): Observable<Patients[]> {
        return this.patientSubject.asObservable();
    }

    disconnect(collectionViewer: CollectionViewer): void {
        this.patientSubject.complete();
        this.loadingSubject.complete();
    }

    loadPatients(sortcolumn = 'name',
        sortDirection = 'asc', pageIndex = 0, pageSize = 20, chronic = '', watchFlag = '', conditionIDs = '',
        staffID = '', popmeasureID = '', outcomemetricId ='', namesearch = '') {

        this.loadingSubject.next(true);
        this.pageNumber = pageIndex + 1;
        this.restService.findPatients(sortcolumn, sortDirection,
            this.pageNumber, pageSize, chronic, watchFlag, conditionIDs, staffID, popmeasureID, outcomemetricId,
            namesearch).pipe(
                catchError(() => of([])),
                finalize(() => this.loadingSubject.next(false))
            )
            .subscribe(patients => this.patientSubject.next(patients));
    }
}