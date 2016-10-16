import {Component, Input, OnChanges} from '@angular/core';
import {GroupService} from './group.service';
import {NotificationsService} from '../notifications';
import {Monster} from '../monster';

@Component({
    selector: 'monster-editable-field',
    templateUrl: 'monster-editable-field.component.html'
})
export class MonsterEditableFieldComponent implements OnChanges {
    @Input() monster: Monster;
    @Input() isDataField: boolean;
    @Input() fieldName: string;

    private oldValue: string;

    constructor(private _groupService: GroupService
        , private _notification: NotificationsService) {
    }

    getValue(): string {
        if (this.isDataField) {
            return this.monster.data[this.fieldName];
        } else {
            return this.monster[this.fieldName];
        }
    }

    updateMonsterField() {
        this._groupService.updateMonster(this.monster.id, this.fieldName, this.getValue())
            .subscribe(
                res => {
                    this._notification.info('Monstre: ' + this.monster.name
                        , 'Modification: ' + this.fieldName.toUpperCase() + ': '
                        + this.oldValue + ' -> ' + res.value);
                    this.oldValue = res.value;
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
