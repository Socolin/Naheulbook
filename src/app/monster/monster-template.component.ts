import {Component, Input, OnInit} from '@angular/core';

import {MonsterTemplate, MonsterTrait} from './monster.model';
import {MonsterTemplateService} from '.';
import {NotificationsService} from '../notifications/notifications.service';
import {removeDiacritics} from '../shared/remove_diacritics';

@Component({
    selector: 'monster-template',
    styleUrls: ['./monster-template.component.scss'],
    templateUrl: './monster-template.component.html'
})
export class MonsterTemplateComponent implements OnInit {
    @Input() monster: MonsterTemplate;
    public traisById: {[id: number]: MonsterTrait};
    public editing: boolean;

    constructor(private _monsterTemplateService: MonsterTemplateService
        , private _notifications: NotificationsService) {
    }

    traitImage(trait: MonsterTrait) {
        return 'assets/img/monster-traits/' + removeDiacritics(trait.name).split(' ').join('_').toLowerCase() + '.png';
    }

    saveMonster() {
        this.editing = false;
        this._monsterTemplateService.editMonster(this.monster).subscribe(() => {
            this._notifications.success('Monster', 'Monstre editer');
        });
    }

    editMonster() {
        this.editing = true;
    }

    ngOnInit(): void {
        this._monsterTemplateService.getMonsterTraitsById().subscribe(
            traitsById => {
                this.traisById = traitsById
            }
        );
    }
}
