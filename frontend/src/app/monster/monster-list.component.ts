import {Component, OnInit, ChangeDetectionStrategy} from '@angular/core';

import {MonsterTemplate, MonsterTemplateSubCategory, MonsterTemplateType} from './monster.model';
import {MonsterTemplateService} from './monster-template.service';
import {
    EditMonsterTemplateDialogComponent,
    EditMonsterTemplateDialogData
} from './edit-monster-template-dialog.component';
import {LoginService} from '../user';
import {forkJoin} from 'rxjs';
import {ActivatedRoute, Router} from '@angular/router';
import {NhbkMatDialog} from '../material-workaround';
import { MatFabButton } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import { MatFormField } from '@angular/material/form-field';
import { MatSelect } from '@angular/material/select';
import { FormsModule } from '@angular/forms';
import { MatOption } from '@angular/material/autocomplete';
import { MonsterTemplateComponent } from './monster-template.component';

@Component({
    selector: 'monster-list',
    templateUrl: './monster-list.component.html',
    styleUrls: ['./monster-list.component.scss'],
    changeDetection: ChangeDetectionStrategy.Eager,
    imports: [MatFabButton, MatIcon, MatFormField, MatSelect, FormsModule, MatOption, MonsterTemplateComponent]
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
        private readonly route: ActivatedRoute,
        private readonly router: Router,
    ) {
    }

    /**
     * The component is also displayed inside a dialog, in that case the url must not be changed.
     */
    private get isRoutedComponent(): boolean {
        return this.route.component === MonsterListComponent;
    }

    selectMonsterType(monsterType: MonsterTemplateType) {
        this.selectedMonsterType = monsterType;
        if (monsterType.subCategories.length) {
            this.selectedMonsterSubCategory = monsterType.subCategories[0];
        } else {
            this.selectedMonsterSubCategory = undefined;
        }
        this.updateUrl();
    }

    selectMonsterSubCategory(subCategory: MonsterTemplateSubCategory) {
        this.selectedMonsterSubCategory = subCategory;
        this.updateUrl();
    }

    private updateUrl() {
        if (!this.isRoutedComponent) {
            return;
        }
        this.router.navigate([], {
            relativeTo: this.route,
            queryParams: {
                type: this.selectedMonsterType?.id,
                subCategory: this.selectedMonsterSubCategory?.id,
            },
            queryParamsHandling: 'merge',
            replaceUrl: true,
        });
    }

    private restoreSelectionFromUrl(monsterTypes: MonsterTemplateType[]) {
        if (!monsterTypes.length) {
            return;
        }
        const queryParams = this.isRoutedComponent ? this.route.snapshot.queryParamMap : undefined;
        const typeId = Number(queryParams?.get('type'));
        const subCategoryId = Number(queryParams?.get('subCategory'));

        const monsterType = monsterTypes.find(x => x.id === typeId) ?? monsterTypes[0];
        this.selectedMonsterType = monsterType;
        this.selectedMonsterSubCategory = monsterType.subCategories.find(x => x.id === subCategoryId)
            ?? monsterType.subCategories[0];
        this.updateUrl();
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
                this.updateUrl();
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
                this.monsterTypes = monsterTypes;
                this.restoreSelectionFromUrl(monsterTypes);
                this.monsters = monsters;
                this.sortMonsterBySubCategory();
            }
        );
        this.isAdmin = !!this.loginService.currentLoggedUser && this.loginService.currentLoggedUser.admin;
    }
}
