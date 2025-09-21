import {Component, Inject, ViewChild} from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogClose } from '@angular/material/dialog';
import { MatStep, MatStepper, MatStepLabel } from '@angular/material/stepper';

import {ActiveStatsModifier} from '../shared';

import {Effect} from './effect.model';
import {EffectService} from './effect.service';
import { MatToolbar } from '@angular/material/toolbar';
import { MatIconButton, MatButton } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import { MatFormField } from '@angular/material/form-field';
import { MatInput } from '@angular/material/input';
import { MatActionList } from '@angular/material/list';
import { MatRipple } from '@angular/material/core';
import { StatModifierEditorComponent } from './stats-modifier-editor.component';

export interface AddEffectDialogData {
    effect?: Effect,
    options?: {
        hideReusable?: boolean
    }
}

@Component({
    templateUrl: './add-effect-dialog.component.html',
    styleUrls: ['./add-effect-dialog.component.scss', '../shared/full-screen-dialog.scss'],
    imports: [MatToolbar, MatIconButton, MatDialogClose, MatIcon, MatButton, MatStepper, MatStep, MatStepLabel, MatFormField, MatInput, MatActionList, MatRipple, StatModifierEditorComponent]
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
