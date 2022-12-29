import {Component, ElementRef, Input, OnChanges, SimpleChanges, ViewChild} from '@angular/core';

import {dateOffset2TimeDuration, timeDuration2DateOffset2} from './util';
import {NhbkDateOffset} from './date.model';
import { MatButtonToggleChange } from '@angular/material/button-toggle';
import { MatSelectChange } from '@angular/material/select';
import {
    DurationSelectorDialogComponent,
    DurationSelectorDialogData,
    DurationSelectorDialogResult
} from './duration-selector-dialog.component';
import {IDurable} from '../api/shared';
import {NhbkMatDialog} from '../material-workaround';

@Component({
    selector: 'duration-selector',
    styleUrls: ['./duration-selector.component.scss'],
    templateUrl: './duration-selector.component.html',
})
export class DurationSelectorComponent implements OnChanges {
    @Input() durable: IDurable;
    @Input() hiddenType: string[] = [];

    public dateOffset: NhbkDateOffset = new NhbkDateOffset();

    @ViewChild('lapCountInput', {static: true})
    lapCountInput: ElementRef;
    @ViewChild('combatCountInput', {static: true})
    combatCountInput: ElementRef;
    @ViewChild('customDurationInput', {static: true})
    customDurationInput: ElementRef;

    constructor(
        private readonly dialog: NhbkMatDialog
    ) {
    }

    updateDuration(event?: MatButtonToggleChange | MatSelectChange) {
        if (event) {
            this.durable.durationType = event.value;
        }

        switch (this.durable.durationType) {
            case 'custom':
                if (this.durable.duration == null) {
                    this.durable.duration = '';
                }
                delete this.durable.lapCount;
                delete this.durable.timeDuration;
                delete this.durable.combatCount;
                break;
            case 'time':
                if (this.durable.timeDuration == null) {
                    this.durable.timeDuration = 0;
                }
                this.dateOffset = timeDuration2DateOffset2(this.durable.timeDuration);
                delete this.durable.lapCount;
                delete this.durable.combatCount;
                delete this.durable.duration;
                break;
            case 'combat':
                if (this.durable.combatCount == null) {
                    this.durable.combatCount = 1;
                }
                delete this.durable.lapCount;
                delete this.durable.timeDuration;
                delete this.durable.duration;
                break;
            case 'lap':
                if (this.durable.lapCount == null) {
                    this.durable.lapCount = 1;
                }
                delete this.durable.timeDuration;
                delete this.durable.combatCount;
                delete this.durable.duration;
                break;
            case 'forever':
                delete this.durable.lapCount;
                delete this.durable.timeDuration;
                delete this.durable.combatCount;
                delete this.durable.duration;
                break;
            default:
                throw new Error('Invalid durable.durationType: ' + this.durable.durationType);
        }
    }

    setTimeDuration(dateOffset: NhbkDateOffset) {
        this.durable.timeDuration = dateOffset2TimeDuration(dateOffset);
    }

    ngOnChanges(changes: SimpleChanges): void {
        if ('durable' in changes) {
            this.updateDuration();
        }
    }

    focusSelector() {
        switch (this.durable.durationType) {
            case 'custom':
                this.customDurationInput.nativeElement.focus();
                break;
            case 'combat':
                this.combatCountInput.nativeElement.focus();
                break;
            case 'lap':
                this.lapCountInput.nativeElement.focus();
                break;
        }
    }

    openDurationSelector() {
        const dialogRef = this.dialog.open<DurationSelectorDialogComponent, DurationSelectorDialogData, DurationSelectorDialogResult>(
            DurationSelectorDialogComponent,
            {
                autoFocus: false,
                data: {
                    duration: this.dateOffset
                }
            }
        );

        dialogRef.afterClosed().subscribe((result) => {
            if (!result) {
                return;
            }
            this.setTimeDuration(result.duration);
        });
    }
}
