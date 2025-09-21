import {Component, OnInit} from '@angular/core';
import { MatDialogRef, MatDialogClose } from '@angular/material/dialog';
import {UsefulDataDialogResult} from './useful-data-dialog-result';
import {PanelNames} from '../useful-data.model';
import { MatToolbar } from '@angular/material/toolbar';
import { MatIconButton } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';

@Component({
    templateUrl: './entropic-spells-dialog.component.html',
    styleUrls: ['../../shared/full-screen-dialog.scss', './entropic-spells-dialog.component.scss'],
    imports: [MatToolbar, MatIconButton, MatDialogClose, MatIcon]
})
export class EntropicSpellsDialogComponent implements OnInit {

    constructor(
        private readonly dialogRef: MatDialogRef<EntropicSpellsDialogComponent, UsefulDataDialogResult>,
    ) {
    }

    ngOnInit() {
    }

    openPanel(panelName: PanelNames, arg?: any) {
        this.dialogRef.close({openPanel: {panelName: panelName, arg}});
    }
}
