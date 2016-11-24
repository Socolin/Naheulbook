import {Component, OnInit, Input, OnChanges, SimpleChanges, Output, EventEmitter} from '@angular/core';
import {NhbkDate, CalendarDate} from './date.model';
import {DateService} from './date.service';

@Component({
    selector: 'date-selector',
    templateUrl: 'date-selector.component.html',
})
export class DateSelectorComponent implements OnInit, OnChanges {
    @Input() date: NhbkDate;
    @Output() onChange: EventEmitter<NhbkDate> = new EventEmitter<NhbkDate>();
    private show: boolean;
    private calendar: CalendarDate[];
    private currentCalendarDate: CalendarDate;
    private relativeDay: number;
    private defaultDate: NhbkDate = new NhbkDate(0, 8, Math.floor(Math.random() * 365), 1498);

    constructor(private _dateService: DateService) {
    }

    openSelector() {
        this.show = true;
    }

    closeSelector() {
        this.show = false;
    }

    validDate() {
        if (!this.currentCalendarDate) {
            return;
        }
        this.date.day = this.relativeDay + this.currentCalendarDate.startDay - 1;
        this.onChange.emit(this.date);
        this.closeSelector();
    }

    setCurrentCalendarDate(cDate: CalendarDate): boolean {
        if (!cDate) {
            return;
        }
        this.currentCalendarDate = cDate;
        this.date.day = this.relativeDay + cDate.startDay - 1;
        return false;
    }

    updateCurrentCalendar() {
        if (!this.calendar) {
            return;
        }
        for (let i = 0; i < this.calendar.length; i++) {
            let calendarDate = this.calendar[i];
            if (calendarDate.startDay <= this.date.day && this.date.day <= calendarDate.endDay) {
                this.currentCalendarDate = calendarDate;
                this.relativeDay = this.date.day - this.currentCalendarDate.startDay + 1;
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
        if (!this.date) {
            this.date = this.defaultDate;
        }
        this._dateService.getCalendarDates().subscribe(
            calendar => {
                this.calendar = calendar;
                this.updateCurrentCalendar();
            }
        )
    }
}
