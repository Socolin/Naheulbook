import {Component, Inject, OnInit, ViewChild} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialogRef} from '@angular/material';
import {MatStep, MatStepper} from '@angular/material/stepper';

import {ActiveStatsModifier} from '../shared';

import {Fighter} from '../group';
import {Effect, EffectService} from '../effect';
import {MatSelectionList} from '@angular/material/list';

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
        private effectService: EffectService,
        public dialogRef: MatDialogRef<GroupAddEffectDialogComponent, GroupAddEffectDialogResult>,
        @Inject(MAT_DIALOG_DATA) public data?: GroupAddEffectDialogData
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
