import {ActiveStatsModifier} from '../shared';
import {assertNever} from '../utils/utils';
import {Guid} from '../api/shared/util';

export enum NhbkActionType {
    addItem = 'addItem',
    removeItem = 'removeItem',
    addEffect = 'addEffect',
    addCustomModifier = 'addCustomModifier',
    addEv = 'addEv',
    addEa = 'addEa',
    custom = 'custom'
}

export type NhbkAction = NhbkAddItemAction
    | NhbkRemoveItemAction
    | NhbkAddEffectAction
    | NhbkAddCustomModifierAction
    | NhbkAddEvAction
    | NhbkAddEaAction
    | NhbkCustomAction;

export interface INhbkAction {
    type: NhbkActionType;
    hidden: boolean;
    data: unknown;
}

export type NhbkAddItemActionData = {
    templateId: Guid;
    itemName?: string;
    quantity?: number;
};

export class NhbkAddItemAction implements INhbkAction {
    type: NhbkActionType.addItem = NhbkActionType.addItem;
    hidden = false;
    data: NhbkAddItemActionData;

    constructor(data: NhbkAddItemActionData) {
        this.data = data;
    }
}

export class NhbkRemoveItemAction implements INhbkAction {
    type: NhbkActionType.removeItem = NhbkActionType.removeItem;
    hidden = false;
    data: never;
}

export type NhbkAddEffectActionData = {
    effectId: number;
    effectData: any;
}

export class NhbkAddEffectAction implements INhbkAction {
    type: NhbkActionType.addEffect = NhbkActionType.addEffect;
    hidden = false;
    data: NhbkAddEffectActionData;

    constructor(data: NhbkAddEffectActionData) {
        this.data = data;
    }
}

export type NhbkAddCustomModifierActionData = { modifier: ActiveStatsModifier };

export class NhbkAddCustomModifierAction implements INhbkAction {
    type: NhbkActionType.addCustomModifier = NhbkActionType.addCustomModifier;
    hidden = false;
    data: NhbkAddCustomModifierActionData;

    constructor(data: NhbkAddCustomModifierActionData) {
        this.data = data;
    }
}

export type NhbkAddEvActionData = {
    ev: number;
}

export class NhbkAddEvAction implements INhbkAction {
    type: NhbkActionType.addEv = NhbkActionType.addEv;
    hidden = false;
    data: NhbkAddEvActionData;

    constructor(data: NhbkAddEvActionData) {
        this.data = data;
    }
}

export type NhbkAddEaActionData = { ea: number }

export class NhbkAddEaAction implements INhbkAction {
    type: NhbkActionType.addEa = NhbkActionType.addEa;
    hidden = false;
    data: NhbkAddEaActionData;

    constructor(data: NhbkAddEaActionData) {
        this.data = data;
    }
}

export type NhbkCustomActionData = {
    text: string;
}

export class NhbkCustomAction implements INhbkAction {
    type: NhbkActionType.custom = NhbkActionType.custom;
    hidden = false;
    data: NhbkCustomActionData;

    constructor(data: NhbkCustomActionData) {
        this.data = data;
    }
}

export class NhbkActionFactory {
    static createFromType(type: NhbkActionType.addItem, data: NhbkAddItemActionData): NhbkAddEvAction;
    static createFromType(type: NhbkActionType.removeItem): NhbkRemoveItemAction;
    static createFromType(type: NhbkActionType.addEffect, data: NhbkAddEffectActionData): NhbkAddEffectAction;
    static createFromType(type: NhbkActionType.addCustomModifier, data: NhbkAddCustomModifierActionData): NhbkAddCustomModifierAction;
    static createFromType(type: NhbkActionType.addEv, data: NhbkAddEvActionData): NhbkAddEvAction;
    static createFromType(type: NhbkActionType.addEa, data: NhbkAddEaActionData): NhbkAddEaAction;
    static createFromType(type: NhbkActionType.custom, data: NhbkCustomActionData): NhbkCustomAction;
    static createFromType(type: NhbkActionType, data?: any): NhbkAction {
        switch (type) {
            case NhbkActionType.addItem:
                return new NhbkAddItemAction(data);
            case NhbkActionType.removeItem:
                return new NhbkRemoveItemAction();
            case NhbkActionType.addEffect:
                return new NhbkAddEffectAction(data);
            case NhbkActionType.addCustomModifier:
                return new NhbkAddCustomModifierAction(data);
            case NhbkActionType.addEv:
                return new NhbkAddEvAction(data);
            case NhbkActionType.addEa:
                return new NhbkAddEaAction(data);
            case NhbkActionType.custom:
                return new NhbkCustomAction(data);
            default:
                assertNever(type);
                // FIXME: Remove that when `asserts` is available
                throw new Error('Waiting for asserts');
        }
    }
}
