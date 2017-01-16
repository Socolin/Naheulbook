import {Component, Output, EventEmitter, Input, ViewChild} from '@angular/core';

import {NhbkDateOffset} from './date.model';
import {OverlayState, OverlayRef, Portal, Overlay} from '@angular/material';

@Component({
    selector: 'date-modifier',
    styleUrls: ['./date-modifier.component.scss'],
    templateUrl: './date-modifier.component.html',
})
export class DateModifierComponent {
    @Output() onChange: EventEmitter<NhbkDateOffset> = new EventEmitter<NhbkDateOffset>();
    @Input() dateOffset: NhbkDateOffset = new NhbkDateOffset();
    @Input() resetOnChange: boolean = true;
    @Input() title: string;
    private forceUpdateDuration: number = 0;

    @ViewChild('dateSelectorDialog')
    public dateSelectorDialog: Portal<any>;
    public dateSelectorOverlayRef: OverlayRef;

    constructor(private _overlay: Overlay) {
    }

    openSelector() {
        let config = new OverlayState();

        config.positionStrategy = this._overlay.position()
            .global()
            .centerHorizontally()
            .centerVertically();
        config.hasBackdrop = true;

        let overlayRef = this._overlay.create(config);
        overlayRef.attach(this.dateSelectorDialog);
        overlayRef.backdropClick().subscribe(() => overlayRef.detach());
        this.dateSelectorOverlayRef = overlayRef;
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
