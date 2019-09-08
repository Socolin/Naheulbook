import {Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges} from '@angular/core';
import {MatDialog, MatSlideToggleChange} from '@angular/material';

import {NotificationsService} from '../notifications';
import {Character, CharacterService, ItemActionService} from '../character';
import {Item, ItemService} from '../item';
import {ActiveStatsModifier, LapCountDecrement, NhbkDialogService} from '../shared';
import {Monster, MonsterData, MonsterService} from '../monster';
import {ItemTemplate} from '../item-template';

import {Fighter, Group} from './group.model';
import {GroupActionService} from './group-action.service';
import {TargetJsonData} from './target.model';
import {AddEffectDialogComponent} from '../effect';
import {
    EditMonsterDialogComponent,
    EditMonsterDialogData,
    EditMonsterDialogResult
} from './edit-monster-dialog.component';
import {CreateItemDialogComponent, openCreateItemDialog} from './create-item-dialog.component';
import {Subject} from 'rxjs';
import {Loot} from '../loot';
import {openCreateGemDialog} from './create-gem-dialog.component';

@Component({
    selector: 'fighter',
    templateUrl: './fighter.component.html',
    styleUrls: ['./fighter.component.scss'],
    providers: [ItemActionService],
})
export class FighterComponent implements OnInit, OnChanges {
    @Input() group: Group;
    @Input() fighter: Fighter;
    @Input() fighters: Fighter[];
    @Input() selected: boolean;
    @Input() expandedView: boolean;
    @Output() onSelect: EventEmitter<Fighter> = new EventEmitter<Fighter>();
    @Input() selectedModifier: ActiveStatsModifier | undefined;
    public selectedItem: Item | undefined;

    constructor(
        private _actionService: GroupActionService,
        private _characterService: CharacterService,
        private _monsterService: MonsterService,
        private _itemActionService: ItemActionService,
        private _itemService: ItemService,
        private _nhbkDialogService: NhbkDialogService,
        private _notification: NotificationsService,
        private dialog: MatDialog,
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
        this._itemService.addItemTo(this.fighter.typeName, this.fighter.id, item.template.id, item.data).subscribe(
            (createdItem) => {
                if (this.fighter.isMonster) {
                    this.fighter.monster.addItem(createdItem);
                } else {
                    this.fighter.character.onAddItem(createdItem);
                }
            });
    }

    displayCharacterSheet(character: Character) {
        this._actionService.emitAction('displayCharacterSheet', this.group, character);
    }

    changeTarget(target: TargetJsonData) {
        if (this.fighter.isMonster) {
            this._monsterService.updateMonsterTarget(this.fighter.id, {id: target.id, isMonster: target.isMonster})
                .subscribe(
                    () => {
                        this.fighter.changeTarget(target);
                        this._notification.info('Monstre', 'Cible changée');
                    }
                );
        } else {
            this._characterService.changeGmData(this.fighter.id, 'target', {
                id: target.id,
                isMonster: target.isMonster
            }).subscribe(
                change => {
                    this.fighter.changeTarget(change.value);
                    this._notification.info('Joueur', 'Cible changée');
                }
            );
        }
    }

    changeColor(element: Fighter, color: string) {
        if (color.indexOf('#') === 0) {
            color = color.substring(1);
        }
        if (element.isMonster) {
            this._monsterService.updateMonsterData(element.id, {...element.monster.data, color})
                .subscribe(
                    () => {
                        element.changeColor(color);
                        this._notification.info('Monstre', 'Couleur changé');
                    }
                );
        } else {
            this._characterService.changeGmData(element.id, 'color', color).subscribe(
                change => {
                    element.changeColor(change.value);
                    this._notification.info('Joueur', 'Couleur changée');
                }
            );
        }
    }

    updateMonsterField(monster: Monster, fieldName: string, newValue: any) {
        if (monster.data[fieldName] === newValue) {
            return;
        }
        const newData = {...monster.data, [fieldName]: newValue};
        this._monsterService.updateMonsterData(monster.id, newData)
            .subscribe(() => {
                monster.changeData(newData);
            });
    }

