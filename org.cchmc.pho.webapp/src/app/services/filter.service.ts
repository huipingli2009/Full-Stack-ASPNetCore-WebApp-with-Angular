import { Injectable } from '@angular/core';
import { Subject, Observable, BehaviorSubject } from 'rxjs';

@Injectable({ providedIn: 'root' })

export class FilterService {

    private isFilteringPatients: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
    private isFilteringOutcomes: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
    private filterType: BehaviorSubject<string> = new BehaviorSubject<string>('');

    getIsFilteringPatients(): Observable<boolean> {
        return this.isFilteringPatients.asObservable();
    }
    getIsFilteringOutcomes(): Observable<boolean> {
        return this.isFilteringOutcomes.asObservable();
    }

    getFilterType(): Observable<string> {
        return this.filterType.asObservable();
    }

    updateIsFilteringPatients(isFiltering: boolean) {
        this.isFilteringPatients.next(isFiltering);
    }
    updateIsFilteringOutcomes(isFiltering: boolean) {
        this.isFilteringOutcomes.next(isFiltering);
    }

    updateFilterType(filterType: string) {
        this.filterType.next(filterType);
    }

}
