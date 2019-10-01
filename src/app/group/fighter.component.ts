import {Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges} from '@angular/core';
import {MatDialog} from '@angular/material';

import {NotificationsService} from '../notifications';
import {CharacterService, ItemActionService} from '../character';
import {Item, ItemService} from '../item';
import {ActiveStatsModifier, LapCountDecrement, NhbkDialogService} from '../shared';
import {Monster, MonsterData, MonsterService} from '../monster';

import {Fighter, Group} from './group.model';
import {GroupActionService} from './group-action.service';
import {TargetJsonData} from './target.model';
import {AddEffectDialogComponent} from '../effect';
import {
    EditMonsterDialogComponent,
    EditMonsterDialogData,
    EditMonsterDialogResult
} from './edit-monster-dialog.component';
import {openCreateItemDialog} from './create-item-dialog.component';
import {openCreateGemDialog} from './create-gem-dialog.component';
import {ModifierDetailsDialogComponent} from './modifier-details-dialog.component';
import {MonsterInventoryDialogComponent} from './monster-inventory-dialog.component';

@Component({
    selector: 'fighter',
    templateUrl: './fighter.component.html',
    styleUrls: ['./fighter.component.scss'],
    providers: [ItemActionService],
})
export class FighterComponent implements OnInit {
    @Input() group: Group;
    @Input() fighter: Fighter;
    @Input() fighters: Fighter[];
    @Input() selected: boolean;
    @Input() expandedView: boolean;
    @Output() onSelect: EventEmitter<Fighter> = new EventEmitter<Fighter>();

    public moreInfo = false;

    constructor(
        private readonly actionService: GroupActionService,
        private readonly characterService: CharacterService,
        private readonly monsterService: MonsterService,
        private readonly itemActionService: ItemActionService,
        private readonly itemService: ItemService,
        private readonly nhbkDialogService: NhbkDialogService,
        private readonly notification: NotificationsService,
        private readonly dialog: MatDialog,
    ) {
    }

    openAddItemDialog() {
        openCreateItemDialog(this.dialog, (item) => {
            this.addItem(item);
        });
    }

    openAddGemDialog() {
        openCreateGemDialog(this.dialog, (item) => {
            this.addItem(item);
        });
    }

    addItem(item: Item) {
        this.itemService.addItemTo(this.fighter.typeName, this.fighter.id, item.template.id, item.data).subscribe(
            (createdItem) => {
                if (this.fighter.isMonster) {
                    this.fighter.monster.addItem(createdItem);
                } else {
                    this.fighter.character.onAddItem(createdItem);
                }
            });
    }

    displayCharacterSheet() {
        this.actionService.emitAction('displayCharacterSheet', this.group, this.fighter.character);
    }

    openInventoryDialog() {
        this.dialog.open(MonsterInventoryDialogComponent, {
            autoFocus: false,
            data: {monster: this.fighter.monster}
        });
    }

    changeTarget(target: TargetJsonData) {
        if (this.fighter.isMonster) {
            this.monsterService.updateMonsterTarget(this.fighter.id, {id: target.id, isMonster: target.isMonster})
                .subscribe(
                    () => {
                        this.fighter.changeTarget(target);
                        this.notification.info('Monstre', 'Cible changée');
                    }
                );
        } else {
            this.characterService.changeGmData(this.fighter.id, 'target', {
                id: target.id,
                isMonster: target.isMonster
            }).subscribe(
                change => {
                    this.fighter.changeTarget(change.value);
                    this.notification.info('Joueur', 'Cible changée');
                }
            );
        }
    }

    changeColor(element: Fighter, color: string) {
        if (color.indexOf('#') === 0) {
            color = color.substring(1);
        }
        if (element.isMonster) {
            this.monsterService.updateMonsterData(element.id, {...element.monster.data, color})
                .subscribe(
                    () => {
                        element.changeColor(color);
                        this.notification.info('Monstre', 'Couleur changé');
                    }
                );
        } else {
            this.characterService.changeGmData(element.id, 'color', color).subscribe(
                change => {
                    element.changeColor(change.value);
                    this.notification.info('Joueur', 'Couleur changée');
                }
            );
        }
    }

    updateMonsterField(monster: Monster, fieldName: string, newValue: any) {
        if (monster.data[fieldName] === newValue) {
            return;
        }
        const newData = {...monster.data, [fieldName]: newValue};
        this.monsterService.updateMonsterData(monster.id, newData)
            .subscribe(() => {
                monster.changeData(newData);
            });
    }

