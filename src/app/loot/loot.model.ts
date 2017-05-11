import {Item} from '../character/item.model';
import {Monster} from '../monster/monster.model';
import {CharacterResume} from '../character/character.model';

export class Loot {
    public id: number;
    public name: string;
    public visibleForPlayer: boolean;
    public monsters: Monster[];
    public items: Item[];
    public computedXp: number;
}

export class LootTookItemMsg {
    item: Item;
    leftItem: Item;
    character: CharacterResume;
    monster?: Monster;
    quantity?: number;
}
