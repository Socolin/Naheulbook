import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {Character} from './character.model';
import {ItemActionService} from './item-action.service';
import {Item} from '../item';
import {ActiveStatsModifier} from '../shared';
import {ItemSlot, ItemTemplate, ItemTemplateService} from '../item-template';
import {toDictionaryByKey} from '../utils/utils';
import { MatCard, MatCardContent, MatCardSubtitle, MatCardHeader, MatCardTitle } from '@angular/material/card';
import { ValueEditorComponent } from '../shared/value-editor.component';
import { MatIcon } from '@angular/material/icon';
import { ItemLineComponent } from './item-line.component';
import { MatMenu, MatMenuContent, MatMenuItem } from '@angular/material/menu';

@Component({
    selector: 'app-combat-tab',
    templateUrl: './combat-tab.component.html',
    styleUrls: ['./combat-tab.component.scss'],
    imports: [MatCard, MatCardContent, ValueEditorComponent, MatCardSubtitle, MatIcon, MatCardHeader, MatCardTitle, ItemLineComponent, MatMenu, MatMenuContent, MatMenuItem]
})
export class CombatTabComponent implements OnInit {
    @Input() character: Character;
    @Input() inGroupTab: boolean;
    @Output() statChanged: EventEmitter<{stat: string, value: any}> = new EventEmitter<{stat: string; value: any}>();
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

    changeStat(stat: string, value: any) {
        this.statChanged.emit({stat, value});
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
