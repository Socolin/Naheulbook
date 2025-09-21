import {Component, OnInit} from '@angular/core';
import {MatDialogRef} from '@angular/material/dialog';

export class EnableShowInSearchResult {
    durationInSeconds: number
}

@Component({
    selector: 'app-enable-show-in-search',
    templateUrl: './enable-show-in-search.component.html',
    styleUrls: ['./enable-show-in-search.component.scss'],
    standalone: false
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
