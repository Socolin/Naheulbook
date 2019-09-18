import {Component, OnInit} from '@angular/core';

import {NotificationsService} from '../notifications';

import {MonsterTemplate, MonsterTemplateCategory} from './monster.model';
import {MonsterTemplateService} from './monster-template.service';
import {MatDialog} from '@angular/material/dialog';
import {EditMonsterTemplateDialogComponent} from './edit-monster-template-dialog.component';

@Component({
    selector: 'monster-list',
    templateUrl: './monster-list.component.html',
    styleUrls: ['./monster-list.component.scss']
})
export class MonsterListComponent implements OnInit {
    public monsters: MonsterTemplate[];
    public categories: MonsterTemplateCategory[] = [];
    public monsterByCategory: {[categoryId: number]: MonsterTemplate[]} = {};

    constructor(
        private _monsterTemplateService: MonsterTemplateService,
        private _notifications: NotificationsService,
        private dialog: MatDialog
    ) {
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

    openCreateMonsterTemplateDialog() {
        const dialogRef = this.dialog.open(EditMonsterTemplateDialogComponent, {
            autoFocus: false,
            minWidth: '100vw',
            height: '100vh',
            data: {}
        });
    }

    ngOnInit() {
        this.getMonsters();
    }
}
