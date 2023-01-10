import {Component, Inject} from '@angular/core';
import {UntypedFormControl, UntypedFormGroup, Validators} from '@angular/forms';
import {MAT_DIALOG_DATA, MatDialogRef} from '@angular/material/dialog';
import {NhbkMatDialog} from '../material-workaround';
import {NameGeneratorDialogComponent, NameGeneratorDialogResult} from '../origin/name-generator-dialog.component';
import {INpcData} from '../api/shared';
import {Npc} from './group.model';

export interface EditNpcDialogData {
    npc?: Npc;
}

export interface EditNpcDialogResult {
    name: string;
    data: INpcData;
}

@Component({
    templateUrl: './edit-npc-dialog.component.html',
    styleUrls: ['../shared/full-screen-dialog.scss', './edit-npc-dialog.component.scss']
})
export class EditNpcDialogComponent {
    public form = new UntypedFormGroup({
        name: new UntypedFormControl(undefined, Validators.required),
        data: new UntypedFormGroup({
            location: new UntypedFormControl(undefined),
            note: new UntypedFormControl(undefined),
            sex: new UntypedFormControl(undefined),
            originName: new UntypedFormControl(undefined)
        })
    });

    constructor(
        private readonly dialog: NhbkMatDialog,
        private readonly dialogRef: MatDialogRef<EditNpcDialogComponent, EditNpcDialogResult>,
        @Inject(MAT_DIALOG_DATA) public readonly data: EditNpcDialogData,
    ) {
        if (data.npc) {
            this.form.reset(data.npc)
        }
    }

    valid() {
        this.dialogRef.close({...this.form.value})
    }

    openRandomNameGeneratorDialog() {
        const dialogRef = this.dialog.open<NameGeneratorDialogComponent, never, NameGeneratorDialogResult>(
            NameGeneratorDialogComponent, {
                width: '600px'
            }
        );

        dialogRef.afterClosed().subscribe((result) => {
            if (!result) {
                return;
            }

            this.form.controls['name'].setValue(result.name);
            this.form.controls['data']['controls']['sex'].setValue(result.sex);
            this.form.controls['data']['controls']['originName'].setValue(result.originName);
        })
    }
}
