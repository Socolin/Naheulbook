import {Component, EventEmitter, Input, Output, ViewChild} from '@angular/core';
import {MatCheckboxChange} from '@angular/material';
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

    @ViewChild('selectorDialog')
    public selectorDialog: Portal<any>;
    public selectorOverlayRef: OverlayRef;

    constructor(private _nhbkDialogService: NhbkDialogService) {
    }

    open(label: string) {
        this.selectorOverlayRef = this._nhbkDialogService.openCenteredBackdropDialog(this.selectorDialog);
        this.label = label;
    }

    valid() {
        this.selectFighters.emit(this.selectedFighters);
        this.close();
    }

    selectFighter(fighter: Fighter, event: MatCheckboxChange) {
        if (event.checked) {
            this.selectedFighters.push(fighter);
        }
        else {
            let i = this.selectedFighters.findIndex(f => f.isMonster === fighter.isMonster && f.id === fighter.id);
            if (i !== -1) {
                this.selectedFighters.splice(i, 1);
            }
        }
    }

    close() {
        this.selectorOverlayRef.detach();
    }
}
