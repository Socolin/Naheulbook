import {Component, OnInit} from '@angular/core';

import {MonsterTemplate, MonsterTemplateCategory, MonsterTemplateType} from './monster.model';
import {MonsterTemplateService} from './monster-template.service';
import {MatDialog} from '@angular/material/dialog';
import {
    EditMonsterTemplateDialogComponent,
    EditMonsterTemplateDialogData
} from './edit-monster-template-dialog.component';
import {LoginService} from '../user';
import {forkJoin} from 'rxjs';

@Component({
    selector: 'monster-list',
    templateUrl: './monster-list.component.html',
    styleUrls: ['./monster-list.component.scss']
})
export class MonsterListComponent implements OnInit {
    public monsterTypes: MonsterTemplateType[];
    public selectedMonsterType: MonsterTemplateType;
    public selectedMonsterCategory?: MonsterTemplateCategory;
    public monsters: MonsterTemplate[];
    public categories: MonsterTemplateCategory[] = [];
    public monsterByCategory: {[categoryId: number]: MonsterTemplate[]} = {};
    public isAdmin = false;

    constructor(
        private loginService: LoginService,
        private monsterTemplateService: MonsterTemplateService,
        private dialog: MatDialog
    ) {
    }

    selectMonsterType(monsterType: MonsterTemplateType) {
        this.selectedMonsterType = monsterType;
        if (monsterType.categories.length) {
            this.selectedMonsterCategory = monsterType.categories[0];
        }
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

    openCreateMonsterTemplateDialog() {
        const dialogRef = this.dialog.open<EditMonsterTemplateDialogComponent, EditMonsterTemplateDialogData, MonsterTemplate>(
            EditMonsterTemplateDialogComponent, {
                autoFocus: false,
                minWidth: '100vw',
                height: '100vh',
                data: {
                    type: this.selectedMonsterType,
                    category: this.selectedMonsterCategory,
                }
            });
        dialogRef.afterClosed().subscribe(result => {
            if (!result) {
                return;
            }
            this.monsters.push(result);
            this.updateSelectCategory(result);
        })
    }

    editMonster(monsterTemplate: MonsterTemplate) {
        const index = this.monsters.findIndex(x => x.id === monsterTemplate.id);
        if (index !== -1) {
            this.monsters[index] = monsterTemplate;
        }
        this.updateSelectCategory(monsterTemplate);
    }

    updateSelectCategory(monsterTemplate: MonsterTemplate) {
        const monsterType = this.monsterTypes.find(x => x.id === monsterTemplate.category.type.id);
        if (!monsterType) {
            this.monsterTypes.push(monsterTemplate.category.type);
            this.selectedMonsterType = monsterTemplate.category.type;
        }
        else if (!(monsterTemplate.category.id in monsterType.categories)) {
            monsterType.categories.push(monsterTemplate.category);
            this.selectedMonsterType = monsterType;
        }

        if (this.selectedMonsterType) {
            const monsterCategory = this.selectedMonsterType.categories.find(x => x.id === monsterTemplate.category.id);
            if (monsterCategory) {
                this.selectedMonsterCategory = monsterCategory;
            } else {
                this.selectMonsterType(this.selectedMonsterType);
            }
        }
        this.sortMonsterByCategory();
    }

    ngOnInit() {
        forkJoin([
            this.monsterTemplateService.getMonsterTypes(),
            this.monsterTemplateService.getMonsterList()
        ]).subscribe(
            ([monsterTypes, monsters]) => {
                this.selectMonsterType(monsterTypes[0]);
                this.monsterTypes = monsterTypes;
                this.monsters = monsters;
                this.sortMonsterByCategory();
            }
        );
        this.isAdmin = this.loginService.currentLoggedUser && this.loginService.currentLoggedUser.admin;
    }
}
