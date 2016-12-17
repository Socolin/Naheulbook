import {Component, Input, Output, EventEmitter} from '@angular/core';
import {Fighter, Group} from './group.model';
import {GroupService} from './group.service';
import {NotificationsService} from '../notifications/notifications.service';
import {Monster} from '../monster/monster.model';
import {CharacterService} from '../character/character.service';
import {GroupActionService} from './group-action.service';
import {Character} from '../character/character.model';

@Component({
    selector: 'fighter',
    templateUrl: './fighter.component.html',
    styleUrls: ['./fighter.component.css']
})
export class FighterComponent {
    @Input() group: Group;
    @Input() fighter: Fighter;
    @Input() fighters: Fighter[];
    @Input() i: number;
    @Input() selected: boolean;
    @Output() onSelect: EventEmitter<number> = new EventEmitter<number>();

    constructor(private _groupService: GroupService
        , private _actionService: GroupActionService
        , private _characterService: CharacterService
        , private _notification: NotificationsService) {
    }

    addItemTo(character: Character) {
        this._actionService.emitAction('openAddItemForm', this.group, character);
    }

    changeTarget(fighter: Fighter, target) {
        if (fighter.isMonster) {
            this._groupService.updateMonster(fighter.id, 'target', {id: target.id, isMonster: target.isMonster})
                .subscribe(
                    () => {
                        fighter.changeTarget(target);
                        fighter.updateTarget(this.fighters);
                        this._notification.info('Monstre', 'Cible changée');
                    }
                );
        } else {
            this._characterService.changeGmData(fighter.id, 'target', {
                id: target.id,
                isMonster: target.isMonster
            }).subscribe(
                change => {
                    fighter.changeTarget(change.value);
                    fighter.updateTarget(this.fighters);
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
            this._groupService.updateMonster(element.id, 'color', color)
                .subscribe(
                    res => {
                        element.changeColor(res.value);
                        this.fighters.forEach(f => f.updateTarget(this.fighters));
                        this._notification.info('Monstre', 'Couleur changé');
                    }
                );
        } else {
            this._characterService.changeGmData(element.id, 'color', color).subscribe(
                change => {
                    element.changeColor(change.value);
                    this.fighters.forEach(f => f.updateTarget(this.fighters));
                    this._notification.info('Joueur', 'Couleur changée');
                }
            );
        }
    }

    updateMonsterField(monster: Monster, fieldName: string, newValue: any) {
        this._groupService.updateMonster(monster.id, fieldName, newValue)
            .subscribe(
                res => {
                    this._notification.info('Monstre: ' + monster.name
                        , 'Modification: ' + fieldName.toUpperCase() + ': '
                        + monster.data[fieldName] + ' -> ' + res.value);
                    this._actionService.emitAction('reorderFighters', this.group);
                    monster.data[fieldName] = res.value;
                }
            );
    }

    changeNumber(element: Fighter, number: number) {
        if (element.isMonster) {
            this._groupService.updateMonster(element.id, 'number', number)
                .subscribe(
                    res => {
                        element.changeNumber(res.value);
                        this.fighters.forEach(f => f.updateTarget(this.fighters));
                        this._notification.info('Monstre', 'Couleur changé');
                    }
                );
        }
    }

    killMonster(monster: Monster) {
        this._actionService.emitAction('killMonster', this.group, monster);
    }

    deleteMonster(monster: Monster) {
        this._actionService.emitAction('deleteMonster', this.group, monster);
    }

    selectFighter() {
        this.onSelect.emit(this.i);
    }
}
