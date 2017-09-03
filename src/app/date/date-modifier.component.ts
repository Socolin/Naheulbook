import {Component, Output, EventEmitter, Input, ViewChild} from '@angular/core';
import {OverlayRef} from '@angular/cdk/overlay';
import {Portal} from '@angular/cdk/portal';

import {NhbkDialogService} from '../shared';

import {NhbkDateOffset} from './date.model';

@Component({
    selector: 'date-modifier',
    styleUrls: ['./date-modifier.component.scss'],
    templateUrl: './date-modifier.component.html',
})
export class DateModifierComponent {
    @Output() onChange: EventEmitter<NhbkDateOffset> = new EventEmitter<NhbkDateOffset>();
    @Input() dateOffset: NhbkDateOffset = new NhbkDateOffset();
    @Input() resetOnChange = true;
    @Input() title: string;
    public forceUpdateDuration = 0;

    @ViewChild('dateSelectorDialog')
    public dateSelectorDialog: Portal<any>;
    public dateSelectorOverlayRef: OverlayRef;

    constructor(private _nhbkDialogService: NhbkDialogService) {
    }

    openSelector() {
        this.dateSelectorOverlayRef = this._nhbkDialogService.openCenteredBackdropDialog(this.dateSelectorDialog);
    }

    closeSelector() {
        this.dateSelectorOverlayRef.detach();
    }

    updateTime(unit: string, value: number) {
        this.dateOffset[unit] += value;
        if (this.dateOffset[unit] < 0) {
            this.dateOffset[unit] = 0;
        }
        this.forceUpdateDuration++;
    }

    resetDate() {
        this.dateOffset = new NhbkDateOffset();
    }

    validDate() {
        this.onChange.emit(this.dateOffset);
        if (this.resetOnChange) {
            this.dateOffset = new NhbkDateOffset();
        }
        this.closeSelector();
    }
}
