import {Component, OnInit} from '@angular/core';

import {NotificationsService} from '../notifications/notifications.service';

import {MonsterTemplate, MonsterTemplateCategory} from './monster.model';
import {MonsterTemplateService} from './monster-template.service';

@Component({
    selector: 'monster-list',
    templateUrl: './monster-list.component.html',
})
export class MonsterListComponent implements OnInit {
    public monsters: MonsterTemplate[];
    public categories: MonsterTemplateCategory[] = [];
    public monsterByCategory: {[categoryId: number]: MonsterTemplate[]} = {};

    public newMonsterTemplate: MonsterTemplate | undefined;

    constructor(private _monsterTemplateService: MonsterTemplateService, private _notifications: NotificationsService) {
    }

    startAddMonster() {
        this.newMonsterTemplate = new MonsterTemplate();
    }

    cancelAddMonster() {
        this.newMonsterTemplate = undefined;
    }

    addMonster() {
        if (!this.newMonsterTemplate) {
            throw new Error('addMonster: `newMonsterTemplate` should be defined');
        }
        if (!this.newMonsterTemplate.category) {
            return;
        }
        const request = {
            ...this.newMonsterTemplate
        };
        delete request.category;
        delete request.categoryId;
        this._monsterTemplateService.addMonster(this.newMonsterTemplate.category.id, request).subscribe(
            monster => {
                this.monsters.push(monster);
                this.sortMonsterByCategory();
                this._notifications.success('Monstre', 'Monstre créée');
            }
        );
        this.newMonsterTemplate = undefined;
    }

    addMonsterContinue() {
        if (!this.newMonsterTemplate) {
            throw new Error('addMonsterContinue: `newMonsterTemplate` should be defined');
        }
        if (!this.newMonsterTemplate.category) {
            return;
        }
        const request = {
            ...this.newMonsterTemplate
        };
        delete request.category;
        delete request.categoryId;
        this._monsterTemplateService.addMonster(this.newMonsterTemplate.category.id, request).subscribe(
            monster => {
                this.monsters.push(monster);
                this.sortMonsterByCategory();
                this._notifications.success('Monstre', 'Monstre créée');
            }
        );
        let monsterTemplate = new MonsterTemplate();
        monsterTemplate.category = this.newMonsterTemplate.category;
        monsterTemplate.data.page = this.newMonsterTemplate.data.page;
        monsterTemplate.locations = this.newMonsterTemplate.locations;
        this.newMonsterTemplate = monsterTemplate;
    }

    sortMonsterByCategory() {
        let monsterByCategory: {[categoryId: number]: MonsterTemplate[]} = [];
        let categories: MonsterTemplateCategory[] = [];
        for (let i = 0; i < this.monsters.length; i++) {
            let monster = this.monsters[i];
            if (!(monster.category.id in monsterByCategory)) {
                categories.push(monster.category);
                monsterByCategory[monster.category.id] = [];
            }
            monsterByCategory[monster.category.id].push(monster);
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
