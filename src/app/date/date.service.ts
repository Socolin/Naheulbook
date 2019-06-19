
import {map} from 'rxjs/operators';
import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {ReplaySubject, Observable} from 'rxjs';
import {CalendarDate} from './date.model';

@Injectable()
export class DateService {
    private calendarDates: ReplaySubject<CalendarDate[]>;

    constructor(private httpClient: HttpClient) {
    }

    getCalendarDates(): Observable<CalendarDate[]> {
        if (!this.calendarDates) {
            this.calendarDates = new ReplaySubject<CalendarDate[]>(1);

            this.httpClient.get<CalendarDate[]>('/api/misc/calendar')
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