    changeNumber(element: Fighter, number: number) {
        if (element.isMonster) {
            this.monsterService.updateMonsterData(element.id, {...element.monster.data, number})
                .subscribe(
                    () => {
                        element.changeNumber(number);
                        this.notification.info('Monstre', 'Couleur changé');
                    }
                );
        }
    }

    changeStat(stat: string, value: any) {
        if (this.fighter.isMonster) {
            let monster = this.fighter.monster;
            if (stat === 'name') {
                this.monsterService.updateMonster(monster.id, {name: value})
                    .subscribe(() => {
                        monster.name = value;
                    });
            } else {
                let monsterData = {...monster.data, [stat]: value};
                this.monsterService.updateMonsterData(monster.id, monsterData)
                    .subscribe(() => {
                        monster.changeData(monsterData);
                    });
            }
        } else {
            this.characterService.changeCharacterStat(this.fighter.character.id, stat, value).subscribe(
                this.fighter.character.onChangeCharacterStat.bind(this.fighter.character)
            );
        }
    }

    killMonster(monster: Monster) {
        this.actionService.emitAction('killMonster', this.group, monster);
    }

    deleteMonster(monster: Monster) {
        this.actionService.emitAction('deleteMonster', this.group, monster);
    }

    openEditMonsterDialog() {
        const dialogRef = this.dialog.open<EditMonsterDialogComponent, EditMonsterDialogData, EditMonsterDialogResult>(
            EditMonsterDialogComponent, {
                autoFocus: false,
                data: {monster: this.fighter.monster},
                minWidth: '100vw', height: '100vh'
            });

        dialogRef.afterClosed().subscribe((result) => {
            if (!result) {
                return;
            }
            const {name, ...data} = result;
            const monsterData = new MonsterData({...this.fighter.monster.data, ...data});
            this.monsterService.updateMonster(this.fighter.id, {name}).subscribe(() => {
                this.fighter.monster.name = name;
            });
            this.monsterService.updateMonsterData(this.fighter.id, data).subscribe(() => {
                this.fighter.monster.data = monsterData;
            });
        });
    }

    openAddEffectDialog() {
        const dialogRef = this.dialog.open(AddEffectDialogComponent, {
            minWidth: '100vw',
            height: '100vh',
            data: {options: {hideReusable: true}}
        });
        dialogRef.afterClosed().subscribe(result => {
            if (!result) {
                return;
            }

            if (result.durationType === 'lap') {
                let fighter = this.group.currentFighter;
                if (!fighter) {
                    fighter = this.fighter;
                }
                result.lapCountDecrement = new LapCountDecrement();
                result.lapCountDecrement.fighterId = fighter.id;
                result.lapCountDecrement.fighterIsMonster = fighter.isMonster;
                result.lapCountDecrement.when = 'BEFORE';
            }

            this.monsterService.addModifier(this.fighter.id, result).subscribe(
                this.fighter.monster.onAddModifier.bind(this.fighter.monster)
            );
        });
    }

    removeModifier(modifier: ActiveStatsModifier) {
        this.monsterService.removeModifier(this.fighter.id, modifier.id).subscribe(() => {
            this.fighter.monster.onRemoveModifier(modifier.id);
        });
    }

    ngOnInit() {
        this.itemActionService.registerAction('ignoreRestrictions').subscribe((event: { item: Item, data: any }) => {
            let item = event.item;
            item.data = {
                ...item.data,
                ignoreRestrictions: event.data
            };
            this.itemService.updateItem(item.id, item.data).subscribe(
                this.fighter.character.onUpdateItem.bind(this.fighter.character)
            );
        });
        this.itemActionService.registerAction('identify').subscribe((event: { item: Item, data: any }) => {
            let item = event.item;
            let itemData = {...item.data, name: item.template.name};
            delete itemData.notIdentified;

            this.itemService.updateItem(item.id, itemData).subscribe(
                this.fighter.character.onUpdateItem.bind(this.fighter.character)
            );
        });
    }

    openModifierDetails(modifier: ActiveStatsModifier) {
        this.dialog.open(ModifierDetailsDialogComponent, {data: {modifier}, autoFocus: false});
    }

    toggleMoreInfo() {
        this.moreInfo = !this.moreInfo;
    }
}
