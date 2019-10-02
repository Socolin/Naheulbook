import {ActiveStatsModifier, DurationChange, IMetadata} from '../shared';
import {ItemTemplate} from '../item-template';
import {IconDescription} from '../shared/icon.model';
import {Skill} from '../skill';
import {Fighter} from '../group';
import {IDurable, IItemData} from '../api/shared';

export class ItemData implements IItemData {
    name: string;
    description?: string;
    quantity?: number;
    icon?: IconDescription;
    charge?: number;
    ug?: number;
    equiped?: number;
    readCount?: number;
    notIdentified?: boolean;
    ignoreRestrictions?: boolean;
    lifetime?: IDurable;
}

export class ItemComputedData {
    incompatible?: boolean;
}

export class Item {
    id: number;
    data: ItemData = new ItemData();
    modifiers: ActiveStatsModifier[];
    containerId?: number;
    template: ItemTemplate;
    computedData: ItemComputedData = new ItemComputedData();

    get price(): number | undefined {
        if (!this.template.data.price) {
            return undefined;
        }

        let priceFactor = 1;
        if (this.data.quantity) {
            priceFactor = this.data.quantity;
        } else if (this.template.data.useUG) {
            priceFactor = this.data.ug || 1;
        }

        return this.template.data.price * priceFactor;
    }

    // Generated field
    content: Item[];
    containerInfo?: IMetadata;

    static fromJson(jsonData: any, skillsById: {[skillId: number]: Skill}): Item {
        let item = new Item();
        Object.assign(item, jsonData, {
            template: ItemTemplate.fromResponse(jsonData.template, skillsById),
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

    updateTime(type: string, data: number | { previous: Fighter; next: Fighter }): DurationChange[] {
        let changes: DurationChange[] = [];
        for (let i = 0; i < this.modifiers.length; i++) {
            let modifier = this.modifiers[i];
            if (modifier.updateDuration(type, data)) {
                changes.push({
                    type: 'itemModifier',
                    itemId: this.id,
                    modifier: modifier
                });
            }
        }

        if (this.data.lifetime && this.data.lifetime.durationType === type) {
            switch (type) {
                case 'combat': {
                    if (this.data.lifetime.combatCount === undefined) {
                        console.error('Invalid duration', this.data);
                        break;
                    }
                    if (this.data.lifetime.combatCount > 0 && typeof(data) === 'number') {
                        this.data.lifetime.combatCount -= data;
                        changes.push({type: 'itemLifetime', itemId: this.id, lifetime: this.data.lifetime});
                    }
                    break;
                }
                case 'time': {
                    if (this.data.lifetime.timeDuration === undefined) {
                        console.error('Invalid duration', this.data);
                        break;
                    }
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

    public getDamageString(): string {
        let damage = '';
        if (this.template.data.damageDice) {
            damage += this.template.data.damageDice + 'D';
        }
        if (this.template.data.bonusDamage) {
            if (damage) {
                damage += '+';
            }
            damage += this.template.data.bonusDamage;
        }
        if (this.template.data.damageType) {
            damage += '(' + this.template.data.damageType + ')';
        }
        return damage;
    }
}

export class PartialItem {
    id: number;
    data: ItemData = new ItemData();
    modifiers: ActiveStatsModifier[];
    containerId: number;

    static fromJson(jsonData: any): PartialItem {
        let item = new PartialItem();
        Object.assign(item, jsonData, {modifiers: ActiveStatsModifier.modifiersFromJson(jsonData.modifiers)});
        return item;
    }
}
