import {ChangeDetectionStrategy, Component, Inject} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialogActions, MatDialogClose, MatDialogContent, MatDialogTitle} from '@angular/material/dialog';
import {MatButton} from '@angular/material/button';
import {UserAdminResponse} from '../api/responses';

export interface DeleteUserDialogData {
    user: UserAdminResponse;
}

@Component({
    templateUrl: './delete-user-dialog.component.html',
    styleUrls: ['./delete-user-dialog.component.scss'],
    changeDetection: ChangeDetectionStrategy.Eager,
    imports: [MatDialogTitle, MatDialogContent, MatDialogActions, MatButton, MatDialogClose]
})
export class DeleteUserDialogComponent {
    constructor(
        @Inject(MAT_DIALOG_DATA) public readonly data: DeleteUserDialogData
    ) {
    }
}
