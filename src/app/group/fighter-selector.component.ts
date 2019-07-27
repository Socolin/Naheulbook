import {Component, EventEmitter, Input, Output, ViewChild} from '@angular/core';
import {MatCheckboxChange, MatListOption} from '@angular/material';
import {OverlayRef} from '@angular/cdk/overlay';
import {Portal} from '@angular/cdk/portal';

import {NhbkDialogService} from '../shared';
import {Fighter, Group} from './group.model';
import {IconDescription} from '../shared/icon.model';

@Component({
    selector: 'fighter-selector',
    templateUrl: 'fighter-selector.component.html',
    styleUrls: ['fighter-selector.component.scss']
})
export class FighterSelectorComponent {
    @Input() group: Group;
    @Output() selectFighters: EventEmitter<Fighter[]> = new EventEmitter<Fighter[]>();

    public title: string;
    public subtitle: string;
    public icon: IconDescription;
    public selectedFighters: Fighter[] = [];

    @ViewChild('selectorDialog', {static: true})
    public selectorDialog: Portal<any>;
    public selectorOverlayRef: OverlayRef;

    constructor(private _nhbkDialogService: NhbkDialogService) {
    }

    open(title: string, subtitle: string, icon?: IconDescription) {
        this.selectorOverlayRef = this._nhbkDialogService.openCenteredBackdropDialog(this.selectorDialog);
        this.title = title;
        this.subtitle = subtitle;
        this.icon = icon;
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
