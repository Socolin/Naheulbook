import {Component, Input, OnInit} from '@angular/core';
import {MatDialog} from '@angular/material/dialog';

import {ItemStatModifier} from '../shared';
import {Origin, OriginService} from '../origin';
import {Job, JobService} from '../job';


import {statModifierSpecialsValues} from './stat-modifier-specials.constants';
import {StatModifierAdvancedDialogComponent, StatModifierAdvancedDialogData} from './stat-modifier-advanced-dialog.component';
import {AddStatModifierDialogComponent} from './add-stat-modifier-dialog.component';

@Component({
    selector: 'modifiers-editor',
    styleUrls: ['./modifiers-editor.component.scss'],
    templateUrl: './modifiers-editor.component.html'
})
export class ModifiersEditorComponent implements OnInit {
    @Input() modifiers: ItemStatModifier[] = [];
    @Input() advancedModifiers = false;
    public specialValueDescriptionByValues: {[name: string]: string} = {};

    public originsByIds: { [originId: string]: Origin };
    public jobsByIds: { [jobId: string]: Job };

    constructor(private originService: OriginService
        , private jobService: JobService
        , private dialog: MatDialog
    ) {
    }

    removeModifier(i: number) {
        this.modifiers.splice(i, 1);
    }

    openAdvanceStatModifierDialog(modifier: ItemStatModifier) {
        if (!this.advancedModifiers) {
            return;
        }
        // tslint:disable-next-line:max-line-length
        const dialogRef = this.dialog.open<StatModifierAdvancedDialogComponent, StatModifierAdvancedDialogData, StatModifierAdvancedDialogData>(
            StatModifierAdvancedDialogComponent,
            {
                data: {
                    selectedJobId: modifier.jobId,
                    selectedOriginId: modifier.originId,
                    specials: modifier.special
                }
            }
        );

        dialogRef.afterClosed().subscribe(result => {
            if (!result) {
                return;
            }
            modifier.originId = result.selectedOriginId;
            modifier.jobId = result.selectedJobId;
            modifier.special = result.specials;
        });
    }

    ngOnInit() {
        for (let special of statModifierSpecialsValues) {
            this.specialValueDescriptionByValues[special.value] = special.description;
        }
        this.originService.getOriginsById().subscribe(originsById => {
            this.originsByIds = originsById;
        });
        this.jobService.getJobsById().subscribe(jobsByIds => {
            this.jobsByIds = jobsByIds;
        });
    }

    openAddStatModifierDialog() {
        const dialogRef = this.dialog.open<AddStatModifierDialogComponent, any, ItemStatModifier>(AddStatModifierDialogComponent);
        dialogRef.afterClosed().subscribe(modifier => {
            if (!modifier) {
                return;
            }
            this.modifiers.push(modifier);
        });
    }
}
