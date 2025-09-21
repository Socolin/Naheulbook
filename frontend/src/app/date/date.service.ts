import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Observable, ReplaySubject} from 'rxjs';
import {CalendarDate} from './date.model';
import {CalendarResponse} from '../api/responses';

@Injectable({providedIn: 'root'})
export class DateService {
    private calendarDates: ReplaySubject<CalendarDate[]>;

    constructor(
        private readonly httpClient: HttpClient,
    ) {
    }

    getCalendarDates(): Observable<CalendarDate[]> {
        if (!this.calendarDates) {
            this.calendarDates = new ReplaySubject<CalendarDate[]>(1);

            this.httpClient.get<CalendarResponse[]>('/api/v2/calendar')
                .subscribe(
                    calendarDates => {
                        this.calendarDates.next(calendarDates);
                        this.calendarDates.complete();
                    },
                    error => {
                        this.calendarDates.error(error);
                    }
                );
        }
        return this.calendarDates;
    }
}
