import {Component, OnInit} from '@angular/core';
import {ActiveStatsModifier} from '../shared';
import { MatDialogRef, MatDialogClose } from '@angular/material/dialog';
import { MatToolbar } from '@angular/material/toolbar';
import { MatIconButton, MatButton } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import { StatModifierEditorComponent } from '../effect/stats-modifier-editor.component';

@Component({
    selector: 'app-add-item-modifier-dialog',
    templateUrl: './add-item-modifier-dialog.component.html',
    styleUrls: ['./add-item-modifier-dialog.component.scss', '../shared/full-screen-dialog.scss'],
    imports: [MatToolbar, MatIconButton, MatDialogClose, MatIcon, MatButton, StatModifierEditorComponent]
})
export class AddItemModifierDialogComponent implements OnInit {

    public newItemModifier: ActiveStatsModifier = new ActiveStatsModifier();

    constructor(
        public dialogRef: MatDialogRef<AddItemModifierDialogComponent>,
    ) {
    }

    ngOnInit() {
    }

    valid() {
        this.dialogRef.close(this.newItemModifier);
    }
}
