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
import { MonsterTemplatesTableComponent } from './monster-templates-table.component';
import { MatInput } from '@angular/material/input';
import { MatLabel, MatPrefix, MatSuffix } from '@angular/material/form-field';
import { MatIconButton, MatButton } from '@angular/material/button';
import { MatButtonToggle, MatButtonToggleGroup } from '@angular/material/button-toggle';
import { MatTooltip } from '@angular/material/tooltip';
import { removeDiacritics } from '../shared';

type MonsterListViewMode = 'cards' | 'table';
const viewModeStorageKey = 'monster-list-view-mode';

export interface MonsterTemplateGroup {
    subCategory?: MonsterTemplateSubCategory;
    monsters: MonsterTemplate[];
}

@Component({
    selector: 'monster-list',
    templateUrl: './monster-list.component.html',
    styleUrls: ['./monster-list.component.scss'],
    changeDetection: ChangeDetectionStrategy.Eager,
    imports: [MatFabButton, MatIcon, MatFormField, MatSelect, FormsModule, MatOption, MonsterTemplateComponent, MonsterTemplatesTableComponent, MatInput, MatLabel, MatPrefix, MatSuffix, MatIconButton, MatButton, MatButtonToggleGroup, MatButtonToggle, MatTooltip]
})
export class MonsterListComponent implements OnInit {
    public monsterTypes: MonsterTemplateType[];
    /** Types containing at least one monster, the only ones shown in the selector. */
    public visibleMonsterTypes: MonsterTemplateType[] = [];
    public selectedMonsterType: MonsterTemplateType;
    public selectedMonsterSubCategory?: MonsterTemplateSubCategory;
    public monsters: MonsterTemplate[];
    public subCategories: MonsterTemplateSubCategory[] = [];
    public monsterBySubCategory: {[subCategoryId: number]: MonsterTemplate[]} = {};
    public isAdmin = false;
    public searchText = '';
    public displayedGroups: MonsterTemplateGroup[] = [];
    public searchResultCount = 0;
    public viewMode: MonsterListViewMode = MonsterListComponent.loadViewMode();
    public highlightedMonsterId?: number;

    get isSearching(): boolean {
        return this.searchText.trim().length > 0;
    }

    private static loadViewMode(): MonsterListViewMode {
        try {
            return localStorage.getItem(viewModeStorageKey) === 'table' ? 'table' : 'cards';
        } catch {
            return 'cards';
        }
    }

    setViewMode(viewMode: MonsterListViewMode) {
        this.viewMode = viewMode;
        try {
            localStorage.setItem(viewModeStorageKey, viewMode);
        } catch {
            // Ignore storage errors (private mode...)
        }
    }

    updateSearch(searchText: string) {
        this.searchText = searchText;
        this.updateDisplayedMonsters();
    }

    private static normalize(text: string): string {
        return removeDiacritics(text).toLowerCase();
    }

    private updateDisplayedMonsters() {
        this.searchResultCount = 0;
        if (!this.monsters) {
            this.displayedGroups = [];
            return;
        }
        if (this.isSearching) {
            const terms = MonsterListComponent.normalize(this.searchText).split(/\s+/).filter(t => t);
            const monsters = this.monsters
                .filter(monster => {
                    const haystack = MonsterListComponent.normalize(
                        `${monster.name} ${monster.subCategory.name} ${monster.subCategory.type.name}`
                    );
                    return terms.every(term => haystack.includes(term));
                })
                .sort((a, b) => a.name.localeCompare(b.name));
            this.searchResultCount = monsters.length;
            this.displayedGroups = [{monsters}];
        } else if (this.selectedMonsterType) {
            this.displayedGroups = this.selectedMonsterType.subCategories
                .map(subCategory => ({subCategory, monsters: this.monsterBySubCategory[subCategory.id] ?? []}))
                .filter(group => group.monsters.length > 0);
        } else {
            this.displayedGroups = [];
        }
    }

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
        this.highlightedMonsterId = undefined;
        this.selectedMonsterType = monsterType;
        if (monsterType.subCategories.length) {
            this.selectedMonsterSubCategory = monsterType.subCategories[0];
        } else {
            this.selectedMonsterSubCategory = undefined;
        }
        this.updateUrl();
        this.updateDisplayedMonsters();
    }

    private updateUrl() {
        if (!this.isRoutedComponent) {
            return;
        }
        this.router.navigate([], {
            relativeTo: this.route,
            queryParams: {
                type: this.selectedMonsterType?.id,
                subCategory: undefined,
                monster: this.highlightedMonsterId,
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
        const monsterId = Number(queryParams?.get('monster'));
        this.highlightedMonsterId = monsterId || undefined;

        const monsterType = monsterTypes.find(x => x.id === typeId) ?? monsterTypes[0];
        this.selectedMonsterType = monsterType;
        this.selectedMonsterSubCategory = monsterType.subCategories[0];
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
        this.visibleMonsterTypes = (this.monsterTypes ?? [])
            .filter(type => type.subCategories.some(subCategory => monsterBySubCategory[subCategory.id]?.length));
        this.updateDisplayedMonsters();
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

    private selectHighlightedMonsterCategory() {
        if (!this.highlightedMonsterId) {
            return;
        }
        const monster = this.monsters.find(m => m.id === this.highlightedMonsterId);
        if (!monster) {
            this.highlightedMonsterId = undefined;
            this.updateUrl();
            return;
        }
        const monsterType = this.monsterTypes.find(t => t.id === monster.subCategory.type.id);
        const subCategory = monsterType?.subCategories.find(c => c.id === monster.subCategory.id);
        if (monsterType && subCategory) {
            this.selectedMonsterType = monsterType;
            this.selectedMonsterSubCategory = subCategory;
            this.updateUrl();
        }
    }

    scrollToSubCategory(subCategory: MonsterTemplateSubCategory) {
        document.getElementById('monster-sub-category-' + subCategory.id)?.scrollIntoView({behavior: 'smooth', block: 'start'});
    }

    private scrollToHighlightedMonster() {
        if (!this.highlightedMonsterId || this.viewMode !== 'cards') {
            return;
        }
        const monsterId = this.highlightedMonsterId;
        setTimeout(() => {
            document.getElementById('monster-' + monsterId)?.scrollIntoView({behavior: 'smooth', block: 'center'});
        });
    }

    ngOnInit() {
        forkJoin([
            this.monsterTemplateService.getMonsterTypes(),
            this.monsterTemplateService.getMonsterList()
        ]).subscribe(
            ([monsterTypes, monsters]) => {
                this.monsterTypes = monsterTypes;
                this.monsters = monsters;
                this.sortMonsterBySubCategory();
                this.restoreSelectionFromUrl(this.visibleMonsterTypes);
                this.selectHighlightedMonsterCategory();
                this.updateDisplayedMonsters();
                this.scrollToHighlightedMonster();
            }
        );
        this.isAdmin = !!this.loginService.currentLoggedUser && this.loginService.currentLoggedUser.admin;
    }
}
