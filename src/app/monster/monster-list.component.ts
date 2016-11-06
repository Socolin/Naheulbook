import {Component, OnInit} from '@angular/core';

import {MonsterTemplate, MonsterTemplateCategory} from "./monster.model";
import {MonsterService} from "./monster.service";
import {NotificationsService} from "../notifications/notifications.service";

@Component({
    selector: 'monster-list',
    templateUrl: 'monster-list.component.html',
})
export class MonsterListComponent implements OnInit {
    public monsters: MonsterTemplate[];
    public categories: MonsterTemplateCategory[] = [];
    public monsterByCategory: {[categoryId: number]: MonsterTemplate[]} = {};

    public newMonster: MonsterTemplate;

    constructor(private _monsterService: MonsterService, private _notifications: NotificationsService) {
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
        this._monsterService.editMonster(this.newMonster).subscribe(
            monster => {
                this.monsters.push(monster);
                this.sortMonsterByCategory();
                this._notifications.success("Monstre", "Monstre créée");
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
        this._monsterService.getMonsterList().subscribe(
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
