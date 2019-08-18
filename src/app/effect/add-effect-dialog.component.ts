import {Component, Inject, ViewChild} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialogRef} from '@angular/material';

import {ActiveStatsModifier} from '../shared';

import {Effect} from './effect.model';
import {EffectService} from './effect.service';
import {MatStepper} from '@angular/material/stepper';

export interface AddEffectDialogData {
    effect?: Effect
}

@Component({
    templateUrl: './add-effect-dialog.component.html',
    styleUrls: ['./add-effect-dialog.component.scss', '../shared/full-screen-dialog.scss'],
})
export class AddEffectDialogComponent {
    @ViewChild('stepper', {static: false})
    private stepper: MatStepper;

    public fromEffect = false;
    public newStatsModifier: ActiveStatsModifier = new ActiveStatsModifier();
    public filteredEffects?: Effect[];
    public selectedStep = 0;

    constructor(
        private effectService: EffectService,
        public dialogRef: MatDialogRef<AddEffectDialogComponent, ActiveStatsModifier>,
        @Inject(MAT_DIALOG_DATA) public data?: AddEffectDialogData
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
        }
        this.dialogRef.close(this.newStatsModifier);
    }

    updateFilter(filter: string) {
        this.effectService.searchEffect(filter).subscribe(effects => this.filteredEffects = effects);
    }

    selectEffect(effect: Effect) {
        this.newStatsModifier = ActiveStatsModifier.fromEffect(effect, {reusable: false});
        this.fromEffect = true;
        this.stepper.next();
    }

    customize() {
        this.newStatsModifier = new ActiveStatsModifier();
        this.fromEffect = false;
    }
}
