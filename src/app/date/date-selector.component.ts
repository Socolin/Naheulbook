import {Component, OnInit, Input, OnChanges, SimpleChanges, Output, EventEmitter, ViewChild} from '@angular/core';
import {OverlayRef} from '@angular/cdk/overlay';
import {Portal} from '@angular/cdk/portal';

import {NhbkDate, CalendarDate} from './date.model';
import {DateService} from './date.service';
import {NhbkDialogService} from '../shared/nhbk-dialog.service';
import {FormControl, FormGroup, Validators} from '@angular/forms';

@Component({
    selector: 'date-selector',
    styleUrls: ['./date-selector.component.scss'],
    templateUrl: './date-selector.component.html',
})
export class DateSelectorComponent implements OnInit, OnChanges {
    @Input() date: NhbkDate;
    @Output() onChange: EventEmitter<NhbkDate> = new EventEmitter<NhbkDate>();

    @ViewChild('dateSelectorDialog', {static: true})
    public dateSelectorDialog: Portal<any>;
    public dateSelectorOverlayRef: OverlayRef | undefined;

    public calendar: CalendarDate[];
    public defaultDate: NhbkDate = new NhbkDate(0, 8, Math.floor(Math.random() * 365), 1498);

    public form = new FormGroup({
        hour: new FormControl('', [Validators.required, Validators.min(0), Validators.max(23)]),
        minute: new FormControl('', [Validators.required, Validators.min(0), Validators.max(59)]),
        relativeDay: new FormControl('', [Validators.required, Validators.min(0), Validators.max(250)]),
        currentCalendarDate: new FormControl('', Validators.required),
        year: new FormControl('', [Validators.required, Validators.min(0), Validators.max(10000)]),
    });

    constructor(private _dateService: DateService,
                private _nhbkDialogService: NhbkDialogService) {
    }

    openSelector() {
        this.dateSelectorOverlayRef = this._nhbkDialogService.openCenteredBackdropDialog(this.dateSelectorDialog);
    }

    closeSelector() {
        if (!this.dateSelectorOverlayRef) {
            return;
        }
        this.dateSelectorOverlayRef.detach();
        this.dateSelectorOverlayRef = undefined;
    }

    validDate() {
        if (!this.form.valid) {
            return;
        }
        this.date.minute = this.form.controls.minute.value;
        this.date.hour = this.form.controls.hour.value;
        this.date.day = this.form.controls.relativeDay.value + this.form.controls.currentCalendarDate.value.startDay - 1;
        this.onChange.emit(this.date);
        this.closeSelector();
    }

    updateCurrentCalendar() {
        if (!this.calendar) {
            return;
        }
        if (!this.date) {
            return;
        }
        let currentCalendarDate: CalendarDate = this.calendar[0];
        let relativeDay = 1;
        for (let i = 0; i < this.calendar.length; i++) {
            let calendarDate = this.calendar[i];
            if (calendarDate.startDay <= this.date.day && this.date.day <= calendarDate.endDay) {
                currentCalendarDate = calendarDate;
                relativeDay = this.date.day - currentCalendarDate.startDay + 1;
                break;
            }
        }
        if (currentCalendarDate == null) {
            return;
        }
        this.form.patchValue(this.date);
        this.form.patchValue({
            'relativeDay': relativeDay,
            'currentCalendarDate': currentCalendarDate
        });
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
