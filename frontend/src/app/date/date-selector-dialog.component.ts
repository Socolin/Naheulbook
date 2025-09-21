import {Component, Inject, OnInit} from '@angular/core';
import {UntypedFormControl, UntypedFormGroup, Validators} from '@angular/forms';
import {DateService} from './date.service';
import {CalendarDate, NhbkDate} from './date.model';
import {MAT_DIALOG_DATA, MatDialogRef} from '@angular/material/dialog';

export interface DateSelectorDialogData {
    date?: NhbkDate;
    title: string;
}

export interface DateSelectorDialogResult {
    date: NhbkDate;
}

@Component({
    selector: 'app-date-selector-dialog',
    templateUrl: './date-selector-dialog.component.html',
    styleUrls: ['./date-selector-dialog.component.scss'],
    standalone: false
})
export class DateSelectorDialogComponent implements OnInit {
    public defaultDate: NhbkDate = new NhbkDate(0, 8, Math.floor(Math.random() * 365), 1498);
    public calendarEntries: CalendarDate[] = [];
    public calendarPerRealMonth: { monthName: string, calendarEntries: CalendarDate[] }[] = [];

    public form = new UntypedFormGroup({
        hour: new UntypedFormControl('', [Validators.required, Validators.min(0), Validators.max(23)]),
        minute: new UntypedFormControl('', [Validators.required, Validators.min(0), Validators.max(59)]),
        relativeDay: new UntypedFormControl('', [Validators.required, Validators.min(0), Validators.max(250)]),
        currentCalendarDate: new UntypedFormControl('', Validators.required),
        year: new UntypedFormControl('', [Validators.required, Validators.min(0), Validators.max(10000)]),
    });

    constructor(
        @Inject(MAT_DIALOG_DATA) public readonly data: DateSelectorDialogData,
        private readonly dateService: DateService,
        private readonly dialogRef: MatDialogRef<DateSelectorDialogComponent, DateSelectorDialogResult>
    ) {
    }

    updateCurrentCalendar() {
        if (!this.calendarEntries) {
            return;
        }

        const currentDate = this.data.date || this.defaultDate;

        let currentCalendarDate: CalendarDate = this.calendarEntries[0];
        let relativeDay = 1;
        for (let i = 0; i < this.calendarEntries.length; i++) {
            let calendarDate = this.calendarEntries[i];
            if (calendarDate.startDay <= currentDate.day && currentDate.day <= calendarDate.endDay) {
                currentCalendarDate = calendarDate;
                relativeDay = currentDate.day - currentCalendarDate.startDay + 1;
                break;
            }
        }

        if (currentCalendarDate == null) {
            return;
        }

        this.form.patchValue(currentDate);
        this.form.patchValue({
            'relativeDay': relativeDay,
            'currentCalendarDate': currentCalendarDate
        });
    }

    valid() {
        const newDate = new NhbkDate(
            this.form.controls.minute.value,
            this.form.controls.hour.value,
            this.form.controls.relativeDay.value + this.form.controls.currentCalendarDate.value.startDay - 1,
            this.form.controls.year.value
        );
        this.dialogRef.close({date: newDate});
    }

    ngOnInit() {
        this.dateService.getCalendarDates().subscribe(
            calendarEntries => {
                this.calendarEntries = calendarEntries;
                let lastMonth = '';
                for (const calendarEntry of calendarEntries) {
                    const realDate = new Date(200, 0, 0, 0, 0, 0, 0);
                    realDate.setDate(realDate.getDate() + calendarEntry.startDay);
                    let realMonth = realDate.toLocaleDateString('fr-FR', {month: 'long'});
                    realMonth = realMonth.charAt(0).toUpperCase() + realMonth.slice(1);
                    if (realMonth !== lastMonth) {
                        this.calendarPerRealMonth.push({monthName: realMonth, calendarEntries: []});
                        lastMonth = realMonth;
                    }
                    this.calendarPerRealMonth[this.calendarPerRealMonth.length - 1].calendarEntries.push(calendarEntry);
                }

                this.updateCurrentCalendar();
            }
        )
    }
}
