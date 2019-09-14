import {Component, OnInit} from '@angular/core';
import {MatDialogRef} from '@angular/material/dialog';
import {UsefulDataDialogResult} from './useful-data-dialog-result';
import {PanelNames} from '../useful-data.model';

@Component({
    templateUrl: './entropic-spells-dialog.component.html',
    styleUrls: ['../../shared/full-screen-dialog.scss', './entropic-spells-dialog.component.scss']
})
export class EntropicSpellsDialogComponent implements OnInit {

    constructor(
        private dialogRef: MatDialogRef<EntropicSpellsDialogComponent, UsefulDataDialogResult>,
    ) {
    }

    ngOnInit() {
    }

    openPanel(panelName: PanelNames, arg?: any) {
        this.dialogRef.close({openPanel: {panelName: panelName, arg}});
    }
}
