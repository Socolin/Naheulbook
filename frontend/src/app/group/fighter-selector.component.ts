import {Component, Inject, ViewChild} from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogContent, MatDialogActions, MatDialogClose } from '@angular/material/dialog';
import { MatSelectionList, MatListOption } from '@angular/material/list';

import {Group} from './group.model';
import {IconDescription} from '../shared/icon.model';
import { MatCardHeader, MatCardAvatar, MatCardTitle, MatCardSubtitle } from '@angular/material/card';
import { IconComponent } from '../shared/icon.component';
import { CdkScrollable } from '@angular/cdk/scrolling';
import { FighterIconComponent } from './fighter-icon.component';
import { MatButton } from '@angular/material/button';

export interface FighterSelectorDialogData {
    group: Group;
    title: string;
    subtitle: string;
    icon?: IconDescription;
}

@Component({
    templateUrl: 'fighter-selector.component.html',
    styleUrls: ['fighter-selector.component.scss'],
    imports: [MatCardHeader, IconComponent, MatCardAvatar, MatCardTitle, MatCardSubtitle, CdkScrollable, MatDialogContent, MatSelectionList, MatListOption, FighterIconComponent, MatDialogActions, MatButton, MatDialogClose]
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
