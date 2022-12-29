import {Component, Inject, OnInit} from '@angular/core';
import {IGroupConfig} from '../api/shared';
import {MAT_DIALOG_DATA, MatDialogRef} from '@angular/material/dialog';

export class GroupConfigDialogData {
    groupConfig: IGroupConfig;
}

export class GroupConfigDialogResult {
    groupConfig: IGroupConfig
}

@Component({
    selector: 'app-group-config-dialog',
    templateUrl: './group-config-dialog.component.html',
    styleUrls: ['./group-config-dialog.component.scss']
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
