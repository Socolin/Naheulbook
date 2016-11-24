import {Item} from '../character/item.model';
import {Monster} from '../monster/monster.model';

export class Loot {
    public id: number;
    public name: string;
    public visibleForPlayer: boolean;
    public monsters: Monster[];
    public items: Item[];
    public computedXp: number;
}
