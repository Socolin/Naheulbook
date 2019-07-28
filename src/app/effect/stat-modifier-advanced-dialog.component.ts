import {Component, Inject, OnInit, ViewChild} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialogRef, MatOption, MatSelect, MatSelectionList} from '@angular/material';
import {Origin, OriginService} from '../origin';
import {Job, JobService} from '../job';

import {statModifierSpecialsValues} from './stat-modifier-specials.constants';

export interface StatModifierAdvancedDialogData {
    selectedJobId?: number;
    selectedOriginId?: number;
    specials?: string[];
}

@Component({
    selector: 'app-stat-modifier-advanced-dialog',
    templateUrl: './stat-modifier-advanced-dialog.component.html',
    styleUrls: ['./stat-modifier-advanced-dialog.component.scss']
})
export class StatModifierAdvancedDialogComponent implements OnInit {
    public origins: Origin[];
    public jobs: Job[];
    public specialValues: any[] = statModifierSpecialsValues;

    @ViewChild(MatSelectionList, {static: true})
    public specialSelection: MatSelectionList;

    @ViewChild('jobSelector', {static: true})
    public jobSelector: MatSelect;

    @ViewChild('originSelector', {static: true})
    public originSelector: MatSelect;


    constructor(
        private originService: OriginService,
        private jobService: JobService,
        public dialogRef: MatDialogRef<StatModifierAdvancedDialogComponent>,
        @Inject(MAT_DIALOG_DATA) public data: StatModifierAdvancedDialogData
    ) {
    }

    ngOnInit() {
        this.jobService.getJobList().subscribe(jobs => {
            this.jobs = jobs;
        });
        this.originService.getOriginList().subscribe(origins => {
            this.origins = origins;
        });
    }

    saveChanges() {
        const originId = this.originSelector.selected ? (this.originSelector.selected as MatOption).value : this.originSelector.value;
        const jobId = this.jobSelector.selected ? (this.jobSelector.selected as MatOption).value : this.jobSelector.value;
        this.dialogRef.close({
            specials: this.specialSelection.selectedOptions.selected.map(s => s.value),
            selectedOriginId: originId,
            selectedJobId: jobId,
        } as StatModifierAdvancedDialogData);
    }
}
