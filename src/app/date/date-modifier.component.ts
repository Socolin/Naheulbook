import {Component, Output, EventEmitter, Input} from '@angular/core';

import {NhbkDateOffset} from './date.model';

@Component({
    selector: 'date-modifier',
    templateUrl: './date-modifier.component.html',
})
export class DateModifierComponent {
    @Output() onChange: EventEmitter<NhbkDateOffset> = new EventEmitter<NhbkDateOffset>();
    private show: boolean;
    @Input() dateOffset: NhbkDateOffset = new NhbkDateOffset();
    @Input() resetOnChange: boolean = true;
    private forceUpdateDuration: number = 0;

    openSelector() {
        this.show = true;
    }

    closeSelector() {
        this.show = false;
    }

    updateTime(unit: string, value: number) {
        this.dateOffset[unit] += value;
        this.forceUpdateDuration++;
    }

    validDate() {
        this.onChange.emit(this.dateOffset);
        if (this.resetOnChange) {
            this.dateOffset = new NhbkDateOffset();
        }
        this.closeSelector();
    }
}
