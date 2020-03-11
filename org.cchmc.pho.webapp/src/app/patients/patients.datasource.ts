import { Patients } from '../models/patients';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { CollectionViewer, DataSource } from '@angular/cdk/collections';
import { catchError, finalize } from 'rxjs/operators';
import { RestService } from '../rest.service';

export class PatientsDataSource implements DataSource<Patients> {

  private lessonsSubject = new BehaviorSubject<Patients[]>([]);
  private loadingSubject = new BehaviorSubject<boolean>(false);

  public loading$ = this.loadingSubject.asObservable();

  constructor(private restService: RestService) {}

  connect(collectionViewer: CollectionViewer): Observable<Patients[]> {
      return this.lessonsSubject.asObservable();
  }

  disconnect(collectionViewer: CollectionViewer): void {
      this.lessonsSubject.complete();
      this.loadingSubject.complete();
  }

  loadPatients(sortcolumn = 'name',
              sortDirection = 'asc', pageIndex = 0, pageSize = 20) {

      this.loadingSubject.next(true);

      this.restService.findPatients(sortcolumn, sortDirection,
          pageIndex, pageSize).pipe(
          catchError(() => of([])),
          finalize(() => this.loadingSubject.next(false))
      )
      .subscribe(patients => this.lessonsSubject.next(patients));
  }    
}