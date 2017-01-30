import {Component, OnInit, Output, EventEmitter, Input, ViewChild, ViewChildren} from '@angular/core';
import {NhbkAction, NhbkActionType} from './nhbk-action.model';
import {Observable} from 'rxjs';
import {ItemService} from '../item/item.service';
import {ItemTemplate} from '../item/item-template.model';
import {AutocompleteValue} from '../shared/autocomplete-input.component';
import {CharacterEffectEditorComponent} from '../effect/character-effect-editor.component';
import {CharacterModifier} from '../character/character.model';

@Component({
    selector: 'nhbk-action-editor',
    styleUrls: ['./nhbk-action-editor.component.scss'],
    templateUrl: './nhbk-action-editor.component.html'
})
export class NhbkActionEditorComponent implements OnInit {
    @Input() action: NhbkAction = new NhbkAction(NhbkAction.VALID_ACTIONS[0].type);
    @Output() onValidate: EventEmitter<NhbkAction> = new EventEmitter<NhbkAction>();
    @Output() onClose: EventEmitter<any> = new EventEmitter<any>();

    public characterModifier: CharacterModifier = new CharacterModifier();
    public validActions: { type: NhbkActionType, displayName: string }[] = NhbkAction.VALID_ACTIONS;

    public autocompleteItemCallback: Observable<AutocompleteValue[]> = this.updateAutocompleteItem.bind(this);
    public selectedItemTemplate: ItemTemplate;

    constructor(private _itemService: ItemService) {
    }

    selectItemTemplate(itemTemplate: ItemTemplate) {
        this.action.data.templateId = itemTemplate.id;
        this.action.data.itemName = itemTemplate.name;
        this.action.data.quantity = itemTemplate.data.quantifiable ? 1 : null;
        this.selectedItemTemplate = itemTemplate;
    }

    updateEffect(newEffect: any) {
        let effect = newEffect.effect;
        let data = newEffect.data;

        if (!effect) {
            return;
        }

        this.action.data.effectId = effect.id;
        this.action.data.effectData = data;
    }

    updateAutocompleteItem(filter: string) {
        return this._itemService.searchItem(filter).map(
            list => list.map(e => new AutocompleteValue(e, e.name))
        );
    }

    validate() {
        if (this.isReady()) {
            if (this.action.type === 'addCustomModifier') {
                this.action.data.modifier = this.characterModifier;
            }
            this.onValidate.emit(this.action);
        }
    }

    close() {
        this.onClose.emit(true);
    }

    isReady() {
        if (this.action.type === 'addItem') {
            return this.action.data.templateId > 0;
        }
        else if (this.action.type === 'addEffect') {
            return this.action.data.effectId > 0;
        }

        return true;
    }

    updateActionType() {
        this.action.data = {};
        if (this.action.type === 'addEv') {
            this.action.data.ev = 1;
        }
        else if (this.action.type === 'addEa') {
            this.action.data.ea = 1;
        }
        else if (this.action.type === 'removeItem') {
        }
        else if (this.action.type === 'addItem') {
            this.action.data.templateId = 0;
            this.action.data.itemName = '';
        }
        else if (this.action.type === 'addCustomModifier') {
            this.action.data.modifier = {};
        }
        else if (this.action.type === 'addEffect') {
            this.action.data.effectId = 0;
            this.action.data.effectData = {};
        }
        else if (this.action.type === 'custom') {
            this.action.data.text = '';
        }
    }

    ngOnInit(): void {
        if (!this.action.data) {
            this.action.data = {};
        }
    }
}
