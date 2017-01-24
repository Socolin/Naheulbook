import {Component, OnInit, Input, OnChanges, SimpleChanges, Output, EventEmitter, ViewChild} from '@angular/core';
import {NhbkDate, CalendarDate} from './date.model';
import {DateService} from './date.service';
import {NhbkDialogService} from '../shared/nhbk-dialog.service';
import {OverlayRef, Portal} from '@angular/material';

@Component({
    selector: 'date-selector',
    styleUrls: ['./date-selector.component.scss'],
    templateUrl: './date-selector.component.html',
})
export class DateSelectorComponent implements OnInit, OnChanges {
    @Input() date: NhbkDate;
    @Output() onChange: EventEmitter<NhbkDate> = new EventEmitter<NhbkDate>();

    @ViewChild('dateSelectorDialog')
    public dateSelectorDialog: Portal<any>;
    public dateSelectorOverlayRef: OverlayRef;

    public calendar: CalendarDate[];
    public currentCalendarDate: CalendarDate;
    public relativeDay: number;
    public defaultDate: NhbkDate = new NhbkDate(0, 8, Math.floor(Math.random() * 365), 1498);

    constructor(private _dateService: DateService,
                private _nhbkDialogService: NhbkDialogService) {
    }

    openSelector() {
        this.dateSelectorOverlayRef = this._nhbkDialogService.openCenteredBackdropDialog(this.dateSelectorDialog);
    }

    closeSelector() {
        this.dateSelectorOverlayRef.detach();
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
