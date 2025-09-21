import {Component, Inject, OnInit} from '@angular/core';
import {Item} from '../item';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogTitle, MatDialogContent, MatDialogActions, MatDialogClose } from '@angular/material/dialog';
import { UntypedFormControl, UntypedFormGroup, Validators, FormsModule, ReactiveFormsModule } from '@angular/forms';
import {IconDescription} from '../shared/icon.model';
import { CdkScrollable } from '@angular/cdk/scrolling';
import { MatFormField } from '@angular/material/form-field';
import { MatInput } from '@angular/material/input';
import { MatButton } from '@angular/material/button';

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
    styleUrls: ['./edit-item-dialog.component.scss'],
    imports: [MatDialogTitle, FormsModule, ReactiveFormsModule, CdkScrollable, MatDialogContent, MatFormField, MatInput, MatDialogActions, MatButton, MatDialogClose]
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
