import {ActiveStatsModifier, DurationChange, IMetadata} from '../shared';
import {ItemTemplate} from '../item-template';
import {IconDescription} from '../shared/icon.model';
import {SkillDictionary} from '../skill';
import {Fighter} from '../group';
import {IDurable, IItemData} from '../api/shared';
import {ItemResponse} from '../api/responses';

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
    shownToGm?: boolean;

    constructor(data?: IItemData) {
        if (data) {
            Object.assign(this, data);
        }
    }
}

export class ItemComputedData {
    incompatible?: boolean;
    modifierBonusDamage?: number;
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

    get shouldBePutInAContainer(): boolean {
        if (this.data.equiped) {
            return false
        }
        if (this.containerId) {
            return false;
        }
        if (this.template.data.container) {
            return false;
        }
        return true;
    }

    // Generated field
    content?: Item[];
    containerInfo?: IMetadata;

    static fromResponse(response: ItemResponse, skillsById: SkillDictionary): Item {
        let item = new Item();
        Object.assign(item, response, {
            template: ItemTemplate.fromResponse(response.template, skillsById),
            modifiers: ActiveStatsModifier.modifiersFromJson(response.modifiers)
        });
        return item;
    }

    static itemsFromJson(responses: ItemResponse[], skillsById: SkillDictionary): Item[] {
        return responses.map(response => Item.fromResponse(response, skillsById));
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
