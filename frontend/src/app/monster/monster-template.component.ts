import {Component, EventEmitter, Input, OnChanges, OnInit, Output, ChangeDetectionStrategy} from '@angular/core';

import {NotificationsService} from '../notifications';

import {MonsterTemplate, MonsterTrait, MonsterTraitDictionary} from './monster.model';
import {MonsterTraitDialogComponent} from './monster-trait-dialog.component';
import {MonsterTemplateService} from './monster-template.service';
import {EditMonsterTemplateDialogComponent, EditMonsterTemplateDialogData} from './edit-monster-template-dialog.component';
import {ItemTemplateDialogComponent} from '../item-template/item-template-dialog.component';
import {ItemTemplate} from '../item-template';
import {NhbkMatDialog} from '../material-workaround';
import { MatCard, MatCardHeader, MatCardTitle, MatCardSubtitle, MatCardContent } from '@angular/material/card';
import { MatTooltip } from '@angular/material/tooltip';
import {removeDiacritics} from '../shared';
import { MatIconButton } from '@angular/material/button';
import { MatMenuTrigger, MatMenu, MatMenuItem } from '@angular/material/menu';
import { MatIcon } from '@angular/material/icon';
import { MatRipple } from '@angular/material/core';
import { IconComponent } from '../shared/icon.component';
import { DecimalPipe } from '@angular/common';
import { MarkdownPipe } from '../markdown/markdown.pipe';

import {MonsterSpellsSummaryComponent} from '../spell/monster-spells-summary.component';

export interface MonsterStatDisplay {
    label: string;
    tooltip: string;
    value: string | number;
}

export function getMonsterTemplateStats(monsterTemplate: MonsterTemplate): MonsterStatDisplay[] {
    const data = monsterTemplate.data;
    const stats: MonsterStatDisplay[] = [];
    const isSet = (value: unknown) => value !== undefined && value !== null && value !== '';
    if (isSet(data.prd) && data.prd !== 0) {
        stats.push({label: 'PRD', tooltip: 'Parade', value: data.prd!});
    }
    if (isSet(data.esq) && data.esq !== 0) {
        stats.push({label: 'ESQ', tooltip: 'Esquive', value: data.esq!});
    }
    stats.push({label: 'EV', tooltip: 'Énergie vitale', value: data.ev});
    if (isSet(data.ea) && data.ea !== 0) {
        stats.push({label: 'EA', tooltip: 'Énergie astrale', value: data.ea});
    }
    stats.push({label: 'PR', tooltip: 'Protection', value: data.pr});
    if (isSet(data.pr_magic) && data.pr_magic !== 0) {
        stats.push({label: 'PR mag.', tooltip: 'Protection magique', value: data.pr_magic});
    }
    if (isSet(data.resm) && data.resm !== 0) {
        stats.push({label: 'RM', tooltip: 'Résistance magique', value: data.resm});
    }
    stats.push({label: 'COU', tooltip: 'Courage', value: data.cou});
    stats.push({label: 'XP', tooltip: 'Expérience gagnée', value: data.xp});
    return stats;
}

export function getMonsterTraitImageUrl(trait: MonsterTrait): string {
    return 'assets/img/monster-traits/' + removeDiacritics(trait.name).split(' ').join('_').toLowerCase() + '.png';
}

@Component({
    selector: 'monster-template',
    styleUrls: ['./monster-template.component.scss'],
    templateUrl: './monster-template.component.html',
    changeDetection: ChangeDetectionStrategy.Eager,
    imports: [MatCard, MatCardHeader, MatCardTitle, MatCardSubtitle, MatIconButton, MatMenuTrigger, MatIcon, MatMenu, MatMenuItem, MatCardContent, MatTooltip, MatRipple, IconComponent, DecimalPipe, MarkdownPipe, MonsterSpellsSummaryComponent]
})
export class MonsterTemplateComponent implements OnInit, OnChanges {
    @Input() monsterTemplate: MonsterTemplate;
    @Input() isAdmin: boolean;
    @Input() showCategory = false;
    @Input() highlighted = false;
    public stats: MonsterStatDisplay[] = [];
    @Output() edit: EventEmitter<MonsterTemplate> = new EventEmitter<MonsterTemplate>();
    public traisById?: MonsterTraitDictionary;

    constructor(
        private readonly dialog: NhbkMatDialog,
        private readonly monsterTemplateService: MonsterTemplateService,
        private readonly notifications: NotificationsService,
    ) {
    }

    openEditMonsterDialog(): void {
        const dialogRef = this.dialog.openFullScreen<EditMonsterTemplateDialogComponent, EditMonsterTemplateDialogData, MonsterTemplate>(
            EditMonsterTemplateDialogComponent, {
                data: {
                    monsterTemplate: this.monsterTemplate
                }
            });
        dialogRef.afterClosed().subscribe((result) => {
            if (!result) {
                return;
            }
            this.notifications.success('Monster', 'Monstre éditer');
            this.edit.next(result);
        });
    }

    ngOnChanges(): void {
        this.stats = getMonsterTemplateStats(this.monsterTemplate);
    }

    traitImageUrl(trait: MonsterTrait): string {
        return getMonsterTraitImageUrl(trait);
    }

    copyLink(): void {
        const url = new URL('/database/monsters', window.location.origin);
        url.searchParams.set('type', this.monsterTemplate.subCategory.type.id.toString());
        url.searchParams.set('monster', this.monsterTemplate.id.toString());
        navigator.clipboard.writeText(url.toString()).then(
            () => this.notifications.success('Monstre', 'Lien copié'),
            () => this.notifications.error('Impossible de copier le lien', url.toString())
        );
    }

    ngOnInit(): void {
        this.monsterTemplateService.getMonsterTraitsById().subscribe(
            traitsById => {
                this.traisById = traitsById
            }
        );
    }

    openItemInfo(itemTemplate: ItemTemplate) {
        this.dialog.open(ItemTemplateDialogComponent, {
            data: {itemTemplate},
            autoFocus: false
        });
    }

    openTraitInfo(trait: MonsterTrait, level?: number) {
        this.dialog.open(MonsterTraitDialogComponent, {data: {trait, level}});
    }
}
