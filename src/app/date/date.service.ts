import {Injectable} from '@angular/core';
import {Http} from '@angular/http';
import {ReplaySubject, Observable} from 'rxjs/Rx';
import {CalendarDate} from "./date.model";

@Injectable()
export class DateService {
    private calendarDates: ReplaySubject<CalendarDate[]>;

    constructor(private _http: Http) {
    }

    getCalendarDates(): Observable<CalendarDate[]> {
        if (!this.calendarDates) {
            this.calendarDates = new ReplaySubject<CalendarDate[]>(1);

            this._http.get('/api/misc/calendar')
                .map(res => res.json())
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
