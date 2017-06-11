import {Component, EventEmitter, Input, Output, ViewChild} from '@angular/core';
import {Fighter, Group} from './group.model';
import {MdCheckboxChange, OverlayRef, Portal} from '@angular/material';
import {NhbkDialogService} from '../shared/nhbk-dialog.service';

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

    selectFighter(fighter: Fighter, event: MdCheckboxChange) {
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

    private close() {
        this.selectorOverlayRef.detach();
    }
}
