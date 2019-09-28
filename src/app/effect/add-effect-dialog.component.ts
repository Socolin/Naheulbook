import {Component, Inject, ViewChild} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialogRef} from '@angular/material';
import {MatStep, MatStepper} from '@angular/material/stepper';

import {ActiveStatsModifier} from '../shared';

import {Effect} from './effect.model';
import {EffectService} from './effect.service';

export interface AddEffectDialogData {
    effect?: Effect,
    options?: {
        hideReusable?: boolean
    }
}

@Component({
    templateUrl: './add-effect-dialog.component.html',
    styleUrls: ['./add-effect-dialog.component.scss', '../shared/full-screen-dialog.scss'],
})
export class AddEffectDialogComponent {
    @ViewChild('stepper', {static: true})
    private stepper: MatStepper;
    @ViewChild('searchStep', {static: true})
    private searchStep: MatStep;

    public fromEffect?: boolean;
    public newStatsModifier: ActiveStatsModifier = new ActiveStatsModifier();
    public filteredEffects?: Effect[];
    public selectedStep = 0;

    constructor(
        private readonly effectService: EffectService,
        private readonly dialogRef: MatDialogRef<AddEffectDialogComponent, ActiveStatsModifier>,
        @Inject(MAT_DIALOG_DATA) public readonly data?: AddEffectDialogData
    ) {
        if (data && data.effect) {
            this.newStatsModifier = ActiveStatsModifier.fromEffect(data.effect, {reusable: false});
            this.fromEffect = true;
            this.selectedStep = 1;
        }
    }

    addNewModifier() {
        if (!this.newStatsModifier.name) {
            this.dialogRef.close(undefined);
        } else {
            this.dialogRef.close(this.newStatsModifier);
        }
    }

    updateFilter(filter: string) {
        this.effectService.searchEffect(filter).subscribe(effects => this.filteredEffects = effects);
    }

    selectEffect(effect: Effect) {
        this.newStatsModifier = ActiveStatsModifier.fromEffect(effect, {reusable: false});
        this.fromEffect = true;
        this.searchStep.completed = true;
        this.stepper.next();
    }

    customize() {
        this.newStatsModifier = new ActiveStatsModifier();
        this.fromEffect = false;
        this.searchStep.completed = true;
        this.stepper.next();
    }
}
