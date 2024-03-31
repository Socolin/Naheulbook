import {Component, Inject, OnInit, Optional} from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import {HttpErrorResponse} from '@angular/common/http';

@Component({
    selector: 'app-error-details-dialog',
    templateUrl: './error-details-dialog.component.html',
    styleUrls: ['./error-details-dialog.component.scss']
})
export class ErrorDetailsDialogComponent implements OnInit {
    public readonly errorType: 'ServerError' | 'BadGateway' | 'Forbidden' | 'BadRequest' | undefined;
    public showMoreInfo = false;

    constructor(
        @Optional() @Inject(MAT_DIALOG_DATA) public data: Error | HttpErrorResponse | any | null
    ) {
        if (this.data instanceof HttpErrorResponse) {
            if (this.data.status === 502) {
                this.errorType = 'BadGateway';
            } else if (this.data.status === 403) {
                this.errorType = 'Forbidden';
            } else if (this.data.status >= 500 && this.data.status < 600) {
                this.errorType = 'ServerError';
            } else if (this.data.status >= 400 && this.data.status < 500) {
                this.errorType = 'BadRequest';
            }
        }
    }

    dataIsNotAndError() {
        return !(this.data instanceof Error);
    }

    ngOnInit() {
    }
}
