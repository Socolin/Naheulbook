import {Component, Input, OnInit} from '@angular/core';
import {Character} from './character.model';
import {ItemActionService} from './item-action.service';
import {Item} from '../item';
import {ActiveStatsModifier} from '../shared';
import {ItemSlot, ItemTemplate, ItemTemplateService} from '../item-template';
import {toDictionary, toDictionaryByKey} from '../utils/utils';

@Component({
    selector: 'app-combat-tab',
    templateUrl: './combat-tab.component.html',
    styleUrls: ['./combat-tab.component.scss']
})
export class CombatTabComponent implements OnInit {
    @Input() character: Character;
    @Input() changeStat: (stat: string, value: any) => void;
    @Input() inGroupTab: boolean;
    slotsByTechNames: { [slotTechName: string]: ItemSlot } = {};

    constructor(
        public readonly itemActionService: ItemActionService,
        public readonly itemTemplateService: ItemTemplateService,
    ) {
    }

    ngOnInit(): void {
        this.itemTemplateService.getSlots().subscribe((slots) => {
            this.slotsByTechNames = toDictionaryByKey(slots, s => s.techName)
        })
    }

    unEquipAllAndEquip(item: Item) {
        for (const weaponItem of this.character.computedData.itemsBySlotsAll[1]) {
            if (weaponItem.data.equiped) {
                this.itemActionService.onAction('unequip', weaponItem);
            }
        }
        this.itemActionService.onAction('equip', item)
    }

    sharpenWeapon(item: Item): void {
        let modifier = new ActiveStatsModifier();
        modifier.durationType = 'combat';
        modifier.combatCount = 3;
        modifier.currentCombatCount = 3;
        modifier.active = true;
        modifier.name = 'Aiguisé';
        modifier.values = [{
            type: 'ADD',
            stat: 'PI',
            value: 1
        }];
        this.itemActionService.onAction('update_modifiers', item, [...item.modifiers, modifier]);
    }

    isItemWeapon(template: ItemTemplate): boolean {
        return ItemTemplate.hasSlot(template, 'WEAPON');
    }
}
