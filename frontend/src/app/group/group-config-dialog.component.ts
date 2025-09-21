import {Component, Inject, OnInit} from '@angular/core';
import {IGroupConfig} from '../api/shared';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogTitle, MatDialogContent, MatDialogClose } from '@angular/material/dialog';
import { CdkScrollable } from '@angular/cdk/scrolling';
import { MatCheckbox } from '@angular/material/checkbox';
import { FormsModule } from '@angular/forms';
import { MatCardActions } from '@angular/material/card';
import { MatButton } from '@angular/material/button';

export class GroupConfigDialogData {
    groupConfig: IGroupConfig;
}

export class GroupConfigDialogResult {
    groupConfig: IGroupConfig
}

@Component({
    selector: 'app-group-config-dialog',
    templateUrl: './group-config-dialog.component.html',
    styleUrls: ['./group-config-dialog.component.scss'],
    imports: [MatDialogTitle, CdkScrollable, MatDialogContent, MatCheckbox, FormsModule, MatCardActions, MatButton, MatDialogClose]
})
export class GroupConfigDialogComponent implements OnInit {
    public config: IGroupConfig

    constructor(
        public readonly dialogRef: MatDialogRef<GroupConfigDialogComponent, GroupConfigDialogResult>,
        @Inject(MAT_DIALOG_DATA) public data: GroupConfigDialogData,
    ) {
        this.config = {...data.groupConfig};
    }

    ngOnInit(): void {
    }

    save() {
        this.dialogRef.close({
            groupConfig: this.config
        })
    }
}
