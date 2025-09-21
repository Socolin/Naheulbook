import {Component, Inject, OnInit} from '@angular/core';

import {NhbkDateOffset} from './date.model';
import {MAT_DIALOG_DATA, MatDialogRef} from '@angular/material/dialog';

export interface DurationSelectorDialogData {
    duration?: NhbkDateOffset
    title?: string
}

export interface DurationSelectorDialogResult {
    duration: NhbkDateOffset
}

type UnitTypes = 'minute' | 'hour' | 'day' | 'week';
const allUnits: { unit: UnitTypes, title: string }[] = [
    {
        unit: 'week',
        title: 'Semaine'
    },
    {
        unit: 'day',
        title: 'Jour'
    },
    {
        unit: 'hour',
        title: 'Heure'
    },
    {
        unit: 'minute',
        title: 'Minute'
    },
];

@Component({
    styleUrls: ['./duration-selector-dialog.component.scss'],
    templateUrl: './duration-selector-dialog.component.html',
    standalone: false
})
export class DurationSelectorDialogComponent implements OnInit {
    static numberHeight = 19;
    public dateOffset: NhbkDateOffset;
    public allUnits: { unit: UnitTypes, title: string }[] = allUnits;
    public scrollingUnit?: UnitTypes;
    public displayedNumbersPerUnit: { [unit: string]: string[] } = {};
    public startY = 0;
    public touchOffsetY: { [unit: string]: number } = {
        minute: -DurationSelectorDialogComponent.numberHeight,
        hour: -DurationSelectorDialogComponent.numberHeight,
        day: -DurationSelectorDialogComponent.numberHeight,
        week: -DurationSelectorDialogComponent.numberHeight,
    };
    public forceUpdateDuration = 0;

    constructor(
        @Inject(MAT_DIALOG_DATA) public readonly data: DurationSelectorDialogData,
        private readonly dialogRef: MatDialogRef<DurationSelectorDialogComponent, DurationSelectorDialogResult>
    ) {
        this.dateOffset = new NhbkDateOffset(data.duration);
    }

    updateTime(unit: UnitTypes, value: number): void {
        let newValue = this.dateOffset[unit] + value;
        switch (unit) {
            case 'minute':
                newValue = this.updateUnit(newValue, 60, ['hour', 'day', 'week']);
                break;
            case 'hour':
                newValue = this.updateUnit(newValue, 24, ['day', 'week']);
                break;
            case 'day':
                newValue = this.updateUnit(newValue, 7, ['week']);
                break;
            case 'week':
                break;
        }

        if (this.dateOffset[unit] < 0) {
            this.dateOffset[unit] = 0;
        }
        this.dateOffset[unit] = newValue < 0 ? 0 : newValue;
        this.forceUpdateDuration++;
        this.updateDisplayedNumbers();
    }

    validDate(): void {
        this.dialogRef.close({duration: this.dateOffset});
    }

    onWheelEvent(unit: UnitTypes, $event: WheelEvent): void {
        this.updateTime(unit, $event.deltaY < 0 ? -1 : 1);
    }

    onTouchStart(unit: UnitTypes, event: TouchEvent): void {
        this.startY = event.changedTouches[0].clientY;
        this.scrollingUnit = unit;
    }

    onTouchMoveEvent(event: TouchEvent): void {
        if (!this.scrollingUnit) {
            return;
        }
        event.preventDefault();
        let touchOffsetY = event.changedTouches[0].clientY - this.startY;

        while (touchOffsetY > DurationSelectorDialogComponent.numberHeight) {
            const oldValue = this.dateOffset[this.scrollingUnit];
            this.updateTime(this.scrollingUnit, -1);
            const newValue = this.dateOffset[this.scrollingUnit];
            if (oldValue === newValue) {
                touchOffsetY = DurationSelectorDialogComponent.numberHeight;
                break;
            } else {
                touchOffsetY -= DurationSelectorDialogComponent.numberHeight;
                this.startY += DurationSelectorDialogComponent.numberHeight;
            }
        }

        while (touchOffsetY < -DurationSelectorDialogComponent.numberHeight) {
            this.updateTime(this.scrollingUnit, 1);
            touchOffsetY += DurationSelectorDialogComponent.numberHeight;
            this.startY -= DurationSelectorDialogComponent.numberHeight;
        }
        this.touchOffsetY[this.scrollingUnit] = -DurationSelectorDialogComponent.numberHeight + touchOffsetY;
    }

    onTouchEnd(): void {
        if (!this.scrollingUnit) {
            return;
        }

        this.touchOffsetY[this.scrollingUnit] = -DurationSelectorDialogComponent.numberHeight;
        this.scrollingUnit = undefined;
    }

    isSpace(number: string) {
        return number === '&nbsp;';
    }

    ngOnInit(): void {
        this.updateDisplayedNumbers();
    }

    private updateDisplayedNumbers(): void {
        this.displayedNumbersPerUnit = {
            minute: this.generateDisplayedNumberForUnit('minute', 60, ['hour', 'day', 'week']),
            hour: this.generateDisplayedNumberForUnit('hour', 24, ['day', 'week']),
            day: this.generateDisplayedNumberForUnit('day', 7, ['week']),
            week: this.generateDisplayedNumberForUnit('week')
        }
    }

    private generateDisplayedNumberForUnit(unit: string, maxValue?: number, nextUnits?: string[]): string[] {
        const displayedNumbers: string[] = [];
        for (let i = -3; i <= 3; i++) {
            const value = i + this.dateOffset[unit];
            if (value < 0) {
                if (nextUnits && nextUnits.find(u => this.dateOffset[u] > 0)) {
                    displayedNumbers.push((value + maxValue).toString());
                } else {
                    displayedNumbers.push('&nbsp;');
                }
            } else {
                if (maxValue) {
                    displayedNumbers.push((value % maxValue).toString());
                } else {
                    displayedNumbers.push(value.toString());
                }
            }
        }
        return displayedNumbers;
    }

    private updateUnit(newValue: number, maxValue: number, nextUnits: UnitTypes[]) {
        while (newValue >= maxValue) {
            this.updateTime(nextUnits[0], 1);
            newValue -= maxValue;
        }
        while (nextUnits.find(u => this.dateOffset[u] > 0) && newValue < 0) {
            newValue += maxValue;
            this.updateTime(nextUnits[0], -1);
        }
        return newValue;
    }
}
