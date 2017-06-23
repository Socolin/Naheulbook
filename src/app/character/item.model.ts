import {ItemTemplate} from '../item';
import {IMetadata} from '../shared/misc.model';
import {IconDescription} from '../shared/icon.model';
import {ActiveStatsModifier, LapCountDecrement, StatsModifier} from '../shared/stat-modifier.model';
import {IDurable} from '../date/durable.model';
import {Skill} from '../skill/skill.model';
import {Fighter} from '../group/group.model';

export class ItemData {
    name: string;
    description: string;
    quantity: number;
    icon: IconDescription;
    charge: number;
    ug: number;
    equiped: number;
    readCount: number;
    notIdentified: boolean;
    lifetime: IDurable;
}

export class Item {
    id: number;
    data: ItemData = new ItemData();
    modifiers: ActiveStatsModifier[];
    container: number;
    template: ItemTemplate;

    get price(): number|undefined {
        if (!this.template.data.price) {
            return undefined;
        }

        let quantity = 1;
        if (this.template.data.quantifiable || this.data.quantity) {
            quantity = +this.data.quantity;
        }
        if (this.template.data.useUG) {
            quantity = this.data.ug;
        }

        return this.template.data.price * quantity;
    }

    // Generated field
    content: Item[];
    containerInfo: IMetadata;

    static fromJson(jsonData: any, skillsById: {[skillId: number]: Skill}): Item {
        let item = new Item();
        Object.assign(item, jsonData, {
            template: ItemTemplate.fromJson(jsonData.template, skillsById),
            modifiers: ActiveStatsModifier.modifiersFromJson(jsonData.modifiers)
        });
        return item;
    }

    static itemsFromJson(itemsData: object[], skillsById: {[skillId: number]: Skill}): Item[] {
        let items: Item[] = [];
        for (let i = 0; i < itemsData.length; i++) {
            items.push(Item.fromJson(itemsData[i], skillsById));
        }
        return items;
    }

    updateTime(type: string, data: number | { previous: Fighter; next: Fighter }): any[] {
        let changes = [];
        for (let i = 0; i < this.modifiers.length; i++) {
            let modifier = this.modifiers[i];
            if (modifier.updateDuration(type, data)) {
                changes.push({type: 'itemModifier', itemId: this.id, modifierIdx: i, modifier: modifier});
            }
        }

        if (this.data.lifetime && this.data.lifetime.durationType === type) {
            switch (type) {
                case 'combat': {
                    if (this.data.lifetime.combatCount > 0 && typeof(data) === 'number') {
                        this.data.lifetime.combatCount -= data;
                        changes.push({type: 'itemLifetime', itemId: this.id, lifetime: this.data.lifetime});
                    }
                    break;
                }
                case 'time': {
                    if (this.data.lifetime.timeDuration > 0 && typeof(data) === 'number') {
                        this.data.lifetime.timeDuration -= data;
                        changes.push({type: 'itemLifetime', itemId: this.id, lifetime: this.data.lifetime});
                    }
                    break;
                }
            }
        }
        return changes;
    }
}

export class PartialItem {
    id: number;
    data: ItemData = new ItemData();
    modifiers: ActiveStatsModifier[];
    container: number;

    static fromJson(jsonData: any): PartialItem {
        let item = new PartialItem();
        Object.assign(item, jsonData, {modifiers: ActiveStatsModifier.modifiersFromJson(jsonData.modifiers)});
        return item;
    }
}
