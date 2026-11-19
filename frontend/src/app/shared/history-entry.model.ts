import {IconDescription} from './icon.model';
import {IActiveStatsModifier, IItemData} from '../api/shared';

export type HistoryEntry = GroupHistoryEntryResponse | CharacterHistoryEntryResponse;

/**
 * Copy of the item at the time of the log, stored in the `data` of the history entry
 */
export interface HistoryItemData {
    templateId: string;
    data: IItemData;
    modifiers?: IActiveStatsModifier[];
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
    action: 'ADD_ITEM' | 'MODIFY_EV' | 'MODIFY_EA' | 'USE_FATE_POINT' | 'ADD_XP' | 'CHANGE_SEX' | 'CHANGE_NAME' | 'EQUIP' | 'UNEQUIP' | 'APPLY_MODIFIER' | 'REMOVE_MODIFIER' | 'ACTIVE_MODIFIER' | 'DISABLE_MODIFIER' | 'GIVE_ITEM' | 'GIVEN_ITEM' | 'LOOT_ITEM' | 'CHANGE_QUANTITY' | 'USE_CHARGE' | 'READ_BOOK' | 'IDENTIFY' | 'LEVEL_UP' | 'ADD_APTITUDE' | 'REMOVE_APTITUDE' | 'ADD_SPECIAL_ATTACK' | 'REMOVE_SPECIAL_ATTACK' | 'USE_SPECIAL_ATTACK' | 'ADD_TALENT' | 'REMOVE_TALENT' | 'ADD_MASTERY' | 'REMOVE_MASTERY' | 'ADD_HANDICAP' | 'REMOVE_HANDICAP';
    date: any;
    gm: boolean;
    isGroup: false;
    data: {
        item?: HistoryItemData;
        modifier?: { name: string };
        effect?: { name: string };
        aptitude?: { id: string, name: string };
        specialAttack?: { id: string, name: string };
        talent?: { id: string, name: string };
        mastery?: { id: string, name: string };
        handicap?: { id: string, name: string };
        success?: boolean;
        characterName?: string;
        level?: number;
        oldValue?: any;
        newValue?: any;
        info?: string;
        [key: string]: any;
    };
}

function icon(name: string, color: string): IconDescription {
    return {name, color, rotation: 0};
}

const evIcon = icon('hearts', '#ff0d26');
const eaIcon = icon('abstract-061', '#8a2be2');
const defaultItemIcon = icon('knapsack', '#8d6e63');
const modifierIcon = icon('magic-swirl', '#7e57c2');
const effectIcon = icon('bubbling-flask', '#26a69a');

const groupHistoryIcons: Partial<Record<GroupHistoryEntryResponse['action'], IconDescription>> = {
    START_COMBAT: icon('crossed-swords', '#c62828'),
    END_COMBAT: icon('truce', '#2e7d32'),
    ADD_TIME: icon('hourglass', '#c9a227'),
    CHANGE_DATE: icon('calendar', '#5c6bc0'),
    EVENT_RP: icon('quill-ink', '#78909c'),
    MANKDEBOL: icon('rolling-dices', '#ef6c00'),
    DEBILIBEUK: icon('rolling-dices', '#ef6c00'),
};

const characterHistoryIcons: Partial<Record<string, IconDescription>> = {
    MODIFY_EV: evIcon,
    MODIFY_EA: eaIcon,
    ADD_XP: icon('trophy', '#d4a017'),
    LEVEL_UP: icon('upgrade', '#43a047'),
    USE_FATE_POINT: icon('clover', '#2e7d32'),
    CHANGE_NAME: icon('id-card', '#78909c'),
    CHANGE_SEX: icon('id-card', '#78909c'),
    APPLY_MODIFIER: modifierIcon,
    REMOVE_MODIFIER: modifierIcon,
    ACTIVE_MODIFIER: modifierIcon,
    DISABLE_MODIFIER: modifierIcon,
    APPLY_EFFECT: effectIcon,
    REMOVE_EFFECT: effectIcon,
    ACTIVE_EFFECT: effectIcon,
    DISABLE_EFFECT: effectIcon,
    ADD_APTITUDE: icon('skills', '#1e88e5'),
    REMOVE_APTITUDE: icon('skills', '#1e88e5'),
    ADD_SPECIAL_ATTACK: icon('sword-spin', '#e65100'),
    REMOVE_SPECIAL_ATTACK: icon('sword-spin', '#e65100'),
    USE_SPECIAL_ATTACK: icon('sword-spin', '#e65100'),
    ADD_TALENT: icon('star-medal', '#6a1b9a'),
    REMOVE_TALENT: icon('star-medal', '#6a1b9a'),
    ADD_MASTERY: icon('crossed-swords', '#2e7d32'),
    REMOVE_MASTERY: icon('crossed-swords', '#2e7d32'),
    ADD_HANDICAP: icon('broken-bone', '#c62828'),
    REMOVE_HANDICAP: icon('broken-bone', '#c62828'),
};

/**
 * Icon to display next to an history entry: the icon of the item the entry is about, or an icon for the action.
 */
export function getHistoryEntryIcon(entry: HistoryEntry): IconDescription | undefined {
    if (entry.isGroup) {
        return groupHistoryIcons[entry.action];
    }
    if (entry.data.item) {
        return entry.data.item.data?.icon ?? defaultItemIcon;
    }
    return characterHistoryIcons[entry.action];
}
