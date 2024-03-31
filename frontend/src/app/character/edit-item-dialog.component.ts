import {Component, Inject, OnInit} from '@angular/core';
import {Item} from '../item';
import { MAT_LEGACY_DIALOG_DATA as MAT_DIALOG_DATA, MatLegacyDialogRef as MatDialogRef } from '@angular/material/legacy-dialog';
import {UntypedFormControl, UntypedFormGroup, Validators} from '@angular/forms';
import {IconDescription} from '../shared/icon.model';

export interface EditItemDialogData {
    item: Item;
}

export interface EditItemDialogResult {
    name: string;
    description: string;
    icon?: IconDescription
}

@Component({
    selector: 'app-edit-item-dialog',
    templateUrl: './edit-item-dialog.component.html',
    styleUrls: ['./edit-item-dialog.component.scss']
})
export class EditItemDialogComponent implements OnInit {
    public form = new UntypedFormGroup({
        name: new UntypedFormControl(),
        description: new UntypedFormControl(),
    });
    public icon?: IconDescription;

    constructor(
        public dialogRef: MatDialogRef<EditItemDialogComponent, EditItemDialogResult>,
        @Inject(MAT_DIALOG_DATA) public data: EditItemDialogData
    ) {
        this.icon = data.item.data.icon;
        this.form.reset({
            name: data.item.data.name,
            description: data.item.data.description,
        });
    }

    ngOnInit() {
    }

    valid() {
        this.dialogRef.close({
            name: this.form.value.name,
            description: this.form.value.description,
        })
    }
}
