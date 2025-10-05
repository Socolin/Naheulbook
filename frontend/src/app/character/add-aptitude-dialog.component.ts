import {Component, Inject, OnInit} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialogClose, MatDialogRef} from '@angular/material/dialog';
import {MatButton, MatIconButton} from '@angular/material/button';
import {MatToolbar} from '@angular/material/toolbar';
import {MatIconModule} from '@angular/material/icon';
import {AptitudeService} from '../aptitude/aptitude.service';
import {AptitudeGroupResponse, AptitudeResponse} from '../api/responses';
import {MatProgressSpinnerModule} from '@angular/material/progress-spinner';
import {MatCardModule} from '@angular/material/card';
import {MatDivider} from '@angular/material/divider';

export type AddAptitudeDialogData = {
    aptitudeGroupId: string;
}

export type AddAptitudeDialogResult = {
    selectedAptitudeIds: string[];
}


type AptitudeTypeGrouping = { type: string, aptitudes: AptitudeResponse[] }

@Component({
    selector: 'app-add-aptitude-dialog',
    imports: [
        MatButton,
        MatDialogClose,
        MatIconModule,
        MatIconButton,
        MatToolbar,
        MatProgressSpinnerModule,
        MatCardModule,
        MatDivider,
    ],
    templateUrl: './add-aptitude-dialog.component.html',
    styleUrl: './add-aptitude-dialog.component.scss'
})
export class AddAptitudeDialogComponent implements OnInit {
    aptitudeGroup?: AptitudeGroupResponse;
    aptitudesGroupedByTypes: AptitudeTypeGrouping[] = [];
    sortedAptitudes: AptitudeResponse[] = [];
    selection: AptitudeResponse[] = [];

    constructor(
        private readonly dialogRef: MatDialogRef<AddAptitudeDialogComponent, AddAptitudeDialogResult>,
        private readonly aptitudeService: AptitudeService,
        @Inject(MAT_DIALOG_DATA) public readonly data: AddAptitudeDialogData,
    ) {
    }

    ngOnInit() {
        this.aptitudeService.getAptitudeGroup(this.data.aptitudeGroupId).subscribe((response) => {
            this.aptitudeGroup = response;
            this.sortedAptitudes = this.aptitudeGroup.aptitudes.sort((a, b) => a.roll - b.roll);
            let lastAptitude: AptitudeResponse | undefined = undefined;
            let aptitudesGroupedByTypes: AptitudeTypeGrouping[] = [];
            let currentGroup: AptitudeTypeGrouping = {type: '', aptitudes: []};
            for (let aptitude of this.sortedAptitudes) {
                if (aptitude.type !== lastAptitude?.type) {
                    currentGroup = {type: aptitude.type, aptitudes: []};
                    aptitudesGroupedByTypes.push(currentGroup);
                }
                currentGroup.aptitudes.push(aptitude);
                lastAptitude = aptitude;
            }
            this.aptitudesGroupedByTypes = aptitudesGroupedByTypes;
        })

    }

    roll() {
        const randomIndex = Math.floor(Math.random() * this.sortedAptitudes.length);
        const selectedAptitude = this.sortedAptitudes[randomIndex];
        if (!this.selection.some(a => a.id === selectedAptitude.id)) {
            this.selection.push(selectedAptitude);
        }
    }

    valid() {
        this.dialogRef.close({selectedAptitudeIds: this.selection.map(a => a.id)});
    }

    removeSelection(index: number) {
        this.selection.splice(index, 1);
    }

    addAptitude(aptitude: AptitudeResponse) {
        this.selection.push(aptitude);
    }
}


