import {Component, OnInit, Input, OnChanges, SimpleChanges} from '@angular/core';
import {NhbkDate, CalendarDate} from './date.model';
import {DateService} from './date.service';

@Component({
    selector: 'date',
    templateUrl: 'date.component.html',
})
export class DateComponent implements OnInit, OnChanges {
    @Input() date: NhbkDate;
    private calendar: CalendarDate[];
    private currentCalendarDate: CalendarDate;
    private relativeDay: number;
    private minute: string;
    private hour: string;
    private period: string;

    constructor(private _dateService: DateService) {
    }

    updateCurrentCalendar() {
        if (!this.date) {
            return;
        }
        if (this.date.minute < 10) {
            this.minute = '0' + this.date.minute.toString();
        }
        else if (this.date.minute) {
            this.minute = this.date.minute.toString();
        }
        else {
            this.minute = '00';
        }

        if (this.date.hour < 10) {
            this.hour = '0' + this.date.hour.toString();
        }
        else if (this.date.hour) {
            this.hour = this.date.hour.toString();
        }
         else {
            this.minute = '00';
        }

        if (!this.calendar) {
            return;
        }
        for (let i = 0; i < this.calendar.length; i++) {
            let calendarDate = this.calendar[i];
            if (calendarDate.startDay <= this.date.day && this.date.day <= calendarDate.endDay) {
                this.currentCalendarDate = calendarDate;
                this.relativeDay = this.date.day - this.currentCalendarDate.startDay + 1;
                if (calendarDate.name.startsWith('Trois jour')) {
                    this.period = this.relativeDay.toString() + 'e jour des ' + this.currentCalendarDate.name;
                } else if (calendarDate.name.startsWith('Fête')) {
                    this.period = this.currentCalendarDate.name;
                } else {
                    this.period = this.relativeDay.toString() + 'e jour de la ' + this.currentCalendarDate.name;
                }
                break;
            }
        }
        if (this.currentCalendarDate == null) {
            this.relativeDay = 1;
            return;
        }
    }

    ngOnChanges(changes: SimpleChanges): void {
        if ('date' in changes) {
            this.updateCurrentCalendar();
        }
    }

    ngOnInit(): void {
        this._dateService.getCalendarDates().subscribe(
            calendar => {
                this.calendar = calendar;
                this.updateCurrentCalendar();
            }
        );
    }
}
