import {Component, Inject, ViewChild} from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogClose } from '@angular/material/dialog';
import { MatStep, MatStepper, MatStepLabel } from '@angular/material/stepper';

import {ActiveStatsModifier} from '../shared';

import {Fighter} from '../group';
import {Effect, EffectService} from '../effect';
import { MatSelectionList, MatActionList, MatListOption } from '@angular/material/list';
import { MatToolbar } from '@angular/material/toolbar';
import { MatIconButton, MatButton } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import { MatFormField } from '@angular/material/form-field';
import { MatInput } from '@angular/material/input';
import { MatRipple } from '@angular/material/core';
import { StatModifierEditorComponent } from '../effect/stats-modifier-editor.component';
import { FighterIconComponent } from './fighter-icon.component';

export interface GroupAddEffectDialogData {
    effect?: Effect,
    fighters: Fighter[]
}

export interface GroupAddEffectDialogResult {
    modifier: ActiveStatsModifier,
    fighters: Fighter[]
}

@Component({
    templateUrl: './group-add-effect-dialog.component.html',
    styleUrls: ['../shared/full-screen-dialog.scss', './group-add-effect-dialog.component.scss'],
    imports: [MatToolbar, MatIconButton, MatDialogClose, MatIcon, MatButton, MatStepper, MatStep, MatStepLabel, MatFormField, MatInput, MatActionList, MatRipple, StatModifierEditorComponent, MatSelectionList, MatListOption, FighterIconComponent]
})
export class GroupAddEffectDialogComponent {
    @ViewChild('stepper', {static: true})
    private stepper: MatStepper;
    @ViewChild('searchStep', {static: true})
    private searchStep: MatStep;
    @ViewChild('customizeStep', {static: true})
    private customizeStep: MatStep;
    @ViewChild('fighterSelection', {static: true})
    public fighterSelection: MatSelectionList;

    public searchCompleted: boolean;
    public fromEffect?: boolean;
    public newStatsModifier: ActiveStatsModifier = new ActiveStatsModifier();
    public filteredEffects?: Effect[];
    public selectedStep = 0;

    constructor(
        private readonly effectService: EffectService,
        public readonly dialogRef: MatDialogRef<GroupAddEffectDialogComponent, GroupAddEffectDialogResult>,
        @Inject(MAT_DIALOG_DATA) public data: GroupAddEffectDialogData,
    ) {
        if (data && data.effect) {
            this.newStatsModifier = ActiveStatsModifier.fromEffect(data.effect, {reusable: false});
            this.fromEffect = true;
            this.selectedStep = 1;
            this.searchCompleted = true;
        }
    }

    addNewModifier() {
        if (!this.newStatsModifier.name) {
            this.dialogRef.close(undefined);
        } else {
            this.dialogRef.close({
                modifier: this.newStatsModifier,
                fighters: this.fighterSelection.selectedOptions.selected.map(s => s.value)
            });
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

    completeCustomization() {
        this.customizeStep.completed = true;
        this.stepper.next();
    }
}
