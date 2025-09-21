import {Component, OnInit} from '@angular/core';

import {MonsterTemplate, MonsterTemplateSubCategory, MonsterTemplateType} from './monster.model';
import {MonsterTemplateService} from './monster-template.service';
import {
    EditMonsterTemplateDialogComponent,
    EditMonsterTemplateDialogData
} from './edit-monster-template-dialog.component';
import {LoginService} from '../user';
import {forkJoin} from 'rxjs';
import {NhbkMatDialog} from '../material-workaround';

@Component({
    selector: 'monster-list',
    templateUrl: './monster-list.component.html',
    styleUrls: ['./monster-list.component.scss'],
    standalone: false
})
export class MonsterListComponent implements OnInit {
    public monsterTypes: MonsterTemplateType[];
    public selectedMonsterType: MonsterTemplateType;
    public selectedMonsterSubCategory?: MonsterTemplateSubCategory;
    public monsters: MonsterTemplate[];
    public subCategories: MonsterTemplateSubCategory[] = [];
    public monsterBySubCategory: {[subCategoryId: number]: MonsterTemplate[]} = {};
    public isAdmin = false;

    constructor(
        private readonly dialog: NhbkMatDialog,
        private readonly loginService: LoginService,
        private readonly monsterTemplateService: MonsterTemplateService,
    ) {
    }

    selectMonsterType(monsterType: MonsterTemplateType) {
        this.selectedMonsterType = monsterType;
        if (monsterType.subCategories.length) {
            this.selectedMonsterSubCategory = monsterType.subCategories[0];
        }
    }

    sortMonsterBySubCategory() {
        let monsterBySubCategory: {[subCategoryId: number]: MonsterTemplate[]} = [];
        let subCategories: MonsterTemplateSubCategory[] = [];
        for (let i = 0; i < this.monsters.length; i++) {
            let monster = this.monsters[i];
            if (!(monster.subCategory.id in monsterBySubCategory)) {
                subCategories.push(monster.subCategory);
                monsterBySubCategory[monster.subCategory.id] = [];
            }
            monsterBySubCategory[monster.subCategory.id].push(monster);
        }
        this.monsterBySubCategory = monsterBySubCategory;
        this.subCategories = subCategories;
    }

    openCreateMonsterTemplateDialog() {
        const dialogRef = this.dialog.openFullScreen<EditMonsterTemplateDialogComponent, EditMonsterTemplateDialogData, MonsterTemplate>(
            EditMonsterTemplateDialogComponent, {
                data: {
                    type: this.selectedMonsterType,
                    subCategory: this.selectedMonsterSubCategory,
                }
            });
        dialogRef.afterClosed().subscribe(result => {
            if (!result) {
                return;
            }
            this.monsters.push(result);
            this.updateSelectSubCategory(result);
        })
    }

    editMonster(monsterTemplate: MonsterTemplate) {
        const index = this.monsters.findIndex(x => x.id === monsterTemplate.id);
        if (index !== -1) {
            this.monsters[index] = monsterTemplate;
        }
        this.updateSelectSubCategory(monsterTemplate);
    }

    updateSelectSubCategory(monsterTemplate: MonsterTemplate) {
        const monsterType = this.monsterTypes.find(x => x.id === monsterTemplate.subCategory.type.id);
        if (!monsterType) {
            this.monsterTypes.push(monsterTemplate.subCategory.type);
            this.selectedMonsterType = monsterTemplate.subCategory.type;
        }
        else if (!(monsterType.subCategories.find(c => c.id === monsterTemplate.subCategory.id))) {
            monsterType.subCategories.push(monsterTemplate.subCategory);
            this.selectedMonsterType = monsterType;
        }

        if (this.selectedMonsterType) {
            const monsterSubCategory = this.selectedMonsterType.subCategories.find(x => x.id === monsterTemplate.subCategory.id);
            if (monsterSubCategory) {
                this.selectedMonsterSubCategory = monsterSubCategory;
            } else {
                this.selectMonsterType(this.selectedMonsterType);
            }
        }
        this.sortMonsterBySubCategory();
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
                this.sortMonsterBySubCategory();
            }
        );
        this.isAdmin = !!this.loginService.currentLoggedUser && this.loginService.currentLoggedUser.admin;
    }
}
