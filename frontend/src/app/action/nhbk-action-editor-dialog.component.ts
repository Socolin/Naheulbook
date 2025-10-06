import {map} from 'rxjs/operators';
import {Component, Inject} from '@angular/core';
import {
    NhbkAction,
    NhbkActionFactory,
    NhbkActionType,
    NhbkAddEaAction,
    NhbkAddEvAction,
    NhbkAddItemAction,
    NhbkCustomAction
} from './nhbk-action.model';
import {Observable} from 'rxjs';

import {ActiveStatsModifier, AutocompleteInputComponent, AutocompleteValue} from '../shared';

import {ItemTemplate, ItemTemplateService} from '../item-template';
import {MAT_DIALOG_DATA, MatDialogClose, MatDialogContent, MatDialogRef, MatDialogTitle} from '@angular/material/dialog';
import {ActiveEffectEditorComponent, Effect, StatModifierEditorComponent} from '../effect';
import {MatFormField, MatPrefix, MatSuffix} from '@angular/material/form-field';
import {MatSelect} from '@angular/material/select';
import {FormsModule} from '@angular/forms';
import {MatOption} from '@angular/material/autocomplete';
import {MatCheckbox} from '@angular/material/checkbox';
import {MatInput} from '@angular/material/input';
import {MatCardActions} from '@angular/material/card';
import {MatButton} from '@angular/material/button';

export interface NhbkActionEditorDialogData {
    action?: NhbkAction
}

@Component({
    styleUrls: ['./nhbk-action-editor-dialog.component.scss'],
    templateUrl: './nhbk-action-editor-dialog.component.html',
    imports: [MatDialogTitle, MatDialogContent, MatFormField, MatSelect, FormsModule, MatOption, MatCheckbox, AutocompleteInputComponent, MatInput, MatPrefix, MatSuffix, StatModifierEditorComponent, ActiveEffectEditorComponent, MatCardActions, MatButton, MatDialogClose]
})
export class NhbkActionEditorDialogComponent {
    action: NhbkAction = NhbkActionFactory.createFromType(NhbkActionType.addEv, {ev: 1});

    public activeModifier: ActiveStatsModifier = new ActiveStatsModifier();
    public actionTypeDefinitions: { type: NhbkActionType, displayName: string }[] = [
        {type: NhbkActionType.addItem, displayName: 'Ajouter un objet'},
        {type: NhbkActionType.removeItem, displayName: 'Retirer un objet'},
        {type: NhbkActionType.addEffect, displayName: 'Ajouter un effet'},
        {type: NhbkActionType.addCustomModifier, displayName: 'Ajouter un modificateur'},
        {type: NhbkActionType.addEv, displayName: 'Redonner E.Vitale'},
        {type: NhbkActionType.addEa, displayName: 'Redonner E.Astrale'},
        {type: NhbkActionType.custom, displayName: 'Custom'},
    ];

    public autocompleteItemCallback: (filter: string) => Observable<AutocompleteValue[]> = this.updateAutocompleteItem.bind(this);
    public selectedItemTemplate: ItemTemplate;

    constructor(
        private readonly itemTemplateService: ItemTemplateService,
        private readonly dialogRef: MatDialogRef<NhbkActionEditorDialogComponent, NhbkAction>,
        @Inject(MAT_DIALOG_DATA) public data: NhbkActionEditorDialogData
    ) {
    }

    public asNhbkAddItemAction(action: NhbkAction): NhbkAddItemAction { return action as NhbkAddItemAction; }
    public asNhbkAddEvAction(action: NhbkAction): NhbkAddEvAction { return action as NhbkAddEvAction; }
    public asNhbkAddEaAction(action: NhbkAction): NhbkAddEaAction { return action as NhbkAddEaAction; }
    public asNhbkCustomAction(action: NhbkAction): NhbkCustomAction { return action as NhbkCustomAction; }

    selectItemTemplate(itemTemplate: ItemTemplate) {
        if (this.action.type !== NhbkActionType.addItem) {
            throw `Invalid action type: ${this.action.type}`;
        }

        this.action.data.templateId = itemTemplate.id;
        this.action.data.itemName = itemTemplate.name;
        this.action.data.quantity = itemTemplate.data.quantifiable ? 1 : undefined;
        this.selectedItemTemplate = itemTemplate;
    }

    updateEffect(newEffect: {effect: Effect, data: any}) {
        if (this.action.type !== NhbkActionType.addEffect) {
            throw `Invalid action type: ${this.action.type}`;
        }

        if (!newEffect.effect) {
            return;
        }

        this.action.data.effectId = newEffect.effect.id;
        this.action.data.effectData = newEffect.data;
    }

    updateAutocompleteItem(filter: string): Observable<AutocompleteValue[]> {
        return this.itemTemplateService.searchItem(filter).pipe(map(
            list => list.map(e => new AutocompleteValue(e, e.name))
        ));
    }

    validate() {
        if (!this.isReady()) {
            return;
        }

        if (this.action.type === NhbkActionType.addCustomModifier) {
            this.action.data.modifier = this.activeModifier;
        }

        this.dialogRef.close(this.action);
    }

    isReady(): boolean {
        if (this.action.type === NhbkActionType.addItem) {
            return !!this.action.data.templateId;
        } else if (this.action.type === NhbkActionType.addEffect) {
            return this.action.data.effectId > 0;
        }

        return true;
    }

    selectActionType(actionType: NhbkActionType) {
        switch (actionType) {
            case NhbkActionType.addItem:
                this.action = NhbkActionFactory.createFromType(NhbkActionType.addItem, {templateId: ''});
                break;
            case NhbkActionType.removeItem:
                this.action = NhbkActionFactory.createFromType(NhbkActionType.removeItem);
                break;
            case NhbkActionType.addEffect:
                this.action = NhbkActionFactory.createFromType(NhbkActionType.addEffect, {effectId: 0, effectData: {}});
                break;
            case NhbkActionType.addCustomModifier:
                this.action = NhbkActionFactory.createFromType(NhbkActionType.addCustomModifier, {modifier: new ActiveStatsModifier()});
                break;
            case NhbkActionType.addEv:
                this.action = NhbkActionFactory.createFromType(NhbkActionType.addEv, {ev: 1});
                break;
            case NhbkActionType.addEa:
                this.action = NhbkActionFactory.createFromType(NhbkActionType.addEa, {ea: 1});
                break;
            case NhbkActionType.custom:
                this.action = NhbkActionFactory.createFromType(NhbkActionType.custom, {text: ''});
                break;
        }
    }
}
