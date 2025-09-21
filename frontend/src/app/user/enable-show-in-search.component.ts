import {Component, OnInit} from '@angular/core';
import { MatDialogRef, MatDialogContent, MatDialogActions, MatDialogClose } from '@angular/material/dialog';
import { CdkScrollable } from '@angular/cdk/scrolling';
import { MatFormField } from '@angular/material/form-field';
import { MatSelect } from '@angular/material/select';
import { FormsModule } from '@angular/forms';
import { MatOption } from '@angular/material/autocomplete';
import { MatButton } from '@angular/material/button';

export class EnableShowInSearchResult {
    durationInSeconds: number
}

@Component({
    selector: 'app-enable-show-in-search',
    templateUrl: './enable-show-in-search.component.html',
    styleUrls: ['./enable-show-in-search.component.scss'],
    imports: [CdkScrollable, MatDialogContent, MatFormField, MatSelect, FormsModule, MatOption, MatDialogActions, MatButton, MatDialogClose]
})
export class EnableShowInSearchComponent implements OnInit {
    public duration = 600;

    constructor(
        private readonly dialogRef: MatDialogRef<EnableShowInSearchComponent, EnableShowInSearchResult>
    ) {
    }

    ngOnInit(): void {
    }

    confirm() {
        this.dialogRef.close({
            durationInSeconds: this.duration
        });
    }
}
