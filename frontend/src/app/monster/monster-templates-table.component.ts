import {AfterViewInit, ChangeDetectionStrategy, Component, EventEmitter, Input, OnChanges, Output, ViewChild} from '@angular/core';
import {MatTableDataSource, MatTableModule} from '@angular/material/table';
import {MatSort, MatSortModule} from '@angular/material/sort';

import {MonsterTemplate} from './monster.model';
import {MonsterTemplateComponent} from './monster-template.component';

@Component({
    selector: 'monster-templates-table',
    templateUrl: './monster-templates-table.component.html',
    styleUrls: ['./monster-templates-table.component.scss'],
    changeDetection: ChangeDetectionStrategy.Eager,
    imports: [MatTableModule, MatSortModule, MonsterTemplateComponent]
})
export class MonsterTemplatesTableComponent implements OnChanges, AfterViewInit {
    @Input() monsterTemplates: MonsterTemplate[] = [];
    @Input() isAdmin = false;
    @Input() showCategory = false;
    @Input() expandedMonsterId?: number;
    @Output() edit = new EventEmitter<MonsterTemplate>();

    @ViewChild(MatSort) sort: MatSort;

    public dataSource = new MatTableDataSource<MonsterTemplate>([]);
    public expandedElement?: MonsterTemplate;

    get columnsToDisplay(): string[] {
        const columns = ['name'];
        if (this.showCategory) {
            columns.push('category');
        }
        columns.push('at', 'dmg', 'defense', 'ev', 'pr', 'cou', 'xp');
        return columns;
    }

    constructor() {
        this.dataSource.sortingDataAccessor = (monster, column) => {
            const data = monster.data;
            switch (column) {
                case 'name':
                    return monster.name.toLocaleLowerCase();
                case 'category':
                    return `${monster.subCategory.type.name} ${monster.subCategory.name}`.toLocaleLowerCase();
                case 'at':
                    return this.maxAttack(monster) ?? -1;
                case 'defense':
                    return data.prd || data.esq || -1;
                case 'ev':
                    return data.ev ?? -1;
                case 'pr':
                    return data.pr ?? -1;
                case 'cou':
                    return data.cou ?? -1;
                case 'xp':
                    return data.xp ?? -1;
            }
            return '';
        };
    }

    ngOnChanges(): void {
        this.dataSource.data = this.monsterTemplates;
        if (this.expandedMonsterId !== undefined) {
            this.expandedElement = this.monsterTemplates.find(m => m.id === this.expandedMonsterId) ?? this.expandedElement;
        }
    }

    ngAfterViewInit(): void {
        this.dataSource.sort = this.sort;
    }

    toggle(monster: MonsterTemplate): void {
        this.expandedElement = this.expandedElement === monster ? undefined : monster;
    }

    maxAttack(monster: MonsterTemplate): number | undefined {
        const values = (monster.data.attacks || []).map(a => a.at).filter(at => at !== undefined && at !== null);
        return values.length ? Math.max(...values) : undefined;
    }

    attacksAt(monster: MonsterTemplate): string {
        return (monster.data.attacks || []).map(a => a.at).join(' / ');
    }

    attacksDmg(monster: MonsterTemplate): string {
        return (monster.data.attacks || []).map(a => a.dmg).filter(x => !!x).join(' / ');
    }

    defense(monster: MonsterTemplate): string {
        if (monster.data.prd) {
            return `PRD ${monster.data.prd}`;
        }
        if (monster.data.esq) {
            return `ESQ ${monster.data.esq}`;
        }
        return '';
    }

    asMonster(value: unknown): MonsterTemplate {
        return value as MonsterTemplate;
    }
}
