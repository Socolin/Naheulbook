export type HistoryEntry = GroupHistoryEntryResponse | CharacterHistoryEntryResponse;

export interface GroupHistoryEntryResponse {
    id: number;
    action: 'START_COMBAT' | 'END_COMBAT' | 'MANKDEBOL' | 'DEBILIBEUK' | 'CHANGE_DATE' | 'ADD_TIME' | 'EVENT_RP';
    date: any;
    info?: string;
    data: any;
}

export interface CharacterHistoryEntryResponse {
    id: number;
    action: 'ADD_ITEM' | 'MODIFY_EV' | 'MODIFY_EA' | 'USE_FATE_POINT' | 'ADD_XP' | 'CHANGE_SEX' | 'CHANGE_NAME' | 'EQUIP' | 'UNEQUIP' | 'APPLY_MODIFIER' | 'REMOVE_MODIFIER' | 'ACTIVE_MODIFIER' | 'DISABLE_MODIFIER' | 'GIVE_ITEM' | 'GIVEN_ITEM' | 'LOOT_ITEM' | 'CHANGE_QUANTITY' | 'USE_CHARGE' | 'READ_BOOK' | 'IDENTIFY' | 'LEVEL_UP';
    date: any;

    info?: string;
    data: any;
    item?: { name?: string };
    modifier?: { name: string };
    effect?: { name: string };
}

