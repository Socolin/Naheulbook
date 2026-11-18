export type HistoryEntry = GroupHistoryEntryResponse | CharacterHistoryEntryResponse;

/**
 * Snapshot of the item at the time of the log, stored in the `data` of the history entry
 */
export interface HistoryItemData {
    name?: string;
    quantity?: number;
    icon?: any;
}

export interface GroupHistoryEntryResponse {
    id: number;
    action: 'START_COMBAT' | 'END_COMBAT' | 'MANKDEBOL' | 'DEBILIBEUK' | 'CHANGE_DATE' | 'ADD_TIME' | 'EVENT_RP';
    date: any;
    gm: boolean;
    isGroup: true;
    data: {
        text?: string;
        info?: string;
        [key: string]: any;
    };
}

export interface CharacterHistoryEntryResponse {
    id: number;
    action: 'ADD_ITEM' | 'MODIFY_EV' | 'MODIFY_EA' | 'USE_FATE_POINT' | 'ADD_XP' | 'CHANGE_SEX' | 'CHANGE_NAME' | 'EQUIP' | 'UNEQUIP' | 'APPLY_MODIFIER' | 'REMOVE_MODIFIER' | 'ACTIVE_MODIFIER' | 'DISABLE_MODIFIER' | 'GIVE_ITEM' | 'GIVEN_ITEM' | 'LOOT_ITEM' | 'CHANGE_QUANTITY' | 'USE_CHARGE' | 'READ_BOOK' | 'IDENTIFY' | 'LEVEL_UP' | 'ADD_APTITUDE' | 'REMOVE_APTITUDE';
    date: any;
    gm: boolean;
    isGroup: false;
    data: {
        item?: HistoryItemData;
        modifier?: { name: string };
        effect?: { name: string };
        aptitude?: { id: string, name: string };
        characterName?: string;
        level?: number;
        oldValue?: any;
        newValue?: any;
        info?: string;
        [key: string]: any;
    };
}
