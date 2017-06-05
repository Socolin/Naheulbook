import {Component, OnInit} from '@angular/core';

import {NotificationsService} from '../notifications/notifications.service';
import {MonsterTemplate, MonsterTemplateCategory, MonsterTemplateService} from '.';

@Component({
    selector: 'monster-list',
    templateUrl: './monster-list.component.html',
})
export class MonsterListComponent implements OnInit {
    public monsters: MonsterTemplate[];
    public categories: MonsterTemplateCategory[] = [];
    public monsterByCategory: {[categoryId: number]: MonsterTemplate[]} = {};

    public newMonster: MonsterTemplate;

    constructor(private _monsterTemplateService: MonsterTemplateService, private _notifications: NotificationsService) {
    }

    startAddMonster() {
        this.newMonster = new MonsterTemplate();
    }

    cancelAddMonster() {
        this.newMonster = null;
    }

    addMonster() {
        if (!this.newMonster.type) {
            return;
        }
        this._monsterTemplateService.editMonster(this.newMonster).subscribe(
            monster => {
                this.monsters.push(monster);
                this.sortMonsterByCategory();
                this._notifications.success('Monstre', 'Monstre créée');
            }
        );
        this.newMonster = null;
    }

    sortMonsterByCategory() {
        let monsterByCategory = [];
        let categories = [];
        for (let i = 0; i < this.monsters.length; i++) {
            let monster = this.monsters[i];
            if (!(monster.type.id in monsterByCategory)) {
                categories.push(monster.type);
                monsterByCategory[monster.type.id] = [];
            }
            monsterByCategory[monster.type.id].push(monster);
        }
        this.monsterByCategory = monsterByCategory;
        this.categories = categories;
    }

    getMonsters() {
        this._monsterTemplateService.getMonsterList().subscribe(
            res => {
                this.monsters = res;
                this.sortMonsterByCategory();
            }
        );
    }

    ngOnInit() {
        this.getMonsters();
    }
}
