export type NhbkActionType = 'addItem' | 'removeItem' | 'addEffect' | 'addCustomModifier' | 'addEv' | 'addEa' | 'custom';

export class NhbkAction {
    static VALID_ACTIONS: { type: NhbkActionType, displayName: string }[] = [
        {type: 'addItem', displayName: 'Ajouter un objet'},
        {type: 'removeItem', displayName: 'Retirer un objet'},
        {type: 'addEffect', displayName: 'Ajouter un effet'},
        {type: 'addCustomModifier', displayName: 'Ajouter un modificateur'},
        {type: 'addEv', displayName: 'Redonner E.Vitale'},
        {type: 'addEa', displayName: 'Redonner E.Astrale'},
        {type: 'custom', displayName: 'Custom'},
    ];

    public type: NhbkActionType;
    public hidden: boolean;
    public data: any;

    constructor(type: NhbkActionType) {
        this.type = type;
    }
}

