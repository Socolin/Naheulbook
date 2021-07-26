import {Component, Input, OnInit} from '@angular/core';
import {Character} from './character.model';
import {ItemActionService} from './item-action.service';
import {Item} from '../item';
import {ActiveStatsModifier} from '../shared';

@Component({
    selector: 'app-combat-tab',
    templateUrl: './combat-tab.component.html',
    styleUrls: ['./combat-tab.component.scss']
})
export class CombatTabComponent implements OnInit {
    @Input() character: Character;
    @Input() changeStat: (stat: string, value: any) => void;
    @Input() inGroupTab: boolean;

    constructor(
        public readonly itemActionService: ItemActionService,
    ) {
    }

    ngOnInit(): void {
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
}
