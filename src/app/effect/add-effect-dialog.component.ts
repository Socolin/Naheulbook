import {Component, Inject, OnInit, ViewChild} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialogRef} from '@angular/material';

import {ActiveStatsModifier} from '../shared';

import {Effect} from './effect.model';
import {ActiveEffectEditorComponent} from './active-effect-editor.component';

export interface AddEffectDialogData {
    effect?: Effect
}

@Component({
    templateUrl: './add-effect-dialog.component.html',
    styleUrls: ['./add-effect-dialog.component.scss', '../shared/full-screen-dialog.scss'],
})
export class AddEffectDialogComponent implements OnInit {
    @ViewChild('activeEffectEditor', {static: true})
    public activeEffectEditor: ActiveEffectEditorComponent;
    public addEffectTypeSelectedTab = 0;

    public newStatsModifier: ActiveStatsModifier = new ActiveStatsModifier();

    constructor(
        public dialogRef: MatDialogRef<AddEffectDialogComponent, ActiveStatsModifier>,
        @Inject(MAT_DIALOG_DATA) public data: AddEffectDialogData
    ) {
        this.newStatsModifier = new ActiveStatsModifier();
    }

    addEffect(newEffect: { effect: Effect, data: any }) {
        let effect = newEffect.effect;
        let data = newEffect.data;

        this.dialogRef.close(ActiveStatsModifier.fromEffect(effect, data));
    }

    addCustomModifier() {
        this.dialogRef.close(this.newStatsModifier);
    }

    ngOnInit() {
        if (this.data && this.data.effect) {
            this.activeEffectEditor.selectEffect(this.data.effect);
        }
    }
}
