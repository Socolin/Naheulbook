import {Component, Inject, OnInit, ViewChild} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialogRef, MatOption, MatSelect, MatSelectionList} from '@angular/material';
import {Origin, OriginService} from '../origin';
import {Job, JobService} from '../job';

import {statModifierSpecialsValues} from './stat-modifier-specials.constants';
import {Guid} from '../api/shared/util';

export interface StatModifierAdvancedDialogData {
    selectedJobId?: number;
    selectedOriginId?: Guid;
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
        private readonly originService: OriginService,
        private readonly jobService: JobService,
        private readonly dialogRef: MatDialogRef<StatModifierAdvancedDialogComponent>,
        @Inject(MAT_DIALOG_DATA) public readonly data: StatModifierAdvancedDialogData
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
