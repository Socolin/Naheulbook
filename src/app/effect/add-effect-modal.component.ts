import {Component, ViewChild, EventEmitter, Output} from '@angular/core';
import {OverlayRef} from '@angular/cdk/overlay';
import {Portal} from '@angular/cdk/portal';

import {Effect} from './effect.model';
import {ActiveStatsModifier} from '../shared/stat-modifier.model';
import {NhbkDialogService} from '../shared/nhbk-dialog.service';
import {ActiveEffectEditorComponent} from './active-effect-editor.component';

@Component({
    selector: 'add-effect-modal',
    templateUrl: './add-effect-modal.component.html',
    styleUrls: ['./add-effect-modal.component.scss'],
})
export class AddEffectModalComponent {
    @Output() createModifier: EventEmitter<ActiveStatsModifier> = new EventEmitter<ActiveStatsModifier>();

    @ViewChild('activeEffectEditor', {static: false})
    public activeEffectEditor: ActiveEffectEditorComponent;

    @ViewChild('addEffectDialog', {static: false})
    public addEffectDialog: Portal<any>;
    public addEffectOverlayRef: OverlayRef;
    public addEffectTypeSelectedTab = 0;

    public newStatsModifier: ActiveStatsModifier = new ActiveStatsModifier();

    constructor(private _nhbkDialogService: NhbkDialogService) {
    }

    open() {
        this.newStatsModifier = new ActiveStatsModifier();
        this.addEffectOverlayRef = this._nhbkDialogService.openCenteredBackdropDialog(this.addEffectDialog);
    }

    openEffect(effect: Effect) {
        this.open();
        // activeEffectEditor is undefined at this time because it just appear, so delay action on it
        setTimeout(() => {
            this.activeEffectEditor.selectEffect(effect);
        }, 0);
    }

    close() {
        this.addEffectOverlayRef.detach();
    }

    addEffect(newEffect: {effect: Effect, data: any}) {
        let effect = newEffect.effect;
        let data = newEffect.data;

        this.createModifier.emit(ActiveStatsModifier.fromEffect(effect, data));
        this.close();
    }

    addCustomModifier() {
        this.createModifier.emit(this.newStatsModifier);
        this.close();
    }
}
