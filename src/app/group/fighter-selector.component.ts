import {Component, EventEmitter, Input, Output, ViewChild} from '@angular/core';
import {MatCheckboxChange, MatListOption} from '@angular/material';
import {OverlayRef} from '@angular/cdk/overlay';
import {Portal} from '@angular/cdk/portal';

import {NhbkDialogService} from '../shared';
import {Fighter, Group} from './group.model';

@Component({
    selector: 'fighter-selector',
    templateUrl: 'fighter-selector.component.html',
    styleUrls: ['fighter-selector.component.scss']
})
export class FighterSelectorComponent {
    @Input() group: Group;
    @Output() selectFighters: EventEmitter<Fighter[]> = new EventEmitter<Fighter[]>();

    public label: string;
    public selectedFighters: Fighter[] = [];

    @ViewChild('selectorDialog', {static: true})
    public selectorDialog: Portal<any>;
    public selectorOverlayRef: OverlayRef;

    constructor(private _nhbkDialogService: NhbkDialogService) {
    }

    open(label: string) {
        this.selectorOverlayRef = this._nhbkDialogService.openCenteredBackdropDialog(this.selectorDialog);
        this.label = label;
    }

    fighterSelectionChange(selected: MatListOption[]) {
        this.selectedFighters = selected.map(s => s.value);
    }

    valid() {
        this.selectFighters.emit(this.selectedFighters);
        this.selectedFighters = [];
        this.close();
    }

    close() {
        this.selectorOverlayRef.detach();
    }
}
