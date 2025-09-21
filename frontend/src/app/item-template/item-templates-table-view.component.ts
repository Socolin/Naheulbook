import {Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges} from '@angular/core';
import {ItemTemplate, ItemTemplateSection} from './item-template.model';
import {GodByTechName} from '../shared';
import {LoginService} from '../user';
import {animate, state, style, transition, trigger} from '@angular/animations';

@Component({
    selector: 'app-item-templates-table-view',
    templateUrl: './item-templates-table-view.component.html',
    styleUrls: ['./item-templates-table-view.component.scss'],
    animations: [
        trigger('detailExpand', [
            state('collapsed', style({ height: '0px', minHeight: '0' })),
            state('expanded', style({ height: '*' })),
            transition('expanded <=> collapsed', animate('225ms cubic-bezier(0.4, 0.0, 0.2, 1)')),
        ]),
    ],
    standalone: false
})
export class ItemTemplatesTableViewComponent implements OnInit, OnChanges {
    @Input()
    itemTemplates: ItemTemplate[];
    @Input()
    itemTemplateSection: ItemTemplateSection;

    @Input()
    originsName: { [originId: string]: string };
    @Input()
    jobsName: { [jobId: string]: string };
    @Input()
    godsByTechName: GodByTechName;

    @Output()
    startEdit: EventEmitter<ItemTemplate> = new EventEmitter<ItemTemplate>()
    @Output()
    startCreateCopy: EventEmitter<ItemTemplate> = new EventEmitter<ItemTemplate>()
    @Input() actions: string[];
    @Output() actionTriggered = new EventEmitter<{action: string, data: any}>();

    baseColumns = [
        {columnName: 'name', priority: 50},
        {columnName: 'price', priority: 500},
        {columnName: 'actions', priority: 5000, loggedUser: true}
    ];
    columnsToDisplay: string[] = [];
    expandedElement?: ItemTemplate;

    columnsDefinitions: { [columnName: string]: { condition: (itemTemplate: ItemTemplate) => boolean, priority: number } } = {
        'dice': {
            condition: (itemTemplate: ItemTemplate) => !!itemTemplate.data.diceDrop,
            priority: 750
        },
        'level': {
            condition: (itemTemplate: ItemTemplate) => !!itemTemplate.data.requireLevel,
            priority: 80
        },
        'damage': {
            condition: (itemTemplate: ItemTemplate) => !!itemTemplate.data.damageDice || !!itemTemplate.data.damageType,
            priority: 100
        },
        'protection': {
            condition: (itemTemplate: ItemTemplate) => !!itemTemplate.data.protection || !!itemTemplate.data.magicProtection,
            priority: 100
        },
        'modifiers': {
            condition: (itemTemplate: ItemTemplate) => itemTemplate.modifiers.length > 0,
            priority: 110
        },
        'rupture': {
            condition: (itemTemplate: ItemTemplate) => !!itemTemplate.data.rupture,
            priority: 130
        },
    }

    constructor(
        public readonly loginService: LoginService,
    ) {
    }

    ngOnInit(): void {
    }

    asItemTemplate(item: any): ItemTemplate {
        return item as ItemTemplate;
    }



    formatDamage(itemTemplate: ItemTemplate): string {
        let damage = '';
        if (itemTemplate.data.damageDice && itemTemplate.data.bonusDamage) {
            if (itemTemplate.data.bonusDamage > 0) {
                damage += itemTemplate.data.damageDice + 'd +' + itemTemplate.data.bonusDamage;
            } else {
                damage += itemTemplate.data.damageDice + 'd ' + itemTemplate.data.bonusDamage;
            }
        } else if (itemTemplate.data.damageDice) {
            damage += itemTemplate.data.damageDice + 'd';
        } else if (itemTemplate.data.bonusDamage) {
            damage += itemTemplate.data.bonusDamage;
        }

        if (itemTemplate.data.damageType) {
            damage += `(${itemTemplate.data.damageType})`;
        }
        return damage;
    }

    formatRupture(itemTemplate: ItemTemplate): string {
        if (itemTemplate.data.rupture !== undefined) {
            if (itemTemplate.data.rupture > 0) {
                return '1 à ' + itemTemplate.data.rupture;
            } else {
                return 'Incassable';
            }
        }
        return '';
    }

    isEditable(itemTemplate: ItemTemplate): boolean {
        if (!this.loginService.currentLoggedUser) {
            return false;
        }
        if (this.loginService.currentLoggedUser.admin) {
            return true;
        }
        if (itemTemplate.source !== 'official' && this.loginService.currentLoggedUser.id === itemTemplate.sourceUserId) {
            return true;
        }
        return false;
    }

    ngOnChanges(changes: SimpleChanges): void {
        if ('itemTemplates' in changes) {
            this.updateVisibleColumns(changes.itemTemplates.currentValue);
        }
    }

    private updateVisibleColumns(itemTemplates: ItemTemplate[]) {
        const columnsCount: {columnName: string; count: number; priority: number}[] = [];
        if (!itemTemplates?.length) {
            return;
        }

        for (const [columnName, columnDef] of Object.entries(this.columnsDefinitions)) {
            let columnData = {columnName: columnName, count: 0, priority: columnDef.priority};
            for (let itemTemplate of itemTemplates) {
                if (columnDef.condition(itemTemplate)) {
                    columnData.count++;
                }
            }
            if (columnData.count > 0) {
                columnsCount.push(columnData);
            }
        }

        this.columnsToDisplay = columnsCount
            .sort((a, b) => b.count - a.count)
            .slice(0, 4)
            .map(e => ({columnName: e.columnName, priority: e.priority}))
            .concat(this.baseColumns.filter(x => (!x.loggedUser) || (x.loggedUser && this.loginService.currentLoggedUser !== undefined)))
            .sort((a, b) => a.priority - b.priority)
            .map(c => c.columnName);
    }
    hasAction(actionName: string) {
        return this.actions && this.actions.indexOf(actionName) !== -1;
    }

    emitAction(actionName: string, data: any) {
        this.actionTriggered.emit({action: actionName, data: data});
    }
}
