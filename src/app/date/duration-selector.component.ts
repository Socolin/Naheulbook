import {Component, Input, OnChanges, SimpleChanges} from '@angular/core';

import {isNullOrUndefined} from 'util';

import {dateOffset2TimeDuration, timeDuration2DateOffset2} from './util';
import {NhbkDateOffset} from './date.model';
import {IDurable} from './durable.model';
import {MdButtonToggleChange, MdSelectChange} from '@angular/material';

@Component({
    selector: 'duration-selector',
    styleUrls: ['./duration-selector.component.scss'],
    templateUrl: './duration-selector.component.html',
})
export class DurationSelectorComponent implements OnChanges {
    @Input() durable: IDurable;
    @Input() hiddenType: string[] = [];

    public dateOffset: NhbkDateOffset = new NhbkDateOffset();

    updateDuration(event?: MdButtonToggleChange|MdSelectChange) {
        if (event) {
            this.durable.durationType = event.value;
        }

        switch (this.durable.durationType) {
            case 'custom':
                if (isNullOrUndefined(this.durable.duration)) {
                    this.durable.duration = '';
                }
                delete this.durable.lapCount;
                delete this.durable.timeDuration;
                delete this.durable.combatCount;
                break;
            case 'time':
                if (isNullOrUndefined(this.durable.timeDuration)) {
                    this.durable.timeDuration = 0;
                }
                this.dateOffset = timeDuration2DateOffset2(this.durable.timeDuration);
                delete this.durable.lapCount;
                delete this.durable.combatCount;
                delete this.durable.duration;
                break;
            case 'combat':
                if (isNullOrUndefined(this.durable.combatCount)) {
                    this.durable.combatCount = 1;
                }
                delete this.durable.lapCount;
                delete this.durable.timeDuration;
                delete this.durable.duration;
                break;
            case 'lap':
                if (isNullOrUndefined(this.durable.lapCount)) {
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
}
