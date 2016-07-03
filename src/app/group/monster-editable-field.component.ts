import {Component, Input, OnChanges} from '@angular/core';
import {GroupService} from './group.service';
import {NotificationsService} from '../notifications';
import {Fighter} from './group.model';

@Component({
    moduleId: module.id,
    selector: 'monster-editable-field',
    templateUrl: 'monster-editable-field.component.html'
})
export class MonsterEditableFieldComponent implements OnChanges {
    @Input() monster: Fighter;
    @Input() fieldName: string;
    @Input() monsterFieldName: string;
    private oldValue: string;

    constructor(private _groupService: GroupService
        , private _notification: NotificationsService) {
    }

    getValue(): string {
        let value = null;
        if (this.fieldName.indexOf('.') === -1) {
            value = this.monster[this.fieldName];
        } else {
            let s = this.fieldName.split('.');
            value = this.monster[s[0]][s[1]];
        }
        return value;
    }

    updateMonsterField() {
        this._groupService.updateMonster(this.monster.id, this.monsterFieldName, this.getValue())
            .subscribe(
                res => {
                    this._notification.info('Monstre: ' + res.name
                        , 'Modification: ' + this.monsterFieldName.toUpperCase() + ': '
                        + this.oldValue + ' -> ' + res[this.monsterFieldName]);
                    this.oldValue = res[this.monsterFieldName];
                },
                err => {
                    console.log(err);
                    this._notification.error('Erreur', 'Erreur serveur');
                }
            );
    }

    ngOnChanges() {
        this.oldValue = this.getValue();
    }
}
/**
 * Created by socolin on 03/07/16.
 */
