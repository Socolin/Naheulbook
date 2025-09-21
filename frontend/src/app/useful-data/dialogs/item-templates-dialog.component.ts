import {Component, OnInit} from '@angular/core';
import { MatDialogRef, MatDialogClose } from '@angular/material/dialog';
import {UsefulDataDialogResult} from './useful-data-dialog-result';
import { MatToolbar } from '@angular/material/toolbar';
import { MatIconButton } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import { ItemTemplateListComponent } from '../../item-template/item-template-list.component';

@Component({
    templateUrl: './item-templates-dialog.component.html',
    styleUrls: ['../../shared/full-screen-dialog.scss', './item-templates-dialog.component.scss'],
    imports: [MatToolbar, MatIconButton, MatDialogClose, MatIcon, ItemTemplateListComponent]
})
export class ItemTemplatesDialogComponent implements OnInit {

    constructor(
        private readonly dialogRef: MatDialogRef<ItemTemplatesDialogComponent, UsefulDataDialogResult>,
    ) {
    }

    ngOnInit() {
    }

    onAction(action: { action: string; data: any }) {
        this.dialogRef.close({action})
    }
}