    changeNumber(element: Fighter, number: number) {
        if (element.isMonster) {
            this._monsterService.updateMonsterData(element.id, {...element.monster.data, number})
                .subscribe(
                    () => {
                        element.changeNumber(number);
                        this._notification.info('Monstre', 'Couleur changé');
                    }
                );
        }
    }

    changeStat(stat: string, value: any) {
        if (this.fighter.isMonster) {
            let monster = this.fighter.monster;
            if (stat === 'name') {
                this._monsterService.updateMonster(monster.id, {name: value})
                    .subscribe(() => {
                        monster.name = value;
                    });
            } else {
                let monsterData = {...monster.data, [stat]: value};
                this._monsterService.updateMonsterData(monster.id, monsterData)
                    .subscribe(() => {
                        monster.changeData(monsterData);
                    });
            }
        } else {
            this._characterService.changeCharacterStat(this.fighter.character.id, stat, value).subscribe(
                this.fighter.character.onChangeCharacterStat.bind(this.fighter.character)
            );
        }
    }

    equipItem(item: Item, event: MatSlideToggleChange) {
        this._monsterService.equipItem(this.fighter.id, item.id, event.checked).subscribe(res => {
            this.fighter.monster.equipItem(res);
        });
    }

    killMonster(monster: Monster) {
        this._actionService.emitAction('killMonster', this.group, monster);
    }

    deleteMonster(monster: Monster) {
        this._actionService.emitAction('deleteMonster', this.group, monster);
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
            this._monsterService.updateMonster(this.fighter.id, {name}).subscribe(() => {
                this.fighter.monster.name = name;
            });
            this._monsterService.updateMonsterData(this.fighter.id, data).subscribe(() => {
                this.fighter.monster.data = monsterData;
            });
        });
    }

    selectFighter() {
        this.onSelect.emit(this.fighter);
    }

    itemHasSlot(template: ItemTemplate, slot: string) {
        return ItemTemplate.hasSlot(template, slot);
    }

    openAddEffectDialog() {
        const dialogRef = this.dialog.open(AddEffectDialogComponent, {minWidth: '100vw', height: '100vh'});
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

            this._monsterService.addModifier(this.fighter.id, result).subscribe(
                this.fighter.monster.onAddModifier.bind(this.fighter.monster)
            );
        });
    }

    selectModifier(modifier: ActiveStatsModifier) {
        if (modifier === this.selectedModifier) {
            this.selectedModifier = undefined;
        } else {
            this.selectedModifier = modifier;
        }
    }

    toggleReusableModifier(modifier: ActiveStatsModifier) {
        this._monsterService.toggleModifier(this.fighter.id, modifier.id).subscribe(
            this.fighter.monster.onUpdateModifier.bind(this.fighter.monster)
        );
    }

    removeModifier(modifier: ActiveStatsModifier) {
        this._monsterService.removeModifier(this.fighter.id, modifier.id).subscribe(() => {
            if (this.selectedModifier && this.selectedModifier.id === modifier.id) {
                this.selectedModifier = undefined;
            }
            this.fighter.monster.onRemoveModifier(modifier.id);
        });
    }

    ngOnChanges(changes: SimpleChanges): void {
        if ('fighter' in changes) {
            this.selectedItem = undefined;
        }
    }

    ngOnInit() {
        this._itemActionService.registerAction('ignoreRestrictions').subscribe((event: { item: Item, data: any }) => {
            let item = event.item;
            item.data = {
                ...item.data,
                ignoreRestrictions: event.data
            };
            this._itemService.updateItem(item.id, item.data).subscribe(
                this.fighter.character.onUpdateItem.bind(this.fighter.character)
            );
        });
        this._itemActionService.registerAction('identify').subscribe((event: { item: Item, data: any }) => {
            let item = event.item;
            let itemData = {...item.data, name: item.template.name};
            delete itemData.notIdentified;

            this._itemService.updateItem(item.id, itemData).subscribe(
                this.fighter.character.onUpdateItem.bind(this.fighter.character)
            );
        });
    }

    openModifierDetails(modifier: ActiveStatsModifier) {


    }
}
