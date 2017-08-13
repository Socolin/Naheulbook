import {Component, Input, Output, EventEmitter, ViewChild, OnChanges, SimpleChanges} from '@angular/core';
import {Fighter, Group} from './group.model';
import {NotificationsService} from '../notifications/notifications.service';
import {CharacterService} from '../character/character.service';
import {GroupActionService} from './group-action.service';
import {Character} from '../character/character.model';
import {ItemTemplate} from '../item/item-template.model';
import {Item} from '../character/item.model';
import {TargetJsonData} from './target.model';
import {MdSlideToggleChange, OverlayRef, Portal} from '@angular/material';
import {NhbkDialogService} from '../shared/nhbk-dialog.service';
import {ActiveStatsModifier, LapCountDecrement} from '../shared/stat-modifier.model';
import {Monster} from '../monster';
import {MonsterService} from '../monster/monster.service';

@Component({
    selector: 'fighter',
    templateUrl: './fighter.component.html',
    styleUrls: ['./fighter.component.scss']
})
export class FighterComponent implements OnChanges {
    @Input() group: Group;
    @Input() fighter: Fighter;
    @Input() fighters: Fighter[];
    @Input() selected: boolean;
    @Input() expandedView: boolean;
    @Output() onSelect: EventEmitter<Fighter> = new EventEmitter<Fighter>();
    @Input() selectedModifier: ActiveStatsModifier | undefined;
    public selectedItem: Item | undefined;

    @ViewChild('editMonsterDialog')
    public editMonsterDialog: Portal<any>;
    public editMonsterOverlayRef: OverlayRef | undefined;

    constructor(private _actionService: GroupActionService
        , private _characterService: CharacterService
        , private _monsterService: MonsterService
        , private _nhbkDialogService: NhbkDialogService
        , private _notification: NotificationsService) {
    }

    addItemTo(fighter: Fighter) {
        this._actionService.emitAction('openAddItemForm', this.group, fighter);
    }

    displayCharacterSheet(character: Character) {
        this._actionService.emitAction('displayCharacterSheet', this.group, character);
    }

    changeTarget(target: TargetJsonData) {
        if (this.fighter.isMonster) {
            this._monsterService.updateMonster(this.fighter.id, 'target', {id: target.id, isMonster: target.isMonster})
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
            this._monsterService.updateMonster(element.id, 'color', color)
                .subscribe(
                    res => {
                        element.changeColor(res.value);
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
        this._monsterService.updateMonster(monster.id, fieldName, newValue)
            .subscribe(
                res => {
                    this._notification.info('Monstre: ' + monster.name
                        , 'Modification: ' + fieldName.toUpperCase() + ': '
                        + monster.data[fieldName] + ' -> ' + res.value);
                    monster.changeData(fieldName, res.value);
                }
            );
    }

    changeNumber(element: Fighter, number: number) {
        if (element.isMonster) {
            this._monsterService.updateMonster(element.id, 'number', number)
                .subscribe(
                    res => {
                        element.changeNumber(res.value);
                        this._notification.info('Monstre', 'Couleur changé');
                    }
                );
        }
    }

    changeStat(stat: string, value: any) {
        if (this.fighter.isMonster) {
            let monster = this.fighter.monster;
            this._monsterService.updateMonster(monster.id, stat, value)
                .subscribe(
                    res => {
                        if (res.fieldName === 'name') {
                            monster.name = res.value;
                        } else {
                            monster.changeData(res.fieldName, res.value);
                        }
                    }
                );
        }
        else {
            this._characterService.changeCharacterStat(this.fighter.character.id, stat, value).subscribe(
                this.fighter.character.onChangeCharacterStat.bind(this.fighter.character)
            );
        }
    }

    equipItem(item: Item, event: MdSlideToggleChange) {
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
        this.editMonsterOverlayRef = this._nhbkDialogService.openCenteredBackdropDialog(this.editMonsterDialog);
    }

    closeEditMonsterDialog() {
        if (!this.editMonsterOverlayRef) {
            return;
        }
        this.editMonsterOverlayRef.detach();
        this.editMonsterOverlayRef = undefined;
    }

    selectFighter() {
        this.onSelect.emit(this.fighter);
    }

    itemHasSlot(template: ItemTemplate, slot: string) {
        return ItemTemplate.hasSlot(template, slot);
    }

    addCustomModifier(modifier: ActiveStatsModifier) {
        if (modifier.durationType === 'lap') {
            let fighter = this.group.currentFighter;
            if (!fighter) {
                fighter = this.fighter;
            }
            modifier.lapCountDecrement = new LapCountDecrement();
            modifier.lapCountDecrement.fighterId = fighter.id;
            modifier.lapCountDecrement.fighterIsMonster = fighter.isMonster;
            modifier.lapCountDecrement.when = 'BEFORE';
        }
        this._monsterService.addModifier(this.fighter.id, modifier).subscribe(
            this.fighter.monster.onAddModifier.bind(this.fighter.monster)
        );
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
        this._monsterService.removeModifier(this.fighter.id, modifier.id).subscribe((deletedModifier: ActiveStatsModifier) => {
            if (this.selectedModifier && this.selectedModifier.id === deletedModifier.id) {
                this.selectedModifier = undefined;
            }
            this.fighter.monster.onRemoveModifier(deletedModifier);
        });
    }
    ngOnChanges(changes: SimpleChanges): void {
        if ('fighter' in changes) {
            this.selectedItem = undefined;
        }
    }
}
