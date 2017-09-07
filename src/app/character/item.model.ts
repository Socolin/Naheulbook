import {ItemTemplate} from '../item';
import {IMetadata} from '../shared/misc.model';
import {IconDescription} from '../shared/icon.model';
import {ActiveStatsModifier} from '../shared/stat-modifier.model';
import {IDurable} from '../date/durable.model';
import {Skill} from '../skill/skill.model';
import {Fighter} from '../group/group.model';
import {Character} from './character.model';

export class ItemData {
    name: string;
    description?: string;
    quantity?: number;
    icon: IconDescription;
    charge?: number;
    ug: number;
    equiped: number;
    readCount: number;
    notIdentified: boolean;
    lifetime: IDurable | null;
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
        if (this.data.quantity) {
            quantity = this.data.quantity;
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
        let changes: any[] = [];
        for (let i = 0; i < this.modifiers.length; i++) {
            let modifier = this.modifiers[i];
            if (modifier.updateDuration(type, data)) {
                changes.push({type: 'itemModifier', itemId: this.id, modifierIdx: i, modifier: modifier});
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

    public incompatibleWith(character: Character): {reason: string, source?: {type: string, name: string}} | undefined {
        if (this.template.data.god) {
            if (character.hasFlag('USE_RELATED_GOD_STUFF')) {
                let relatedGods = character.getFlagDatas('RELATED_GOD');
                if (relatedGods) {
                    let foundGod = false;
                    for (let relatedGod of relatedGods) {
                        if (this.template.data.god === relatedGod.data) {
                            foundGod = true;
                            break;
                        }
                    }
                    if (!foundGod) {
                        return {reason: 'bad_god'};
                    }
                }
            }

            let relatedGods = character.getFlagDatas('RELATED_GOD');
            if (relatedGods) {
                let foundGod = false;
                for (let relatedGod of relatedGods) {
                    if (this.template.data.god === relatedGod.data) {
                        foundGod = true;
                        break;
                    }
                }
                if (!foundGod) {
                    return {reason: 'bad_god'};
                }
            }
            else {
                return {reason: 'no_god'};
            }
        }

        if (this.template.data.bruteWeapon) {
            if (!character.hasFlag('ARME_BOURRIN')) {
                return {reason: 'no_arme_bourrin'};
            }
        }

        if (this.template.data.enchantment) {
            if (ItemTemplate.hasSlot(this.template, 'WEAPON')) {
                let flag = character.getFlagDatas('NO_MAGIC_WEAPON');
                if (flag) {
                    return {reason: 'no_magic_weapon', source: flag[0].source};
                }
            }
            else if (this.template.slots && this.template.slots.length) {
                let flag = character.getFlagDatas('NO_MAGIC_ARMOR');
                if (flag) {
                    return {reason: 'no_magic_armor', source: flag[0].source};
                }
            }
            else {
                let flag = character.getFlagDatas('NO_MAGIC_OBJECT');
                if (flag) {
                    return {reason: 'no_magic_object', source: flag[0].source};
                }
            }
        }
        if (this.template.data.itemTypes) {
            const noWeaponTypes = character.getFlagDatas('NO_WEAPON_TYPE');
            if (noWeaponTypes) {
                for (let itemType of this.template.data.itemTypes) {
                    for (let noWeaponType of noWeaponTypes) {
                        if (itemType === noWeaponType.data) {
                            return {reason: 'bad_equipment_type', source: noWeaponType.source};
                        }
                    }
                }
            }

            if (this.template.data.isItemTypeName('LIVRE')
                && !character.hasFlag('ERUDITION')) {
                return {reason: 'cant_read'};
            }
        }

        return undefined;
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
    container: number;

    static fromJson(jsonData: any): PartialItem {
        let item = new PartialItem();
        Object.assign(item, jsonData, {modifiers: ActiveStatsModifier.modifiersFromJson(jsonData.modifiers)});
        return item;
    }
}
