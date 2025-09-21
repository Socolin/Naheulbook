import {Component, Inject, OnInit, ViewChild} from '@angular/core';
import {MatOption} from '@angular/material/core';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogTitle, MatDialogContent, MatDialogActions, MatDialogClose } from '@angular/material/dialog';
import { MatSelectionList, MatListOption } from '@angular/material/list';
import {MatSelect} from '@angular/material/select';
import {Origin, OriginService} from '../origin';
import {Job, JobService} from '../job';

import {statModifierSpecialsValues} from './stat-modifier-specials.constants';
import {Guid} from '../api/shared/util';
import { CdkScrollable } from '@angular/cdk/scrolling';
import { MatFormField } from '@angular/material/form-field';
import { FormsModule } from '@angular/forms';
import { MatOption as MatOption_1 } from '@angular/material/autocomplete';
import { MatButton } from '@angular/material/button';

export interface StatModifierAdvancedDialogData {
    selectedJobId?: Guid;
    selectedOriginId?: Guid;
    specials?: string[];
}

@Component({
    selector: 'app-stat-modifier-advanced-dialog',
    templateUrl: './stat-modifier-advanced-dialog.component.html',
    styleUrls: ['./stat-modifier-advanced-dialog.component.scss'],
    imports: [MatDialogTitle, CdkScrollable, MatDialogContent, MatFormField, MatSelect, FormsModule, MatOption_1, MatSelectionList, MatListOption, MatDialogActions, MatButton, MatDialogClose]
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
