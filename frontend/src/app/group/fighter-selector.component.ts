import {Component, Inject, ViewChild} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialogRef} from '@angular/material/dialog';
import {MatSelectionList} from '@angular/material/list';

import {Group} from './group.model';
import {IconDescription} from '../shared/icon.model';

export interface FighterSelectorDialogData {
    group: Group;
    title: string;
    subtitle: string;
    icon?: IconDescription;
}

@Component({
    templateUrl: 'fighter-selector.component.html',
    styleUrls: ['fighter-selector.component.scss']
})
export class FighterSelectorComponent {
    @ViewChild(MatSelectionList, {static: true})
    public fighterSelection: MatSelectionList;

    constructor(
        public dialogRef: MatDialogRef<FighterSelectorComponent>,
        @Inject(MAT_DIALOG_DATA) public data: FighterSelectorDialogData
    ) {
    }

    valid() {
        this.dialogRef.close(this.fighterSelection.selectedOptions.selected.map(s => s.value));
    }
}
